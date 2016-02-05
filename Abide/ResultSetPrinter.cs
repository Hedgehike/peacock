using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abide
{
    class ResultSetPrinter
    {
        private readonly RecordParser parser;

        public ResultSetPrinter(RecordParser parser)
        {
            this.parser = parser;
        }

        public string GetFormattedResults()
        {
            StringBuilder stringBuilder = new StringBuilder();

            var results = parser.ParseData();
            foreach (string key in results.First().Keys)
            {
                stringBuilder.Append(key.PadRight(40, ' ') + " | ");
            }
            stringBuilder.Append("\n" + "".PadRight(stringBuilder.Length, '-') + "\n");
            foreach (Dictionary<string, dynamic> row in results)
            {
                foreach (KeyValuePair<string, dynamic> column in row)
                {
                    stringBuilder.Append(column.Value.ToString().PadRight(40, ' ') + " | ");
                }
                stringBuilder.Append("\n");
            }
            return stringBuilder.ToString();
        }
    }
}