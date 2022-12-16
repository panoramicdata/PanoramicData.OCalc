namespace PanoramicData.OCalc;

internal class OCalcParser
{
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

		var parseResult = new ParseResult();

		var parameters = new List<Token>();
		Token? @operator = null;
		foreach (var token in lexResult.Tokens)
		{
			switch (token.Type)
			{
				case TokenType.Number:
					parameters.Add(token);
					break;
				case TokenType.Operator:
					@operator = token;
					break;
				case TokenType.Identifier:
					break;
				case TokenType.String:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		if (@operator is not null)
		{
			foreach (var parameter in parameters)
			{
				parseResult.Tokens.Add(parameter);
			}

			parseResult.Tokens.Add(@operator);
		}

		parseResult.Success = true;

		return parseResult;
	}
}