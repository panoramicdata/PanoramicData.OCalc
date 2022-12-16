using FluentAssertions;

namespace PanoramicData.OCalc.Test.Parsing;

public class BasicParseTests
{
	[Theory]
	[InlineData(
		"1 + 2",
		"1;2;+"
		)
	]
	[InlineData(
		"1 + 2 / 3",
		"1;+level;2;3;/;-level;+"
		)
	]
	[InlineData(
		"(1 + 2) / 3",
		"1;2;+;3;/"
		)
	]
	[InlineData(
		"set(a, 1)",
		"a;1;Root.set;execute"
		)
	]
	[InlineData(
		"set(a, [1, 2, 3])",
		"a;+level;1;2;3;[];-level;set"
		)
	]
	[InlineData(
		"a[1]",
		"a;1;atIndex"
		)
	]
	[InlineData(
		"delay(<TimeSpan>.fromSeconds(1))",
		"1;TimeSpan.fromSeconds;execute;Root.delay;execute"
		)
	]
	[InlineData(
		"<Task>.DelayAsync(1000)",
		"1000;Task.DelayAsync;execute"
		)
	]
	[InlineData(
		"<MerakiClient>(<MerakiClientOptions>('apiKey'))",
		"'apiKey';MerakiClientOptions.new;staticExecute;MerakiClient.new"
		)
	]
	[InlineData(
		"a ?? b ?? c",
		"a;b;nullCoalesc;c;nullCoalesce"
		)
	]
	[InlineData(
		"a == 1 ? b : c",
		"a;1;==;b;c;ternary"
		)
	]
	public void ParseTests(string expressionText, string rpnTokens)
	{
		var lexResult = OCalcLexer.Lex(expressionText);
		lexResult.Type.Should().Be(LexResultType.Success);

		var parseResult = OCalcParser.Parse(lexResult);
		parseResult.Success.Should().BeTrue();

		parseResult
			.Tokens
			.Select(t => t.Text)
			.Should()
			.BeEquivalentTo(rpnTokens.Split(';'));
	}
}