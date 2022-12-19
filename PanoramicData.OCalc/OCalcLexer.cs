using PanoramicData.OCalc.Extensions;
using System.Text;

namespace PanoramicData.OCalc;

internal class OCalcLexer
{
	internal static LexResult Lex(string expressionText)
	{
		var lexResult = new LexResult();
		try
		{
			var currentTokenText = new StringBuilder();
			var isEscaped = false;
			var isQuoted = false;
			var commentMode = CommentMode.None;
			var twoCharOperatorMode = TwoCharOperatorMode.None;
			foreach (var c in expressionText)
			{
				switch (c)
				{
					case ' ':
					case '\t':
					case '\r':
					case '\n':
						HandlePossibleTwoCharOperatorMode(
							ref twoCharOperatorMode,
							ref commentMode,
							lexResult.Tokens,
							currentTokenText
						);

						// Add the current token to the list
						if (currentTokenText.Length > 0)
						{
							lexResult.Tokens.AddRange(GetTokens(currentTokenText.ToString()));
							currentTokenText.Clear();
						}

						break;
					case '.':
						HandlePossibleTwoCharOperatorMode(
							ref twoCharOperatorMode,
							ref commentMode,
							lexResult.Tokens,
							currentTokenText
						);

						// Are we mid-number?
						if (currentTokenText.Length > 0 && IsInteger(currentTokenText.ToString()))
						{
							currentTokenText.Append(c);
							break;
						}

						// Add the operator to the list
						lexResult.Tokens.Add(new Token(c, TokenType.Operator));

						break;
					case '\'':
						HandlePossibleTwoCharOperatorMode(
							ref twoCharOperatorMode,
							ref commentMode,
							lexResult.Tokens,
							currentTokenText
						);

						if (isQuoted)
						{
							isQuoted = false;
							lexResult.Tokens.Add(new Token(currentTokenText.ToString(), TokenType.String));
							currentTokenText.Clear();
							break;
						}

						isQuoted = !isQuoted;
						break;
					case '\\':
						HandlePossibleTwoCharOperatorMode(
							ref twoCharOperatorMode,
							ref commentMode,
							lexResult.Tokens,
							currentTokenText
						);

						if (isEscaped)
						{
							currentTokenText.Append(c);
							isEscaped = false;
							break;
						}

						isEscaped = true;
						break;
					case '&':
					case '|':
					case '^':
					case '!':
					case '?':
					case '=':
					case ':':
					case '+':
					case '-':
					case '*':
					case '/':
					case '%':
					case ',':
					case '(':
					case ')':
					case '<':
					case '>':
					case '[':
					case ']':
						if (isQuoted)
						{
							// Add the character to the current token
							currentTokenText.Append(c);
							break;
						}

						switch (c)
						{
							case '(':
							case ')':
							case '[':
							case ']':
								if (currentTokenText.Length > 0)
								{
									lexResult.Tokens.AddRange(GetTokens(currentTokenText.ToString()));
									currentTokenText.Clear();
								}

								lexResult.Tokens.Add(new Token(c, TokenType.Operator));
								continue;
							case '/':
								commentMode = commentMode switch
								{
									CommentMode.None => CommentMode.Possible,
									CommentMode.Possible => CommentMode.True,
									_ => CommentMode.True
								};
								continue;
						}

						// Add the current token to the list
						if (currentTokenText.Length > 0)
						{
							switch (twoCharOperatorMode)
							{
								case TwoCharOperatorMode.None:
									lexResult.Tokens.AddRange(GetTokens(currentTokenText.ToString()));
									currentTokenText.Clear();
									lexResult.Tokens.Add(new Token(c, TokenType.Operator));
									break;
								default:
									var compoundTokenString = currentTokenText.ToString() + c;
									switch (compoundTokenString)
									{
										case "!=":
										case "??":
										case ">=":
										case "<=":
										case "^^":
										case "||":
										case "&&":
										case "?.":
										case "==":
											lexResult.Tokens.Add(new Token(compoundTokenString, TokenType.Operator));
											break;
										default:
											var currentText = currentTokenText.ToString();
											lexResult.Tokens.AddRange(GetTokens(currentText));
											break;
									}

									twoCharOperatorMode = TwoCharOperatorMode.None;
									break;
							}

							currentTokenText.Clear();
						}
						else
						{
							twoCharOperatorMode = TwoCharOperatorMode.Possible;
							currentTokenText.Append(c);
						}

						break;
					default:
						HandlePossibleTwoCharOperatorMode(
							ref twoCharOperatorMode,
							ref commentMode,
							lexResult.Tokens,
							currentTokenText
						);

						// Add the character to the current token
						currentTokenText.Append(c);
						break;
				}

				if (commentMode == CommentMode.True)
				{
					break;
				}
			}

			// Add the current token to the list
			if (currentTokenText.Length > 0)
			{
				HandlePossibleTwoCharOperatorMode(
					ref twoCharOperatorMode,
					ref commentMode,
					lexResult.Tokens,
					currentTokenText
				);

				lexResult.Tokens.AddRange(GetTokens(currentTokenText.ToString()));
				currentTokenText.Clear();
			}

			lexResult.Type = LexResultType.Success;

			return lexResult;
		}
		catch (Exception ex)
		{
			lexResult.Type = LexResultType.Failure;
			lexResult.Exception = ex;
			return lexResult;
		}
	}

	private static void HandlePossibleTwoCharOperatorMode(
		ref TwoCharOperatorMode twoCharOperatorMode,
		ref CommentMode commentMode,
		List<Token> tokens,
		StringBuilder text)
	{
		if (twoCharOperatorMode == TwoCharOperatorMode.Possible)
		{
			twoCharOperatorMode = TwoCharOperatorMode.None;
			tokens.Add(new Token(text.ToString(), TokenType.Operator));
			text.Clear();
		}

		if (commentMode == CommentMode.Possible)
		{
			commentMode = CommentMode.None;
			tokens.Add(new Token("/", TokenType.Operator));
			text.Clear();
		}
	}

	private static List<Token> GetTokens(string text)
	{
		if (IsNumber(text))
		{
			return new List<Token> { new Token(text, TokenType.Number) };
		}

		if (IsBoolean(text))
		{
			return new List<Token> { new Token(text, TokenType.Boolean) };
		}

		return text switch
		{
			"delay" or "set" or "log" or "list" or "if" => new List<Token>
				{
					new Token("_", TokenType.Identifier),
					new Token(".", TokenType.Operator),
					new Token(text.UpperCaseFirstLetter(), TokenType.Identifier),
				},
			_ => new List<Token> { new Token(text, TokenType.Identifier) },
		};
	}

	private static bool IsBoolean(string text)
		=> text == "true" || text == "false";

	internal static bool IsNumber(string currentTokenText)
	{
		if (currentTokenText.Length == 0)
		{
			return false;
		}

		foreach (var x in currentTokenText)
		{
			switch (x)
			{
				case '.':
					if (currentTokenText.Count(c => c == '.') > 1)
					{
						return false;
					}

					break;
				default:
					if (!char.IsDigit(x))
					{
						return false;
					}
					break;
			}
		}

		return !currentTokenText.EndsWith('.');
	}

	internal static bool IsInteger(string currentTokenText)
		=> currentTokenText.All(char.IsDigit);
}