using PanoramicData.OCalc.Extensions;

namespace PanoramicData.OCalc
{
	internal class FunctionParseNode : ParseNode
	{
		public FunctionParseNode()
		{
			Parent = this;
		}

		public string? Method { get; internal set; }

		public List<ParseNode> Parameters { get; } = new();

		public bool IsEmptyOrComplete
			=> IsComplete || (IdentifierToken is null && Method == null && Parameters.Count == 0 || Method != null && Parameters.Count > 0);

		public bool IsComplete { get; internal set; }

		internal override string GetExpressionString()
			=> $"{Method}({string.Join(", ", Parameters.Select(p => p.GetExpressionString().StripOuterParens()))})";

		internal void PromoteIdentifierIfRequired()
		{
			if (IdentifierToken is null)
			{
				return;
			}

			if (Method is null)
			{
				Method = IdentifierToken.Text.Contains('.')
					? IdentifierToken.Text
					: $"{IdentifierToken.Text}.ctor";
			}
			else
			{
				Parameters.Add(new IdentifierParseNode(IdentifierToken));
			}

			IdentifierToken = null;
		}
	}
}