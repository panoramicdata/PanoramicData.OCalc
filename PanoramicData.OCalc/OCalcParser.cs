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
		FunctionParseNode currentParseNode = parseResult.ParseNode;
		try
		{
			foreach (var token in lexResult.Tokens)
			{
				switch (token.Type)
				{
					case TokenType.Number:
					case TokenType.Boolean:
					case TokenType.String:
						if (currentParseNode.IsEmptyOrComplete)
						{
							var functionParseNode1 = new FunctionParseNode
							{
								Parent = currentParseNode,
							};
							functionParseNode1.Parameters.Add(
								new ConstantParseNode(token)
								{
									Parent = functionParseNode1
								}
							);
							currentParseNode.Parameters.Add(functionParseNode1);
							currentParseNode = functionParseNode1;
							break;
						}

						currentParseNode.Parameters.Add(
									new ConstantParseNode(token)
									{
										Parent = currentParseNode
									}
								);
						break;
					case TokenType.Identifier:
						if (currentParseNode.IdentifierToken is not null)
						{
							throw new ParseException($"Unexpected second identifier {token.Text}.");
						}

						currentParseNode.IdentifierToken = token;
						break;
					case TokenType.Operator:
						switch (token.Text)
						{
							case "(":
								currentParseNode.PromoteIdentifierIfRequired();
								var newFunctionParseNode = new FunctionParseNode
								{
									Parent = currentParseNode
								};
								currentParseNode.Parameters.Add(newFunctionParseNode);
								currentParseNode = newFunctionParseNode;
								break;

							case ")":
								currentParseNode = currentParseNode.Parent ?? throw new ParseException("Unmatched )");
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

							default:
								currentParseNode.Method = string.IsNullOrWhiteSpace(currentParseNode.Method)
									? "_." + token.Text
									: currentParseNode.Method + token.Text;
								break;
						}

						break;
					case TokenType.StaticMethod:
						if (currentParseNode.IsEmptyOrComplete)
						{
							var functionNode = new FunctionParseNode
							{
								Parent = currentParseNode,
								Method = token.Text
							};
							currentParseNode.Parameters.Add(functionNode);
							currentParseNode = functionNode;
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