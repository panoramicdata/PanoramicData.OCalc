namespace PanoramicData.OCalc
{
	internal class IdentifierParseNode : ParseNode
	{
		public IdentifierParseNode(Token identifierToken)
		{
			IdentifierToken = identifierToken;
		}

		internal override string GetExpressionString() => IdentifierToken!.Text;
	}
}