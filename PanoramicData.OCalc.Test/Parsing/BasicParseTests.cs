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
		"1 + 2",
		"_.+(1, 2)"
		)
	]
	[InlineData(
		"1 + 2 * Math.Sin(a + b * c) + 4",
		"_.+(_.+(1, _.*(2, Math.Sin(_.+(a, _.*(b, c)))), 4)"
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
		"_.&&(_.Log('a'), _.Log('b'))"
		)
	]
	[InlineData(
		"1, 2, 3",
		"1, 2, 3"
		)
	]
	[InlineData(
		"set('a', list(1, 2, 3))",
		"_.Set('a', _.List(1, 2, 3))"
		)
	]
	[InlineData(
		"a[1]",
		"_.[](a, 1)"
		)
	]
	[InlineData(
		"Task.Delay(TimeSpan.FromSeconds(1))",
		"Task.Delay(TimeSpan.FromSeconds(1))"
		)
	]
	[InlineData(
		"Task.DelayAsync(1000)",
		"Task.DelayAsync(1000)"
		)
	]
	[InlineData(
		"Math.Max(1,2)",
		"Math.Max(1, 2)"
		)
	]
	[InlineData(
		"MerakiClient(MerakiClientOptions('apiKey'))",
		"MerakiClient.ctor(MerakiClientOptions.ctor('apiKey'))"
		)
	]
	[InlineData(
		"a ?? b ?? c",
		"_.??(_.??(a, b), c)"
		)
	]
	[InlineData(
		"a == 1 ? b : c",
		"_.If(_.==(a, 1), b, c)"
		)
	]
	[InlineData(
		"a == 1",
		"_.==(a, 1)"
		)
	]
	[InlineData(
		"!a",
		"_.!(a)"
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
		var stringValue = parseResult.GetExpressionString();
		stringValue.Should().Be(expectedExecutionString);
	}
}