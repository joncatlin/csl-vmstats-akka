using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Akka.TestKit.NUnit3;
using Akka.Actor;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace transforms.Tests
{
    [TestFixture]
    class RemoveBaseNoiseTests
    {
        static IActorRef actor;

        static void Main(string[] args)
        {
            var config = GetConfiguration();
            var vmstatsActorSystem = ActorSystem.Create("vmstats", config);

            // Create an instance of the actor to be tested
            actor = vmstatsActorSystem.ActorOf(Props.Create(() => new RemoveBaseNoise()));

            // Call the tests to be performed
            Success_all_values_are_zero();
            Success_default_rolling_avg_length();
            Success_specific_rolling_avg_length();
            Success_complex_rolling_avg_length();
        }

        [Test]
        public static void Success_all_values_are_zero()
        {
            // Create the data for the test
            var d = new SortedDictionary<long, float>();
            d.Add(1, 1.0F);
            d.Add(2, 1.0F);
            d.Add(3, 1.0F);
            d.Add(4, 1.0F);
            d.Add(5, 1.0F);
            d.Add(6, 1.0F);
            d.Add(7, 1.0F);
            d.Add(8, 1.0F);
            d.Add(9, 1.0F);
            d.Add(10, 1.0F);
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
            var msg = new Transform(d);

            actor.Tell(msg);
        }

        [Test]
        public static void Success_default_rolling_avg_length()
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
            var msg = new Transform(d);

            actor.Tell(msg);
        }


        [Test]
        public static void Success_specific_rolling_avg_length()
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
            p.Add(RemoveBaseNoise.ROLLING_AVG_LENGTH, "15");
            var msg = new Transform(d,p);

            actor.Tell(msg);
        }

        [Test]
        public static void Success_complex_rolling_avg_length()
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

            // Avg should be 1
            var p = new Dictionary<string, string>();
            p.Add(RemoveBaseNoise.ROLLING_AVG_LENGTH, "5");
            var msg = new Transform(d, p);
            actor.Tell(msg);

            // Avg should be 13/6
            p = new Dictionary<string, string>();
            p.Add(RemoveBaseNoise.ROLLING_AVG_LENGTH, "6");
            msg = new Transform(d, p);
            actor.Tell(msg);

            // Avg should be 19/10
            p = new Dictionary<string, string>();
            p.Add(RemoveBaseNoise.ROLLING_AVG_LENGTH, "10");
            msg = new Transform(d, p);
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
