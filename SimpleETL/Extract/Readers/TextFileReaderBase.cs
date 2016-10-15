using System.IO;

namespace SimpleETL.Extract
{
    internal abstract class TextFileReaderBase : FileReaderBase
    {
        public TextFileReaderBase(string filePath) : base(filePath) { }

        protected override string DataSource
        {
            get
            {
                return this.DirectoryName;
            }
        }

        protected override string CommandText
        {
            get
            {
                return "SELECT * FROM [" + this.FileName + "]";
            }
        }

        protected override string FileType
        {
            get
            {
                return "text";
            }
        }

        protected override void BeforeConnect()
        {
            base.BeforeConnect();

            var path = Path.Combine(this.DirectoryName, "schema.ini");
            File.WriteAllText(path, this.GetSchemaIniContent());
        }

        protected abstract string GetSchemaIniContent();
    }
}