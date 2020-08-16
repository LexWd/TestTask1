using System;

namespace TestTask1
{
	internal static class Program
	{
		private static TagStorage _storage;

		public static void Main()
		{
			var menuItems = new []
			{
					LocalizationStrings.DownloadTagsTreeItem,
					LocalizationStrings.ShowTagsInfoItem,
					LocalizationStrings.DeleteTagItem,
					LocalizationStrings.AddNewTagItem,
					LocalizationStrings.RenameTagItem,
                    LocalizationStrings.Exit
			};

			while (true)
			{
				var chosenItem = ConsoleMenu.Run("Menu", menuItems);
				switch (chosenItem)
				{
					case 0:
						if (_storage == null)
						{
							_storage = new TagStorage();
							break;
						}

						_storage.Save(_storage.TagRoot, null);
                        Console.WriteLine("Saved in new file");

						break;
					case 1:
						_storage?.TagRoot.Write();
						break;
					case 2:
						Console.WriteLine("Input full name of tag");
						var tagToDelete = _storage.Search(Console.ReadLine());
						TagStorage.DeleteTag(tagToDelete);
						break;
					case 3:
						OnAddTagItemChosen();
						break;
					case 4:
						OnRenameTagItemChosen();
						break;
                    case 5:
                        _storage?.Save ( _storage.TagRoot, null ) ;
                        Environment.Exit (0) ;
                        break;
					default:
						Console.WriteLine("Incorrect");
						break;
				}

				Console.ReadKey();
			}
		}

		private static void OnAddTagItemChosen()
		{
			Console.WriteLine("Input full name of parent tag");
			var parent = Console.ReadLine();

			Console.WriteLine("Input name of new tag");
			var name = Console.ReadLine();

			Console.WriteLine("Enter type of new tag");
			var stringType = Console.ReadLine();
			TagType type;
			try
			{
				type = TagItemExtensions.ConvertStringToTagType(stringType);
			}
			catch (NotSupportedTagTypeException)
			{
				Console.WriteLine("Tag type is incorrect, it will be none");
				type = TagType.None;
			}

			if (_storage != null)
			{
				var newItem = new TagItem(name, _storage.Search(parent), type);
			}
		}

		private static void OnRenameTagItemChosen()
		{
			Console.WriteLine("Enter full name of tag");
			var tempPath = Console.ReadLine();
			Console.WriteLine();
			Console.WriteLine("Enter new name of tag");
			var tempNewName = Console.ReadLine();
			Console.WriteLine();
			_storage?.Search(tempPath).ReName(tempNewName);
		}
	}
}