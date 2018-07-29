grammar Vmstats;

/*
 * Parser rules
 */
/*
all					: transform_series EOF ;
*/
transform_series	: (combine (':' transform)* | transform_pipeline) ;
transform_pipeline	: metric_name '->' transform (':' transform)* ;
transform			: transform_name ('{' parameter (',' parameter)? '}')? ; 
parameter			: parameter_name '=' value_name ;
combine				: '(' transform_series '+' transform_series ('+' transform_series)* ')' ;
metric_name			: ID ;
transform_name		: ID ;
parameter_name		: ID ;
value_name			: ID ;

/*
 * Lexer Rules
 */
ID					: ([a-z] | [A-Z] | [0-9] | '_')+;
WHITESPACE          : (' ' | '\t' | '\r' | '\n')+ -> skip;
