namespace PanoramicData.OCalc;

public class OCalcExpression
{
	private string _expressionText;

	public OCalcExpression(string expressionText)
	{
		_expressionText = expressionText;
	}

	public async Task<object?> EvaluateAsync(CancellationToken cancellationToken)
	{
		// Lex
		var tokens = OCalcLexer.Lex(_expressionText);

		// Parse
		var parseResult = OCalcParser.Parse(tokens);

		// Evaluate
		var internalExpression = new OCalcInternalExpression(parseResult);
		return await internalExpression
			.EvaluateAsync(cancellationToken)
			.ConfigureAwait(false);
	}
}