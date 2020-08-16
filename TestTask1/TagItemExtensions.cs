namespace TestTask1
{
	public static class TagItemExtensions
	{
		public static TagType ConvertStringToTagType(string name)
		{
			switch (name?.ToLower())
			{
				case "bool":
					return TagType.Bool;
				case "int":
					return TagType.Int;
				case "double":
					return TagType.Double;
				case "none":
					return TagType.None;
				default:
					throw new NotSupportedTagTypeException();
			}
		}
	}
}
