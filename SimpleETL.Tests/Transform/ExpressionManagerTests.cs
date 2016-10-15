using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using SimpleETL.Transform.Expression;

namespace SimpleETL.Tests
{
    [TestClass]
    public class ExpressionManagerTests
    {

        [TestMethod]
        [TestCategory("Expression")]
        public void Expression_Test()
        {
            var sut = new ExpressionManager();
            sut.Initialize(new Dictionary<string, string>()
            {
                {  "expr1", "\"Hello\" + F1 + \"World\"" },
                {  "expr2", "UCase(Left(F1, 3)) + LCase(Mid(F1, 4))" },
                {  "expr3", "Sqr(F2)+Log(Exp(F3))" },
                {  "expr4", "Month(F4)+Day(F4)+Year(F4)" }
            });

            var dt = GetTestDataTable();
            sut.EvaluateExpressions(dt);

            dt.Columns.Count.Should().Be(4+4);
            dt.Rows[0]["expr1"].Should().Be("HelloabcDEFGHWorld");
            dt.Rows[0]["expr2"].Should().Be("ABCdefgh");
            dt.Rows[0]["expr3"].Should().Be("14");
            dt.Rows[0]["expr4"].Should().Be("2021");
        }

        [TestMethod]
        [TestCategory("Expression")]
        public void Expression_CustomFunction_Test()
        {
            var sut = new ExpressionManager();
            sut.Initialize(new Dictionary<string, string>()
            {
                {  "expr1", "CustomFunc1(F4)" }
            }, @"
Function CustomFunc1(dt)
    CustomFunc1 = Month(dt) + Day(dt) + Year(dt)
End Function
");

            var dt = GetTestDataTable();
            sut.EvaluateExpressions(dt);

            dt.Columns.Count.Should().Be(4 + 1);
            dt.Rows[0]["expr1"].Should().Be("2021");
        }

        [TestMethod]
        [TestCategory("Expression")]
        public void Expression_Test_Corner_Cases()
        {
            var sut = new ExpressionManager();
            sut.Initialize(new Dictionary<string, string>() { });


            var dt = GetTestDataTable();
            sut.EvaluateExpressions(dt);

            dt.Columns.Count.Should().Be(4);

            var sut2 = new ExpressionManager();
            Action action1 = () => { sut.Dispose(); };  // Initialized
            Action action2 = () => { sut2.Dispose(); }; // Not Initialized

            action1.ShouldNotThrow();
            action2.ShouldNotThrow();
        }

        private DataTable GetTestDataTable()
        {
            var dt = new DataTable();

            dt.Columns.Add(new DataColumn("F1", typeof(string)));
            dt.Columns.Add(new DataColumn("F2", typeof(int)));
            dt.Columns.Add(new DataColumn("F3", typeof(int)));
            dt.Columns.Add(new DataColumn("F4", typeof(DateTime)));

            var row = dt.NewRow();
            row["F1"] = "abcDEFGH";
            row["F2"] = 16;
            row["F3"] = 10;
            row["F4"] = "1/4/2016";
            dt.Rows.Add(row);

            return dt;
        }
    }
}
