using System;
using System.Diagnostics;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Transform
{
    internal class StringValidator : ValidatorBase
    {
        public override object EmptyValue
        {
            get
            {
                return string.Empty;
            }
        }

        public override object Parse(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            Debug.Assert(sourceValue != null);

            if (sourceValue.ToString().Length > colTypeInfo.ColumnSize)
                throw new FormatException();
            else
                return sourceValue;
        }

        protected override object GetReplacedValue(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            Debug.Assert(sourceValue != null);

            return sourceValue.ToString().Substring(0, (int)colTypeInfo.ColumnSize);
        }
    }
}