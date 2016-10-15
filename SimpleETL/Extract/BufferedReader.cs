using System.Data;
using System.Collections.Generic;

namespace SimpleETL.Extract
{
    internal class BufferedReader
    {
        private int _skipRows = 0;
        private bool _skipped = false;
        private int _batchSize = 5000;

        private readonly IDataReader _reader;
        private DataTable _dt = null;

        public BufferedReader(IDataReader reader)
        {
            _reader = reader;
        }

        public BufferedReader(IDataReader reader, int skipRows, int batchSize) 
        {
            _reader = reader;
            _skipRows = skipRows;
            _batchSize = batchSize;
        }

        public IEnumerable<DataTable> GetDataTables()
        {
            while (Read())
            {
                yield return _dt;
            }
        }

        private bool Read()
        {
            if (!_skipped)
            {
                _skipped = true;
                SkipRows(_reader, _skipRows);
            }

            _dt = CreateDataTable(_reader);

            int rowCount = 0;
            while (rowCount < _batchSize)
            {
                if (_reader.Read())
                {
                    rowCount++;
                    AddRow(_dt, _reader);
                }
                else
                    break;
            }

            return rowCount > 0;
        }

        private void SkipRows(IDataReader reader, int rows)
        {
            if (rows == 0) return;

            int i = rows;
            while (i > 0 && reader.Read()) i--;
        }

        private DataTable CreateDataTable(IDataReader reader)
        {
            var dt = new DataTable();

            dt.Clear();
            for (int i=0; i<reader.FieldCount; i++)
            {
                string colName = reader.GetName(i);
                dt.Columns.Add(colName);
            }

            return dt;
        }

        private void AddRow(DataTable dt, IDataReader reader)
        {
            var row = dt.NewRow();

            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i);

            dt.Rows.Add(row);
        }
    }
}