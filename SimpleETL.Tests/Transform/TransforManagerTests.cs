using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleETL.Transform;

namespace SimpleETL
{
    [TestClass]
    public class TransforManagerTests
    {
        [TestMethod]
        [TestCategory("Transform")]
        public void Transfom_Test()
        {
            Test(new Dictionary<string, string>() {
                { "d1", "S4" },
                { "d2", "S3" },
                { "d3", "S1" }
            }, "D1");

            Test(new Dictionary<string, string>() {
                { "D1", "s4" },
                { "D2", "s3" },
                { "D3", "s1" }
            }, string.Empty);
        }

        private void Test(Dictionary<string, string> colMapping, string idColName)
        {
            var sut = new TransformManager();
            sut.Initialize(colMapping, idColName);

            var dtSource = GetTestDataTable();
            var dt = sut.Transform(dtSource);

            dt.Rows.Count.Should().Be(2);
            dt.Columns.Count.Should().Be(3);

            dt.Rows[0]["D1"].Should().Be("Row1-S4");
            dt.Rows[0]["D2"].Should().Be("Row1-S3");
            dt.Rows[0]["D3"].Should().Be("Row1-S1");

            dt.Rows[1]["D1"].Should().Be("Row2-S4");
            dt.Rows[1]["D2"].Should().Be("Row2-S3");
            dt.Rows[1]["D3"].Should().Be("Row2-S1");
        }

        private DataTable GetTestDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add("S1");
            dt.Columns.Add("S2");
            dt.Columns.Add("S3");
            dt.Columns.Add("S4");

            var row = dt.NewRow();
            row["S1"] = "Row1-S1";
            row["S2"] = "Row1-S2";
            row["S3"] = "Row1-S3";
            row["S4"] = "Row1-S4";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["S1"] = "Row2-S1";
            row["S2"] = "Row2-S2";
            row["S3"] = "Row2-S3";
            row["S4"] = "Row2-S4";
            dt.Rows.Add(row);

            return dt;
        }
    }
}
