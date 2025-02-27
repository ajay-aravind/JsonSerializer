using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonSerializer
{
    /// <summary>
    /// used to build the result string
    /// </summary>
    internal class JsonWriter
    {
        private StringBuilder builder = new StringBuilder();

        public string ResultJson {
            get
            {
                return builder.ToString();
            }
        }

        internal void WriteNull()
        {
            builder.Append("null");
        }

        internal void WriteStringValue(string str)
        {
            builder.Append("\"");
            builder.Append(str);
            builder.Append("\"");
        }

        internal void WriteKey(string str)
        {
            builder.Append("\"");
            builder.Append(str);
            builder.Append("\" : ");
        }

        internal void WriteStringAsIs(string str)
        {
            builder.Append(str);
        }

    }
}
