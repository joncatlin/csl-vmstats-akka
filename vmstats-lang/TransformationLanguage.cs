using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using transforms;

namespace vmstats.lang
{
    public class TransformationLanguage
    {
        #region Instance variables
        private readonly ILoggingAdapter _log;
        #endregion

        public TransformationLanguage (ILoggingAdapter log)
        {
            //  Save the logging context
        }

        private static void Main(string[] args)
        {
            string complexTransformationPipeline = "( CPUMAX->RBN:RSP{param1=value1} + MEMMAX->RBN:RSP{param2=value2,param3=value3} + IOMAX->RBN:RSP ) : RBN {param4=value4}";
            string simpleTransformationPipeline = "CPUMAX->RBN:RSP{param1=value1}";
            string errorTransformationPipeline = "CPUMAX -> RBN : 'JON' {param-1=value$4}";

            // Get the configuration of the akka system
//            var config = ConfigurationFactory.ParseString(GetConfiguration());

            // Create the container for all the actors
            var sys = ActorSystem.Create("vmstats-lang-tests"/*, config*/);

            // Create some transform actors so that we can test the DSL
            var actor = sys.ActorOf(Props.Create(() => new RemoveBaseNoiseActor()));

            // Pick one of the defined above pipelines to use in the test
            string cmd = simpleTransformationPipeline;

            // translate the DSL in the test text and execute the tranform pipeline it represents
            var tp = new TransformationLanguage(null);
            tp.DecodeAndExecute(cmd);

            // Wait for the actor system to terminate so we have time to debug things
            sys.WhenTerminated.Wait();
        }


        public void DecodeAndExecute(string commandsToDecode)
        {
            try
            {
                // Create an ANTLR input stream to process the DSL entered
                AntlrInputStream inputStream = new AntlrInputStream(commandsToDecode);

                // Get the lexer for the language
                VmstatsLexer lexer = new VmstatsLexer(inputStream);

                // Get a list of matched tokens 
                CommonTokenStream tokens = new CommonTokenStream(lexer);

                // Pass the tokens to the parser
                VmstatsParser parser = new VmstatsParser(tokens);

                // Specify the entry point to the stream of tokens
                VmstatsParser.Transform_pipelineContext context = parser.transform_pipeline();

                // Create an instance of our listener class to be called as we walk the language
                MyListener myListener = new MyListener(_log);

                // Create a tree walker to walk the AST
                ParseTreeWalker walker = new ParseTreeWalker();

                // Now walk the AST having the listener be called during all the events
                walker.Walk(myListener, context);
            }
            catch (Exception ex)
            {
                _log.Error("Error processing transform DSL statement. Reason: " + ex);
            }
        }


        public static void RouteTransform (ActorSystem sys, TransformSeries series)
        {
            // Get the name of the first transform in the series
            var tname = series.Transforms.Peek().Name;

            // look up the actor with the name of the transform and send the transform series 
            // to it for the first step in the processing. Each transform actor in turn will then
            // use this method to route to the next transform in the series.
            sys.ActorSelection("*/Transforms/" + tname).Tell(series);
        }
    }
}
