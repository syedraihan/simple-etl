using System.Collections.Generic;

namespace SimpleETL.Extract
{
    internal abstract class ExcelFileReaderBase : FileReaderBase
    {
        public ExcelFileReaderBase(string filePath) : base(filePath)
        {
            this.TableName = "Sheet1";
        }

        public string TableName { get; set; }

        protected override IEnumerable<string> ExtendedProperties
        {
            get
            {
                yield return "IMEX=1";
            }
        }

        protected override string CommandText
        {
            get
            {
                return "SELECT * FROM [" + this.TableName + "$]";
            }
        }
    }
}