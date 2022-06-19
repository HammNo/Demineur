using System;
using System.Drawing;

namespace Démineur
{
    class Program
    {
        public static void Controle(Grid game)
        {
            game.Print_grid();
            game.Move(0, 0);
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (game.Game_State == Grid.Game_state.Continue)
            {
                switch (info.Key)
                {
                    case ConsoleKey.Enter:
                        game.Reveal();
                        break;
                    case ConsoleKey.DownArrow:
                        game.Move(1,0);
                        break;
                    case ConsoleKey.UpArrow:
                        game.Move(-1, 0);
                        break;
                    case ConsoleKey.LeftArrow:
                        game.Move(0, -1);
                        break;
                    case ConsoleKey.RightArrow:
                        game.Move(0, 1);
                        break;
                    case ConsoleKey.F:
                        game.SetFlag();
                        break;
                    default:
                        break;
                }
                switch (game.Game_State)
                {
                    case Grid.Game_state.Lost:
                        Console.WriteLine("Vous avez perdu!");
                        break;
                    case Grid.Game_state.Won:
                        Console.WriteLine("Vous avez gagné!");
                        break;
                    default:
                        info = Console.ReadKey(true);
                        break;
                }
            }
        }
        public static bool Options()
        {
            string answ = "";
            do
            {
                Console.Write("Options -- [n] Nouvelle partie [q] Quitter : ");
                answ = (Console.ReadLine()).ToUpper();
            } while (answ != "N" && answ != "Q");
            if (answ == "N") return true;
            else return false;
        }
        static void Main(string[] args)
        {
            while (Options()){
                Grid game = new Grid(10, 20, 25);
                Controle(game);
                Console.WriteLine();
            }
        }
    }
}
