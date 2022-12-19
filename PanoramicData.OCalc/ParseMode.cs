namespace PanoramicData.OCalc
{
	internal enum ParseMode
	{
		None,
		OpenSquare,
		TernaryTerm1,
		TernaryTerm2,
		PendingOperator,
		PossibleClassDefinition,
		ClassDefinition,
		MethodDefinition,
		ParameterList,
		NullCoalesce,
		UnknownIdentifier
	}
}