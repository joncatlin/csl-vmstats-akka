using System.Collections.Generic;
using NUnit.Framework;
using Akka.TestKit.NUnit3;
using Akka.Actor;
using vmstats;

namespace transforms.Tests
{
    [TestFixture] //using NUnit
    class RemoveBaseNoiseTests : TestKit
    {
        static void Main(string[] args)
        {
            var test = new RemoveBaseNoiseTests();
/*
            var config = GetConfiguration();
            Sys = ActorSystem.Create("vmstats", config);

            // Create an instance of the actor to be tested
            actor = vmstatsActorSystem.ActorOf(Props.Create(() => new RemoveBaseNoise()));
*/
            // Call the tests to be performed
            test.Success_all_values_are_zero();
            test.Success_default_rolling_avg_length();
            test.Success_specific_rolling_avg_length();
            test.Success_complex_rolling_avg_length();
        }

        [Test]
        public void Success_all_values_are_zero()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            for (int x=1; x<=20; x++)
            {
                d.Add(x, 1.0F);
            }
            var metric = new Metric("Success_all_values_are_zero", d);
            var msg = new Transform(metric);

            var actor = Sys.ActorOf(Props.Create(() => new RemoveBaseNoiseActor()));
            actor.Tell(msg);
            var result = ExpectMsg<Result>();

            // Create the data for the expected result
            d = new SortedDictionary<long, float>();
            for (int x = 1; x <= 20; x++)
            {
                d.Add(x, 0.0F);
            }

            Assert.AreEqual(d, result.Measurements.Values);
            Assert.AreEqual("Success_all_values_are_zero:RBN", result.Measurements.Name);
        }


        [Test]
        public void Success_default_rolling_avg_length()
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
            var metric = new Metric("Success_default_rolling_avg_length", d);
            var msg = new Transform(metric);

            // Perform the test
            var actor = Sys.ActorOf(Props.Create(() => new RemoveBaseNoiseActor()));
            actor.Tell(msg);
 //           var result = ExpectMsg<Result>();

            // Create the expected result
            d = new SortedDictionary<long, float>();
            d.Add(1, 0.0F);
            d.Add(2, 1.0F);
            d.Add(3, 2.0F);
            d.Add(4, 3.0F);
            d.Add(5, 4.0F);
            d.Add(6, 5.0F);
            d.Add(7, 6.0F);
            d.Add(8, 7.0F);
            d.Add(9, 8.0F);
            d.Add(10, 9.0F);
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

            // Check results
//            Assert.AreEqual(d, result.Measurements.Values);
//            Assert.AreEqual("Success_all_values_are_zero:RBN", result.Measurements.Name);

        }


        [Test]
        public void Success_specific_rolling_avg_length()
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
            var p = new Dictionary<string, string>();
            p.Add(RemoveBaseNoiseActor.ROLLING_AVG_LENGTH, "15");
            var metric = new Metric("", d);
            var msg = new Transform(metric, p);

            var actor = Sys.ActorOf(Props.Create(() => new RemoveBaseNoiseActor()));
            actor.Tell(msg);
        }

        [Test]
        public void Success_complex_rolling_avg_length()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            d.Add(1, 1.0F);
            d.Add(2, 1.0F);
            d.Add(3, 8.0F);
            d.Add(4, 1.0F);
            d.Add(5, 1.0F);
            d.Add(6, 1.0F);
            d.Add(7, 9.0F);
            d.Add(8, 1.0F);
            d.Add(9, 1.0F);
            d.Add(10, 1.0F);
            d.Add(11, 1.0F);
            d.Add(12, 10.0F);
            d.Add(13, 1.0F);
            d.Add(14, 1.0F);
            d.Add(15, 1.0F);
            d.Add(16, 1.0F);
            d.Add(17, 1.0F);
            d.Add(18, 20.0F);
            d.Add(19, 21.0F);
            d.Add(20, 1.0F);

            // Create the actor
            var actor = Sys.ActorOf(Props.Create(() => new RemoveBaseNoiseActor()));

            // Avg should be 1
            var p = new Dictionary<string, string>();
            p.Add(RemoveBaseNoiseActor.ROLLING_AVG_LENGTH, "5");
            var metric = new Metric("", d);
            var msg = new Transform(metric, p);
            actor.Tell(msg);

            // Avg should be 13/6
            p = new Dictionary<string, string>();
            p.Add(RemoveBaseNoiseActor.ROLLING_AVG_LENGTH, "6");
            metric = new Metric("", d);
            msg = new Transform(metric, p);
            actor.Tell(msg);

            // Avg should be 19/10
            p = new Dictionary<string, string>();
            p.Add(RemoveBaseNoiseActor.ROLLING_AVG_LENGTH, "10");
            metric = new Metric("", d);
            msg = new Transform(metric, p);
            actor.Tell(msg);



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
