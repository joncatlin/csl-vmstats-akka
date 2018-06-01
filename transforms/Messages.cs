using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;

namespace transforms
{
    #region Message classes

    /// <summary>
    /// This class holds a metric that is to be transformed, The transformation 
    /// can be supplied with or without parameters to override the default transformation
    /// parameters.
    /// </summary>
    public class Transform
    {
        public Transform(Metric metric, Dictionary<string, string>paramaters)
        {
            Measurements = metric;
            Parameters = paramaters;
        }

        public Transform(Metric metric)
        {
            Measurements = metric;
            Parameters = new Dictionary<string, string>();
        }

        public Metric Measurements { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }

    /// <summary>
    /// This class holds a series of transforms to be calculated on a single Metric.
    /// </summary>
    public class TransformSeries
    {
        public TransformSeries(Metric metric, Stack<string>transformNames)
        {
            Measurements = metric;
            TransformNames = transformNames;
        }

        public Metric Measurements { get; }
        public Stack<string> TransformNames { get; }
    }

    /// <summary>
    /// This class holds a series of transforms to be calculated on a group of different 
    /// Metrics. It specifies which transform to perform on which Metric.
    /// 
    /// TODO. This is a starting mechanism only. It really needs a tree structure for the
    /// potential complex ways that transforms can be calculated and then combined. So starting
    /// this is a simple implementation.
    /// </summary>
    public class TransformSeriesGroup
    {
        public TransformSeriesGroup(List<TransformSeries> transforms)
        {
            Transforms = transforms;
        }

        public List<TransformSeries> Transforms { get; }
    }

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

    /// <summary>
    /// This class holds a list of metrics that should be combined into a single metric.
    /// </summary>
    public class Combine
    {
        public Combine(List<Metric> metrics)
        {
            Metrics = metrics;
        }

        public List<Metric> Metrics { get; set; }
    }


    #endregion

}





