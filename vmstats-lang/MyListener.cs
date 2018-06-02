using System;
using System.Collections.Generic;
using System.Text;

namespace vmstats.lang
{
    public class MyListener : VmstatsBaseListener
    {
        public override void EnterTransform_pipeline(VmstatsParser.Transform_pipelineContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void ExitTransform_pipeline(VmstatsParser.Transform_pipelineContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void EnterParameter(VmstatsParser.ParameterContext context)
        {
            Console.WriteLine(context.Payload.GetChild(0));
        }

        public override void ExitParameter(VmstatsParser.ParameterContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void EnterCombine(VmstatsParser.CombineContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void ExitCombine(VmstatsParser.CombineContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void EnterMetric_name(VmstatsParser.Metric_nameContext context)
        {
            Console.WriteLine(context.Payload.GetChild(0));
        }

        public override void ExitMetric_name(VmstatsParser.Metric_nameContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void EnterTransform_name(VmstatsParser.Transform_nameContext context)
        {
            Console.WriteLine(context.Payload.GetChild(0));
        }

        public override void ExitTransform_name(VmstatsParser.Transform_nameContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void EnterParameter_name(VmstatsParser.Parameter_nameContext context)
        {
            Console.WriteLine(context.Payload.GetChild(0));
        }

        public override void ExitParameter_name(VmstatsParser.Parameter_nameContext context)
        {
            Console.WriteLine("hi jon");
        }

        public override void EnterValue_name(VmstatsParser.Value_nameContext context)
        {
            Console.WriteLine(context.Payload.GetChild(0));
        }

        public override void ExitValue_name(VmstatsParser.Value_nameContext context)
        {
            Console.WriteLine("hi jon");
        }


        /*
                public virtual void EnterTransform([NotNull] VmstatsParser.TransformContext context) { }
                /// <summary>
                /// Exit a parse tree produced by <see cref="VmstatsParser.transform"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void ExitTransform([NotNull] VmstatsParser.TransformContext context) { }
                /// <summary>
                /// Enter a parse tree produced by <see cref="VmstatsParser.parameter"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void EnterParameter([NotNull] VmstatsParser.ParameterContext context) { }
                /// <summary>
                /// Exit a parse tree produced by <see cref="VmstatsParser.parameter"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void ExitParameter([NotNull] VmstatsParser.ParameterContext context) { }
                /// <summary>
                /// Enter a parse tree produced by <see cref="VmstatsParser.combine"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void EnterCombine([NotNull] VmstatsParser.CombineContext context) { }
                /// <summary>
                /// Exit a parse tree produced by <see cref="VmstatsParser.combine"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void ExitCombine([NotNull] VmstatsParser.CombineContext context) { }
                /// <summary>
                /// Enter a parse tree produced by <see cref="VmstatsParser.metric_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void EnterMetric_name([NotNull] VmstatsParser.Metric_nameContext context) { }
                /// <summary>
                /// Exit a parse tree produced by <see cref="VmstatsParser.metric_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void ExitMetric_name([NotNull] VmstatsParser.Metric_nameContext context) { }
                /// <summary>
                /// Enter a parse tree produced by <see cref="VmstatsParser.transform_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void EnterTransform_name([NotNull] VmstatsParser.Transform_nameContext context) { }
                /// <summary>
                /// Exit a parse tree produced by <see cref="VmstatsParser.transform_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void ExitTransform_name([NotNull] VmstatsParser.Transform_nameContext context) { }
                /// <summary>
                /// Enter a parse tree produced by <see cref="VmstatsParser.parameter_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void EnterParameter_name([NotNull] VmstatsParser.Parameter_nameContext context) { }
                /// <summary>
                /// Exit a parse tree produced by <see cref="VmstatsParser.parameter_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void ExitParameter_name([NotNull] VmstatsParser.Parameter_nameContext context) { }
                /// <summary>
                /// Enter a parse tree produced by <see cref="VmstatsParser.value_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void EnterValue_name([NotNull] VmstatsParser.Value_nameContext context) { }
                /// <summary>
                /// Exit a parse tree produced by <see cref="VmstatsParser.value_name"/>.
                /// <para>The default implementation does nothing.</para>
                /// </summary>
                /// <param name="context">The parse tree.</param>
                public virtual void ExitValue_name([NotNull] VmstatsParser.Value_nameContext context) { }

                /// <inheritdoc/>
                /// <remarks>The default implementation does nothing.</remarks>
                public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
                /// <inheritdoc/>
                /// <remarks>The default implementation does nothing.</remarks>
                public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
                /// <inheritdoc/>
                /// <remarks>The default implementation does nothing.</remarks>
                public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
                /// <inheritdoc/>
                /// <remarks>The default implementation does nothing.</remarks>
                public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
                */
    }
}