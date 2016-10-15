using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleETL.Extract;

namespace SimpleETL.Tests
{
    [TestClass]
    public class ExcelFileReaderTests
    {
        [TestMethod]
        [TestCategory("Reader")]
        public void Excell_File_Read_Test()
        {
            Test(new ExcelFileReader(@"_Data\excell.xlsx"));
            Test(new Excel97FileReader(@"_Data\excell.xls"));
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
            rdr.GetValue(0).Should().Be(1d);
            rdr.GetValue(1).Should().Be("text data1");
            rdr.GetValue(2).Should().Be(new DateTime(1960, 1, 1));
            rdr.GetValue(3).Should().Be(12.3);
            rdr.GetValue(4).Should().Be(true);

            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be(2d);
            rdr.GetValue(1).Should().Be("text data2");
            rdr.GetValue(2).Should().Be(new DateTime(1970, 2, 3));
            rdr.GetValue(3).Should().Be(4.56);
            rdr.GetValue(4).Should().Be(false);

            rdr.Read().Should().BeTrue();
            rdr.GetValue(0).Should().Be(3d);
            rdr.GetValue(1).Should().Be("text data3");
            rdr.GetValue(2).Should().Be(new DateTime(2016, 4, 5));
            rdr.GetValue(3).Should().Be(7.89);
            rdr.GetValue(4).Should().Be(true);

            rdr.Read().Should().BeFalse();
        }
    }
}
