using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleETL.Extract;

namespace SimpleETL.Tests
{
    [TestClass]
    public class DelimitedFileReaderTests
    {
        [TestMethod]
        [TestCategory("Reader")]
        public void Delimited_File_Read_Test()
        {
            Test(new DelimitedFileReader(@"_Data\comma.csv"));
            Test(new DelimitedFileReader(@"_Data\comma.csv") { ColumnDelimeter = "," });
            Test(new DelimitedFileReader(@"_Data\tab.txt") { ColumnDelimeter = "tab", TextDelimiter = "\"" });
            Test(new DelimitedFileReader(@"_Data\space.txt") { ColumnDelimeter = "space", TextDelimiter = "'" });
            Test(new DelimitedFileReader(@"_Data\delimited.txt") { ColumnDelimeter = "|" });

            Test_No_Header(new DelimitedFileReader(@"_Data\noheader.csv") { HeaderRow = false });
        }

        private void Test(FileReaderBase sut)
        {
            IDataReader rdr = sut.GetReader();

            CheckColumnNames(rdr);
            CheckData(rdr);
        }

        private void CheckColumnNames(IDataReader rdr)
        {
            rdr.FieldCount.Should().Be(5);

            rdr.GetName(0).Should().Be("int");
            rdr.GetName(1).Should().Be("text");
            rdr.GetName(2).Should().Be("date");
            rdr.GetName(3).Should().Be("double");
            rdr.GetName(4).Should().Be("bool");
        }

        private void CheckData(IDataReader rdr)
        {
            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be("1");
            rdr.GetValue(1).Should().Be("text data1");
            rdr.GetValue(2).Should().Be("1/1/1960");
            rdr.GetValue(3).Should().Be("12.3");
            rdr.GetValue(4).Should().Be("true");

            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be("2");
            rdr.GetValue(1).Should().Be("text data2");
            rdr.GetValue(2).Should().Be("2/3/1970");
            rdr.GetValue(3).Should().Be("4.56");
            rdr.GetValue(4).Should().Be("false");

            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be("3");
            rdr.GetValue(1).Should().Be("text data3");
            rdr.GetValue(2).Should().Be("4/5/2016");
            rdr.GetValue(3).Should().Be("7.89");
            rdr.GetValue(4).Should().Be("true");

            rdr.Read().Should().BeFalse();
        }

        private void Test_No_Header(FileReaderBase sut)
        {
            IDataReader rdr = sut.GetReader();

            rdr.FieldCount.Should().Be(5);
            rdr.GetName(0).Should().Be("F1");
            rdr.GetName(1).Should().Be("F2");
            rdr.GetName(2).Should().Be("F3");
            rdr.GetName(3).Should().Be("F4");
            rdr.GetName(4).Should().Be("F5");

            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be(1);
            rdr.GetValue(1).Should().Be("text data1");
            rdr.GetValue(2).Should().Be(new DateTime(1960, 1, 1));
            rdr.GetValue(3).Should().Be(12.3);
            rdr.GetValue(4).Should().Be("true");

            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be(2);
            rdr.GetValue(1).Should().Be("text data2");
            rdr.GetValue(2).Should().Be(new DateTime(1970, 2, 3));
            rdr.GetValue(3).Should().Be(4.56);
            rdr.GetValue(4).Should().Be("false");

            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be(3);
            rdr.GetValue(1).Should().Be("text data3");
            rdr.GetValue(2).Should().Be(new DateTime(2016, 4, 5));
            rdr.GetValue(3).Should().Be(7.89);
            rdr.GetValue(4).Should().Be("true");

            rdr.Read().Should().BeFalse();
        }
    }
}
