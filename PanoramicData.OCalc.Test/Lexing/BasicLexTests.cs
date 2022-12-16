using FluentAssertions;

namespace PanoramicData.OCalc.Test.Lexing;

public class BasicLexTests
{
	[Theory]
	[InlineData(
		"1",
		"1",
		TokenType.Number
		)
	]
	[InlineData("	set(a, 1) // true",
		"set;(;a;,;1;)",
		TokenType.Identifier,
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
		"1.1;+;2.2",
		TokenType.Number,
		TokenType.Operator,
		TokenType.Number)
	]
	[InlineData("a == 1 ? b : c",
		"a;==;1;?;b;:;c",
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Identifier,
		TokenType.Operator,
		TokenType.Identifier)
	]
	[InlineData("[1, 2, 3]",
		"[;1;,;2;,;3;]",
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator,
		TokenType.Number,
		TokenType.Operator)
	]
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