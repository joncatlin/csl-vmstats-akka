// define a grammar called language
grammar Language;

/*
 * Parser rules
 */
statement			: line+ EOF;
line				: vm_selection ':' analysis NEWLINE;
vm_selection		: '\\' PATTERN '\\';
analysis			: stats_name '->' route;
stats_name			: ID;
route				: stats_name | end_point;
end_point			: '"' ID '"';



/*
 * Lexer rules
 */
ID					: [a-z]+ ;
WHITESPACE          : (' ' | '\t')+ -> skip;
NEWLINE             : ('\r'? '\n' | '\r')+ ;
PATTERN				: ([a-z] | [A-Z] | '-' | '+' | '*' | '\\' | '[' | ']' | '(' | ')')+;