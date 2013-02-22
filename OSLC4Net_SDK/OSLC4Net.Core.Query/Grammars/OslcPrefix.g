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
grammar OslcPrefix;

options {
	language = 'CSharp3';
	output=AST; 
}

tokens {
	PREFIX_LIST = 'prefix_list';
	PREFIX = 'prefix';
}

@namespace { OSLC4Net.Core.Query.Impl }

@members {
    public OslcPrefixParser(string prefixes) :
		this(new CommonTokenStream(new OslcPrefixLexer(new ANTLRStringStream(prefixes))))
    {
    }
	
	public object Result
	{
		get { return oslc_prefixes().Tree; }
	}       
}


oslc_prefixes    : prefix_binding ( ',' prefix_binding )* -> ^( 'prefix_list' prefix_binding (prefix_binding)* ) 
	;
	
prefix_binding : PN_PREFIX '=' IRI_REF -> ^( 'prefix' PN_PREFIX IRI_REF )
    ;

// $>

// $<Lexer

WS
    : (' '| '\t'| EOL)+ { Skip(); }
    ;

fragment
EOL
    : '\n' | '\r'
    ;

PN_PREFIX
    : PN_CHARS_BASE ((PN_CHARS|DOT)* PN_CHARS)?
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
PN_CHARS
    : PN_CHARS_U
    | MINUS
    | DIGIT
    | '\u00B7' 
    | '\u0300'..'\u036F'
    | '\u203F'..'\u2040'
    ;

fragment
PN_CHARS_U
    : PN_CHARS_BASE | '_'
    ;

fragment
DIGIT
    : '0'..'9'
    ;

IRI_REF
    : LESS ( options {greedy=false;} : ~(LESS | GREATER | '"' | OPEN_CURLY_BRACE | CLOSE_CURLY_BRACE | '|' | '^' | '\\' | '`' | ('\u0000'..'\u0020')) )* GREATER 
    ;

COMMA
    : ','
    ;

DOT
    : '.'
    ;

MINUS
    : '-'
    ;

OPEN_CURLY_BRACE
    : '{'
    ;

CLOSE_CURLY_BRACE
    : '}'
    ;

LESS
    : '<'
    ;

GREATER
    : '>'
    ;

// $>
