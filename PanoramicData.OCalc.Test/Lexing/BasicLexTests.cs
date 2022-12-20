using FluentAssertions;
using Xunit.Abstractions;

namespace PanoramicData.OCalc.Test.Lexing;

public class BasicLexTests : BaseTest
{
	public BasicLexTests(ITestOutputHelper helper) : base(helper)
	{
	}

	[Theory]
	[InlineData(
		"1",
		"1",
		TokenType.Number
		)
	]
	[InlineData(
		"-1",
		"-1",
		TokenType.Number
		)
	]
	[InlineData(
		"-1 + -1",
		"-1;_.+;-1",
		TokenType.Number,
		TokenType.StaticMethod,
		TokenType.Number
		)
	]
	[InlineData(
		"-1+-1",
		"-1;_.+;-1",
		TokenType.Number,
		TokenType.StaticMethod,
		TokenType.Number
		)
	]
	[InlineData("	set(a, 1) // true",
		"_.Set;(;a;,;1;)",
		TokenType.StaticMethod,
		TokenType.Operator,
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator
		)
	]
	[InlineData("a[x]",
		"a;[;x;]",
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Identifier,
		TokenType.Operator)
	]
	[InlineData("1.1",
		"1.1",
		TokenType.Number
		)
	]
	[InlineData("'1.1'",
		"1.1",
		TokenType.String
		)
	]
	[InlineData("1.1 + 2.2",
		"1.1;_.+;2.2",
		TokenType.Number,
		TokenType.StaticMethod,
		TokenType.Number)
	]
	[InlineData("(1 + 2) / 3",
		"(;1;_.+;2;);_./;3",
		TokenType.Operator,
		TokenType.Number,
		TokenType.StaticMethod,
		TokenType.Number,
		TokenType.Operator,
		TokenType.StaticMethod,
		TokenType.Number)
	]
	[InlineData("a == 1 ? b : c",
		"a;_.==;1;?;b;:;c",
		TokenType.Identifier,
		TokenType.StaticMethod,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Identifier)
	]
	[InlineData(
		"[1, 2, 3]",
		"[;1;,;2;,;3;]",
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator)
	]
	[InlineData(
		"true && false",
		"true;_.&&;false",
		TokenType.Boolean,
		TokenType.StaticMethod,
		TokenType.Boolean)
	]
	[InlineData(
		"!a",
		"_.!;a",
		TokenType.StaticMethod,
		TokenType.Identifier)
	]
	[InlineData(
		"Math.Max(1, 2)",
		"Math.Max;(;1;,;2;)",
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator)
	]
	[InlineData(
		"Task.Delay(TimeSpan.FromSeconds(1))",
		"Task.Delay;(;TimeSpan.FromSeconds;(;1;);)",
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Operator
		)
	]
	[Trait("Lexing", "Tokenization")]
	public void TokenizationTests(
		string expressionText,
		string tokenTexts,
		params TokenType[] tokenTypes)
	{
		var expectedTokenTexts = tokenTexts.Split(';');
		expectedTokenTexts.Length.Should().Be(tokenTypes.Length);

		var lexResult = OCalcLexer.Lex(expressionText);
		lexResult.Type.Should().Be(LexResultType.Success);

		// Check the token types
		lexResult.Tokens
			.Select(t => t.Type)
			.Should()
			.BeEquivalentTo(tokenTypes);
		lexResult.Tokens
			.Select(t => t.Text)
			.Should()
			.BeEquivalentTo(tokenTexts.Split(';'));
	}
}