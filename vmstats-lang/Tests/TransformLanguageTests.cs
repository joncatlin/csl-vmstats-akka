using System.Collections.Generic;
using NUnit.Framework;
using Akka.TestKit.NUnit3;
using Akka.Actor;
using vmstats;
using Akka.Configuration;
using Newtonsoft.Json;
using transforms;
using System;
using Antlr4.Runtime;

namespace vmstats.lang.Tests
{
    [TestFixture] //using NUnit
    class TransformationLanguageTests
    {
        [Test]
        public void When_DSLWithTwoTransformsAndSingleParameterUsed_Expect_SingleValidTransformPipelineBuilt()
        {
            // Build a string with some of the DSL in order to test
            string simpleTransformationPipeline = "CPUMAX->" + RemoveBaseNoiseActor.TRANSFORM_NAME +
                ":" + RemoveSpikeActor.TRANSFORM_NAME + "{ " + RemoveSpikeActor.SPIKE_WINDOW_LENGTH + " = 3}";

            // Create the container for all the actors
            var sys = ActorSystem.Create("vmstats-lang-tests"/*, config*/);

            // Create some transform actors so that we can test the DSL
            //var actor1 = sys.ActorOf(Props.Create(() => new RemoveBaseNoiseActor()), "Transforms-" + RemoveBaseNoiseActor.TRANSFORM_NAME);
            //var actor2 = sys.ActorOf(Props.Create(() => new RemoveSpikeActor()), "Transforms-" + RemoveSpikeActor.TRANSFORM_NAME);

            // Pick one of the defined above pipelines to use in the test
            string cmd = simpleTransformationPipeline;

            // Create a collection to hold all of the transform_pipelines found by the listener when we
            // decode the DSL.
            Queue<BuildTransformSeries> result = new Queue<BuildTransformSeries>();

            // translate the DSL in the test text and execute the tranform pipeline it represents
            var tp = new TransformationLanguage(sys.Log);
            tp.DecodeAndExecute(cmd, result);

            // Generate the expected results
            var expectedResult = generateTestData_When_DSLWithTwoTransformsAndSingleParameterUsed_Expect_SingleValidTransformPipelineBuilt();

            // Check expected results match real results
            Assert.AreNotEqual(expectedResult.Peek().GroupID, result.Peek().GroupID,
                "GroupID, Results and Expected values are equal.\nExpected values = {0}\nActual result values = {1}. ",
                new object[] { JsonConvert.SerializeObject(expectedResult.Peek().GroupID),
                    JsonConvert.SerializeObject(result.Peek().GroupID) });
            Assert.AreEqual(expectedResult.Peek().MetricName, result.Peek().MetricName,
                "MetricName, Results and Expected values are different.\nExpected values = {0}\nActual result values = {1}. ",
                new object[] { JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(result) });
            var e = JsonConvert.SerializeObject(expectedResult.Peek().Transforms);
            var a = JsonConvert.SerializeObject(result.Peek().Transforms);
            Assert.AreEqual(e, a, 
                "Transforms, Results and Expected values are different.\nExpected values = {0}\nActual result values = {1}. ", 
                new object[] { e, a }
                );
        }

        public Queue<BuildTransformSeries> generateTestData_When_DSLWithTwoTransformsAndSingleParameterUsed_Expect_SingleValidTransformPipelineBuilt()
        {
            // Create the data for the test
            var metricName = "CPUMAX";
            var transforms = new Queue<Transform>();
            Guid groupID = new Guid();

            var parameters = new Dictionary<string, string>();
            parameters.Add(RemoveSpikeActor.SPIKE_WINDOW_LENGTH, "3");

            transforms.Enqueue(new Transform(RemoveBaseNoiseActor.TRANSFORM_NAME, new Dictionary<string, string>()));
            transforms.Enqueue(new Transform(RemoveSpikeActor.TRANSFORM_NAME, parameters));

            var q = new Queue<BuildTransformSeries>();
            q.Enqueue(new BuildTransformSeries(metricName, transforms, groupID));
            return q;
        }

        [Test]
        public void When_DSLIsInvlaid_Expect_Exception()
        {
            // Build a string containing errors in the DSL
            string errorTransformationPipeline = "CPUMAX && ^^ -> RBN : 'JON' {param-1=value$4}";

            // Pick one of the defined above pipelines to use in the test
            string cmd = errorTransformationPipeline;

            // Create the container for all the actors
            var sys = ActorSystem.Create("vmstats-lang-tests"/*, config*/);

            // Create a collection to hold all of the transform_pipelines found by the listener when we
            // decode the DSL.
            Queue<BuildTransformSeries> result = new Queue<BuildTransformSeries>();

            // translate the DSL in the test text and execute the tranform pipeline it represents
            var tp = new TransformationLanguage(sys.Log);
            VmstatsLangException ex = Assert.Throws<VmstatsLangException>(delegate { tp.DecodeAndExecute(cmd, result); },
                "Unexpected exceptiion thrown.");
        }
    }
}
