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

		var parseResult = new ParseResult
		{
			Success = true
		};

		var currentParseNode = new FunctionParseNode();
		try
		{
			foreach (var token in lexResult.Tokens)
			{
				switch (token.Type)
				{
					case TokenType.Number:
					case TokenType.Boolean:
					case TokenType.String:
						currentParseNode.Parameters.Add(
									new ConstantParseNode(token)
									{
										Parent = currentParseNode
									}
								);
						break;
					case TokenType.Identifier:
						currentParseNode.PromoteIdentifierIfRequired();
						currentParseNode.IdentifierToken = token;
						break;
					case TokenType.Operator:
						switch (token.Text)
						{
							case "(":
								if (currentParseNode.Method is null)
								{
									currentParseNode.PromoteIdentifierIfRequired();
									var newFunctionParseNode = new FunctionParseNode
									{
										Parent = currentParseNode
									};
									currentParseNode.Parameters.Add(newFunctionParseNode);
									currentParseNode = newFunctionParseNode;
									break;
								}

								break;

							case ")":
								currentParseNode.IsComplete = true;
								currentParseNode = currentParseNode.Parent ?? throw new ParseException("Null parent on )");
								break;

							case "[":
								currentParseNode.Method = "_.[]";
								currentParseNode.PromoteIdentifierIfRequired();
								break;
							case "]":
								break;

							case ",":
								if (currentParseNode.IdentifierToken is not null)
								{
									currentParseNode.Parameters.Add(new IdentifierParseNode(currentParseNode.IdentifierToken));
									currentParseNode.IdentifierToken = null;
								}

								break;

							case "?":
								currentParseNode.PromoteIdentifierIfRequired();
								// Make space.
								var newFunction2 = new FunctionParseNode
								{
									Method = "_.If"
								};
								if (currentParseNode.Parent != currentParseNode)
								{
									newFunction2.Parent = currentParseNode.Parent;
								}

								newFunction2.Parameters.Add(currentParseNode);
								currentParseNode.Parent = newFunction2;
								currentParseNode = newFunction2;
								break;

							case ":":
								currentParseNode.PromoteIdentifierIfRequired();
								currentParseNode = currentParseNode.Parent ?? throw new ParseException("Missing parent");
								break;

							default:
								currentParseNode.Method = string.IsNullOrWhiteSpace(currentParseNode.Method)
									? "_." + token.Text
									: currentParseNode.Method + token.Text;
								break;
						}

						break;
					case TokenType.StaticMethod:
						if (currentParseNode.IsComplete)
						{
							// Are we at the root?
							if (currentParseNode.Parent == currentParseNode)
							{
								// Yes.  Make space.
								var newFunction2 = new FunctionParseNode();
								newFunction2.Parameters.Add(currentParseNode);
								currentParseNode.Parent = newFunction2;
								currentParseNode = newFunction2;
							}
						}

						if (currentParseNode.Method is not null)
						{
							currentParseNode.PromoteIdentifierIfRequired();

							// Do we take precedence?
							if (FirstIsLesserThanSecond(currentParseNode.Method, token.Text))
							{
								// Yes. Insert above
								var newFunction3 = new FunctionParseNode
								{
									Method = token.Text,
									Parent = currentParseNode
								};
								var stolenParameter = currentParseNode.Parameters.Last();
								currentParseNode.Parameters.Remove(stolenParameter);
								currentParseNode.Parameters.Add(newFunction3);
								newFunction3.Parameters.Add(stolenParameter);
								currentParseNode = newFunction3;
								break;
							}

							// No.  Insert below
							var newFunction2 = new FunctionParseNode
							{
								Parent = currentParseNode,
								Method = token.Text
							};
							currentParseNode.Parameters.Add(newFunction2);
							currentParseNode = newFunction2;
							break;
						}

						currentParseNode.Method = token.Text;
						currentParseNode.PromoteIdentifierIfRequired();

						break;
					default:
						throw new ParseException($"Token type {token.Type} is not supported.");
				}
			}

			currentParseNode.PromoteIdentifierIfRequired();

			while (currentParseNode.Parent != currentParseNode)
			{
				currentParseNode = currentParseNode.Parent ?? throw new ParseException("Could not find node parent");
			}

			parseResult.ParseNode = currentParseNode;
			parseResult.Success = true;
		}
		catch (ParseException exception)
		{
			parseResult.Success = false;
			parseResult.FailureText = exception.Message;
		}

		return parseResult;
	}

	private static bool FirstIsLesserThanSecond(string first, string second) =>
		(first == "_.+" || first == "_.-")
		&&
		!(second == "_.+" || second == "_.-");
}