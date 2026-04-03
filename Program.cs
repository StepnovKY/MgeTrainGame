using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MgeArcade
{
    class Program
    {
        // ANSI цвета для консоли
        public const string RESET = "\x1b[0m";
        public const string RED = "\x1b[31m";
        public const string GREEN = "\x1b[32m";
        public const string YELLOW = "\x1b[33m";
        public const string BLUE = "\x1b[34m";
        public const string CYAN = "\x1b[36m";
        public const string WHITE = "\x1b[37m";
        public const string GRAY = "\x1b[90m";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();
                Console.Write($"{CYAN}=== MGE ARCADE: ELITE EDITION ==={RESET}\n");
                Console.WriteLine($"{YELLOW}1.{RESET} МГЕ ПАРОВОЗИК (Snake)");
                Console.WriteLine($"{YELLOW}2.{RESET} МГЕ ГОНЯЯЯЛО (Hardcore Road)");
                Console.WriteLine($"{RED}ESC.{RESET} ВЫХОД");

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1) TrainGame.Run();
                else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2) RacerGame.Run();
                else if (key == ConsoleKey.Escape) break;
            }
        }

        public static bool AskRestart(string gameName, int score)
        {
            Console.Write($"{RED}\n--- {gameName} ОКОНЧЕН ---{RESET}\n");
            Console.WriteLine($"{WHITE}РЕЗУЛЬТАТ: {YELLOW}{score}{RESET}");
            Console.WriteLine($"\n[{CYAN}R{RESET}] - РЕСТАРТ | [{RED}ESC{RESET}] - В МЕНЮ");
            while (true)
            {
                var k = Console.ReadKey(true).Key;
                if (k == ConsoleKey.R) return true;
                if (k == ConsoleKey.Escape) return false;
            }
        }
    }

    class TrainGame
    {
        public static void Run() { bool retry = true; while (retry) { int score = StartGame(); retry = Program.AskRestart("ПАРОВОЗИК", score); } }

        static int StartGame()
        {
            Console.Clear();
            int width = 30, height = 15, headX = 15, headY = 7, score = 0;
            var train = new List<(int x, int y)> { (headX, headY) };
            var target = (x: 10, y: 5);
            var rand = new Random();
            string dir = "RIGHT";

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var k = Console.ReadKey(true).Key;
                    if (k == ConsoleKey.UpArrow && dir != "DOWN") dir = "UP";
                    else if (k == ConsoleKey.DownArrow && dir != "UP") dir = "DOWN";
                    else if (k == ConsoleKey.LeftArrow && dir != "RIGHT") dir = "LEFT";
                    else if (k == ConsoleKey.RightArrow && dir != "LEFT") dir = "RIGHT";
                    else if (k == ConsoleKey.Escape) return score;
                }

                // Прыжки сосунка
                if (rand.Next(100) < 15)
                {
                    int nx = target.x + rand.Next(-1, 2);
                    int ny = target.y + rand.Next(-1, 2);
                    if (nx > 0 && nx < width - 1 && ny >= 0 && ny < height) target = (nx, ny);
                }

                if (dir == "UP") headY--;
                else if (dir == "DOWN") headY++;
                else if (dir == "LEFT") headX--; else if (dir == "RIGHT") headX++;

                if (headX <= 0 || headX >= width - 1 || headY < 0 || headY >= height ||
                    train.Skip(1).Any(t => t.x == headX && t.y == headY)) return score;

                if (headX == target.x && headY == target.y)
                {
                    score += 10;
                    target = (rand.Next(1, width - 1), rand.Next(1, height - 1));
                    train.Add((-1, -1));
                }

                for (int i = train.Count - 1; i > 0; i--) train[i] = train[i - 1];
                train[0] = (headX, headY);

                Console.SetCursorPosition(0, 0);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{Program.YELLOW}{new string('=', width)}{Program.RESET}".PadRight(55));
                for (int y = 0; y < height; y++)
                {
                    sb.Append($"{Program.YELLOW}|{Program.RESET}");
                    for (int x = 1; x < width - 1; x++)
                    {
                        int tIdx = train.FindIndex(seg => seg.x == x && seg.y == y);
                        if (tIdx == 0) sb.Append($"{Program.CYAN}М{Program.RESET}");
                        else if (tIdx == 1) sb.Append($"{Program.WHITE}Г{Program.RESET}");
                        else if (tIdx > 1) sb.Append($"{Program.WHITE}Е{Program.RESET}");
                        else if (x == target.x && y == target.y) sb.Append($"{Program.GREEN}+{Program.RESET}");
                        else sb.Append(" ");
                    }
                    sb.AppendLine($"{Program.YELLOW}|{Program.RESET}".PadRight(15));
                }
                sb.AppendLine($"{Program.YELLOW}{new string('=', width)}{Program.RESET}".PadRight(55));
                sb.AppendLine($"{Program.WHITE}СЧЕТ: {Program.YELLOW}{score}{Program.RESET} | {Program.RED}ESC - ВЫЙТИ{Program.RESET}".PadRight(60));
                Console.Write(sb.ToString());
                Thread.Sleep(100);
            }
        }
    }

    class RacerGame
    {
        public static void Run() { bool retry = true; while (retry) { int score = StartGame(); retry = Program.AskRestart("ГОНЯЯЯЛО", score); } }
        static int StartGame()
        {
            Console.Clear();
            int w = 18, h = 18, carX = 9, score = 0, hp = 3;
            var objects = new List<(int x, int y, int type)>();
            var rand = new Random();

            while (hp > 0)
            {
                if (Console.KeyAvailable)
                {
                    var k = Console.ReadKey(true).Key;
                    if (k == ConsoleKey.LeftArrow && carX > 1) carX--;
                    else if (k == ConsoleKey.RightArrow && carX < w - 2) carX++;
                    else if (k == ConsoleKey.Escape) return score;
                }

                if (rand.Next(100) < 18) objects.Add((rand.Next(1, w - 1), 0, 0));
                if (rand.Next(100) < 13) objects.Add((rand.Next(2, w - 2), 0, 1));

                for (int i = objects.Count - 1; i >= 0; i--)
                {
                    var obj = objects[i];
                    int nextY = obj.y + 1;
                    if (nextY == h - 1)
                    {
                        if (obj.type == 0 && obj.x == carX) { score += 50; objects.RemoveAt(i); continue; }
                        else if (obj.type == 1 && Math.Abs(obj.x - carX) <= 1) { hp--; objects.RemoveAt(i); continue; }
                    }
                    if (nextY >= h) objects.RemoveAt(i);
                    else objects[i] = (obj.x, nextY, obj.type);
                }

                Console.SetCursorPosition(0, 0);
                StringBuilder sb = new StringBuilder();

                // Отрисовка ХП
                sb.Append($"{Program.WHITE} ЖИЗНИ: ");
                for (int i = 0; i < 3; i++) sb.Append(i < hp ? $"{Program.RED}♥ {Program.RESET}" : $"{Program.GRAY}. {Program.RESET}");
                sb.AppendLine($"| {Program.WHITE}ОЧКИ: {Program.YELLOW}{score}{Program.RESET}".PadRight(30));

                sb.AppendLine($"{Program.GRAY}{new string('=', w + 2)}{Program.RESET}".PadRight(45));

                for (int y = 0; y < h; y++)
                {
                    sb.Append($"{Program.GRAY}||{Program.RESET}");

                    // Создаем строку кадра для объектов
                    string rowStr = "";
                    for (int x = 1; x < w + 1; x++)
                    {
                        var o = objects.FirstOrDefault(ob => ob.x == x - 1 && ob.y == y);
                        // Проверка на препятствие рядом (широкий блок)
                        var nearBlock = objects.FirstOrDefault(ob => ob.type == 1 && ob.y == y && Math.Abs(ob.x - (x - 1)) <= 1);

                        if (y == h - 1 && x - 1 == carX) rowStr += $"{Program.CYAN}M{Program.RESET}";
                        else if (nearBlock != default) rowStr += $"{Program.RED}#{Program.RESET}";
                        else if (o != default && o.type == 0) rowStr += $"{Program.GREEN}S{Program.RESET}";
                        else rowStr += " ";
                    }
                    sb.Append(rowStr);
                    sb.AppendLine($"{Program.GRAY}||{Program.RESET}".PadRight(20));
                }
                sb.AppendLine($"{Program.GRAY}{new string('=', w + 2)}{Program.RESET}".PadRight(45));
                sb.AppendLine($"{Program.WHITE}[←][→] - РУЛИ | {Program.RED}ESC - МЕНЮ{Program.RESET}".PadRight(50));

                Console.Write(sb.ToString());
                Thread.Sleep(60);
            }
            return score;
        }
    }
}
