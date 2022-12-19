using PanoramicData.OCalc.Extensions;

namespace PanoramicData.OCalc
{
	internal class FunctionParseNode : ParseNode
	{
		public string? Method { get; internal set; }

		public List<ParseNode> Parameters { get; } = new();

		public bool IsEmptyOrComplete
			=> IdentifierToken is null && Method == null && Parameters.Count == 0 || Method != null && Parameters.Count > 0;

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