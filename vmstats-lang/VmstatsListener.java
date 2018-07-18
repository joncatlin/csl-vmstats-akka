// Generated from Vmstats.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link VmstatsParser}.
 */
public interface VmstatsListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#transform_series}.
	 * @param ctx the parse tree
	 */
	void enterTransform_series(VmstatsParser.Transform_seriesContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#transform_series}.
	 * @param ctx the parse tree
	 */
	void exitTransform_series(VmstatsParser.Transform_seriesContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#transform_pipeline}.
	 * @param ctx the parse tree
	 */
	void enterTransform_pipeline(VmstatsParser.Transform_pipelineContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#transform_pipeline}.
	 * @param ctx the parse tree
	 */
	void exitTransform_pipeline(VmstatsParser.Transform_pipelineContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#transform}.
	 * @param ctx the parse tree
	 */
	void enterTransform(VmstatsParser.TransformContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#transform}.
	 * @param ctx the parse tree
	 */
	void exitTransform(VmstatsParser.TransformContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#parameter}.
	 * @param ctx the parse tree
	 */
	void enterParameter(VmstatsParser.ParameterContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#parameter}.
	 * @param ctx the parse tree
	 */
	void exitParameter(VmstatsParser.ParameterContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#combine}.
	 * @param ctx the parse tree
	 */
	void enterCombine(VmstatsParser.CombineContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#combine}.
	 * @param ctx the parse tree
	 */
	void exitCombine(VmstatsParser.CombineContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#metric_name}.
	 * @param ctx the parse tree
	 */
	void enterMetric_name(VmstatsParser.Metric_nameContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#metric_name}.
	 * @param ctx the parse tree
	 */
	void exitMetric_name(VmstatsParser.Metric_nameContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#transform_name}.
	 * @param ctx the parse tree
	 */
	void enterTransform_name(VmstatsParser.Transform_nameContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#transform_name}.
	 * @param ctx the parse tree
	 */
	void exitTransform_name(VmstatsParser.Transform_nameContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#parameter_name}.
	 * @param ctx the parse tree
	 */
	void enterParameter_name(VmstatsParser.Parameter_nameContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#parameter_name}.
	 * @param ctx the parse tree
	 */
	void exitParameter_name(VmstatsParser.Parameter_nameContext ctx);
	/**
	 * Enter a parse tree produced by {@link VmstatsParser#value_name}.
	 * @param ctx the parse tree
	 */
	void enterValue_name(VmstatsParser.Value_nameContext ctx);
	/**
	 * Exit a parse tree produced by {@link VmstatsParser#value_name}.
	 * @param ctx the parse tree
	 */
	void exitValue_name(VmstatsParser.Value_nameContext ctx);
}