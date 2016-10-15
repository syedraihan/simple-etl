using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace SimpleETL.Transform
{
    internal class TransformManager
    {
        private string _idColName;
        protected IDictionary<string, string> _columnMappings;

        public TransformManager() { }

        public void Initialize(IDictionary<string, string> columnMappings, string idColName)
        {
            _columnMappings = columnMappings;
            _idColName = idColName;
        }

        public virtual DataTable Transform(DataTable dtSource)
        {
            Debug.Assert(_columnMappings != null);

            if (_columnMappings.Count == 0)
                return dtSource;

            return Transform(dtSource, _columnMappings);
        }

        protected DataTable Transform(DataTable dtSource, IDictionary<string, string> columnMappings)
        {
            DataTable dtDestination = CreateDataTable(_columnMappings.Keys);

            foreach (DataRow rowSource in dtSource.Rows)
            {
                var rowDestination = dtDestination.NewRow();

                object rowId = GetRowId(rowSource, columnMappings);
                foreach (string key in columnMappings.Keys)
                {
                    string destColumn = key;
                    string sourceColumn = columnMappings[key];

                    rowDestination[destColumn] = GetColumnValue(rowId, destColumn, rowSource[sourceColumn]);
                }

                dtDestination.Rows.Add(rowDestination);
            }

            return dtDestination;
        }

        protected virtual object GetColumnValue(object rowId, string destColumn, object sourceValue)
        {
            return sourceValue;
        }

        private DataTable CreateDataTable(ICollection<string> columns)
        {
            var dt = new DataTable();

            foreach (string colName in columns)
                dt.Columns.Add(colName);

            return dt;
        }

        private object GetRowId(DataRow dr, IDictionary<string, string> columnMappings)
        {
            return columnMappings.ContainsKey(_idColName) ? dr[columnMappings[_idColName]] : string.Empty;
        }
    }
}