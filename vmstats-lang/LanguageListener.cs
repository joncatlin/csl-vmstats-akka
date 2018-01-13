//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Language.g4 by ANTLR 4.7.1

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
/// <see cref="LanguageParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public interface ILanguageListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="LanguageParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] LanguageParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LanguageParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] LanguageParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LanguageParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLine([NotNull] LanguageParser.LineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LanguageParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLine([NotNull] LanguageParser.LineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LanguageParser.vm_selection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVm_selection([NotNull] LanguageParser.Vm_selectionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LanguageParser.vm_selection"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVm_selection([NotNull] LanguageParser.Vm_selectionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LanguageParser.analysis"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnalysis([NotNull] LanguageParser.AnalysisContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LanguageParser.analysis"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnalysis([NotNull] LanguageParser.AnalysisContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LanguageParser.stats_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStats_name([NotNull] LanguageParser.Stats_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LanguageParser.stats_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStats_name([NotNull] LanguageParser.Stats_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LanguageParser.route"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRoute([NotNull] LanguageParser.RouteContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LanguageParser.route"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRoute([NotNull] LanguageParser.RouteContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="LanguageParser.end_point"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEnd_point([NotNull] LanguageParser.End_pointContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="LanguageParser.end_point"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEnd_point([NotNull] LanguageParser.End_pointContext context);
}
