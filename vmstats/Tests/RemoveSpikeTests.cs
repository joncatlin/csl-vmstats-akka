using System.Collections.Generic;
using NUnit.Framework;
using Akka.TestKit.NUnit3;
using Akka.Actor;
using vmstats;
using Akka.Configuration;
using Newtonsoft.Json;

namespace vmstats.Tests
{
    [TestFixture] //using NUnit
    class RemoveSpikeTests : TestKit
    {
        /*
        static void Main(string[] args)
        {
            var test = new RemoveSpikeTests();

            // Get the configuration of the akka system
            var config = ConfigurationFactory.ParseString(GetConfiguration());

            // Create the container for all the actors
            var sys = ActorSystem.Create("vmstats-test", config);

            var msg = test.generateTestData_When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase();

            // Create the actor and send it the data to be transformed
            var actor = sys.ActorOf(Props.Create(() => new RemoveSpikeActor()));
            actor.Tell(msg);

            // Wait for the actor system to terminate so we have time to debug things
            sys.WhenTerminated.Wait();
        }

        [Test]
        public void When_ThereAreOnlySpikesInData_Expect_ResultValuesAreAllBase()
        {
            var msg = generateTestData_When_ThereAreOnlySpikesInData_Expect_ResultValuesAreAllBase();

            // Create the actor and send it the data to be transformed
            var actor = Sys.ActorOf(Props.Create(() => new RemoveSpikeActor()));
            actor.Tell(msg);
            var result = ExpectMsg<Result>();

            // Create the expected results
            var expectedResult = generateResults_When_ThereAreOnlySpikesInData_Expect_ResultValuesAreAllBase();

            // Check expected results match real results
            CollectionAssert.AreEqual(expectedResult.Values, result.Measurements.Values, 
                "Resuls and Expected values are different.\nExpected values = {0}\nActual result values = {1}. ", 
                new object[] { JsonConvert.SerializeObject(expectedResult.Values), JsonConvert.SerializeObject(result.Measurements.Values) }
                );
            Assert.AreEqual(expectedResult.Name, result.Measurements.Name);
        }

        public Transform generateTestData_When_ThereAreOnlySpikesInData_Expect_ResultValuesAreAllBase()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            d.Add(1, 0.0F);
            d.Add(2, 2.0F);
            d.Add(3, 0.0F);
            d.Add(4, 4.0F);
            d.Add(5, 0.0F);
            d.Add(6, 0.0F);
            d.Add(7, 7.0F);
            d.Add(8, 0.0F);
            d.Add(9, 0.0F);
            d.Add(10, 10.0F);
            d.Add(11, 0.0F);
            d.Add(12, 1.0F);
            d.Add(13, 0.0F);
            d.Add(14, 1.0F);
            d.Add(15, 0.0F);
            d.Add(16, 0.0F);
            d.Add(17, 0.0F);
            d.Add(18, 0.0F);
            d.Add(19, 1.0F);
            d.Add(20, 0.0F);
            var metric = new Metric("When_ThereAreOnlySpikesInData_Expect_ResultValuesAreAllBase", d);
            return new Transform(metric);

        }

        public Metric generateResults_When_ThereAreOnlySpikesInData_Expect_ResultValuesAreAllBase()
        {
            // Create the data for the expected result
            var d = new SortedDictionary<long, float>();
            d.Add(1, 0.0F);
            d.Add(2, 0.0F);
            d.Add(3, 0.0F);
            d.Add(4, 0.0F);
            d.Add(5, 0.0F);
            d.Add(6, 0.0F);
            d.Add(7, 0.0F);
            d.Add(8, 0.0F);
            d.Add(9, 0.0F);
            d.Add(10, 0.0F);
            d.Add(11, 0.0F);
            d.Add(12, 0.0F);
            d.Add(13, 0.0F);
            d.Add(14, 0.0F);
            d.Add(15, 0.0F);
            d.Add(16, 0.0F);
            d.Add(17, 0.0F);
            d.Add(18, 0.0F);
            d.Add(19, 0.0F);
            d.Add(20, 0.0F);
            return new Metric("When_ThereAreOnlySpikesInData_Expect_ResultValuesAreAllBase:RSP", d);

        }

        [Test]
        public void When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase()
        {
            // Create the data for the test
            var msg = generateTestData_When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase();

            // Perform the test
            var actor = Sys.ActorOf(Props.Create(() => new RemoveSpikeActor()));
            actor.Tell(msg);
            var result = ExpectMsg<Result>();

            // Create the expected results
            var expectedResult = generateResults_When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase();

            // Check results
            CollectionAssert.AreEqual(expectedResult.Values, result.Measurements.Values,
                "Resuls and Expected values are different.\nExpected values = {0}\nActual result values = {1}. ",
                new object[] { JsonConvert.SerializeObject(expectedResult.Values), JsonConvert.SerializeObject(result.Measurements.Values) }
                );

            Assert.AreEqual(expectedResult.Name, result.Measurements.Name);
        }

        public Transform generateTestData_When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            d.Add(1, 1.0F);
            d.Add(2, 2.0F);
            d.Add(3, 3.0F);
            d.Add(4, 0.0F);
            d.Add(5, 5.0F);
            d.Add(6, 0.0F);
            d.Add(7, 7.0F);
            d.Add(8, 8.0F);
            d.Add(9, 0.0F);
            d.Add(10, 10.0F);
            d.Add(11, 0.0F);
            d.Add(12, 1.0F);
            d.Add(13, 1.0F);
            d.Add(14, 0.0F);
            d.Add(15, 0.0F);
            d.Add(16, 1.0F);
            d.Add(17, 0.0F);
            d.Add(18, 0.0F);
            d.Add(19, 1.0F);
            d.Add(20, 1.0F);
            var metric = new Metric("When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase", d);
            return new Transform(metric);
        }

        public Metric generateResults_When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase()
        {
            // Create the expected result
            var d = new SortedDictionary<long, float>();
            d.Add(1, 1.0F);
            d.Add(2, 2.0F);
            d.Add(3, 3.0F);
            d.Add(4, 0.0F);
            d.Add(5, 0.0F);
            d.Add(6, 0.0F);
            d.Add(7, 7.0F);
            d.Add(8, 8.0F);
            d.Add(9, 0.0F);
            d.Add(10, 0.0F);
            d.Add(11, 0.0F);
            d.Add(12, 1.0F);
            d.Add(13, 1.0F);
            d.Add(14, 0.0F);
            d.Add(15, 0.0F);
            d.Add(16, 0.0F);
            d.Add(17, 0.0F);
            d.Add(18, 0.0F);
            d.Add(19, 1.0F);
            d.Add(20, 1.0F);
            return new Metric("When_ThereAreSomeSpikesInData_Expect_OnlySpikesSetToBase:RSP", d);
        }


        [Test]
        public void When_ThereAreSomeSpikesInDataAndSpecifyingNonDefaultValues_Expect_OnlySpikesSetToSuppliedBase()
        {
            // Create the data for the test
            var msg = generateTestData_When_ThereAreSomeSpikesInDataAndSpecifyingNonDefaultValues_Expect_OnlySpikesSetToSuppliedBase();

            // Perform the test
            var actor = Sys.ActorOf(Props.Create(() => new RemoveSpikeActor()));
            actor.Tell(msg);
            var result = ExpectMsg<Result>();

            // Create the expected results
            var expectedResult = generateResults_When_ThereAreSomeSpikesInDataAndSpecifyingNonDefaultValues_Expect_OnlySpikesSetToSuppliedBase();

            // Check results
            CollectionAssert.AreEqual(expectedResult.Values, result.Measurements.Values,
                "Resuls and Expected values are different.\nExpected values = {0}\nActual result values = {1}. ",
                new object[] { JsonConvert.SerializeObject(expectedResult.Values), JsonConvert.SerializeObject(result.Measurements.Values) }
                );
            Assert.AreEqual(expectedResult.Name, result.Measurements.Name);
        }

        public Transform generateTestData_When_ThereAreSomeSpikesInDataAndSpecifyingNonDefaultValues_Expect_OnlySpikesSetToSuppliedBase()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            d.Add(1, 1.0F);
            d.Add(2, 2.0F);
            d.Add(3, 3.0F);
            d.Add(4, 4.0F);
            d.Add(5, 2.0F);
            d.Add(6, 1.0F);
            d.Add(7, 3.0F);
            d.Add(8, 2.0F);
            d.Add(9, 1.0F);
            d.Add(10, 1.0F);
            d.Add(11, 10.0F);
            d.Add(12, 1.0F);
            d.Add(13, 1.0F);
            d.Add(14, 1.0F);
            d.Add(15, 11.0F);
            d.Add(16, 12.0F);
            d.Add(17, 13.0F);
            d.Add(18, 14.0F);
            d.Add(19, 2.0F);
            d.Add(20, 0.0F);
            var metric = new Metric("When_ThereAreSomeSpikesInDataAndSpecifyingNonDefaultValues_Expect_OnlySpikesSetToSuppliedBase", d);

            // Create the parameters to override the default values
            var p = new Dictionary<string, string>();
            p.Add(RemoveSpikeActor.SPIKE_WINDOW_LENGTH, "3");
            p.Add(RemoveSpikeActor.BASE_WINDOW_LENGTH, "2");
            p.Add(RemoveSpikeActor.BASE_VALUE, "5");
            return new Transform(metric, p);
        }

        public Metric generateResults_When_ThereAreSomeSpikesInDataAndSpecifyingNonDefaultValues_Expect_OnlySpikesSetToSuppliedBase()
        {
            // Create the expected result
            var d = new SortedDictionary<long, float>();
            d.Add(1, 1.0F);
            d.Add(2, 2.0F);
            d.Add(3, 5.0F);
            d.Add(4, 5.0F);
            d.Add(5, 5.0F);
            d.Add(6, 1.0F);
            d.Add(7, 3.0F);
            d.Add(8, 2.0F);
            d.Add(9, 5.0F);
            d.Add(10, 5.0F);
            d.Add(11, 5.0F);
            d.Add(12, 1.0F);
            d.Add(13, 1.0F);
            d.Add(14, 1.0F);
            d.Add(15, 11.0F);
            d.Add(16, 12.0F);
            d.Add(17, 13.0F);
            d.Add(18, 14.0F);
            d.Add(19, 2.0F);
            d.Add(20, 0.0F);
            return new Metric("When_ThereAreSomeSpikesInDataAndSpecifyingNonDefaultValues_Expect_OnlySpikesSetToSuppliedBase:RSP", d);
        }


        private static string GetConfiguration()
        {
            string config = @"
                akka {  
                    stdout-loglevel = DEBUG
                    loglevel = DEBUG
                    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                    log-config-on-start = on

                    actor
                    {
                      debug
                      {
                        receive = on      # log any received message
                        autoreceive = on  # log automatically received messages, e.g. PoisonPill
                        lifecycle = on    # log actor lifecycle changes
                        event-stream = on # log subscription changes for Akka.NET event stream
                        unhandled = on    # log unhandled messages sent to actors
                      }
                    }
                  }
                }

            ";

            return config;
        }

*/
    }
}
