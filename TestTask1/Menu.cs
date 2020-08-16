using System;

namespace TestTask1
{
	public static class ConsoleMenu
	{
		public static int Run(string headLine, params string[] paragraphs)
		{
			Console.Clear();
			Console.WriteLine(headLine);
			var paragraph = 0;
			const int x = 2;
			const int y = 2;
			var oldParagraph = 0;
			Console.CursorVisible = false;
			for (var i = 0; i < paragraphs.Length; i++)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(x, y + i);
				Console.Write(paragraphs[i]);
			}

			var choice = false;
			while (true)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.SetCursorPosition(x, y + oldParagraph);
				Console.Write(paragraphs[oldParagraph] + " ");

				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.White;
				Console.SetCursorPosition(x, y + paragraph);
				Console.Write(paragraphs[paragraph]);

				oldParagraph = paragraph;

				var key = Console.ReadKey();

				switch (key.Key)
				{
					case ConsoleKey.DownArrow:
						paragraph++;
						break;
					case ConsoleKey.UpArrow:
						paragraph--;
						break;
					case ConsoleKey.Enter:
						choice = true;
						break;
				}

				if (paragraph >= paragraphs.Length)
				{
					paragraph = 0;
				}
				else if (paragraph < 0)
				{
					paragraph = paragraphs.Length - 1;
				}

				if (!choice)
					continue;

				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.CursorVisible = true;
				Console.Clear();
				break;
			}

			Console.Clear();
			Console.CursorVisible = true;
			return paragraph;
		}
	}
}
