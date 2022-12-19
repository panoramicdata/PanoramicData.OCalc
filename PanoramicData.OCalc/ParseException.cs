namespace PanoramicData.OCalc
{
	[Serializable]
	internal class ParseException : Exception
	{
		public ParseException(string? message) : base(message)
		{
		}

		public ParseException(ParseMode ParseMode, Token token)
			: base($"Invalid ParseMode {ParseMode} when receiving token {token}")
		{
		}
	}
}