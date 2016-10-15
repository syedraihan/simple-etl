using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlTypes;
using FluentAssertions;
using SimpleETL.Transform;
using SimpleETL.Transform.DbSchema;

namespace SimpleETL.Tests
{
    [TestClass]
    public class ValidatorTests
    {
        [TestMethod]
        [TestCategory("Validator")]
        public void String_Validator_Test()
        {
            var sut = new StringValidator();

            var col1 = new ColumnTypeInfo() { AllowNull = true, ColumnSize = 10 };
            var col2 = new ColumnTypeInfo() { AllowNull = false, ColumnSize = 10 };

            Assert_Valid(sut.Validate(col1, "0123456789"));
            Assert_Valid(sut.Validate(col1, "This is it"));
            Assert_Valid(sut.Validate(col1, DBNull.Value));
            Assert_Valid(sut.Validate(col1, null));

            Assert_Invalid_Replaced(sut.Validate(col2, "0123456789....."), "0123456789");
            Assert_Invalid_Replaced(sut.Validate(col2, DBNull.Value), string.Empty);

            sut.EmptyValue.Should().Be(string.Empty);
        }

        [TestMethod]
        [TestCategory("Validator")]
        public void Integer_Validator_Test()
        {
            var sut = new IntegerValidator();

            var col1 = new ColumnTypeInfo() { AllowNull = true };
            var col2 = new ColumnTypeInfo() { AllowNull = false };

            Assert_Valid(sut.Validate(col1, "10"));
            Assert_Valid(sut.Validate(col1, "-10"));
            Assert_Valid(sut.Validate(col1, DBNull.Value));
            Assert_Valid(sut.Validate(col1, null));

            Assert_Invalid(sut.Validate(col1, "Foo"));
            Assert_Invalid(sut.Validate(col1, "10.1"));

            Assert_Invalid_Replaced(sut.Validate(col2, "Foo"), 0);
            Assert_Invalid_Replaced(sut.Validate(col2, "10.1"), 0);
            Assert_Invalid_Replaced(sut.Validate(col2, DBNull.Value), 0);
        }

        [TestMethod]
        [TestCategory("Validator")]
        public void Boolean_Validator_Test()
        {
            var sut = new BooleanValidator();

            var col1 = new ColumnTypeInfo() { AllowNull = true };
            var col2 = new ColumnTypeInfo() { AllowNull = false };

            Assert_Valid(sut.Validate(col1, "true"));
            Assert_Valid(sut.Validate(col1, "TRUE"));
            Assert_Valid(sut.Validate(col1, "True"));
            Assert_Valid(sut.Validate(col1, "false"));
            Assert_Valid(sut.Validate(col1, "FALSE"));
            Assert_Valid(sut.Validate(col1, "False"));

            Assert_Invalid(sut.Validate(col1, "Foo"));

            Assert_Invalid_Replaced(sut.Validate(col2, "Foo"), false);
            Assert_Invalid_Replaced(sut.Validate(col2, DBNull.Value), false);
        }

        [TestMethod]
        [TestCategory("Validator")]
        public void Decimal_Validator_Test()
        {
            var sut = new DecimalValidator();

            var col1 = new ColumnTypeInfo() { AllowNull = true, Precision = 3, Scale = 1 };
            var col2 = new ColumnTypeInfo() { AllowNull = false, Precision = 3, Scale = 1 };

            Assert_Valid(sut.Validate(new ColumnTypeInfo() { Precision = 5, Scale = 2 }, 123.45));
            Assert_Valid(sut.Validate(new ColumnTypeInfo() { Precision = 9, Scale = 4 }, 12345.6789));

            Assert_Invalid(sut.Validate(col1, "Foo"));
            Assert_Invalid(sut.Validate(col1, 12345.6789));

            Assert_Invalid_Replaced(sut.Validate(col2, "Foo"), 0d);
            Assert_Invalid_Replaced(sut.Validate(col2, 12345.6789), 0d);
            Assert_Invalid_Replaced(sut.Validate(col2, DBNull.Value), 0d);
        }

        [TestMethod]
        [TestCategory("Validator")]
        public void DateTime_Validator_Test()
        {
            var sut = new DateTimeValidator();

            var col1 = new ColumnTypeInfo() { AllowNull = true };
            var col2 = new ColumnTypeInfo() { AllowNull = false };

            Assert_Valid(sut.Validate(col1, "1/1/2016"));
            Assert_Valid(sut.Validate(col1, new DateTime(2016, 1, 1)));

            Assert_Invalid(sut.Validate(col1, "Foo"));
            Assert_Invalid(sut.Validate(col1, "2/29/2015"));

            Assert_Invalid_Replaced(sut.Validate(col2, "Foo"), SqlDateTime.MinValue);
            Assert_Invalid_Replaced(sut.Validate(col2, "2/29/2015"), SqlDateTime.MinValue);
            Assert_Invalid_Replaced(sut.Validate(col2, DBNull.Value), SqlDateTime.MinValue);
        }

        private void Assert_Valid(ValidationResult result)
        {
            result.IsValid.Should().BeTrue();
            result.ReplacedValue.Should().BeNull();
        }

        private void Assert_Invalid(ValidationResult result)
        {
            Assert_Invalid_Replaced(result, DBNull.Value);
        }

        private void Assert_Invalid_Replaced(ValidationResult result, object replacedValue)
        {
            result.IsValid.Should().BeFalse();
            result.ReplacedValue.Should().Be(replacedValue);
        }
    }
}
