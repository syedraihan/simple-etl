using System;
using System.Data;

namespace SimpleETL
{
    public class BatchCompletedEventArgs : EventArgs
    {
        public int BatchNo { get; set; }
    }
}
