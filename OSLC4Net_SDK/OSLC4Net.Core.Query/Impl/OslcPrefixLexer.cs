// $ANTLR 3.1.2 c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g 2013-02-21 18:46:25

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

public partial class OslcPrefixLexer : Lexer
{
	public const int EOF=-1;
	public const int T__21=21;
	public const int CLOSE_CURLY_BRACE=4;
	public const int COMMA=5;
	public const int DIGIT=6;
	public const int DOT=7;
	public const int EOL=8;
	public const int GREATER=9;
	public const int IRI_REF=10;
	public const int LESS=11;
	public const int MINUS=12;
	public const int OPEN_CURLY_BRACE=13;
	public const int PN_CHARS=14;
	public const int PN_CHARS_BASE=15;
	public const int PN_CHARS_U=16;
	public const int PN_PREFIX=17;
	public const int PREFIX=18;
	public const int PREFIX_LIST=19;
	public const int WS=20;

    // delegates
    // delegators

	public OslcPrefixLexer() {}
	public OslcPrefixLexer( ICharStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public OslcPrefixLexer( ICharStream input, RecognizerSharedState state )
		: base( input, state )
	{

	}
	public override string GrammarFileName { get { return "c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g"; } }

	// $ANTLR start "PREFIX"
	private void mPREFIX()
	{
		try
		{
			int _type = PREFIX;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:7:10: ( 'prefix' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:7:10: 'prefix'
			{
			Match("prefix"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PREFIX"

	// $ANTLR start "PREFIX_LIST"
	private void mPREFIX_LIST()
	{
		try
		{
			int _type = PREFIX_LIST;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:8:15: ( 'prefix_list' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:8:15: 'prefix_list'
			{
			Match("prefix_list"); 


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PREFIX_LIST"

	// $ANTLR start "T__21"
	private void mT__21()
	{
		try
		{
			int _type = T__21;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:9:9: ( '=' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:9:9: '='
			{
			Match('='); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "T__21"

	// $ANTLR start "WS"
	private void mWS()
	{
		try
		{
			int _type = WS;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:54:7: ( ( ' ' | '\\t' | EOL )+ )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:54:7: ( ' ' | '\\t' | EOL )+
			{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:54:7: ( ' ' | '\\t' | EOL )+
			int cnt1=0;
			for ( ; ; )
			{
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( ((LA1_0>='\t' && LA1_0<='\n')||LA1_0=='\r'||LA1_0==' ') )
				{
					alt1=1;
				}


				switch ( alt1 )
				{
				case 1:
					// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:
					{
					input.Consume();


					}
					break;

				default:
					if ( cnt1 >= 1 )
						goto loop1;

					EarlyExitException eee1 = new EarlyExitException( 1, input );
					throw eee1;
				}
				cnt1++;
			}
			loop1:
				;


			 Skip(); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "WS"

	// $ANTLR start "EOL"
	private void mEOL()
	{
		try
		{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:60:7: ( '\\n' | '\\r' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:
			{
			if ( input.LA(1)=='\n'||input.LA(1)=='\r' )
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "EOL"

	// $ANTLR start "PN_PREFIX"
	private void mPN_PREFIX()
	{
		try
		{
			int _type = PN_PREFIX;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:63:7: ( PN_CHARS_BASE ( ( PN_CHARS | DOT )* PN_CHARS )? )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:63:7: PN_CHARS_BASE ( ( PN_CHARS | DOT )* PN_CHARS )?
			{
			mPN_CHARS_BASE(); 
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:63:21: ( ( PN_CHARS | DOT )* PN_CHARS )?
			int alt3=2;
			int LA3_0 = input.LA(1);

			if ( ((LA3_0>='-' && LA3_0<='.')||(LA3_0>='0' && LA3_0<='9')||(LA3_0>='A' && LA3_0<='Z')||LA3_0=='_'||(LA3_0>='a' && LA3_0<='z')||LA3_0=='\u00B7'||(LA3_0>='\u00C0' && LA3_0<='\u00D6')||(LA3_0>='\u00D8' && LA3_0<='\u00F6')||(LA3_0>='\u00F8' && LA3_0<='\u037D')||(LA3_0>='\u037F' && LA3_0<='\u1FFF')||(LA3_0>='\u200C' && LA3_0<='\u200D')||(LA3_0>='\u203F' && LA3_0<='\u2040')||(LA3_0>='\u2070' && LA3_0<='\u218F')||(LA3_0>='\u2C00' && LA3_0<='\u2FEF')||(LA3_0>='\u3001' && LA3_0<='\uD7FF')||(LA3_0>='\uF900' && LA3_0<='\uFDCF')||(LA3_0>='\uFDF0' && LA3_0<='\uFFFD')) )
			{
				alt3=1;
			}
			switch ( alt3 )
			{
			case 1:
				// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:63:22: ( PN_CHARS | DOT )* PN_CHARS
				{
				// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:63:22: ( PN_CHARS | DOT )*
				for ( ; ; )
				{
					int alt2=2;
					int LA2_0 = input.LA(1);

					if ( (LA2_0=='-'||(LA2_0>='0' && LA2_0<='9')||(LA2_0>='A' && LA2_0<='Z')||LA2_0=='_'||(LA2_0>='a' && LA2_0<='z')||LA2_0=='\u00B7'||(LA2_0>='\u00C0' && LA2_0<='\u00D6')||(LA2_0>='\u00D8' && LA2_0<='\u00F6')||(LA2_0>='\u00F8' && LA2_0<='\u037D')||(LA2_0>='\u037F' && LA2_0<='\u1FFF')||(LA2_0>='\u200C' && LA2_0<='\u200D')||(LA2_0>='\u203F' && LA2_0<='\u2040')||(LA2_0>='\u2070' && LA2_0<='\u218F')||(LA2_0>='\u2C00' && LA2_0<='\u2FEF')||(LA2_0>='\u3001' && LA2_0<='\uD7FF')||(LA2_0>='\uF900' && LA2_0<='\uFDCF')||(LA2_0>='\uFDF0' && LA2_0<='\uFFFD')) )
					{
						int LA2_1 = input.LA(2);

						if ( ((LA2_1>='-' && LA2_1<='.')||(LA2_1>='0' && LA2_1<='9')||(LA2_1>='A' && LA2_1<='Z')||LA2_1=='_'||(LA2_1>='a' && LA2_1<='z')||LA2_1=='\u00B7'||(LA2_1>='\u00C0' && LA2_1<='\u00D6')||(LA2_1>='\u00D8' && LA2_1<='\u00F6')||(LA2_1>='\u00F8' && LA2_1<='\u037D')||(LA2_1>='\u037F' && LA2_1<='\u1FFF')||(LA2_1>='\u200C' && LA2_1<='\u200D')||(LA2_1>='\u203F' && LA2_1<='\u2040')||(LA2_1>='\u2070' && LA2_1<='\u218F')||(LA2_1>='\u2C00' && LA2_1<='\u2FEF')||(LA2_1>='\u3001' && LA2_1<='\uD7FF')||(LA2_1>='\uF900' && LA2_1<='\uFDCF')||(LA2_1>='\uFDF0' && LA2_1<='\uFFFD')) )
						{
							alt2=1;
						}


					}
					else if ( (LA2_0=='.') )
					{
						alt2=1;
					}


					switch ( alt2 )
					{
					case 1:
						// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:
						{
						input.Consume();


						}
						break;

					default:
						goto loop2;
					}
				}

				loop2:
					;


				mPN_CHARS(); 

				}
				break;

			}


			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "PN_PREFIX"

	// $ANTLR start "PN_CHARS_BASE"
	private void mPN_CHARS_BASE()
	{
		try
		{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:69:7: ( 'A' .. 'Z' | 'a' .. 'z' | '\\u00C0' .. '\\u00D6' | '\\u00D8' .. '\\u00F6' | '\\u00F8' .. '\\u02FF' | '\\u0370' .. '\\u037D' | '\\u037F' .. '\\u1FFF' | '\\u200C' .. '\\u200D' | '\\u2070' .. '\\u218F' | '\\u2C00' .. '\\u2FEF' | '\\u3001' .. '\\uD7FF' | '\\uF900' .. '\\uFDCF' | '\\uFDF0' .. '\\uFFFD' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:
			{
			if ( (input.LA(1)>='A' && input.LA(1)<='Z')||(input.LA(1)>='a' && input.LA(1)<='z')||(input.LA(1)>='\u00C0' && input.LA(1)<='\u00D6')||(input.LA(1)>='\u00D8' && input.LA(1)<='\u00F6')||(input.LA(1)>='\u00F8' && input.LA(1)<='\u02FF')||(input.LA(1)>='\u0370' && input.LA(1)<='\u037D')||(input.LA(1)>='\u037F' && input.LA(1)<='\u1FFF')||(input.LA(1)>='\u200C' && input.LA(1)<='\u200D')||(input.LA(1)>='\u2070' && input.LA(1)<='\u218F')||(input.LA(1)>='\u2C00' && input.LA(1)<='\u2FEF')||(input.LA(1)>='\u3001' && input.LA(1)<='\uD7FF')||(input.LA(1)>='\uF900' && input.LA(1)<='\uFDCF')||(input.LA(1)>='\uFDF0' && input.LA(1)<='\uFFFD') )
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "PN_CHARS_BASE"

	// $ANTLR start "PN_CHARS"
	private void mPN_CHARS()
	{
		try
		{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:86:7: ( PN_CHARS_U | MINUS | DIGIT | '\\u00B7' | '\\u0300' .. '\\u036F' | '\\u203F' .. '\\u2040' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:
			{
			if ( input.LA(1)=='-'||(input.LA(1)>='0' && input.LA(1)<='9')||(input.LA(1)>='A' && input.LA(1)<='Z')||input.LA(1)=='_'||(input.LA(1)>='a' && input.LA(1)<='z')||input.LA(1)=='\u00B7'||(input.LA(1)>='\u00C0' && input.LA(1)<='\u00D6')||(input.LA(1)>='\u00D8' && input.LA(1)<='\u00F6')||(input.LA(1)>='\u00F8' && input.LA(1)<='\u037D')||(input.LA(1)>='\u037F' && input.LA(1)<='\u1FFF')||(input.LA(1)>='\u200C' && input.LA(1)<='\u200D')||(input.LA(1)>='\u203F' && input.LA(1)<='\u2040')||(input.LA(1)>='\u2070' && input.LA(1)<='\u218F')||(input.LA(1)>='\u2C00' && input.LA(1)<='\u2FEF')||(input.LA(1)>='\u3001' && input.LA(1)<='\uD7FF')||(input.LA(1)>='\uF900' && input.LA(1)<='\uFDCF')||(input.LA(1)>='\uFDF0' && input.LA(1)<='\uFFFD') )
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "PN_CHARS"

	// $ANTLR start "PN_CHARS_U"
	private void mPN_CHARS_U()
	{
		try
		{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:96:7: ( PN_CHARS_BASE | '_' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:
			{
			if ( (input.LA(1)>='A' && input.LA(1)<='Z')||input.LA(1)=='_'||(input.LA(1)>='a' && input.LA(1)<='z')||(input.LA(1)>='\u00C0' && input.LA(1)<='\u00D6')||(input.LA(1)>='\u00D8' && input.LA(1)<='\u00F6')||(input.LA(1)>='\u00F8' && input.LA(1)<='\u02FF')||(input.LA(1)>='\u0370' && input.LA(1)<='\u037D')||(input.LA(1)>='\u037F' && input.LA(1)<='\u1FFF')||(input.LA(1)>='\u200C' && input.LA(1)<='\u200D')||(input.LA(1)>='\u2070' && input.LA(1)<='\u218F')||(input.LA(1)>='\u2C00' && input.LA(1)<='\u2FEF')||(input.LA(1)>='\u3001' && input.LA(1)<='\uD7FF')||(input.LA(1)>='\uF900' && input.LA(1)<='\uFDCF')||(input.LA(1)>='\uFDF0' && input.LA(1)<='\uFFFD') )
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "PN_CHARS_U"

	// $ANTLR start "DIGIT"
	private void mDIGIT()
	{
		try
		{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:101:7: ( '0' .. '9' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:
			{
			if ( (input.LA(1)>='0' && input.LA(1)<='9') )
			{
				input.Consume();

			}
			else
			{
				MismatchedSetException mse = new MismatchedSetException(null,input);
				Recover(mse);
				throw mse;}


			}

		}
		finally
		{
		}
	}
	// $ANTLR end "DIGIT"

	// $ANTLR start "IRI_REF"
	private void mIRI_REF()
	{
		try
		{
			int _type = IRI_REF;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:104:7: ( LESS ( options {greedy=false; } :~ ( LESS | GREATER | '\"' | OPEN_CURLY_BRACE | CLOSE_CURLY_BRACE | '|' | '^' | '\\\\' | '`' | ( '\\u0000' .. '\\u0020' ) ) )* GREATER )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:104:7: LESS ( options {greedy=false; } :~ ( LESS | GREATER | '\"' | OPEN_CURLY_BRACE | CLOSE_CURLY_BRACE | '|' | '^' | '\\\\' | '`' | ( '\\u0000' .. '\\u0020' ) ) )* GREATER
			{
			mLESS(); 
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:104:12: ( options {greedy=false; } :~ ( LESS | GREATER | '\"' | OPEN_CURLY_BRACE | CLOSE_CURLY_BRACE | '|' | '^' | '\\\\' | '`' | ( '\\u0000' .. '\\u0020' ) ) )*
			for ( ; ; )
			{
				int alt4=2;
				int LA4_0 = input.LA(1);

				if ( (LA4_0=='!'||(LA4_0>='#' && LA4_0<=';')||LA4_0=='='||(LA4_0>='?' && LA4_0<='[')||LA4_0==']'||LA4_0=='_'||(LA4_0>='a' && LA4_0<='z')||(LA4_0>='~' && LA4_0<='\uFFFF')) )
				{
					alt4=1;
				}
				else if ( (LA4_0=='>') )
				{
					alt4=2;
				}


				switch ( alt4 )
				{
				case 1:
					// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:104:40: ~ ( LESS | GREATER | '\"' | OPEN_CURLY_BRACE | CLOSE_CURLY_BRACE | '|' | '^' | '\\\\' | '`' | ( '\\u0000' .. '\\u0020' ) )
					{
					input.Consume();


					}
					break;

				default:
					goto loop4;
				}
			}

			loop4:
				;


			mGREATER(); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "IRI_REF"

	// $ANTLR start "COMMA"
	private void mCOMMA()
	{
		try
		{
			int _type = COMMA;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:108:7: ( ',' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:108:7: ','
			{
			Match(','); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "COMMA"

	// $ANTLR start "DOT"
	private void mDOT()
	{
		try
		{
			int _type = DOT;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:112:7: ( '.' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:112:7: '.'
			{
			Match('.'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "DOT"

	// $ANTLR start "MINUS"
	private void mMINUS()
	{
		try
		{
			int _type = MINUS;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:116:7: ( '-' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:116:7: '-'
			{
			Match('-'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "MINUS"

	// $ANTLR start "OPEN_CURLY_BRACE"
	private void mOPEN_CURLY_BRACE()
	{
		try
		{
			int _type = OPEN_CURLY_BRACE;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:120:7: ( '{' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:120:7: '{'
			{
			Match('{'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "OPEN_CURLY_BRACE"

	// $ANTLR start "CLOSE_CURLY_BRACE"
	private void mCLOSE_CURLY_BRACE()
	{
		try
		{
			int _type = CLOSE_CURLY_BRACE;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:124:7: ( '}' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:124:7: '}'
			{
			Match('}'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "CLOSE_CURLY_BRACE"

	// $ANTLR start "LESS"
	private void mLESS()
	{
		try
		{
			int _type = LESS;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:128:7: ( '<' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:128:7: '<'
			{
			Match('<'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "LESS"

	// $ANTLR start "GREATER"
	private void mGREATER()
	{
		try
		{
			int _type = GREATER;
			int _channel = DefaultTokenChannel;
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:132:7: ( '>' )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:132:7: '>'
			{
			Match('>'); 

			}

			state.type = _type;
			state.channel = _channel;
		}
		finally
		{
		}
	}
	// $ANTLR end "GREATER"

	public override void mTokens()
	{
		// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:10: ( PREFIX | PREFIX_LIST | T__21 | WS | PN_PREFIX | IRI_REF | COMMA | DOT | MINUS | OPEN_CURLY_BRACE | CLOSE_CURLY_BRACE | LESS | GREATER )
		int alt5=13;
		alt5 = dfa5.Predict(input);
		switch ( alt5 )
		{
		case 1:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:10: PREFIX
			{
			mPREFIX(); 

			}
			break;
		case 2:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:17: PREFIX_LIST
			{
			mPREFIX_LIST(); 

			}
			break;
		case 3:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:29: T__21
			{
			mT__21(); 

			}
			break;
		case 4:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:35: WS
			{
			mWS(); 

			}
			break;
		case 5:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:38: PN_PREFIX
			{
			mPN_PREFIX(); 

			}
			break;
		case 6:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:48: IRI_REF
			{
			mIRI_REF(); 

			}
			break;
		case 7:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:56: COMMA
			{
			mCOMMA(); 

			}
			break;
		case 8:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:62: DOT
			{
			mDOT(); 

			}
			break;
		case 9:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:66: MINUS
			{
			mMINUS(); 

			}
			break;
		case 10:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:72: OPEN_CURLY_BRACE
			{
			mOPEN_CURLY_BRACE(); 

			}
			break;
		case 11:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:89: CLOSE_CURLY_BRACE
			{
			mCLOSE_CURLY_BRACE(); 

			}
			break;
		case 12:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:107: LESS
			{
			mLESS(); 

			}
			break;
		case 13:
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:1:112: GREATER
			{
			mGREATER(); 

			}
			break;

		}

	}


	#region DFA
	DFA5 dfa5;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa5 = new DFA5( this );
	}

	class DFA5 : DFA
	{

		const string DFA5_eotS =
			"\x1\xFFFF\x1\x4\x3\xFFFF\x1\xD\x6\xFFFF\x1\x4\x2\xFFFF\x3\x4\x1\x14\x1"+
			"\x4\x1\xFFFF\x3\x4\x1\x19\x1\xFFFF";
		const string DFA5_eofS =
			"\x1A\xFFFF";
		const string DFA5_minS =
			"\x1\x9\x1\x72\x3\xFFFF\x1\x21\x6\xFFFF\x1\x65\x2\xFFFF\x1\x66\x1\x69"+
			"\x1\x78\x1\x2D\x1\x6C\x1\xFFFF\x1\x69\x1\x73\x1\x74\x1\x2D\x1\xFFFF";
		const string DFA5_maxS =
			"\x1\xFFFD\x1\x72\x3\xFFFF\x1\xFFFF\x6\xFFFF\x1\x65\x2\xFFFF\x1\x66\x1"+
			"\x69\x1\x78\x1\xFFFD\x1\x6C\x1\xFFFF\x1\x69\x1\x73\x1\x74\x1\xFFFD\x1"+
			"\xFFFF";
		const string DFA5_acceptS =
			"\x2\xFFFF\x1\x3\x1\x4\x1\x5\x1\xFFFF\x1\x7\x1\x8\x1\x9\x1\xA\x1\xB\x1"+
			"\xD\x1\xFFFF\x1\xC\x1\x6\x5\xFFFF\x1\x1\x4\xFFFF\x1\x2";
		const string DFA5_specialS =
			"\x1A\xFFFF}>";
		static readonly string[] DFA5_transitionS =
			{
				"\x2\x3\x2\xFFFF\x1\x3\x12\xFFFF\x1\x3\xB\xFFFF\x1\x6\x1\x8\x1\x7\xD"+
				"\xFFFF\x1\x5\x1\x2\x1\xB\x2\xFFFF\x1A\x4\x6\xFFFF\xF\x4\x1\x1\xA\x4"+
				"\x1\x9\x1\xFFFF\x1\xA\x42\xFFFF\x17\x4\x1\xFFFF\x1F\x4\x1\xFFFF\x208"+
				"\x4\x70\xFFFF\xE\x4\x1\xFFFF\x1C81\x4\xC\xFFFF\x2\x4\x62\xFFFF\x120"+
				"\x4\xA70\xFFFF\x3F0\x4\x11\xFFFF\xA7FF\x4\x2100\xFFFF\x4D0\x4\x20\xFFFF"+
				"\x20E\x4",
				"\x1\xC",
				"",
				"",
				"",
				"\x1\xE\x1\xFFFF\x19\xE\x1\xFFFF\x1F\xE\x1\xFFFF\x1\xE\x1\xFFFF\x1\xE"+
				"\x1\xFFFF\x1A\xE\x3\xFFFF\xFF82\xE",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\xF",
				"",
				"",
				"\x1\x10",
				"\x1\x11",
				"\x1\x12",
				"\x2\x4\x1\xFFFF\xA\x4\x7\xFFFF\x1A\x4\x4\xFFFF\x1\x13\x1\xFFFF\x1A\x4"+
				"\x3C\xFFFF\x1\x4\x8\xFFFF\x17\x4\x1\xFFFF\x1F\x4\x1\xFFFF\x286\x4\x1"+
				"\xFFFF\x1C81\x4\xC\xFFFF\x2\x4\x31\xFFFF\x2\x4\x2F\xFFFF\x120\x4\xA70"+
				"\xFFFF\x3F0\x4\x11\xFFFF\xA7FF\x4\x2100\xFFFF\x4D0\x4\x20\xFFFF\x20E"+
				"\x4",
				"\x1\x15",
				"",
				"\x1\x16",
				"\x1\x17",
				"\x1\x18",
				"\x2\x4\x1\xFFFF\xA\x4\x7\xFFFF\x1A\x4\x4\xFFFF\x1\x4\x1\xFFFF\x1A\x4"+
				"\x3C\xFFFF\x1\x4\x8\xFFFF\x17\x4\x1\xFFFF\x1F\x4\x1\xFFFF\x286\x4\x1"+
				"\xFFFF\x1C81\x4\xC\xFFFF\x2\x4\x31\xFFFF\x2\x4\x2F\xFFFF\x120\x4\xA70"+
				"\xFFFF\x3F0\x4\x11\xFFFF\xA7FF\x4\x2100\xFFFF\x4D0\x4\x20\xFFFF\x20E"+
				"\x4",
				""
			};

		static readonly short[] DFA5_eot = DFA.UnpackEncodedString(DFA5_eotS);
		static readonly short[] DFA5_eof = DFA.UnpackEncodedString(DFA5_eofS);
		static readonly char[] DFA5_min = DFA.UnpackEncodedStringToUnsignedChars(DFA5_minS);
		static readonly char[] DFA5_max = DFA.UnpackEncodedStringToUnsignedChars(DFA5_maxS);
		static readonly short[] DFA5_accept = DFA.UnpackEncodedString(DFA5_acceptS);
		static readonly short[] DFA5_special = DFA.UnpackEncodedString(DFA5_specialS);
		static readonly short[][] DFA5_transition;

		static DFA5()
		{
			int numStates = DFA5_transitionS.Length;
			DFA5_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA5_transition[i] = DFA.UnpackEncodedString(DFA5_transitionS[i]);
			}
		}

		public DFA5( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 5;
			this.eot = DFA5_eot;
			this.eof = DFA5_eof;
			this.min = DFA5_min;
			this.max = DFA5_max;
			this.accept = DFA5_accept;
			this.special = DFA5_special;
			this.transition = DFA5_transition;
		}
		public override string GetDescription()
		{
			return "1:0: Tokens : ( PREFIX | PREFIX_LIST | T__21 | WS | PN_PREFIX | IRI_REF | COMMA | DOT | MINUS | OPEN_CURLY_BRACE | CLOSE_CURLY_BRACE | LESS | GREATER );";
		}
	}

 
	#endregion

}
