using FluentAssertions;
using Xunit.Abstractions;

namespace PanoramicData.OCalc.Test.Parsing;

public class BasicParseTests : BaseTest
{
	public BasicParseTests(ITestOutputHelper helper) : base(helper)
	{
	}

	[Theory]
	[InlineData(
		"1 + 2 * sin(a + b * c) + 4",
		"_.+(_.+(1, _.*(2, Math.Sin(_.+(a, _.*(b, c)))), 4)"
		)
	]
	[InlineData(
		"1 + 2",
		"_.+(1, 2)"
		)
	]
	[InlineData(
		"1 + 2 / 3",
		"_.+(1, _./(2, 3))"
		)
	]
	[InlineData(
		"(1 + 2) / 3",
		"_./(_.+(1, 2), 3)"
		)
	]
	[InlineData(
		"set(a, 1)",
		"_.Set(a, 1)"
		)
	]
	[InlineData(
		"log('woo')",
		"_.Log('woo')"
		)
	]
	[InlineData(
		"true && false",
		"_.&&(true, false)"
		)
	]
	[InlineData(
		"log('a') && log('b')",
		"_.&&(_.Log(a), _.Log(b))"
		)
	]
	[InlineData(
		"1, 2, 3",
		"1, 2, 3"
		)
	]
	[InlineData(
		"set('a', list(1, 2, 3))",
		"_.Set(a, _.List(1, 2, 3))"
		)
	]
	[InlineData(
		"a[1]",
		"_.AtIndex(a, 1)"
		)
	]
	[InlineData(
		"delay(<TimeSpan>.FromSeconds(1))",
		"_.Delay(TimeSpan.FromSeconds(1))"
		)
	]
	[InlineData(
		"<Task>.DelayAsync(1000)",
		"Task.DelayAsync(1000)"
		)
	]
	[InlineData(
		"<Math>.Max(1,2)",
		"Math.Max(1, 2)"
		)
	]
	[InlineData(
		"<MerakiClient>(<MerakiClientOptions>('apiKey'))",
		"MerakiClient.ctor(MerakiClientOptions.ctor('apiKey'))"
		)
	]
	[InlineData(
		"a ?? b ?? c",
		"_.NullCoalesce(a, b, c)"
		)
	]
	[InlineData(
		"a == 1 ? b : c",
		"_.If(_.Equals(a, 1), b, c)"
		)
	]
	[InlineData(
		"a == 1",
		"_.Equals(a, 1)"
		)
	]
	[InlineData(
		"!a",
		"_.Not(a)"
		)
	]
	[Trait("Parsing", "Basic")]
	public void ParseTests(string expressionText, string expectedExecutionString)
	{
		var lexResult = OCalcLexer.Lex(expressionText);
		lexResult.Type.Should().Be(LexResultType.Success);

		var parseResult = OCalcParser.Parse(lexResult);
		parseResult.FailureText.Should().BeEmpty();
		parseResult.Success.Should().BeTrue();
		parseResult.ParseObject.ToString().Should().Be(expectedExecutionString);
	}
}