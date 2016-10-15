using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleETL.Transform.DbSchema
{
    internal class ColumnTypeInfo
    {
        private IDictionary<string, string> _dataTypeMappings = new Dictionary<string, string>()
        {
            { "char",       "string" },
            { "varchar",    "string" },
            { "text",       "string" },
            { "nchar",      "string" },
            { "nvarchar",   "string" },
            { "ntext",      "string" },
            { "datetime",   "datetime" },
            { "int",        "int" },
            { "decimal",    "decimal" },
            { "bit",        "bool" },
        };

        public string ColumnName { get; set; }

        public string SqlDataType { get; set; }

        public int ColumnSize { get; set; }

        public bool AllowNull { get; set; }

        public short Precision { get; set; }

        public short Scale { get; set; }

        public string DataType
        {
            get
            {
                Debug.Assert(_dataTypeMappings.ContainsKey(this.SqlDataType));
                return _dataTypeMappings[this.SqlDataType];
            }
        }

    }
}