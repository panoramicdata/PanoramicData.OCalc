namespace PanoramicData.OCalc;

internal class OCalcInternalExpression
{
	private ParseResult _parseResult;

	public OCalcInternalExpression(ParseResult parseResult)
	{
		_parseResult = parseResult;
	}

	internal Task<object?> EvaluateAsync(CancellationToken cancellationToken)
		=> throw new NotImplementedException();
}