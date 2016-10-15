using System;
using System.Data.SqlTypes;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Transform
{ 
    internal class DecimalValidator : ValidatorBase
    {
        public override object EmptyValue
        {
            get
            {
                return 0d;
            }
        }

        public override object Parse(ColumnTypeInfo colTypeInfo, object sourceValue)
        {
            SqlDecimal decimalData = SqlDecimal.Parse(sourceValue.ToString());
            return SqlDecimal.ConvertToPrecScale(decimalData, colTypeInfo.Precision, colTypeInfo.Scale);
        }
    }
}