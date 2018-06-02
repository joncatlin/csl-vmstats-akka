grammar vmstats;
/*
 * Parser rules
 */
transform_pipeline	: metric_name '->' transform (':' transform)? ;
transform			: transform_name ('{' parameter (',' parameter)? '}') ; 
parameter			: parameter_name '=' value_name ;
combine				: '(' transform_pipeline ',' transform_pipeline (',' transform_pipeline)? ')' ;
metric_name			: ID ;
transform_name		: ID ;
parameter_name		: ID ;
value_name			: ID ;
/*
compileUnit
	:	EOF
	;
*/
/*
 * Lexer Rules
 */
ID					: ([a-z] | [A-Z] | [0-9])+;
//WHITESPACE          : (' ' | '\t')+ -> skip;
WS
	:	' ' -> channel(HIDDEN)
	;
