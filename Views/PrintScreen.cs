using System;
using Spectre.Console;

namespace haggling_ui.Views
{
    public static class PrintScreen
    {
        public static void PrintTitleScreen()
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("THE BASAR")
                    .Centered()
                    .Color(Color.Gold1));

            string[] menuItems = { "Start", "Optionen", "Beenden" };
            int selectedIndex = 0;

            ConsoleKey key;
            do
            {
                DrawMenu(menuItems, selectedIndex);
                var input = Console.ReadKey(true);
                key = input.Key;

                if (key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                else if (key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex + 1) % menuItems.Length;

            } while (key != ConsoleKey.Enter);

            if (selectedIndex == 0)
               RunGame();
            else if (selectedIndex == 1)
                ShowOptions();
            else if (selectedIndex == 2)
                ExitGame();
        }

        private static void DrawMenu(string[] items, int selected)
        {
            Console.SetCursorPosition(0, 10);
            for (int i = 0; i < items.Length; i++)
            {
                if (i == selected)
                    AnsiConsole.MarkupLine($"[yellow]>> {items[i]}[/]");
                else
                    AnsiConsole.MarkupLine($"   {items[i]}");
            }
        }

        private static void RunGame()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold green]Spiel gestartet! (noch nicht implementiert)[/]");
            Console.ReadKey(true);
            PrintTitleScreen();
        }
      

        private static void ShowOptions()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold cyan]Optionen (noch nicht implementiert)[/]");
            Console.ReadKey(true);
            PrintTitleScreen();
        }

        private static void ExitGame()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold red]Spiel beendet.[/]");
            Environment.Exit(0);
        }
        
    }
}
