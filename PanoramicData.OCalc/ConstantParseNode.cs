namespace PanoramicData.OCalc
{
	internal class ConstantParseNode : ParseNode
	{
		private readonly Token _token;

		internal ConstantParseNode(Token token)
		{
			_token = token;
		}

		internal override string GetExpressionString()
			=> _token.Type == TokenType.String ? $"'{_token.Text}'" : _token.Text;
	}
}