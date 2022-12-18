namespace PanoramicData.OCalc
{
	internal class ParseObject
	{
		public Token? PendingOperator { get; set; } = null;

		public List<Token> Tokens { get; set; } = new();

		public List<Token> Parameters { get; set; } = new();

		internal void Close()
		{
			Tokens.AddRange(Parameters);
			Parameters.Clear();
			if (PendingOperator is not null)
			{
				Tokens.Add(PendingOperator);
				PendingOperator = null;
			}
		}
	}
}