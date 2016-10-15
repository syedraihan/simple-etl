using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SimpleETL.Transform.DbSchema
{
    internal class DbSchemaHelper
    {
        public IDictionary<string, ColumnTypeInfo> GetSchema(string connString, string tableName)
        {
            var retVal = new Dictionary<string, ColumnTypeInfo>();

            var dt = GetSchemaTable(connString, tableName);
            foreach (DataRow row in dt.Rows)
            {
                string colName = (string)row["ColumnName"];

                retVal.Add(colName, new ColumnTypeInfo()
                {
                    ColumnName = colName,
                    SqlDataType = (string)row["DataTypeName"],
                    ColumnSize = (int)row["ColumnSize"],
                    AllowNull = (bool)row["AllowDBNull"],
                    Precision = (short)row["NumericPrecision"],
                    Scale = (short)row["NumericScale"]
                });
            }

            return retVal;
        }

        private DataTable GetSchemaTable(string connString, string tableName)
        {
            var query = string.Format("SELECT * FROM {0} where 1=0", tableName);

            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        return reader.GetSchemaTable();
                    }
                }
            }
        }
    }
}