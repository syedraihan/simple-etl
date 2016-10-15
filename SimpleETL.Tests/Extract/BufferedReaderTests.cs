using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleETL.Extract;

namespace SimpleETL.Tests
{
    [TestClass]
    public class BufferedReaderTests
    {
        [TestMethod]
        [TestCategory("Reader")]
        public void BufferedFileReader_Batch_Test()
        {
            var sut = new BufferedReader(GetReader());
            var result = sut.GetDataTables().ToList();

            result.Count.Should().Be(1);
            result[0].Rows.Count.Should().Be(26);
            result[0].Rows[0][0].Should().Be("1");
            result[0].Rows[25][0].Should().Be("26");

            sut = new BufferedReader(GetReader(), 0, 15);
            result = sut.GetDataTables().ToList();

            result.Count.Should().Be(2);
            result[0].Rows.Count.Should().Be(15);
            result[0].Rows[0][0].Should().Be("1");
            result[0].Rows[14][0].Should().Be("15");

            result[1].Rows.Count.Should().Be(11);
            result[1].Rows[0][0].Should().Be("16");
            result[1].Rows[10][0].Should().Be("26");
        }

        [TestMethod]
        [TestCategory("Reader")]
        public void BufferedFileReader_Skip_Test()
        {
            var sut = new BufferedReader(GetReader(), 6, 10);

            var result = sut.GetDataTables().ToList();

            result.Count.Should().Be(2);
            result[0].Rows.Count.Should().Be(10);
            result[0].Rows[0][0].Should().Be("7");
            result[0].Rows[9][0].Should().Be("16");

            result[1].Rows.Count.Should().Be(10);
            result[1].Rows[0][0].Should().Be("17");
            result[1].Rows[9][0].Should().Be("26");
        }

        private IDataReader GetReader()
        {
            var reader = new DelimitedFileReader(@"_Data\letters.csv");
            return reader.GetReader();
        }
    }
}
