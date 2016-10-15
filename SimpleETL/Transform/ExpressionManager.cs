using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.ClearScript.Windows;

namespace SimpleETL.Transform.Expression
{
    internal class ExpressionManager : IDisposable
    {
        private const string EXPRESSION_TEMPLATE = "Function [{0}]\n[{0}] = {1}\nEnd Function\n";
        private const string ASSIGNMENT_TEMPLATE = "[{0}] = \"{1}\"\n";
        private const string EXTRA_FUNCTIONS = "\nFunction IIf(bClause, sTrue, sFalse)\nIf CBool(bClause) Then\nIIf = sTrue\nElse\nIIf = sFalse\nEnd If\nEnd Function";

        private VBScriptEngine _engine;
        private IDictionary<string, string> _expressions;

        public ExpressionManager() {}

        public void Initialize(IDictionary<string, string> expressions)
        {
            _expressions = expressions;
            string script = GetExpressionScript(_expressions, EXPRESSION_TEMPLATE);
            _engine = new VBScriptEngine();
            _engine.Execute(script);
            _engine.Execute(EXTRA_FUNCTIONS);
        }

        public void Initialize(IDictionary<string, string> expressions, string customFunctions)
        {
            Initialize(expressions);
            _engine.Execute(customFunctions);
        }

        public void EvaluateExpressions(DataTable dt)
        {
            Debug.Assert(_expressions != null);
            Debug.Assert(_engine != null);

            if (_expressions.Count == 0)
                return;

            int valueColCount = dt.Columns.Count;
            AddColumnsForExpression(dt, _expressions);

            foreach (DataRow row in dt.Rows)
            {
                string script = GetRowValueScript(row, dt.Columns, valueColCount, ASSIGNMENT_TEMPLATE);
                _engine.Execute(script);

                foreach (var key in _expressions.Keys)
                {
                    string expressionColName = key;
                    row[expressionColName] = _engine.Evaluate(expressionColName);
                }
            }
        }

        private void AddColumnsForExpression(DataTable dt, IDictionary<string, string> expressions)
        {
            foreach(var key in expressions.Keys)
            {
                string colName = key;
                dt.Columns.Add(colName);
            }
        }

        private string GetExpressionScript(IDictionary<string, string> expressions, string expressionTemplate)
        {
            var sb = new StringBuilder();

            foreach (var key in expressions.Keys)
            {
                string exprColName = key;
                string expression = expressions[key];

                sb.Append(string.Format(expressionTemplate, exprColName, expression));
            }

            return sb.ToString();
        }

        private string GetRowValueScript(DataRow row, DataColumnCollection columns, int valueColCount, string assignmentTemplate)
        {
            var sb = new StringBuilder();

            for (int i=0; i<valueColCount; i++)
            {
                DataColumn column = columns[i];
                sb.Append(string.Format(assignmentTemplate, column.ColumnName, Escape(row[column])));
            }

            return sb.ToString();
        }

        private string Escape(object value)
        {
            return value.ToString().Replace("\"", "\"\"");
        }

        public void Dispose()
        {
            if (_engine != null)
                _engine.Dispose();
        }
    }
}