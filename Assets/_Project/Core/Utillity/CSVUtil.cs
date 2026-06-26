using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ProjectATLAS.Utility
{
    public class CSVUtil
    {
        /// <summary>
        /// Converts a string array to a properly escaped CSV line.
        /// </summary>
        public static string ArrayToCsv(string[] array)
        {
            if (array == null || array.Length == 0)
                return string.Empty;
        
            return string.Join(",", array.Select(EscapeCsvValue));
        }
        
        /// <summary>
        /// Parses a CSV line into a string array, supporting quotes and commas.
        /// </summary>
        public static string[] CsvToArray(string csvLine)
        {
            if (string.IsNullOrEmpty(csvLine))
                return Array.Empty<string>();
            
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;
            
            for (int i = 0; i < csvLine.Length; i++)
            {
                char c = csvLine[i];
                
                if (c == '"')
                {
                    if (inQuotes && i + 1 < csvLine.Length && csvLine[i + 1] == '"')
                    {
                        // Escaped quote
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        // Toggle quote state
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // Field separator
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            
            result.Add(current.ToString());
            return result.ToArray();
        }
        
        private static string EscapeCsvValue(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                // Escape inner quotes by doubling them
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
