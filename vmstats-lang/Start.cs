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
                string complexTransformationPipeline = "( CPUMAX->RBN:RSP{param1=value1} + MEMMAX->RBN:RSP{param2=value2,param3=value3} + IOMAX->RBN:RSP ) : RBN {param4=value4}";
                string simpleTransformationPipeline = "CPUMAX->RBN:RSP{param1=value1}";
                string input = simpleTransformationPipeline;
                Console.WriteLine(input);

                // Create an ANTLR input stream to process the DSL entered
                AntlrInputStream inputStream = new AntlrInputStream(input);

                // Get the lexer for the language
                VmstatsLexer lexer = new VmstatsLexer(inputStream);

                // Get a list of matched tokens 
                CommonTokenStream tokens = new CommonTokenStream(lexer);

                // Pass the tokens to the parser
                VmstatsParser parser = new VmstatsParser(tokens);

                // Specify the entry point to the stream of tokens
                VmstatsParser.Transform_pipelineContext context = parser.transform_pipeline();

                // Create an instance of our listener class to be called as we walk the language
                MyListener myListener = new MyListener(null);

                // Create a tree walker to walk the AST
                ParseTreeWalker walker = new ParseTreeWalker();

                // Now walk the AST having the listener be called during all the events
                walker.Walk(myListener, context);


                // Wait until key pressed in order to analyze the results
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
