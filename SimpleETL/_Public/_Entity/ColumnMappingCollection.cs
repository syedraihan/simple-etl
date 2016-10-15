using System;
using System.Collections.Generic;

namespace SimpleETL
{
    public class ColumnMappingCollection : Dictionary<string, string>
    {
        public string IdColumn { get; set; } = string.Empty;
    }
}