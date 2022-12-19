namespace PanoramicData.OCalc;

internal class ParseResult
{
	public bool Success { get; internal set; }
	public string FailureText { get; internal set; } = string.Empty;
	public List<Token> Tokens { get; internal set; } = new();
	public ParseObject ParseObject { get; internal set; }
}