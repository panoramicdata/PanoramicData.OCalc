using System.Text;

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

		var parseObject = new ParseObject
		{
		};
		var parseMode = ParseMode.None;
		StringBuilder className = new();
		StringBuilder methodName = new();
		Stack<ParseObject> parseObjectStack = new();

		foreach (var token in lexResult.Tokens)
		{
			switch (token.Type)
			{
				case TokenType.Identifier:
					switch (parseMode)
					{
						case ParseMode.PossibleClassDefinition:
						case ParseMode.ClassDefinition:
							parseMode = ParseMode.ClassDefinition;
							className.Append(token.Text);
							continue;
						case ParseMode.MethodDefinition:
							methodName.Append(token.Text);
							continue;
					}

					switch (token.Text)
					{
						case "delay":
							className.Append("Task");
							methodName.Append("DelayAsync");
							parseMode = ParseMode.MethodDefinition;
							continue;
						case "set":
							className.Append('_');
							methodName.Append("set");
							parseMode = ParseMode.MethodDefinition;
							continue;
						default:
							parseObject.PendingOperator = token;
							continue;
					}
				case TokenType.String:
				case TokenType.Number:
					parseObject.Parameters.Add(token);
					continue;
				case TokenType.Operator:
					switch (token.Text)
					{
						case "(":
							switch (parseMode)
							{
								case ParseMode.ClassDefinition:
									parseObject.PendingOperator = new Token($"{className}.ctor", TokenType.Method);
									parseMode = ParseMode.None;
									className.Clear();
									methodName.Clear();
									parseMode = ParseMode.PendingOperator;
									continue;
								case ParseMode.MethodDefinition:
									parseObject.PendingOperator = new Token($"{className}.{methodName}", TokenType.Method);
									parseMode = ParseMode.None;
									className.Clear();
									methodName.Clear();
									parseMode = ParseMode.ParameterList;
									parseObjectStack.Push(parseObject);
									parseObject = new();
									continue;
								default:
									parseObjectStack.Push(parseObject);
									parseObject = new();
									continue;
							}
						case ")":
							parseMode = ParseMode.None;

							// get a static empty list of Tokens
							var currentParseObject = parseObjectStack.Count > 0 ? parseObjectStack.Pop() : new ParseObject();
							currentParseObject.Tokens.AddRange(parseObject.Tokens);
							currentParseObject.Parameters.AddRange(parseObject.Parameters);
							parseObject = currentParseObject;
							continue;
						case "<":
							switch (parseMode)
							{
								case ParseMode.None:
									parseMode = ParseMode.PossibleClassDefinition;
									continue;
								default:
									throw new ParseException($"Invalid parse mode when receiving {token}.");
							}
						case ">":
							switch (parseMode)
							{
								case ParseMode.ClassDefinition:
									parseMode = ParseMode.PossibleMethodDefinition;
									continue;
								default:
									throw new ParseException($"Invalid parse mode when receiving {token}.");
							}
						case ".":
							switch (parseMode)
							{
								case ParseMode.PossibleMethodDefinition:
									parseMode = ParseMode.MethodDefinition;
									continue;
								default:
									throw new ParseException($"Invalid parse mode when receiving {token}.");
							}
						case "[":
							parseMode = ParseMode.Index;
							continue;
						case "]":
							if (parseMode != ParseMode.Index)
							{
								throw new ParseException($"Invalid parse mode when receiving {token}.");
							}

							parseMode = ParseMode.None;
							parseObject.Tokens.Add(new Token("_.atIndex", TokenType.Function));
							continue;
						case "?":
							parseMode = ParseMode.TernaryTerm1;
							continue;
						case ":":
							if (parseMode != ParseMode.TernaryTerm1)
							{
								throw new ParseException($"Invalid parse mode when receiving {token}.");
							}

							parseMode = ParseMode.MethodDefinition;
							className.Append('_');
							methodName.Append("if");
							continue;
						default:
							parseMode = ParseMode.PendingOperator;
							parseObject.PendingOperator = token;
							continue;
					};
				default:
					throw new NotSupportedException($"Unexpected token {token}");
			}
		}

		if (parseObjectStack.Count > 0)
		{
			throw new ParseException("Unbalanced parentheses.");
		}

		parseObject.Close();

		return new ParseResult
		{
			Tokens = parseObject.Tokens,
			Success = true
		};
	}
}