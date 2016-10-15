using System;
using System.Data.SqlTypes;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Transform
{
    internal abstract class ValidatorBase
    {
        public ValidationResult Validate(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            var result = new ValidationResult();

            if (sourceValue == DBNull.Value || sourceValue == null)
            {
                if (!colTypeInfo.AllowNull)
                {
                    result.IsValid = false;
                    result.ReplacedValue = EmptyValue;
                }
                else
                {
                    result.IsValid = true;
                }
            }
            else
            {
                try
                {
                    object replacedValue = Parse(colTypeInfo, sourceValue);
                    result.IsValid = true;
                }
                catch (Exception ex) when (ex is FormatException || ex is SqlTruncateException)
                {
                    result.IsValid = false;
                    result.ReplacedValue = GetReplacedValue(colTypeInfo, sourceValue);
                }
            }

            return result;
        }

        protected virtual object GetReplacedValue(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            return colTypeInfo.AllowNull ? DBNull.Value : this.EmptyValue;
        }

        public abstract object EmptyValue { get; }

        public abstract object Parse(ColumnTypeInfo colTypeInfo, object sourceValue);
    }
}