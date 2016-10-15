using System;

namespace SimpleETL
{
    public class DataValidationErrorEventArgs : EventArgs
    {
        public object RowId { get; set; }

        public string ColumnName { get; set; }

        public object OriginalValue { get; set; }

        public object ReplacedValue { get; set; }
    }
}