using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.IO.Csv;

namespace GlueCsvEditor.Data
{
    public class CsvData
    {
        protected string _csvPath;
        protected char _delimiter;
        protected RuntimeCsvRepresentation _csv;

        public CsvData(string csvPath, char delimiter = ',')
        {
            _csvPath = csvPath;
            _delimiter = delimiter;
            Reload();
        }

        /// <summary>
        /// Adds a new row at the specified index
        /// </summary>
        /// <returns></returns>
        public void AddRow(int index)
        {
            _csv.Records.Insert(index, new string[_csv.Headers.Length]);
        }

        /// <summary>
        /// Removes the specified data row
        /// </summary>
        /// <param name="index"></param>
        public void RemoveRow(int index)
        {
            if (index >= _csv.Records.Count)
                throw new ArgumentOutOfRangeException("index");

            _csv.Records.RemoveAt(index);
        }

        /// <summary>
        /// Adds a column to the CSV
        /// </summary>
        /// <param name="index"></param>
        public void AddColumn(int index)
        {
            // Add this column to the RCR
            var headers = new List<CsvHeader>(_csv.Headers);
            headers.Insert(index, new CsvHeader { Name = string.Empty, OriginalText = string.Empty });
            _csv.Headers = headers.ToArray();

            // Add the column to all the records
            for (int x = 0; x < _csv.Records.Count; x++)
            {
                var values = new List<string>(_csv.Records[x]);
                values.Insert(index, string.Empty);
                _csv.Records[x] = values.ToArray();
            }
        }

        /// <summary>
        /// Removes the specified column from the csv data
        /// </summary>
        /// <param name="index"></param>
        public void RemoveColumn(int index)
        {
            // Remove this column to the RCR
            var headers = new List<CsvHeader>(_csv.Headers);
            headers.RemoveAt(index);
            _csv.Headers = headers.ToArray();

            // Remove the column to all the records
            for (int x = 0; x < _csv.Records.Count; x++)
            {
                var values = new List<string>(_csv.Records[x]);
                values.RemoveAt(index);
                _csv.Records[x] = values.ToArray();
            }
        }

        /// <summary>
        /// Returns the number of records in the csv
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount()
        {
            return _csv.Records.Count;
        }

        /// <summary>
        /// Gets the value in the specified cell
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetValue(int row, int column)
        {
            if (row >= _csv.Records.Count)
                throw new ArgumentOutOfRangeException("row");

            if (column >= _csv.Records[row].Length)
                throw new ArgumentOutOfRangeException("column");

            return _csv.Records[row][column];
        }

        /// <summary>
        /// Updates the value in the specified row and column
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void UpdateValue(int row, int column, string value)
        {
            if (row >= _csv.Records.Count)
                throw new ArgumentOutOfRangeException("row");

            if (column >= _csv.Records[row].Length)
                throw new ArgumentOutOfRangeException("column");

            _csv.Records[row][column] = value;
        }

        /// <summary>
        /// Retrieves a list of headers for the CSV
        /// </summary>
        /// <returns></returns>
        public List<string> GetHeaders()
        {
            return _csv.Headers
                       .Select(x => x.OriginalText)
                       .ToList();
        }

        /// <summary>
        /// Retrieves information about the specific column
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public CsvColumnHeader GetHeader(int column)
        {
            if (column >= _csv.Headers.Length)
                throw new ArgumentOutOfRangeException("column");

            bool isList;
            var header = _csv.Headers[column];
            string type = CsvHeader.GetClassNameFromHeader(header.OriginalText) ?? "string";

            int typeDataIndex = header.Name.IndexOf("(");
            if (typeDataIndex < 0)
                typeDataIndex = header.Name.Length;

            // Strip out the List< and > values
            if (type.Contains("List<"))
            {
                isList = true;
                type = type.Replace("List<", "");
                if (type.Contains(">"))
                    type = type.Remove(type.LastIndexOf(">"), 1);
            }
            else
            {
                isList = false;
            }

            return new CsvColumnHeader
            {
                Name = header.Name.Substring(0, typeDataIndex),
                Type = type,
                IsRequired = header.IsRequired,
                IsList = isList
            };
        }

        /// <summary>
        /// Sets the specified column header with specific values
        /// </summary>
        /// <param name="column"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="isRequired"></param>
        /// <param name="isList"></param>
        /// <returns>Returns the new display string for the header</returns>
        public string SetHeader(int column, string name, string type, bool isRequired, bool isList)
        {
            if (column >= _csv.Headers.Length)
                throw new ArgumentOutOfRangeException("column");

            // Form the new text value
            var text = new StringBuilder();
            text.Append(name.Trim());
            text.Append(" (");

            if (isList)
                text.Append("List<");

            text.Append(type.Trim());

            if (isList)
                text.Append(">");

            if (isRequired)
                text.Append(", required");

            text.Append(")");

            // Update the header details
            var header = _csv.Headers[column];
            header.OriginalText = text.ToString();
            header.Name = text.ToString();
            header.IsRequired = isRequired;

            _csv.Headers[column] = header;
            return text.ToString();
        }

        /// <summary>
        /// Reloads the CSV data from disk
        /// </summary>
        public void Reload()
        {
            CsvFileManager.Delimiter = _delimiter;
            _csv = CsvFileManager.CsvDeserializeToRuntime(_csvPath);
        }

        /// <summary>
        /// Saves all csv data
        /// </summary>
        public void SaveCsv()
        {
            _csv.RemoveHeaderWhitespaceAndDetermineIfRequired();
            CsvFileManager.Delimiter = _delimiter;
            CsvFileManager.Serialize(_csv, _csvPath);
        }
    }
}
