using System.Data.SqlTypes;
using System.Diagnostics;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Transform
{
    internal class BooleanValidator : ValidatorBase
    {
        public override object EmptyValue
        {
            get
            {
                return false;
            }
        }

        public override object Parse(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            Debug.Assert(sourceValue != null);
            return SqlBoolean.Parse(sourceValue.ToString());
        }
    }
}