using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;
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
            _log = log;
        }

        /*
        private static void Main(string[] args)
        {
            // Build a string with some of the DSL in order to test
            string simpleTransformationPipeline = "CPUMAX->" + RemoveBaseNoiseActor.TRANSFORM_NAME + 
                ":" + RemoveSpikeActor.TRANSFORM_NAME + "{ " + RemoveSpikeActor.SPIKE_WINDOW_LENGTH +" = 3}";

            // Build a string containing errors in the DSL
            string errorTransformationPipeline = "CPUMAX -> RBN : 'JON' {param-1=value$4}";

            // Create the container for all the actors
            var sys = ActorSystem.Create("vmstats-lang-tests", config);

            // Create some transform actors so that we can test the DSL
            var actor1 = sys.ActorOf(Props.Create(() => new RemoveBaseNoiseActor()), "Transforms-" + RemoveBaseNoiseActor.TRANSFORM_NAME);
            var actor2 = sys.ActorOf(Props.Create(() => new RemoveSpikeActor()), "Transforms-" + RemoveSpikeActor.TRANSFORM_NAME);

            // Pick one of the defined above pipelines to use in the test
            string cmd = simpleTransformationPipeline;

            // Create a collection to hold all of the transform_pipelines found by the listener when we
            // decode the DSL.
            Queue<BuildTransformSeries> series = new Queue<BuildTransformSeries>();

            // translate the DSL in the test text and execute the tranform pipeline it represents
            var tp = new TransformationLanguage(sys.Log, series);
            tp.DecodeAndExecute(cmd);

            // Wait for the actor system to terminate so we have time to debug things
            sys.WhenTerminated.Wait();
        }
*/

        public void DecodeAndExecute(string commandsToDecode, Queue<BuildTransformSeries> series)
        {
/*
            try
            {
*/
                // Create an ANTLR input stream to process the DSL entered
                AntlrInputStream inputStream = new AntlrInputStream(commandsToDecode);

                // Get the lexer for the language
                VmstatsLexer lexer = new VmstatsLexer(inputStream);

                // Get a list of matched tokens 
                CommonTokenStream tokens = new CommonTokenStream(lexer);

                // Pass the tokens to the parser
                VmstatsParser parser = new VmstatsParser(tokens);

                parser.RemoveErrorListeners();

                // Specify the entry point to the stream of tokens
                VmstatsParser.Transform_pipelineContext context = parser.transform_pipeline();

                // Create an instance of our listener class to be called as we walk the language
                MyListener myListener = new MyListener(_log, series);

                // Create an instance of our ERROR listener class and add it to the parser so we can catch errors
                //                MyErrorListener myErrorListener = new MyErrorListener();
                //                lexer.RemoveErrorListeners();
                //                lexer.AddErrorListener(myErrorListener);


                //                parser.CompileParseTreePattern()
                parser.ErrorHandler = new BailErrorStrategy();

                // Create a tree walker to walk the AST
                ParseTreeWalker walker = new ParseTreeWalker();

                // Now walk the AST having the listener be called during all the events
                walker.Walk(myListener, context);

            _log.Debug("Walked the tree to decode the DSL. Result: " + JsonConvert.SerializeObject(series));
/*
            }
            catch (RecognitionException ex)
            {
                // Cannot get the code to throw any exceptions when the grammer is very wrong
                _log.Error("Error processing transform DSL statement. Reason: " + ex);
            } catch (Exception ex)
            {
                // Cannot get the code to throw any exceptions when the grammer is very wrong
                _log.Error("Error processing transform DSL statement. Reason: " + ex);
            }
*/
        }
    }
}
