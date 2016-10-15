using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SimpleETL.Extract
{
    internal class ReaderFactory
    {
        private const string FILE_PATH_KEY = "filepath";

        public ReaderFactory() { }

        public FileReaderBase CreateReader(string connString)
        {
            const string HEADER_ROW_KEY = "headerrow";
            FileReaderBase reader = null;

            var properties = Parse(connString);
            Debug.Assert(properties.ContainsKey(FILE_PATH_KEY));

                                reader = CreateExcelFileReader(properties);
            if (reader == null) reader = CreateDelimitedFileReader(properties);

            if (properties.ContainsKey(HEADER_ROW_KEY))
                reader.HeaderRow = bool.Parse(properties[HEADER_ROW_KEY]);

            return reader;
        }

        private IDictionary<string, string> Parse(string connString)
        {
            var retVal = new Dictionary<string, string>();

            var rows = connString.ToLower().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(string row in rows)
            {
                var cols = row.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (cols.Length < 2)
                    retVal.Add(FILE_PATH_KEY, cols[0].Trim()); 
                else
                    retVal.Add(cols[0].Trim(), cols[1].Trim()); 
            }

            return retVal;
        }

        private ExcelFileReaderBase CreateExcelFileReader(IDictionary<string, string> properties)
        {
            const string TABLE_NAME_KEY = "tablename";
            ExcelFileReaderBase reader = null;

            string filePath = properties[FILE_PATH_KEY];
            string extension = Path.GetExtension(filePath);

            if (extension.Equals(".xlsx"))
                reader = new ExcelFileReader(filePath);

            else if (extension.Equals(".xls"))
                reader = new Excel97FileReader(filePath);

            else
                return null;

            if (properties.ContainsKey(TABLE_NAME_KEY))
                reader.TableName = properties[TABLE_NAME_KEY];

            return reader;
        }

        private DelimitedFileReader CreateDelimitedFileReader(IDictionary<string, string> properties)
        {
            const string COLUMN_DELIMETER_KEY = "columndelimeter";
            const string TEXT_DELIMETER_KEY = "textdelimeter";

            string filePath = properties[FILE_PATH_KEY];
            var reader = new DelimitedFileReader(filePath);

            if (properties.ContainsKey(COLUMN_DELIMETER_KEY))
                reader.ColumnDelimeter = properties[COLUMN_DELIMETER_KEY];

            if (properties.ContainsKey(TEXT_DELIMETER_KEY))
                reader.TextDelimiter = properties[TEXT_DELIMETER_KEY];

            return reader;
        }
    }
}