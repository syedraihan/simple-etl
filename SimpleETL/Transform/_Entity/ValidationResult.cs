using System;

namespace SimpleETL.Transform
{
    internal class ValidationResult
    {
        public bool IsValid { get; set; }

        public object ReplacedValue { get; set; }
    }
}