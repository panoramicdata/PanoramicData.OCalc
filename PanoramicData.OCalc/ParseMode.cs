namespace PanoramicData.OCalc
{
	internal enum ParseMode
	{
		None,
		Index,
		TernaryTerm1,
		TernaryTerm2,
		PendingOperator,
		PossibleClassDefinition,
		ClassDefinition,
		PossibleMethodDefinition,
		MethodDefinition,
		ParameterList
	}
}