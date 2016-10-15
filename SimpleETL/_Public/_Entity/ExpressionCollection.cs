using System.Collections.Generic;

namespace SimpleETL
{
    public class ExpressionCollection : Dictionary<string, string>
    {
        public string CustomFunctions { get; set; } = string.Empty;
    }
}