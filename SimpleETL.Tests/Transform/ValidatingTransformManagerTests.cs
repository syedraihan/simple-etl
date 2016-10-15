using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleETL.Transform;
using SimpleETL.Transform.DbSchema;
using System.Data.SqlTypes;

namespace SimpleETL.Tests
{
    [TestClass]
    public class ValidatingTransformManagerTests
    {
        private ColumnTypeInfo col10 = new ColumnTypeInfo() { ColumnName = "c10", SqlDataType = "char", ColumnSize = 10 };
        private ColumnTypeInfo col21 = new ColumnTypeInfo() { ColumnName = "c21", SqlDataType = "int", AllowNull = true };
        private ColumnTypeInfo col31 = new ColumnTypeInfo() { ColumnName = "c31", SqlDataType = "decimal", AllowNull = true, Precision = 5, Scale = 2 };
        private ColumnTypeInfo col41 = new ColumnTypeInfo() { ColumnName = "c41", SqlDataType = "bit", AllowNull = true };
        private ColumnTypeInfo col51 = new ColumnTypeInfo() { ColumnName = "c51", SqlDataType = "datetime", AllowNull = true };

        private ColumnTypeInfo col22 = new ColumnTypeInfo() { ColumnName = "c22", SqlDataType = "int", AllowNull = false };
        private ColumnTypeInfo col32 = new ColumnTypeInfo() { ColumnName = "c32", SqlDataType = "decimal", AllowNull = false, Precision = 5, Scale = 2 };
        private ColumnTypeInfo col42 = new ColumnTypeInfo() { ColumnName = "c42", SqlDataType = "bit", AllowNull = false };
        private ColumnTypeInfo col52 = new ColumnTypeInfo() { ColumnName = "c52", SqlDataType = "datetime", AllowNull = false };

        [TestMethod]
        [TestCategory("Transform")]
        public void ValidatingTransform_String_Test()
        {
            Assert_NoError(col10, "0123456789");
            Assert_Validation_Error(col10, "0123456789...", "0123456789");
        }

        [TestMethod]
        [TestCategory("Transform")]
        public void ValidatingTransform_NoError_Test()
        {
            Assert_NoError(col21, "100");       // good init
            Assert_NoError(col31, "123.45");    // good decimal
            Assert_NoError(col41, "true");
            Assert_NoError(col41, "True");
            Assert_NoError(col41, "FALSE");
            Assert_NoError(col41, "0");         // Zero is False
            Assert_NoError(col41, "10");        // Non-zero is True
            Assert_NoError(col51, "1/1/2016");  // good date
        }

        [TestMethod]
        [TestCategory("Transform")]
        public void ValidatingTransform_Error_AllowNull_Test()
        {
            Assert_Validation_Error(col21, "Foo");                  // int
            Assert_Validation_Error(col31, "Foo");                  // decimal
            Assert_Validation_Error(col31, "123456789.1234567");    // decimal
            Assert_Validation_Error(col41, "Foo");                  // bool
            Assert_Validation_Error(col51, "Foo");                  // datetime
            Assert_Validation_Error(col51, "2/30/2016");            // datetime
        }

        [TestMethod]
        [TestCategory("Transform")]
        public void ValidatingTransform_Error_NotNull_Test()
        {
            Assert_Validation_Error(col22, "Foo", "0");
            Assert_Validation_Error(col32, "Foo", "0");
            Assert_Validation_Error(col32, "123456789.1234567", "0");
            Assert_Validation_Error(col42, "Foo", "False");
            Assert_Validation_Error(col52, "Foo", SqlDateTime.MinValue.ToString());
            Assert_Validation_Error(col52, "2/30/2016", SqlDateTime.MinValue.ToString()); 
        }

        private void Assert_NoError(ColumnTypeInfo col, object value)
        {
            var sut = GetSUT(col);
            var dt = GetTestDataTable(value);

            sut.MonitorEvents();
            var result = sut.Transform(dt);
            sut.ShouldNotRaise("DataValidationError");

            result.Rows.Count.Should().Be(1);
            result.Columns.Count.Should().Be(1);
            result.Rows[0]["D1"].Should().Be(value);
        }

        private void Assert_Validation_Error(ColumnTypeInfo col, object value)
        {
            var sut = GetSUT(col);
            var dt = GetTestDataTable(value);

            sut.MonitorEvents();
            var result = sut.Transform(dt);
            sut.ShouldRaise("DataValidationError")
               .WithSender(sut)
               .WithArgs<DataValidationErrorEventArgs>(e => e.RowId.Equals(value))
               .WithArgs<DataValidationErrorEventArgs>(e => e.ColumnName.Equals("D1"))
               .WithArgs<DataValidationErrorEventArgs>(e => e.OriginalValue.Equals(value))
               .WithArgs<DataValidationErrorEventArgs>(e => e.ReplacedValue.Equals(DBNull.Value));

            result.Rows.Count.Should().Be(1);
            result.Columns.Count.Should().Be(1);
            result.Rows[0]["D1"].Should().Be(DBNull.Value);
        }

        private void Assert_Validation_Error(ColumnTypeInfo col, object value, object replacedValue)
        {
            var sut = GetSUT(col);
            var dt = GetTestDataTable(value);

            sut.MonitorEvents();
            var result = sut.Transform(dt);
            sut.ShouldRaise("DataValidationError")
               .WithSender(sut)
               .WithArgs<DataValidationErrorEventArgs>(e => e.RowId.Equals(value))
               .WithArgs<DataValidationErrorEventArgs>(e => e.ColumnName.Equals("D1"))
               .WithArgs<DataValidationErrorEventArgs>(e => e.OriginalValue.Equals(value));

            result.Rows.Count.Should().Be(1);
            result.Columns.Count.Should().Be(1);
            result.Rows[0]["D1"].Should().Be(replacedValue);
        }

        private ValidatingTransformManager GetSUT(ColumnTypeInfo col)
        {
            var sut = new ValidatingTransformManager();

            var columnMappings = new Dictionary<string, string>() { { "D1", "S1" } };
            var schema = new Dictionary<string, ColumnTypeInfo>() { { "D1", col } };
            sut.Initialize(columnMappings, "D1", schema);

            return sut;
        }

        private DataTable GetTestDataTable(object value)
        {
            var dt = new DataTable();
            dt.Columns.Add("S1");

            var row = dt.NewRow();
            row["S1"] = value;
            dt.Rows.Add(row);

            return dt;
        }
    }
}
