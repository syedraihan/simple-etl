using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Transform
{
    internal class ValidatingTransformManager : TransformManager
    {
        public event EventHandler<DataValidationErrorEventArgs> DataValidationError;

        private IDictionary<string, ColumnTypeInfo> _schema;
        private IDictionary<string, ValidatorBase> _validators;

        public ValidatingTransformManager() { }

        public void Initialize(IDictionary<string, string> columnMappings, string idColName, IDictionary<string, ColumnTypeInfo> schema)
        {
            this.Initialize(columnMappings, idColName);

            _schema = schema;
            _validators = new Dictionary<string, ValidatorBase>()
            {
                { "string",     new StringValidator() },
                { "datetime",   new DateTimeValidator() },
                { "int",        new IntegerValidator() },
                { "decimal",    new DecimalValidator() },
                { "bool",       new BooleanValidator() },
            };
        }

        public override DataTable Transform(DataTable dtSource)
        {
            Debug.Assert(_columnMappings != null);

            if (_columnMappings.Count == 0)
                foreach (DataColumn col in dtSource.Columns)
                    _columnMappings.Add(col.ColumnName, col.ColumnName);             

            return Transform(dtSource, _columnMappings);
        }

        protected override object GetColumnValue(object rowId, string destColumn, object sourceValue)
        {
            Debug.Assert(_schema != null);
            Debug.Assert(_schema.ContainsKey(destColumn));

            var colTypeInfo = _schema[destColumn];
            var validator = _validators[colTypeInfo.DataType];

            var result = validator.Validate(colTypeInfo, sourceValue);

            if (result.IsValid)
                return sourceValue;
            else
            {
                OnDataValidationError(new DataValidationErrorEventArgs()
                {
                    RowId = rowId,
                    ColumnName = destColumn,
                    OriginalValue = sourceValue,
                    ReplacedValue = result.ReplacedValue
                });

                return result.ReplacedValue;
            }
        }

        protected virtual void OnDataValidationError(DataValidationErrorEventArgs e)
        {
            DataValidationError?.Invoke(this, e);
        }
    }
}