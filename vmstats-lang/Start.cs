using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace vmstats.lang
{
    public class Start
    {
        private static void Main(string[] args)
        {
            try
            {
                string input = "";
                StringBuilder text = new StringBuilder();
                Console.WriteLine("The grammer to be parsed:");

                // to type the EOF character and end the input: use CTRL+D, then press <enter>     
                /*
                                while ((input = Console.ReadLine()) != "\u0004")
                                {
                                    text.AppendLine(input);
                                }
                */
                text.AppendLine(Console.ReadLine());

                AntlrInputStream inputStream = new AntlrInputStream(text.ToString());

                // Get the lexer for the language
                VmstatsLexer lexer = new VmstatsLexer(inputStream);

                // Get a list of matched tokens 
                CommonTokenStream tokens = new CommonTokenStream(lexer);

                // Pass the tokens to the parser
                VmstatsParser parser = new VmstatsParser(tokens);

                // Specify the entry point to the stream of tokens
                VmstatsParser.Transform_pipelineContext context = parser.transform_pipeline();

                // Create an instance of our listener class to be called as we walk the language
                MyListener myListener = new MyListener();

                ParseTreeWalker walker = new ParseTreeWalker();

//                parser.BuildParseTree = true;
                walker.Walk(myListener, context);
                //                IParseTree tree = parser.



/*
                parser.pro
                Transform_PipelineTreeWalker 
                    
                    walker = new transform_PipelineWalker();

//                AntlrInputStream inputStream = new AntlrInputStream(fileStream);
//                ValuesLexer lexer = new ValuesLexer(inputStream);
//                CommonTokenStream tokenStream = new CommonTokenStream(lexer);
//                ValuesParser parser = new ValuesParser(tokenStream);
//                ValuesParser.ParseContext context = parser.parse();
//                ValuesListener listener = new ValuesListener();
                ParseTreeWalker walker = new ParseTreeWalker();
                bool built = parser.BuildParseTree;
                walker.Walk(listener, context);
                foreach (double d in listener.doubles)
                    Console.WriteLine(d);
                Console.ReadKey();

                ParseTreeWalker walker = new ParseTreeWalker();
                AntlrDrinkListener listener = new AntlrDrinkListener();
                walker.walk(listener, drinkSentenceContext);

                int i = 0;
/*
                myParser.StatementContext context = myParser.statement();
                LanguageVisitor visitor = new LanguageVisitor();
                visitor.Visit(context);
                foreach (var line in visitor.Lines)
                {
                    Console.WriteLine("{0} has said \"{1}\"", line.Person, line.Text);
                }
*/
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
