    using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace SimpleETL.Extract
{
    internal abstract class FileReaderBase 
    {
        protected readonly string _filePath;
        public FileReaderBase(string filePath)
        {
            _filePath = filePath;
            this.HeaderRow = true;
        }

        public bool HeaderRow { get; set; }

        public string FilePath
        {
            get { return _filePath; }
        }

        protected string DirectoryName
        {
            get
            {
                return Path.GetDirectoryName(_filePath);
            }
        }

        protected string FileName
        {
            get
            {
                return Path.GetFileName(_filePath);
            }
        }

        protected abstract string FileType { get; }

        protected virtual string DataSource
        {
            get
            {
                return _filePath;
            }
        }

        protected virtual IEnumerable<string> ExtendedProperties
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        protected abstract string CommandText { get; }

        protected virtual string GetConnectionString()
        {
            var conBilder = new OleDbConnectionStringBuilder();

            conBilder.Provider = "Microsoft.ACE.OLEDB.12.0";
            conBilder.DataSource = this.DataSource;                                
            conBilder.Add("Extended Properties", this.GetExtendedPropertyString());

            return conBilder.ToString();
        }

        protected virtual string GetExtendedPropertyString()
        {
            var properties = new List<string>();

            properties.Add(this.FileType);
            properties.Add(string.Format("HDR={0}", this.HeaderRow ? "Yes" : "No"));

            foreach (var property in this.ExtendedProperties)
            {
                properties.Add(property);
            }

            return string.Join(";", properties.ToArray());
        }

        protected virtual void BeforeConnect() {}

        public IDataReader GetReader()
        {
            this.BeforeConnect();
            var conn = new OleDbConnection(this.GetConnectionString());
            conn.Open();

            using (var cmd = new OleDbCommand(this.CommandText, conn))
            {
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

    }
}