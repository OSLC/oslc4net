/******************************************************************************
 * Copyright (c) 2013 IBM Corporation.
 *
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * and Eclipse Distribution License v. 1.0 which accompanies this distribution. 
 *
 * The Eclipse Public License is available at http://www.eclipse.org/legal/epl-v10.html
 * and the Eclipse Distribution License is available at 
 * http://www.eclipse.org/org/documents/edl-v10.php.
 *
 * Contributors: 
 *
 *    Steve Pitchke - initial API and implementation
 *******************************************************************************/
grammar OslcOrderBy;

options {
	output=AST;
	language='CSharp3';
}

tokens {
	OSLC_ORDER_BY = 'oslc_order_by';
	SORT_TERMS = 'sort_terms';
	SORT_TERM = 'sort_term';
	SCOPED_SORT_TERM = 'scoped_sort_term';
	IDENTIFIER = 'identifier';
	PREFIXED_NAME = 'prefixed_name';
	NAMESPACE = 'ns';
}

@namespace { OSLC4Net.Core.Query.Impl }

@members {
    public OslcOrderByParser(string orderBy) :
		this(new CommonTokenStream(new OslcPrefixLexer(new ANTLRStringStream(orderBy))))
    {
    }
	
	public object Result
	{
		get { return oslc_order_by().Tree; }
	}
}

oslc_order_by : sort_terms 
	;

sort_terms  : sort_term ( ',' sort_term )* 
	; 

sort_term   : ( DIRECTION identifier ) | scoped_sort_term
	;

scoped_sort_term : identifier '{' sort_terms '}'
    ;

identifier    : prefixedName | ns;

prefixedName
    : PN_PREFIX? ':' PN_LOCAL
    ;

ns
    : PN_PREFIX? ':'
    ;

// $>

// $<Lexer

WS
    : (' '| '\t'| EOL)+ {  Skip(); }
    ;

DIRECTION       
    : PLUS
    | MINUS
    ;

fragment
PN_CHARS_U
    : PN_CHARS_BASE | '_'
    ;

fragment
PN_CHARS
    : PN_CHARS_U
    | MINUS
    | DIGIT
    | '\u00B7' 
    | '\u0300'..'\u036F'
    | '\u203F'..'\u2040'
    ;

PN_PREFIX
    : PN_CHARS_BASE ((PN_CHARS|DOT)* PN_CHARS)?
    ;

PN_LOCAL
    : ( PN_CHARS_U | DIGIT ) ((PN_CHARS|DOT)* PN_CHARS)?
    ;

fragment
PN_CHARS_BASE
    : 'A'..'Z'
    | 'a'..'z'
    | '\u00C0'..'\u00D6'
    | '\u00D8'..'\u00F6'
    | '\u00F8'..'\u02FF'
    | '\u0370'..'\u037D'
    | '\u037F'..'\u1FFF'
    | '\u200C'..'\u200D'
    | '\u2070'..'\u218F'
    | '\u2C00'..'\u2FEF'
    | '\u3001'..'\uD7FF'
    | '\uF900'..'\uFDCF'
    | '\uFDF0'..'\uFFFD'
    ;

fragment
DIGIT
    : '0'..'9'
    ;

fragment
DOT
    : '.'
    ;

fragment
EOL
    : '\n' | '\r'
    ;

fragment
PLUS
    : '+'
    ;

fragment
MINUS
    : '-'
    ;

ASTERISK
    : '*'
    ;

// $>
