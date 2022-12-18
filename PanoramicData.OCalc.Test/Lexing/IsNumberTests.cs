using FluentAssertions;

namespace PanoramicData.OCalc.Test.Lexing;

[Trait("Lexing", "Number detection")]
public class IsNumberTests
{
	[Theory]
	[InlineData("0", true)]
	[InlineData("1..1", false)]
	[InlineData("2.", false)]
	[InlineData(".3", true)]
	[InlineData("0.4", true)]
	[InlineData("x", false)]
	[InlineData("", false)]
	public void IsNumber_Succeeds(string text, bool expected)
	{
		var actual = OCalcLexer.IsNumber(text);
		actual.Should().Be(expected);
	}
}
