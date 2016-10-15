using System;
using System.IO;
using System.Text;

namespace SimpleETL.Extract
{
    internal class DelimitedFileReader : TextFileReaderBase
    {
        public DelimitedFileReader(string filePath) : base(filePath)
        {
        }

        public string ColumnDelimeter { get; set; }

        public string TextDelimiter { get; set; }

        protected override string GetSchemaIniContent()
        {
            var sb = new StringBuilder();

            sb.AppendLine(string.Format("[{0}]", Path.GetFileName(this.FileName)));
            sb.AppendLine("ColNameHeader=" + this.HeaderRow.ToString());
            AddColumnDilimeterInfo(sb);
            sb.AppendLine(string.Format("TextDelimiter='{0}'", this.TextDelimiter));
            sb.AppendLine("MaxScanRows=0");

            return sb.ToString();
        }

        private void AddColumnDilimeterInfo(StringBuilder sb)
        {
            if (string.IsNullOrEmpty(this.ColumnDelimeter))
                return;

            string delim = this.ColumnDelimeter.ToLowerInvariant();

            if (delim.Equals(","))
                sb.AppendLine("Format=CSVDelimited");

            else if (delim.Equals("tab"))
                sb.AppendLine("Format=TabDelimited");

            else if (delim.Equals("space"))
            {
                sb.AppendLine("Format=Delimited( )");
                sb.AppendLine("Delimiter=' '");
            }
            else
            {
                sb.AppendLine("Format=Delimited(" + delim + ")");
                sb.AppendLine("Delimiter=" + delim);
            }
        }
    }
}