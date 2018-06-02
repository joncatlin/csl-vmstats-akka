using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

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
                Console.WriteLine("The grammer to be parsed");

                // to type the EOF character and end the input: use CTRL+D, then press <enter>     
                /*
                                while ((input = Console.ReadLine()) != "\u0004")
                                {
                                    text.AppendLine(input);
                                }
                */
                input = Console.ReadLine();

                AntlrInputStream inputStream = new AntlrInputStream(text.ToString());

                vmstatsLexer myLexer = new vmstatsLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(myLexer);
                vmstatsParser myParser = new vmstatsParser(commonTokenStream);

                vmstatsParser.Transform_pipelineContext transform_pipelineContext = myParser.transform_pipeline();

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


    public class MyVisitor : vmstatsBaseVisitor<object>
    {
        public List<VisitTransform_pipeline>
    }
}
