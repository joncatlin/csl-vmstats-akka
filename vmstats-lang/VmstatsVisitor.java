// Generated from Vmstats.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.tree.ParseTreeVisitor;

/**
 * This interface defines a complete generic visitor for a parse tree produced
 * by {@link VmstatsParser}.
 *
 * @param <T> The return type of the visit operation. Use {@link Void} for
 * operations with no return type.
 */
public interface VmstatsVisitor<T> extends ParseTreeVisitor<T> {
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#transform_pipeline}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitTransform_pipeline(VmstatsParser.Transform_pipelineContext ctx);
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#transform}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitTransform(VmstatsParser.TransformContext ctx);
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#parameter}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitParameter(VmstatsParser.ParameterContext ctx);
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#combine}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitCombine(VmstatsParser.CombineContext ctx);
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#metric_name}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitMetric_name(VmstatsParser.Metric_nameContext ctx);
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#transform_name}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitTransform_name(VmstatsParser.Transform_nameContext ctx);
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#parameter_name}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitParameter_name(VmstatsParser.Parameter_nameContext ctx);
	/**
	 * Visit a parse tree produced by {@link VmstatsParser#value_name}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitValue_name(VmstatsParser.Value_nameContext ctx);
}