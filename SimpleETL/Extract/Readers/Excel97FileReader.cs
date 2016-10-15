using System;

namespace SimpleETL.Extract
{
    internal class Excel97FileReader : ExcelFileReaderBase
    {
        public Excel97FileReader(string filePath) : base(filePath) { }

        protected override string FileType
        {
            get
            {
                return "Excel 12.0 Xml";
            }
        }
    }
}