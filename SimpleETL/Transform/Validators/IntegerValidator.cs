using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Transform
{
    internal class IntegerValidator : ValidatorBase
    {
        public override object EmptyValue
        {
            get
            {
                return 0;
            }
        }

        public override object Parse(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            Debug.Assert(sourceValue != null);
            return SqlInt32.Parse(sourceValue.ToString());
        }
    }
}