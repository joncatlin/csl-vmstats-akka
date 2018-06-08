using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using vmstats;

namespace transforms
{
    #region Message classes

    /// <summary>
    /// This class holds a single transform, comprisiong a name and optionally some parameters
    /// The transformation can be supplied with or without parameters to override the default 
    /// settings within the transformation actor.
    /// </summary>
    public class Transform
    {
        public Transform(string name, Dictionary<string, string>paramaters)
        {
            Name = name;
            Parameters = paramaters;
        }

        public Transform(string name)
        {
            Name = name;
            Parameters = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }

    /// <summary>
    /// This class holds a single Metric and a series of Transforms to be performed upon it. It also
    /// contains a unique ID so that if a transform actor is part of a consistent hashing routing group
    /// then all the transforms for this particular transform series will be routed to the same actor.
    /// 
    /// This is required in order for the Combine transform to work properly
    /// </summary>
    public class TransformSeries : IConsistentHashable
    {
        public TransformSeries(Metric metric, Queue<Transform> transforms, Guid groupID)
        {
            Measurements = metric;
            Transforms = transforms;
            GroupID = groupID;
        }

        public Guid GroupID { get; private set; }
        public object ConsistentHashKey { get { return GroupID; } }

        public Metric Measurements { get; private set; }
        public Queue<Transform> Transforms { get; private set; }
    }

    /// <summary>
    /// This class requests a Metric be obtained from the population of MetricStoreActors and then formed into
    /// a TransformSeries and sent to the TransformActor population for processing.
    /// </summary>
    public class BuildTransformSeries
    {
        public BuildTransformSeries(string metricName, Queue<Transform> transforms, Guid groupID)
        {
            MetricName = metricName;
            Transforms = transforms;
            GroupID = groupID;
        }

        public Guid GroupID { get; private set; }
        public string MetricName { get; private set; }
        public Queue<Transform> Transforms { get; private set; }
    }

    /*
    /// <summary>
    /// This class holds the result of a transformation.
    /// </summary>
    public class Result
    {
        public Result(Metric metric)
        {
            Measurements = metric;
        }

        public Metric Measurements { get; set; }
    }
*/
    /* DO NOT USE THIS. Instead use a transform series with a specific transform containing parameters of the unique ids 
     * of the transforms to combine
        /// <summary>
        /// This class holds a list of unique ids for TransformSeries that are to be combined into a single Metric
        /// </summary>
        public class Combine
        {
            public Combine(List<Metric> metrics)
            {
                Metrics = metrics;
            }

            public List<Metric> Metrics { get; set; }
        }
     */


    #endregion

}





