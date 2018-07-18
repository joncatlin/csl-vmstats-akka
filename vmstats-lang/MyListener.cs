using System;
using System.Collections.Generic;
using Akka.Event;
using Antlr4.Runtime.Tree;
using vmstats_shared;

namespace vmstats.lang
{
    public class MyListener : VmstatsBaseListener
    {
        #region Instance variables
        private Queue<Messages.Transform> transforms;
        private Messages.Transform currentTransform;
        private Metric currentMetric;
        private string currentMetricName;
        private string currentTransformName;
        private string currentParameterName;
        private string currentValue;
        private Dictionary<string, string> currentParameters;
        private Queue<Messages.BuildTransformSeries> series;
        private readonly ILoggingAdapter _log;
        private bool _errorFound = false;
        private bool inCombine = false;
        private int numberTransformPipelinesInCombine = 0;
        private Queue<Guid> currentCombineID = new Queue<Guid>();

        #endregion

        public MyListener(ILoggingAdapter log, Queue<Messages.BuildTransformSeries>series)
        {
            _log = log;

            // Save the container for the BuildTransformSeries to be built by this listener class
            this.series = series;
        }

        public override void EnterTransform_series(VmstatsParser.Transform_seriesContext context)
        {
            Console.WriteLine("EnterTransform_series");
        }


        public override void ExitTransform_series(VmstatsParser.Transform_seriesContext context)
        {
            Console.WriteLine("ExitTransform_series");
        }


        public override void EnterTransform_pipeline(VmstatsParser.Transform_pipelineContext context)
        {
            Console.WriteLine("EnterTransform_pipeline");

            // Create a new series of transforms
            transforms = new Queue<Messages.Transform>();
        }

        public override void ExitTransform_pipeline(VmstatsParser.Transform_pipelineContext context)
        {
            Console.WriteLine("ExitTransform_pipeline");

            if (inCombine)
            {
                // Add the combine to the end of the list of transforms
                var parameters = new Dictionary<string, string>();
                parameters.Add(CombineTransformActor.TRANSFORM_PARAM_COUNT_NAME, numberTransformPipelinesInCombine.ToString());
                var transform = new Messages.Transform(CombineTransformActor.TRANSFORM_NAME, parameters);
                transforms.Enqueue(transform);
            }

            // Create a TransformSeries out of the information collected and add it to all the 
            // transform_pipelines found so far.
            series.Enqueue(new Messages.BuildTransformSeries(currentMetricName, transforms, currentCombineID.Peek()));
        }

        public override void EnterTransform(VmstatsParser.TransformContext context)
        {
            Console.WriteLine("EnterTransform");

            // Reset the current transform and parameters
            currentTransformName = "";
            currentParameters = new Dictionary<string, string>();
        }

        public override void ExitTransform(VmstatsParser.TransformContext context)
        {
            Console.WriteLine("ExitTransform");

            // Create a transform out of the information collected and add it to the series
            var transform = new Messages.Transform(currentTransformName, currentParameters);
            transforms.Enqueue(transform);
        }

        public override void EnterParameter(VmstatsParser.ParameterContext context)
        {
            Console.WriteLine("EnterParameter - {0}", context.Payload.GetChild(0));
        }

        public override void ExitParameter(VmstatsParser.ParameterContext context)
        {
            Console.WriteLine("ExitParameter");
        }

        public override void EnterCombine(VmstatsParser.CombineContext context)
        {
            Console.WriteLine("EnterCombine");

            // Determine the number of transform pipelines in this combine. Remove 2 for the braces. 
            // Divide by 2 and then round up, to account for the + between each transform
            var count = context.ChildCount;
            numberTransformPipelinesInCombine = ((count - 2) / 2) + 1;

            // Create a new id for this combine, so all transforms created from now will contain this id
            currentCombineID.Enqueue(Guid.NewGuid());
            inCombine = true;
        }

        public override void ExitCombine(VmstatsParser.CombineContext context)
        {
            Console.WriteLine("ExitCombine");

            // Remove this Combines id from the queue as it is not needed anymore and reset all the combine
            // variables
            currentCombineID.Dequeue();
            numberTransformPipelinesInCombine = 0;
            inCombine = false;
        }

        public override void EnterMetric_name(VmstatsParser.Metric_nameContext context)
        {
            Console.WriteLine("EnterMetric_name - {0}", context.Payload.GetChild(0));

            // Save the metric name
            currentMetricName = context.Payload.GetChild(0).ToString();
        }

        public override void ExitMetric_name(VmstatsParser.Metric_nameContext context)
        {
            Console.WriteLine("ExitMetric_name");
        }

        public override void EnterTransform_name(VmstatsParser.Transform_nameContext context)
        {
            Console.WriteLine("EnterTransform_name - {0}", context.Payload.GetChild(0));

            // Save the transform name
            currentTransformName = context.Payload.GetChild(0).ToString();
        }

        public override void ExitTransform_name(VmstatsParser.Transform_nameContext context)
        {
            Console.WriteLine("ExitTransform_name");
        }

        public override void EnterParameter_name(VmstatsParser.Parameter_nameContext context)
        {
            Console.WriteLine("EnterParameter_name - {0}", context.Payload.GetChild(0));

            // Save the parameter name
            currentParameterName = context.Payload.GetChild(0).ToString();
        }

        public override void ExitParameter_name(VmstatsParser.Parameter_nameContext context)
        {
            Console.WriteLine("ExitParameter_name");
        }

        public override void EnterValue_name(VmstatsParser.Value_nameContext context)
        {
            Console.WriteLine("EnterValue_name - {0}", context.Payload.GetChild(0));

            // Save the name of the current value
            currentValue = context.Payload.GetChild(0).ToString();

        }

        public override void ExitValue_name(VmstatsParser.Value_nameContext context)
        {
            Console.WriteLine("ExitValue_name");

            // Add the parameter and value to the set of current parameters
            if (!currentParameters.TryAdd(currentParameterName, currentValue))
            {
                // Log an error
                _log.Error("Error in DSL for transform name - {0}. Trying to add parameter with name={1} and value={2}."
                    + "Likely cause parameter with same name already exists for transform.", currentTransformName, currentParameterName, currentValue);
            }
        }

        /*
         * THIS IS A MASSIVE HACK!!!!!!
         * For some reason none of the error handling seems to get called and hence cannot find a way to throw an exception when the grammer
         * being parsed is rubbish. This code will signal an error by setting a flag in the object returned
         */
        public override void VisitErrorNode(IErrorNode node)
        {
            throw new VmstatsLangException("Invalid Expression:");
        }
    }
}