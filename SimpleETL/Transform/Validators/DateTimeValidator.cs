using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Transform
{
    internal class DateTimeValidator : ValidatorBase
    {
        public override object EmptyValue
        {
            get
            {
                return SqlDateTime.MinValue;
            }
        }

        public override object Parse(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            Debug.Assert(sourceValue != null);
            return SqlDateTime.Parse(sourceValue.ToString());
        }
    }
}