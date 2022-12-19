namespace PanoramicData.OCalc;

internal class OCalcParser
{
	internal static readonly List<Token> _emptyTokenList = new();

	internal static ParseResult Parse(LexResult lexResult)
	{

		if (lexResult.Type != LexResultType.Success)
		{
			return new ParseResult
			{
				Success = false,
				FailureText = "Cannot parse a lex result that is not a success."
			};
		}

		var parseObject = new ParseObject();
		var parseResult = new ParseResult
		{
			Success = true,
			ParseObject = parseObject
		};
		try
		{
			foreach (var token in lexResult.Tokens)
			{
				parseObject = parseObject.AddToken(token);
			}

			parseObject.Close();

			parseResult.Success = true;
		}
		catch (ParseException exception)
		{
			parseResult.Success = false;
			parseResult.FailureText = exception.Message;
		}

		return parseResult;
	}
}