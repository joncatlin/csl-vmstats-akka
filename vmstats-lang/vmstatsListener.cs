//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from vmstats.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="vmstatsParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface IvmstatsListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.transform_pipeline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTransform_pipeline([NotNull] vmstatsParser.Transform_pipelineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.transform_pipeline"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTransform_pipeline([NotNull] vmstatsParser.Transform_pipelineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.transform"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTransform([NotNull] vmstatsParser.TransformContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.transform"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTransform([NotNull] vmstatsParser.TransformContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameter([NotNull] vmstatsParser.ParameterContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.parameter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameter([NotNull] vmstatsParser.ParameterContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.combine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCombine([NotNull] vmstatsParser.CombineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.combine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCombine([NotNull] vmstatsParser.CombineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.metric_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMetric_name([NotNull] vmstatsParser.Metric_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.metric_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMetric_name([NotNull] vmstatsParser.Metric_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.transform_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTransform_name([NotNull] vmstatsParser.Transform_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.transform_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTransform_name([NotNull] vmstatsParser.Transform_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.parameter_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameter_name([NotNull] vmstatsParser.Parameter_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.parameter_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameter_name([NotNull] vmstatsParser.Parameter_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="vmstatsParser.value_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValue_name([NotNull] vmstatsParser.Value_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="vmstatsParser.value_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValue_name([NotNull] vmstatsParser.Value_nameContext context);
}
