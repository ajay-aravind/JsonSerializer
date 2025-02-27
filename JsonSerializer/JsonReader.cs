using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonSerializer
{
    internal class JsonReader
    {
        private string inputJson;
        private StringReader stringReader;

        public JsonReader(string inputJson)
        {
            this.inputJson = inputJson;
            this.stringReader = new StringReader(inputJson);
        }

        //public Token GetNextToken()
        //{
        //    int nextChar = this.stringReader.Read();

        //    switch ((char)nextChar)
        //    {
        //        case '{': return Token.CurlyBraceStart;
        //        case '}': return Token.CurlyBraceEnd;
        //        case '[': return Token.SquareBraceStart;
        //        case ']': return Token.SquareBraceEnd;
        //        case ',': return Token.Comma;
        //        case 'n': return Token.Null;
        //        case ':': return Token.Semicolon;
        //        case 't': return Token.True;
        //        case 'f': return Token.False;
        //    }
        //}
    }

    enum Token
    {
        CurlyBraceStart,
        CurlyBraceEnd,
        SquareBraceStart,
        SquareBraceEnd,
        Comma,
        Semicolon,
        Quote,
        True,
        False,
        Null
    }
}
