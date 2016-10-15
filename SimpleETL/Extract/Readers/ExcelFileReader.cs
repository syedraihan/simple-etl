namespace SimpleETL.Extract
{
    internal class ExcelFileReader : ExcelFileReaderBase
    {
        public ExcelFileReader(string filePath) : base(filePath) { }

        protected override string FileType
        {
            get
            {
                return "Excel 8.0";
            }
        }
    }
}