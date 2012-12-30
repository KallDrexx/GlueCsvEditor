using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.IO.Csv;
using GlueCsvEditor.KnownValues;

namespace GlueCsvEditor.Data
{
    public class CsvData
    {
        protected string _csvPath;
        protected char _delimiter;
        protected CachedTypes _cachedTypes;
        protected RuntimeCsvRepresentation _csv;

        public CsvData(string csvPath, CachedTypes cachedTypes, char delimiter = ',')
        {
            _csvPath = csvPath;
            _delimiter = delimiter;
            _cachedTypes = cachedTypes;
            Reload();
        }

        /// <summary>
        /// Adds a new row at the specified index
        /// </summary>
        /// <returns></returns>
        public void AddRow(int index)
        {
            // If index is less than 0, set it to be zero as the minimum
            if (index < 0)
                index = 0;

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
            string headerName = "NewColumn" + index;

            // Add this column to the RCR
            var headers = new List<CsvHeader>(_csv.Headers);
            headers.Insert(index, new CsvHeader { Name = headerName, OriginalText = headerName + " (string)" });
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
        public List<string> GetHeaderText()
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
        public CsvColumnHeader GetHeaderDetails(int column)
        {
            if (column >= _csv.Headers.Length)
                throw new ArgumentOutOfRangeException("column");

            bool isList;
            var header = _csv.Headers[column];
            string type = CsvHeader.GetClassNameFromHeader(header.OriginalText) ?? "string";

            int typeDataIndex = header.Name.IndexOf("(", StringComparison.Ordinal);
            if (typeDataIndex < 0)
                typeDataIndex = header.Name.Length;

            // Strip out the List< and > values
            if (type.Contains("List<"))
            {
                isList = true;
                type = type.Replace("List<", "");
                if (type.Contains(">"))
                    type = type.Remove(type.LastIndexOf(">", StringComparison.Ordinal), 1);
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
            _csv.RemoveHeaderWhitespaceAndDetermineIfRequired();
            return text.ToString();
        }

        /// <summary>
        /// Searches the CSV for the next cell containing a string, 
        /// starting from the specified row and column
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="ignoreStartingCell"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        public FoundCell FindNextValue(string searchString, int startRow, int startColumn, bool ignoreStartingCell = false, bool reverse = false)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return null;            

            int row = startRow;
            int column = startColumn;
            searchString = searchString.Trim();
            bool isFirstSearchedCell = true;

            // Traverse through the records of the RCR until we find the next match
            do
            {
                if ((ignoreStartingCell && !isFirstSearchedCell) || !ignoreStartingCell)
                    if (_csv.Records[row][column].IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                        return new FoundCell { ColumnIndex = column, RowIndex = row };

                // This cell doesn't have the record, go to the next
                if (!reverse)
                {
                    column++;
                    if (column >= _csv.Headers.Length)
                    {
                        column = 0;
                        row++;

                        if (row >= _csv.Records.Count)
                            row = 0;
                    }
                }

                else
                {
                    column--;
                    if (column < 0)
                    {
                        column = _csv.Headers.Length - 1;
                        row--;

                        if (row < 0)
                            row = _csv.Records.Count - 1;
                    }
                }

                isFirstSearchedCell = false;

            } while (row != startRow || column != startColumn);

            return null;
        }

        /// <summary>
        /// Reloads the CSV data from disk
        /// </summary>
        public void Reload()
        {
            CsvFileManager.Delimiter = _delimiter;
            _csv = CsvFileManager.CsvDeserializeToRuntime(_csvPath);
            _csv.RemoveHeaderWhitespaceAndDetermineIfRequired();
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

        /// <summary>
        /// Retrieves any known values for the specified cell
        /// </summary>
        /// <param name="column"></param>
        public IEnumerable<string> GetKnownValues(int column)
        {
            string type = CsvHeader.GetClassNameFromHeader(_csv.Headers[column].OriginalText);
            type = type ?? string.Empty;

            // Remove the List<> if exists
            type = type.Replace("List<", "").Replace(">", "");

            var foundTypes = GetKnownValuesForType(type);
            var knownValues = foundTypes as string[] ?? foundTypes.ToArray();
            if (!knownValues.Any())
                return new UsedRcrColumnValueRetriever(_csv, column).GetKnownValues(type);

            return knownValues;
        }

        public IEnumerable<string> GetKnownValuesForType(string type)
        {
            if (!_cachedTypes.IsCacheReady)
                return new string[0];

            if (type != null)
            {
                // Remove the List<> if exists
                type = type.Replace("List<", "").Replace(">", "");

                // This list is prioritized.  The first retriever to get a value is the only one used
                var knownValueRetrievers = new List<IKnownValueRetriever>
                    {
                    new EnumReflectionValueRetriever(),
                    new FrbStateValueRetriever(),
                    new ParsedEnumValueRetriever(_cachedTypes.ProjectEnums),
                    new InterfaceImplementationsValueRetriever(_cachedTypes.ProjectClasses)
                };

                // Loop through the value retrievers until one returns a valid results
                foreach (var retriever in knownValueRetrievers)
                {
                    var values = retriever.GetKnownValues(type);
                    var knownValuesForType = values as string[] ?? values.ToArray();
                    if (knownValuesForType.Any())
                        return knownValuesForType;
                }
            }

            // No values were found
            return new string[0];
        }

        public IEnumerable<ComplexTypeProperty> GetKnownProperties(int columnIndex)
        {
            if (!_cachedTypes.IsCacheReady)
                return new ComplexTypeProperty[0];

            string type = CsvHeader.GetClassNameFromHeader(_csv.Headers[columnIndex].OriginalText);
            if (!string.IsNullOrWhiteSpace(type))
            {
                // Remove the List<> if exists
                type = type.Replace("List<", "").Replace(">", "");

                // Check if the type matches a ParsedClass
                var parsedClass = _cachedTypes.ProjectClasses
                                              .FirstOrDefault(x => string.Concat(x.Namespace, ".", x.Name).Equals(type, StringComparison.OrdinalIgnoreCase));

                if (parsedClass != null)
                {
                    return parsedClass.ParsedProperties
                                      .Select(x => new ComplexTypeProperty
                                      {
                                          Name = x.Name,
                                          Type = x.Type.Name
                                      })
                                      .ToArray();
                }
            }

            return new ComplexTypeProperty[0];
        }
    }
}
