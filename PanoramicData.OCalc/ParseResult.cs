using PanoramicData.OCalc.Extensions;

namespace PanoramicData.OCalc;

internal class ParseResult
{
	public bool Success { get; internal set; }
	public string FailureText { get; internal set; } = string.Empty;
	public FunctionParseNode ParseNode { get; internal set; } = new();

	internal string GetExpressionString()
		=> ParseNode
			.GetExpressionString()
			.StripOuterParens();
}