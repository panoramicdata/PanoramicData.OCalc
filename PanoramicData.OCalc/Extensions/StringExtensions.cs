namespace PanoramicData.OCalc.Extensions;

internal static class StringExtensions
{
	public static string UpperCaseFirstLetter(this string text)
		=> string.IsNullOrEmpty(text) ? string.Empty : char.ToUpper(text[0]) + text[1..];
}