namespace PanoramicData.OCalc
{
	internal abstract class ParseNode
	{
		public FunctionParseNode? Parent { get; internal set; }

		internal Token? IdentifierToken { get; set; }

		internal abstract string GetExpressionString();
	}
}