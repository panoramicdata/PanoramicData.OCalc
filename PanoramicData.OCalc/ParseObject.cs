namespace PanoramicData.OCalc
{
	internal class ParseObject : ParseNode
	{
		public static ParseObject Root { get; } = new();

		public string ClassName { get; private set; } = string.Empty;

		public string MethodName { get; private set; } = string.Empty;

		public ParseMode ParseMode { get; set; }

		public void SetClassAndMethod(string className, string methodName)
		{
			ClassName = className;
			MethodName = methodName;
		}

		public string ClassAndMethod
			=> string.IsNullOrEmpty(ClassName) ? string.Empty
			: $"{ClassName}.{(MethodName == string.Empty ? "ctor" : MethodName)}";

		public List<ParseNode> Parameters { get; } = new();

		internal void AddParameter(ParseNode value)
		{
			value.SetParent(this);
			Parameters.Add(value);
		}

		internal void Close()
		{
		}

		internal ParseObject AddToken(Token token)
		{
			switch (token.Type)
			{
				case TokenType.Identifier:
					switch (ParseMode)
					{
						case ParseMode.PossibleClassDefinition:
						case ParseMode.ClassDefinition:
							ParseMode = ParseMode.ClassDefinition;
							SetClassAndMethod(token.Text, "ctor");
							return this;
						case ParseMode.MethodDefinition:
							SetClassAndMethod(ClassName, token.Text);
							return this;
						default:
							Parameters.Add(new IdentifierParseNode(token.Text));
							return this;
					}
				case TokenType.Boolean:
					AddParameter(new BooleanParseNode(token.Text));
					return this;
				case TokenType.String:
					AddParameter(new StringParseNode(token.Text));
					return this;
				case TokenType.Number:
					AddParameter(new NumberParseNode(token.Text));
					return this;
				case TokenType.Operator:
					switch (token.Text)
					{
						case "(":
							var newParseObject = new ParseObject();
							AddParameter(newParseObject);
							return newParseObject;
						case ")":
							Close();
							return Parent;
						case "<":
							switch (ParseMode)
							{
								case ParseMode.None:
								case ParseMode.ParameterList:
									ParseMode = ParseMode.PossibleClassDefinition;
									return this;
								default:
									throw new ParseException(ParseMode, token);
							}
						case ">":
							switch (ParseMode)
							{
								case ParseMode.ClassDefinition:
									ParseMode = ParseMode.MethodDefinition;
									return this;
								default:
									throw new ParseException(ParseMode, token);
							}
						case ".":
							switch (ParseMode)
							{
								case ParseMode.MethodDefinition:
									return this;
								default:
									throw new ParseException(ParseMode, token);
							}
						case "[":
							var parseObject = new ParseObject();
							switch (ParseMode)
							{
								case ParseMode.None:
									ParseMode = ParseMode.OpenSquare;
									return this;
								case ParseMode.ParameterList:
									return this;
								default:
									throw new ParseException(ParseMode, token);
							}
						case "]":
							ParseMode = ParseMode.None;
							if (ParseMode == ParseMode.OpenSquare)
							{
								ClassName = "_";
								MethodName = "[]";
								return this;
							}

							ClassName = "_";
							MethodName = "AtIndex";
							return this;
						case "??":
							ClassName = "_";
							MethodName = "NullCoalesce";
							ParseMode = ParseMode.NullCoalesce;
							return this;
						case "==":
							ClassName = "_";
							MethodName = "Equals";
							return this;
						case "*":
						case "/":
							if (ClassAndMethod == "_.+" || ClassAndMethod == "_.-")
							{
								var lastIndex = Parameters.Count - 1;
								var lastTerm = Parameters[lastIndex];
								Parameters.RemoveAt(lastIndex);
								ParseMode = ParseMode.ParameterList;
								newParseObject = new ParseObject
								{
									ClassName = "_",
									MethodName = token.Text,
								};
								newParseObject.Parameters.Add(lastTerm);
								return newParseObject;
							}

							ClassName = "_";
							MethodName = token.Text;
							return this;
						case "&&":
						case "||":
						case "^^":
						case "&":
						case "|":
						case "^":
						case "+":
						case "-":
							ClassName = "_";
							MethodName = token.Text;
							return this;
						case "!":
							ClassName = "_";
							MethodName = "Not";
							return this;
						case ",":
							ParseMode = ParseMode.ParameterList;
							return this;
						case "?":
							ParseMode = ParseMode.TernaryTerm1;
							return this;
						case ":":
							ParseMode = ParseMode.ParameterList;
							ClassName = "_";
							MethodName = "If";
							return this;
						default:
							throw new ParseException(ParseMode, token);
					};
				default:
					throw new NotSupportedException($"Unexpected token {token}");
			}
		}

		public override string ToString()
			=> ClassAndMethod == string.Empty ? string.Join(", ", Parameters)
				: $"{ClassAndMethod}({string.Join(", ", Parameters)})";
	}
}