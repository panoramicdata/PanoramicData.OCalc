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
		var parseMode = ParseMode.None;
		foreach (var token in lexResult.Tokens)
		{
			switch (token.Type)
			{
				case TokenType.Number:
				case TokenType.Identifier:
				case TokenType.String:
					parameters.Add(token);
					if (parseMode == ParseMode.TernaryTerm2)
					{
						var newToken = new Token("_.if", TokenType.Function);
						AddParametersAndOperator(parseResult.Tokens, parameters, ref newToken);
						parseMode = ParseMode.None;
					}

					break;
				case TokenType.Operator:
					switch (token.Text)
					{
						case "[":
							parseMode = ParseMode.Index;
							continue;
						case "]":
							if (parseMode != ParseMode.Index)
							{
								throw new ParseException("Unexpected ]");
							}

							parseMode = ParseMode.None;
							@operator = new Token("_.atIndex", TokenType.Function);
							continue;
						case "?":
							AddParametersAndOperator(parseResult.Tokens, parameters, ref @operator);
							parseMode = ParseMode.TernaryTerm1;
							continue;
						case ":":
							if (parseMode != ParseMode.TernaryTerm1)
							{
								throw new ParseException("Unexpected :");
							}

							parseMode = ParseMode.TernaryTerm2;

							continue;
					}

					@operator = token;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		AddParametersAndOperator(parseResult.Tokens, parameters, ref @operator);

		parseResult.Success = true;

		return parseResult;
	}

	private static void AddParametersAndOperator(List<Token> tokens, List<Token> parameters, ref Token? @operator)
	{
		if (@operator is not null)
		{
			foreach (var parameter in parameters)
			{
				tokens.Add(parameter);
			}

			tokens.Add(@operator);
			parameters.Clear();
			@operator = null;
		}
	}
}