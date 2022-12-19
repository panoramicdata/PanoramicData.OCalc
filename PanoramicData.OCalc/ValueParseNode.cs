namespace PanoramicData.OCalc
{
	internal abstract class ValueParseNode : ParseNode
	{
		protected ValueParseNode(string value)
		{
			Value = value;
		}

		public string Value { get; }

		override public string ToString()
			=> Value;
	}
}