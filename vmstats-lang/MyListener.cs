using System;
using System.Collections.Generic;
using System.Text;
using vmstats;
using transforms;
using Akka.Event;

namespace vmstats.lang
{
    public class MyListener : VmstatsBaseListener
    {
        #region Instance variables
        private Stack<Transform> transforms = new Stack<Transform>();
        private Transform currentTransform;
        private Metric currentMetric;
        private string currentMetricName;
        private string currentTransformName;
        private string currentParameterName;
        private string currentValue;
        private Dictionary<string, string> currentParameters;
        //        public Transform(Metric metric, Dictionary<string, string> paramaters)

        private readonly ILoggingAdapter _log;
        #endregion

        public MyListener(ILoggingAdapter log)
        {
            _log = log;
        }

        public override void EnterTransform_pipeline(VmstatsParser.Transform_pipelineContext context)
        {
            Console.WriteLine("EnterTransform_pipeline");
        }

        public override void ExitTransform_pipeline(VmstatsParser.Transform_pipelineContext context)
        {
            Console.WriteLine("ExitTransform_pipeline");
        }

        public override void EnterTransform(VmstatsParser.TransformContext context)
        {
            Console.WriteLine("EnterTransform");
        }

        public override void ExitTransform(VmstatsParser.TransformContext context)
        {
            Console.WriteLine("ExitTransform");
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
        }

        public override void ExitCombine(VmstatsParser.CombineContext context)
        {
            Console.WriteLine("ExitCombine");
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
    }
}