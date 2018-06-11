using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace vmstats.lang
{
    public class VmstatsLangException : Exception
    {
        public VmstatsLangException()
        {
        }

        public VmstatsLangException(string message)
            : base(message)
        {
        }

        public VmstatsLangException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    class MyErrorListener : IAntlrErrorListener<int>
    {
//        void SyntaxError(TextWriter output, IRecognizer recognizer, TSymbol offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e);



        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            var newmsg = "Error while lexing Vmstats DSL at line :" + e.OffendingToken.Column + e.OffendingToken.Line + e.Message;
            var ex = new VmstatsLangException(newmsg, e);
            throw ex;
        }

        //IAntlrErrorListener<int> implementation; this one actually gets called.

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new VmstatsLangException("Invalid Expression: " + msg, e);
        }
    }
}
