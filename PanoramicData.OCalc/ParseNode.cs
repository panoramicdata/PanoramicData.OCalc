namespace PanoramicData.OCalc
{
	internal abstract class ParseNode
	{
		public ParseNode()
		{
			Parent = ParseObject.Root;
		}

		public ParseObject Parent { get; private set; }

		internal void SetParent(ParseObject parseObject)
			=> Parent = parseObject;
	}
}