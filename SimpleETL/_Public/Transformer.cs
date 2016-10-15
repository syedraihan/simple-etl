using System;
using System.Data;
using System.Collections.Generic;
using SimpleETL.Extract;
using SimpleETL.Transform;
using SimpleETL.Transform.Expression;
using SimpleETL.Transform.DbSchema;
using SimpleETL.Load;

namespace SimpleETL
{
    public class Transformer 
    {
        private const int DEFAILT_BATCH_SIZE = 5000;
        private const int DEFAILT_PREVIEW_SIZE = 20;
        private const int DEFAUT_ERROR_LIMIT = Int32.MaxValue;
        private const bool DEFAULT_VALIDATE = false;
        private const int DEFAULT_SKIP_ROWS = 0;

        private readonly string _sourceConn;
        private readonly string _destinationConn;
        private readonly string _destinationTable;
        private readonly ColumnMappingCollection _colMappings;
        private readonly ExpressionCollection _expressions;

        public event EventHandler<DataValidationErrorEventArgs> DataValidationError;
        public event EventHandler<RowInsertErrorEventArgs> RowInsertError;
        public event EventHandler<BatchCompletedEventArgs> BatchCompleted;

        #region Constructor Overloads

        public Transformer(string sourceConnection)
        {
            _sourceConn = sourceConnection;
            _destinationConn = string.Empty;
            _destinationTable = string.Empty;
            _colMappings = new ColumnMappingCollection();
            _expressions = new ExpressionCollection();
        }

        public Transformer(string sourceConnection, ColumnMappingCollection columnMappings) : this(sourceConnection)
        {
            _colMappings = columnMappings;
        }

        public Transformer(string sourceConnection, ExpressionCollection expressions) : this(sourceConnection)
        {
            _expressions = expressions;
        }

        public Transformer(string sourceConnection, ColumnMappingCollection columnMappings, ExpressionCollection expressions) : this(sourceConnection)
        {
            _colMappings = columnMappings;
            _expressions = expressions;
        }

        public Transformer(string sourceConnection, string destinationConnection, string destinationTable) : this(sourceConnection)
        {
            _destinationConn = destinationConnection;
            _destinationTable = destinationTable;
        }

        public Transformer(string sourceConnection, string destinationConnection, string destinationTable, ColumnMappingCollection columnMappings) : this(sourceConnection, destinationConnection, destinationTable)
        {
            _colMappings = columnMappings;
        }

        public Transformer(string sourceConnection, string destinationConnection, string destinationTable, ExpressionCollection expressions) : this(sourceConnection, destinationConnection, destinationTable)
        {
            _expressions = expressions;
        }

        public Transformer(string sourceConnection, string destinationConnection, string destinationTable, ColumnMappingCollection columnMappings, ExpressionCollection expressions) : this(sourceConnection, destinationConnection, destinationTable)
        {
            _colMappings = columnMappings;
            _expressions = expressions;
        }

        #endregion

        #region Method Overloads

        public IEnumerable<DataTable> Preview()
        {
            return Preview(DEFAULT_VALIDATE);
        }

        public IEnumerable<DataTable> Preview(bool validate)
        {
            return Preview(validate, DEFAULT_SKIP_ROWS);
        }

        public IEnumerable<DataTable> Preview(bool validate, int skipRows)
        {
            return Preview(validate, skipRows, DEFAILT_PREVIEW_SIZE);
        }

        public void Execute()
        {
            Execute(DEFAULT_VALIDATE);
        }

        public void Execute(bool validate)
        {
            Execute(validate, DEFAULT_SKIP_ROWS);
        }

        public void Execute(bool validate, int skipRows)
        {
            Execute(validate, skipRows, DEFAILT_BATCH_SIZE);
        }

        public void Execute(bool validate, int skipRows, int batchSize)
        {
            Execute(validate, skipRows, batchSize, DEFAUT_ERROR_LIMIT);
        }

        #endregion

        public IEnumerable<DataTable> Preview(bool validate, int skipRows, int batchSize)
        {
            if (validate && _destinationConn.Length == 0)
                throw new InvalidOperationException("No destination was provided.");

            var reader = GetReader(skipRows, batchSize);
            var expr = GetExpressionManager();
            var trans = GetTransformManager(validate);

            foreach(var dt in reader.GetDataTables())
            {
                expr.EvaluateExpressions(dt);
                yield return trans.Transform(dt);
            }
        }

        public void Execute(bool validate, int skipRows, int batchSize, int errorLimit)
        {
            if (_destinationConn.Length == 0)
                throw new InvalidOperationException("No destination was provided.");

            using (var sbc = new SqlBulkCopyManager())
            {
                sbc.Initialize(_destinationConn, _destinationTable);
                sbc.RowInsertError += new EventHandler<RowInsertErrorEventArgs>(delegate (object sender, RowInsertErrorEventArgs e) { OnRowInsertError(e); });
                sbc.BatchCompleted += new EventHandler<BatchCompletedEventArgs>(delegate (object sender, BatchCompletedEventArgs e) { OnBatchCompleted(e); });

                foreach (DataTable dt in Preview(validate, skipRows, batchSize))
                    sbc.Load(dt, errorLimit);
            }
        }

        private BufferedReader GetReader(int skipRows, int batchSize)
        {
            FileReaderBase rdr = (new ReaderFactory()).CreateReader(_sourceConn);
            return new BufferedReader(rdr.GetReader(), skipRows, batchSize);
        }

        private ExpressionManager GetExpressionManager()
        {
            var expr = new ExpressionManager();
            expr.Initialize(_expressions, _expressions.CustomFunctions);

            return expr;
        }

        private TransformManager GetTransformManager(bool validate)
        {
            TransformManager manager;
            if (!validate)
            {
                manager = new TransformManager();
                manager.Initialize(_colMappings, _colMappings.IdColumn);
            }
            else
            {
                var dsh = new DbSchemaHelper();
                var schema = dsh.GetSchema(_destinationConn, _destinationTable);

                var trans = new ValidatingTransformManager();
                trans.Initialize(_colMappings, _colMappings.IdColumn, schema);
                trans.DataValidationError += new EventHandler<DataValidationErrorEventArgs>(delegate (object sender, DataValidationErrorEventArgs e) { OnDataValidationError(e); });
                manager = trans;
            }

            return manager;
        }

        protected virtual void OnDataValidationError(DataValidationErrorEventArgs e)
        {
            DataValidationError?.Invoke(this, e);
        }

        protected virtual void OnRowInsertError(RowInsertErrorEventArgs e)
        {
            RowInsertError?.Invoke(this, e);
        }

        protected virtual void OnBatchCompleted(BatchCompletedEventArgs e)
        {
            BatchCompleted?.Invoke(this, e);
        }
    }
}