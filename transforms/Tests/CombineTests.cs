using System.Collections.Generic;
using NUnit.Framework;
using Akka.TestKit.NUnit3;
using Akka.Actor;
using vmstats;
using Akka.Configuration;
using Newtonsoft.Json;

namespace transforms.Tests
{
    [TestFixture] //using NUnit
    class CombineTests : TestKit
    {
        static void Main(string[] args)
        {
            var test = new CombineTests();

            // Get the configuration of the akka system
            var config = ConfigurationFactory.ParseString(GetConfiguration());

            // Create the container for all the actors
            var sys = ActorSystem.Create("vmstats-test", config);

            /***************************************************************************
             * Call the test to be performed
             ***************************************************************************/
            var metrics = new List<Metric>();
            metrics.Add(test.generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_1());
            metrics.Add(test.generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_2());
            metrics.Add(test.generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_3());

            // Create the actor and send it the data to be transformed
            var actor = sys.ActorOf(Props.Create(() => new CombineActor()));
            actor.Tell(new Combine(metrics));

            // Wait for the actor system to terminate so we have time to debug things
            sys.WhenTerminated.Wait();
        }

        [Test]
        public void When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach()
        {
            var metrics = new List<Metric>();
            metrics.Add(generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_1());
            metrics.Add(generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_2());
            metrics.Add(generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_3());

            // Create the actor and send it the data to be transformed
            var actor = Sys.ActorOf(Props.Create(() => new CombineActor()));
            actor.Tell(new Combine(metrics));
            var result = ExpectMsg<Result>();

            // Create the expected results
            var expectedResult = generateResults_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach();

            // Check expected results match real results
            CollectionAssert.AreEqual(expectedResult.Values, result.Measurements.Values,
                "Resuls and Expected values are different.\nExpected values = {0}\nActual result values = {1}. ",
                new object[] { JsonConvert.SerializeObject(expectedResult.Values), JsonConvert.SerializeObject(result.Measurements.Values) }
                );
            Assert.AreEqual(expectedResult.Name, result.Measurements.Name);
        }

        public Metric generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_1()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            d.Add(1, 1.0F);
            d.Add(2, 2.0F);
            d.Add(3, 3.0F);
            d.Add(4, 4.0F);
            d.Add(5, 5.0F);
            d.Add(6, 6.0F);
            d.Add(7, 7.0F);
            d.Add(8, 8.0F);
            d.Add(9, 9.0F);
            d.Add(10, 10.0F);
            d.Add(11, 1.0F);
            d.Add(12, 1.0F);
            d.Add(13, 1.0F);
            d.Add(14, 1.0F);
            d.Add(15, 1.0F);
            d.Add(16, 1.0F);
            d.Add(17, 1.0F);
            d.Add(18, 1.0F);
            d.Add(19, 1.0F);
            d.Add(20, 1.0F);
            return new Metric("When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_1", d);
        }

        public Metric generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_2()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            d.Add(1, 9.0F);
            d.Add(2, 8.0F);
            d.Add(3, 7.0F);
            d.Add(4, 6.0F);
            d.Add(5, 5.0F);
            d.Add(6, 4.0F);
            d.Add(7, 3.0F);
            d.Add(8, 2.0F);
            d.Add(9, 1.0F);
            d.Add(10, 10.0F);
            d.Add(11, 1.0F);
            d.Add(12, 1.0F);
            d.Add(13, 1.0F);
            d.Add(14, 1.0F);
            d.Add(15, 1.0F);
            d.Add(16, 1.0F);
            d.Add(17, 1.0F);
            d.Add(18, 1.0F);
            d.Add(19, 1.0F);
            d.Add(20, 1.0F);
            return new Metric("When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_2", d);
        }

        public Metric generateTestData_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_3()
        {
            // Create the data for the test
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
            d.Add(10, 10.0F);
            d.Add(11, 8.0F);
            d.Add(12, 8.0F);
            d.Add(13, 8.0F);
            d.Add(14, 8.0F);
            d.Add(15, 8.0F);
            d.Add(16, 8.0F);
            d.Add(17, 8.0F);
            d.Add(18, 8.0F);
            d.Add(19, 8.0F);
            d.Add(20, 8.0F);
            return new Metric("When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_3", d);
        }

        public Metric generateResults_When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach()
        {
            // Create the data for the expected result
            var d = new SortedDictionary<long, float>();
            d.Add(1, 10.0F);
            d.Add(2, 10.0F);
            d.Add(3, 10.0F);
            d.Add(4, 10.0F);
            d.Add(5, 10.0F);
            d.Add(6, 10.0F);
            d.Add(7, 10.0F);
            d.Add(8, 10.0F);
            d.Add(9, 10.0F);
            d.Add(10, 30.0F);
            d.Add(11, 10.0F);
            d.Add(12, 10.0F);
            d.Add(13, 10.0F);
            d.Add(14, 10.0F);
            d.Add(15, 10.0F);
            d.Add(16, 10.0F);
            d.Add(17, 10.0F);
            d.Add(18, 10.0F);
            d.Add(19, 10.0F);
            d.Add(20, 10.0F);

            return new Metric("(When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_1+When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_2+When_ThreeMetricsAreCombined_Expect_ResultMetricContainsSumOfEach_3):COM", d);
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


    }
}
