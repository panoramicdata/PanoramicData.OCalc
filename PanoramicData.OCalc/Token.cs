using System.Diagnostics;

namespace PanoramicData.OCalc;

[DebuggerDisplay("{Type}:{Text}")]
internal class Token
{
	internal TokenType Type { get; private set; }

	internal string Text { get; private set; }

	internal Token(char ch, TokenType tokenType)
	{
		Text = ch.ToString();
		Type = tokenType;
	}

	internal Token(string tokenText, TokenType tokenType)
	{
		Text = tokenText;
		Type = tokenType;
	}

	public override string ToString() => $"{Type}:{Text}";
}