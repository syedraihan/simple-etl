using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SimpleETL.Load
{
    internal class SqlBulkCopyManager : IDisposable
    {
        protected SqlBulkCopy _bulkCopy;
        private SqlConnection _conn;
        private int _errorCount = 0;
        private int _batchNo = 0;

        public event EventHandler<RowInsertErrorEventArgs> RowInsertError;
        public event EventHandler<BatchCompletedEventArgs> BatchCompleted;

        public SqlBulkCopyManager() { }

        public void Initialize(string destConn, string destTable)
        {
            _conn = new SqlConnection(destConn);
            _conn.Open();

            _bulkCopy = new SqlBulkCopy(_conn, SqlBulkCopyOptions.TableLock, null);
            _bulkCopy.DestinationTableName = destTable;
            _bulkCopy.BulkCopyTimeout = 0;
        }

        public void Load(DataTable dt)
        {
            Load(dt, Int32.MaxValue);
        }

        public void Load(DataTable dt, int errorLimit)
        {
            Debug.Assert(_conn != null);
            Debug.Assert(_conn.State == ConnectionState.Open);
            Debug.Assert(_bulkCopy != null);

            if (_bulkCopy.ColumnMappings.Count == 0)
                AddColumnMappings(dt);

            try
            {
                _bulkCopy.WriteToServer(dt);
            }
            catch (InvalidOperationException)
            {
                RetryBatch(dt, errorLimit);
            }

            _batchNo++;
            OnBatchCompleted(new BatchCompletedEventArgs() { BatchNo = _batchNo });
        }

        protected virtual void RetryBatch(DataTable dt, int errorLimit)
        {
            foreach(DataRow row in dt.Rows)
            {
                try
                {
                    _bulkCopy.WriteToServer(new DataRow[] { row });
                }
                catch (InvalidOperationException ex)
                {
                    _errorCount++;
                    if (_errorCount >= errorLimit)
                        throw new InvalidOperationException("Error limit exceeded!");

                    OnRowInsertError(new RowInsertErrorEventArgs()
                    {
                        Row = row,
                        Message = ex.Message
                    });
                }
            }
        }

        public void Dispose()
        {
            if (_conn != null)
            if (_conn.State == ConnectionState.Open)
                _conn.Close();

            if (_bulkCopy != null)
                _bulkCopy.Close();
        }

        protected virtual void OnRowInsertError(RowInsertErrorEventArgs e)
        {
            RowInsertError?.Invoke(this, e);
        }

        protected virtual void OnBatchCompleted(BatchCompletedEventArgs e)
        {
            BatchCompleted?.Invoke(this, e);
        }

        private void AddColumnMappings(DataTable dt)
        {
            foreach (DataColumn col in dt.Columns)
            {
                _bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(col.ColumnName, col.ColumnName));
            }
        }
    }
}