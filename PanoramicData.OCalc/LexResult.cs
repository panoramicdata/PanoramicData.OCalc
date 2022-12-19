namespace PanoramicData.OCalc;

internal class LexResult
{
	public List<Token> Tokens { get; } = new List<Token>();

	public LexResultType Type { get; internal set; } = LexResultType.Unknown;

	public Exception? Exception { get; internal set; }
}