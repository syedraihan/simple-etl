using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleETL.Extract;

namespace SimpleETL.Tests
{
    [TestClass]
    public class ReaderFactoryTests
    {
        [TestMethod]
        [TestCategory("Reader")]
        public void Reader_Factory_Tests()
        {
            var sut = new ReaderFactory();

            // File Path
            sut.CreateReader("foo.bar").FilePath.Should().Be("foo.bar");
            sut.CreateReader("FilePath=foo.bar").FilePath.Should().Be("foo.bar");
            sut.CreateReader("FILEPATH=foo.bar").FilePath.Should().Be("foo.bar");

            // File Type
            sut.CreateReader("foo.xls").GetType().Name.Should().Be("Excel97FileReader");
            sut.CreateReader("foo.xlsx").GetType().Name.Should().Be("ExcelFileReader");
            sut.CreateReader("foo.bar").GetType().Name.Should().Be("DelimitedFileReader");

            // Header Row
            sut.CreateReader("foo.bar").HeaderRow.Should().Be(true);
            sut.CreateReader("foo.bar;headerrow=false").HeaderRow.Should().Be(false);

            // Col delim
            ((DelimitedFileReader)sut.CreateReader("foo.bar")).ColumnDelimeter.Should().BeNull();
            ((DelimitedFileReader)sut.CreateReader("foo.bar;columndelimeter=,")).ColumnDelimeter.Should().Be(",");
            ((DelimitedFileReader)sut.CreateReader("foo.bar;columndelimeter=tab")).ColumnDelimeter.Should().Be("tab");

            // text delim
            ((DelimitedFileReader)sut.CreateReader("foo.bar")).TextDelimiter.Should().BeNull();
            ((DelimitedFileReader)sut.CreateReader("foo.bar;textdelimeter=\"")).TextDelimiter.Should().Be("\"");

            // sheet name
            ((ExcelFileReaderBase)sut.CreateReader("foo.xls")).TableName.Should().Be("Sheet1");
            ((ExcelFileReaderBase)sut.CreateReader("foo.xlsx")).TableName.Should().Be("Sheet1");
            ((ExcelFileReaderBase)sut.CreateReader("foo.xls;tablename=bar")).TableName.Should().Be("bar");
            ((ExcelFileReaderBase)sut.CreateReader("foo.xlsx;tablename=bar")).TableName.Should().Be("bar");

            // other assumptions
            sut.CreateReader(";;foo.bar;;").FilePath.Should().Be("foo.bar");
            sut.CreateReader("FilePath=foo.bar=;").FilePath.Should().Be("foo.bar");
            sut.CreateReader("FilePath=foo.bar=xx;").FilePath.Should().Be("foo.bar");
        }
    }
}
