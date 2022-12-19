namespace PanoramicData.OCalc
{
	internal class StringParseNode : ValueParseNode
	{
		public StringParseNode(string value) : base(value)
		{
		}

		override public string ToString()
			=> $"'{Value}'";
	}
}