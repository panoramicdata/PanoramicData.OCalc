using System.Diagnostics;

namespace PanoramicData.OCalc;

[DebuggerDisplay("{Type}: {Text}")]
internal class Token
{
	public TokenType Type { get; private set; }
	public string Text { get; private set; }

	public Token(char ch, TokenType tokenType)
	{
		Text = ch.ToString();
		Type = tokenType;
	}

	public Token(string tokenText, TokenType tokenType)
	{
		Text = tokenText;
		Type = tokenType;
	}
}