// Generated from Vmstats.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class VmstatsParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		ID=10, WHITESPACE=11;
	public static final int
		RULE_transform_pipeline = 0, RULE_transform = 1, RULE_parameter = 2, RULE_combine = 3, 
		RULE_metric_name = 4, RULE_transform_name = 5, RULE_parameter_name = 6, 
		RULE_value_name = 7;
	public static final String[] ruleNames = {
		"transform_pipeline", "transform", "parameter", "combine", "metric_name", 
		"transform_name", "parameter_name", "value_name"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "'->'", "':'", "'{'", "','", "'}'", "'='", "'('", "'+'", "')'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, null, null, null, null, null, null, null, null, null, "ID", "WHITESPACE"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "Vmstats.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public VmstatsParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class Transform_pipelineContext extends ParserRuleContext {
		public Metric_nameContext metric_name() {
			return getRuleContext(Metric_nameContext.class,0);
		}
		public List<TransformContext> transform() {
			return getRuleContexts(TransformContext.class);
		}
		public TransformContext transform(int i) {
			return getRuleContext(TransformContext.class,i);
		}
		public CombineContext combine() {
			return getRuleContext(CombineContext.class,0);
		}
		public Transform_pipelineContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_transform_pipeline; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterTransform_pipeline(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitTransform_pipeline(this);
		}
	}

	public final Transform_pipelineContext transform_pipeline() throws RecognitionException {
		Transform_pipelineContext _localctx = new Transform_pipelineContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_transform_pipeline);
		int _la;
		try {
			setState(27);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ID:
				enterOuterAlt(_localctx, 1);
				{
				setState(16);
				metric_name();
				setState(17);
				match(T__0);
				setState(18);
				transform();
				setState(23);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==T__1) {
					{
					{
					setState(19);
					match(T__1);
					setState(20);
					transform();
					}
					}
					setState(25);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				}
				break;
			case T__6:
				enterOuterAlt(_localctx, 2);
				{
				setState(26);
				combine();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TransformContext extends ParserRuleContext {
		public Transform_nameContext transform_name() {
			return getRuleContext(Transform_nameContext.class,0);
		}
		public List<ParameterContext> parameter() {
			return getRuleContexts(ParameterContext.class);
		}
		public ParameterContext parameter(int i) {
			return getRuleContext(ParameterContext.class,i);
		}
		public TransformContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_transform; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterTransform(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitTransform(this);
		}
	}

	public final TransformContext transform() throws RecognitionException {
		TransformContext _localctx = new TransformContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_transform);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(29);
			transform_name();
			setState(38);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__2) {
				{
				setState(30);
				match(T__2);
				setState(31);
				parameter();
				setState(34);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__3) {
					{
					setState(32);
					match(T__3);
					setState(33);
					parameter();
					}
				}

				setState(36);
				match(T__4);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParameterContext extends ParserRuleContext {
		public Parameter_nameContext parameter_name() {
			return getRuleContext(Parameter_nameContext.class,0);
		}
		public Value_nameContext value_name() {
			return getRuleContext(Value_nameContext.class,0);
		}
		public ParameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterParameter(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitParameter(this);
		}
	}

	public final ParameterContext parameter() throws RecognitionException {
		ParameterContext _localctx = new ParameterContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_parameter);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(40);
			parameter_name();
			setState(41);
			match(T__5);
			setState(42);
			value_name();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CombineContext extends ParserRuleContext {
		public List<Transform_pipelineContext> transform_pipeline() {
			return getRuleContexts(Transform_pipelineContext.class);
		}
		public Transform_pipelineContext transform_pipeline(int i) {
			return getRuleContext(Transform_pipelineContext.class,i);
		}
		public CombineContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_combine; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterCombine(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitCombine(this);
		}
	}

	public final CombineContext combine() throws RecognitionException {
		CombineContext _localctx = new CombineContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_combine);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(44);
			match(T__6);
			setState(45);
			transform_pipeline();
			setState(46);
			match(T__7);
			setState(47);
			transform_pipeline();
			setState(52);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__7) {
				{
				{
				setState(48);
				match(T__7);
				setState(49);
				transform_pipeline();
				}
				}
				setState(54);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(55);
			match(T__8);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Metric_nameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(VmstatsParser.ID, 0); }
		public Metric_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_metric_name; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterMetric_name(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitMetric_name(this);
		}
	}

	public final Metric_nameContext metric_name() throws RecognitionException {
		Metric_nameContext _localctx = new Metric_nameContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_metric_name);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(57);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Transform_nameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(VmstatsParser.ID, 0); }
		public Transform_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_transform_name; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterTransform_name(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitTransform_name(this);
		}
	}

	public final Transform_nameContext transform_name() throws RecognitionException {
		Transform_nameContext _localctx = new Transform_nameContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_transform_name);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(59);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Parameter_nameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(VmstatsParser.ID, 0); }
		public Parameter_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter_name; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterParameter_name(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitParameter_name(this);
		}
	}

	public final Parameter_nameContext parameter_name() throws RecognitionException {
		Parameter_nameContext _localctx = new Parameter_nameContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_parameter_name);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(61);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Value_nameContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(VmstatsParser.ID, 0); }
		public Value_nameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_value_name; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).enterValue_name(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof VmstatsListener ) ((VmstatsListener)listener).exitValue_name(this);
		}
	}

	public final Value_nameContext value_name() throws RecognitionException {
		Value_nameContext _localctx = new Value_nameContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_value_name);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(63);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3\rD\4\2\t\2\4\3\t"+
		"\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\3\2\3\2\3\2\3\2\3\2"+
		"\7\2\30\n\2\f\2\16\2\33\13\2\3\2\5\2\36\n\2\3\3\3\3\3\3\3\3\3\3\5\3%\n"+
		"\3\3\3\3\3\5\3)\n\3\3\4\3\4\3\4\3\4\3\5\3\5\3\5\3\5\3\5\3\5\7\5\65\n\5"+
		"\f\5\16\58\13\5\3\5\3\5\3\6\3\6\3\7\3\7\3\b\3\b\3\t\3\t\3\t\2\2\n\2\4"+
		"\6\b\n\f\16\20\2\2\2@\2\35\3\2\2\2\4\37\3\2\2\2\6*\3\2\2\2\b.\3\2\2\2"+
		"\n;\3\2\2\2\f=\3\2\2\2\16?\3\2\2\2\20A\3\2\2\2\22\23\5\n\6\2\23\24\7\3"+
		"\2\2\24\31\5\4\3\2\25\26\7\4\2\2\26\30\5\4\3\2\27\25\3\2\2\2\30\33\3\2"+
		"\2\2\31\27\3\2\2\2\31\32\3\2\2\2\32\36\3\2\2\2\33\31\3\2\2\2\34\36\5\b"+
		"\5\2\35\22\3\2\2\2\35\34\3\2\2\2\36\3\3\2\2\2\37(\5\f\7\2 !\7\5\2\2!$"+
		"\5\6\4\2\"#\7\6\2\2#%\5\6\4\2$\"\3\2\2\2$%\3\2\2\2%&\3\2\2\2&\'\7\7\2"+
		"\2\')\3\2\2\2( \3\2\2\2()\3\2\2\2)\5\3\2\2\2*+\5\16\b\2+,\7\b\2\2,-\5"+
		"\20\t\2-\7\3\2\2\2./\7\t\2\2/\60\5\2\2\2\60\61\7\n\2\2\61\66\5\2\2\2\62"+
		"\63\7\n\2\2\63\65\5\2\2\2\64\62\3\2\2\2\658\3\2\2\2\66\64\3\2\2\2\66\67"+
		"\3\2\2\2\679\3\2\2\28\66\3\2\2\29:\7\13\2\2:\t\3\2\2\2;<\7\f\2\2<\13\3"+
		"\2\2\2=>\7\f\2\2>\r\3\2\2\2?@\7\f\2\2@\17\3\2\2\2AB\7\f\2\2B\21\3\2\2"+
		"\2\7\31\35$(\66";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}