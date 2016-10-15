using System;
using System.Data;

namespace SimpleETL
{
    public class RowInsertErrorEventArgs : EventArgs
    {
        public DataRow Row { get; set; }

        public string Message { get; set; }
    }
}