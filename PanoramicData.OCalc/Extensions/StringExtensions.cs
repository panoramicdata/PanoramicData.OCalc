namespace PanoramicData.OCalc.Extensions;

internal static class StringExtensions
{
	public static string UpperCaseFirstLetter(this string text)
		=> string.IsNullOrEmpty(text) ? string.Empty : char.ToUpper(text[0]) + text[1..];

	public static string StripOuterParens(this string expressionString)
	{
		while (expressionString.StartsWith('(') && expressionString.EndsWith(')'))
		{
			expressionString = expressionString[1..^1];
		}

		return expressionString;
	}
}