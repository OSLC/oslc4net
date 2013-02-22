// $ANTLR 3.1.2 c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g 2013-02-21 18:46:24

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;


using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;

namespace  OSLC4Net.Core.Query.Impl 
{
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
public partial class OslcPrefixParser : Parser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "CLOSE_CURLY_BRACE", "COMMA", "DIGIT", "DOT", "EOL", "GREATER", "IRI_REF", "LESS", "MINUS", "OPEN_CURLY_BRACE", "PN_CHARS", "PN_CHARS_BASE", "PN_CHARS_U", "PN_PREFIX", "PREFIX", "PREFIX_LIST", "WS", "'='"
	};
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

	public OslcPrefixParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public OslcPrefixParser( ITokenStream input, RecognizerSharedState state )
		: base( input, state )
	{
		InitializeTreeAdaptor();
		if ( TreeAdaptor == null )
			TreeAdaptor = new CommonTreeAdaptor();
	}
		
	// Implement this function in your helper file to use a custom tree adaptor
	partial void InitializeTreeAdaptor();
	ITreeAdaptor adaptor;

	public ITreeAdaptor TreeAdaptor
	{
		get
		{
			return adaptor;
		}
		set
		{
			this.adaptor = value;
		}
	}

	public override string[] TokenNames { get { return OslcPrefixParser.tokenNames; } }
	public override string GrammarFileName { get { return "c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g"; } }


	    public OslcPrefixParser(string prefixes) :
			this(new CommonTokenStream(new OslcPrefixLexer(new ANTLRStringStream(prefixes))))
	    {
	    }
		
		public object Result
		{
			get { return oslc_prefixes().Tree; }
		}       


	#region Rules
	public class oslc_prefixes_return : ParserRuleReturnScope
	{
		internal object tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "oslc_prefixes"
	// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:43:0: oslc_prefixes : prefix_binding ( ',' prefix_binding )* -> ^( 'prefix_list' prefix_binding ( prefix_binding )* ) ;
	private OslcPrefixParser.oslc_prefixes_return oslc_prefixes(  )
	{
		OslcPrefixParser.oslc_prefixes_return retval = new OslcPrefixParser.oslc_prefixes_return();
		retval.start = input.LT(1);

		object root_0 = null;

		IToken char_literal2=null;
		OslcPrefixParser.prefix_binding_return prefix_binding1 = default(OslcPrefixParser.prefix_binding_return);
		OslcPrefixParser.prefix_binding_return prefix_binding3 = default(OslcPrefixParser.prefix_binding_return);

		object char_literal2_tree=null;
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_prefix_binding=new RewriteRuleSubtreeStream(adaptor,"rule prefix_binding");
		try
		{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:43:20: ( prefix_binding ( ',' prefix_binding )* -> ^( 'prefix_list' prefix_binding ( prefix_binding )* ) )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:43:20: prefix_binding ( ',' prefix_binding )*
			{
			PushFollow(Follow._prefix_binding_in_oslc_prefixes69);
			prefix_binding1=prefix_binding();

			state._fsp--;

			stream_prefix_binding.Add(prefix_binding1.Tree);
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:43:35: ( ',' prefix_binding )*
			for ( ; ; )
			{
				int alt1=2;
				int LA1_0 = input.LA(1);

				if ( (LA1_0==COMMA) )
				{
					alt1=1;
				}


				switch ( alt1 )
				{
				case 1:
					// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:43:37: ',' prefix_binding
					{
					char_literal2=(IToken)Match(input,COMMA,Follow._COMMA_in_oslc_prefixes73);  
					stream_COMMA.Add(char_literal2);

					PushFollow(Follow._prefix_binding_in_oslc_prefixes75);
					prefix_binding3=prefix_binding();

					state._fsp--;

					stream_prefix_binding.Add(prefix_binding3.Tree);

					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;




			{
			// AST REWRITE
			// elements: PREFIX_LIST, prefix_binding, prefix_binding
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (object)adaptor.Nil();
			// 43:59: -> ^( 'prefix_list' prefix_binding ( prefix_binding )* )
			{
				// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:43:62: ^( 'prefix_list' prefix_binding ( prefix_binding )* )
				{
				object root_1 = (object)adaptor.Nil();
				root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(PREFIX_LIST, "PREFIX_LIST"), root_1);

				adaptor.AddChild(root_1, stream_prefix_binding.NextTree());
				// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:43:94: ( prefix_binding )*
				while ( stream_prefix_binding.HasNext )
				{
					adaptor.AddChild(root_1, stream_prefix_binding.NextTree());

				}
				stream_prefix_binding.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}

			}

			retval.stop = input.LT(-1);

			retval.tree = (object)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (object)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "oslc_prefixes"

	public class prefix_binding_return : ParserRuleReturnScope
	{
		internal object tree;
		public override object Tree { get { return tree; } }
	}

	// $ANTLR start "prefix_binding"
	// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:46:0: prefix_binding : PN_PREFIX '=' IRI_REF -> ^( 'prefix' PN_PREFIX IRI_REF ) ;
	private OslcPrefixParser.prefix_binding_return prefix_binding(  )
	{
		OslcPrefixParser.prefix_binding_return retval = new OslcPrefixParser.prefix_binding_return();
		retval.start = input.LT(1);

		object root_0 = null;

		IToken PN_PREFIX4=null;
		IToken char_literal5=null;
		IToken IRI_REF6=null;

		object PN_PREFIX4_tree=null;
		object char_literal5_tree=null;
		object IRI_REF6_tree=null;
		RewriteRuleITokenStream stream_PN_PREFIX=new RewriteRuleITokenStream(adaptor,"token PN_PREFIX");
		RewriteRuleITokenStream stream_21=new RewriteRuleITokenStream(adaptor,"token 21");
		RewriteRuleITokenStream stream_IRI_REF=new RewriteRuleITokenStream(adaptor,"token IRI_REF");

		try
		{
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:46:18: ( PN_PREFIX '=' IRI_REF -> ^( 'prefix' PN_PREFIX IRI_REF ) )
			// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:46:18: PN_PREFIX '=' IRI_REF
			{
			PN_PREFIX4=(IToken)Match(input,PN_PREFIX,Follow._PN_PREFIX_in_prefix_binding105);  
			stream_PN_PREFIX.Add(PN_PREFIX4);

			char_literal5=(IToken)Match(input,21,Follow._21_in_prefix_binding107);  
			stream_21.Add(char_literal5);

			IRI_REF6=(IToken)Match(input,IRI_REF,Follow._IRI_REF_in_prefix_binding109);  
			stream_IRI_REF.Add(IRI_REF6);



			{
			// AST REWRITE
			// elements: PREFIX, PN_PREFIX, IRI_REF
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			retval.tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.tree:null);

			root_0 = (object)adaptor.Nil();
			// 46:40: -> ^( 'prefix' PN_PREFIX IRI_REF )
			{
				// c:\\Users\\pitschke\\oslc4net\\OSLC4Net_SDK\\Query\\Grammars\\OslcPrefix.g:46:43: ^( 'prefix' PN_PREFIX IRI_REF )
				{
				object root_1 = (object)adaptor.Nil();
				root_1 = (object)adaptor.BecomeRoot((object)adaptor.Create(PREFIX, "PREFIX"), root_1);

				adaptor.AddChild(root_1, stream_PN_PREFIX.NextNode());
				adaptor.AddChild(root_1, stream_IRI_REF.NextNode());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.tree = root_0;
			}

			}

			retval.stop = input.LT(-1);

			retval.tree = (object)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.tree, retval.start, retval.stop);

		}
		catch ( RecognitionException re )
		{
			ReportError(re);
			Recover(input,re);
		retval.tree = (object)adaptor.ErrorNode(input, retval.start, input.LT(-1), re);

		}
		finally
		{
		}
		return retval;
	}
	// $ANTLR end "prefix_binding"
	#endregion Rules


	#region Follow sets
	static class Follow
	{
		public static readonly BitSet _prefix_binding_in_oslc_prefixes69 = new BitSet(new ulong[]{0x22UL});
		public static readonly BitSet _COMMA_in_oslc_prefixes73 = new BitSet(new ulong[]{0x20000UL});
		public static readonly BitSet _prefix_binding_in_oslc_prefixes75 = new BitSet(new ulong[]{0x22UL});
		public static readonly BitSet _PN_PREFIX_in_prefix_binding105 = new BitSet(new ulong[]{0x200000UL});
		public static readonly BitSet _21_in_prefix_binding107 = new BitSet(new ulong[]{0x400UL});
		public static readonly BitSet _IRI_REF_in_prefix_binding109 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace  OSLC4Net.Core.Query.Impl 
