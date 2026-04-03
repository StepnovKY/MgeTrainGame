using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MgeTrainGame
{
    class Program
    {
        static int width = 30;
        static int height = 20;
        static int headX, headY;
        static List<(int x, int y)> train = new List<(int, int)>();
        static (int x, int y) target;
        static string direction;
        static bool gameOver;
        static bool exitRequested = false; // Флаг для выхода по Esc
        static int score;
        static Random rand = new Random();

        static string[] mgeQuotes = {
            "НУ-НУ, СОСУНОК!", "БРО, НАДО БОЛЬШЕ ТРЕНИРОВАТЬСЯ", "MGE BROTHER, RELAX",
            "ТЫ ДАЖЕ НЕ СТАРАЛСЯ, СЫНОК", "ПОПРОБУЙ ЕЩЕ РАЗ, ДЕСАНТНИК", "STAY MAD, KID"
        };

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "MGE TRAIN: SOSUNOK CHASE";

            while (!exitRequested)
            {
                Setup();
                RunGame();

                if (exitRequested) break; // Если нажали Esc во время игры

                if (!ShowGameOverScreen()) break; // Если после смерти не нажали 'R'
            }

            Console.Clear();
            Console.WriteLine("Выход из MGE... Увидимся на арене!");
            Thread.Sleep(1000);
        }

        static void Setup()
        {
            score = 0;
            gameOver = false;
            direction = "RIGHT";
            headX = width / 2;
            headY = height / 2;
            train.Clear();
            train.Add((headX, headY));
            SpawnTarget();
        }

        static void RunGame()
        {
            while (!gameOver && !exitRequested)
            {
                Draw();
                Input();
                Logic();
                Thread.Sleep(100);
            }
        }

        static void SpawnTarget()
        {
            target = (rand.Next(1, width - 1), rand.Next(1, height - 1));
        }

        static void Draw()
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('=', width + 1));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || x == width - 1) { Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("|"); }
                    else if (x == headX && y == headY) { Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("М"); }
                    else if (x == target.x && y == target.y) { Console.ForegroundColor = ConsoleColor.Green; Console.Write("+"); }
                    else if (IsBody(x, y, out int index))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(index == 1 ? "Г" : "Е");
                    }
                    else Console.Write(" ");
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('=', width + 1));
            Console.WriteLine($" СЧЕТ МГЕ: {score} | ESC - ВЫХОД | ЛОВИ СОСУНКА! ");
        }

        static bool IsBody(int x, int y, out int index)
        {
            for (int i = 1; i < train.Count; i++)
            {
                if (train[i].x == x && train[i].y == y) { index = i; return true; }
            }
            index = -1; return false;
        }

        static void Input()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape) exitRequested = true;
                if (key == ConsoleKey.UpArrow && direction != "DOWN") direction = "UP";
                if (key == ConsoleKey.DownArrow && direction != "UP") direction = "DOWN";
                if (key == ConsoleKey.LeftArrow && direction != "RIGHT") direction = "LEFT";
                if (key == ConsoleKey.RightArrow && direction != "LEFT") direction = "RIGHT";
            }
        }

        static void Logic()
        {
            // Прыгающий сосунок (15% шанс прыжка)
            if (rand.Next(0, 100) < 15)
            {
                int nX = target.x + rand.Next(-1, 2);
                int nY = target.y + rand.Next(-1, 2);
                if (nX > 0 && nX < width - 1 && nY >= 0 && nY < height) target = (nX, nY);
            }

            if (direction == "UP") headY--;
            if (direction == "DOWN") headY++;
            if (direction == "LEFT") headX--;
            if (direction == "RIGHT") headX++;

            if (headX <= 0 || headX >= width - 1 || headY < 0 || headY >= height) gameOver = true;

            if (headX == target.x && headY == target.y)
            {
                score += 10;
                SpawnTarget();
                train.Add((0, 0)); // Добавляем новый вагон "Е" в конец
            }

            // Двигаем хвост (каждый вагон встает на место предыдущего)
            for (int i = train.Count - 1; i > 0; i--) train[i] = train[i - 1];

            // ОБНОВЛЯЕМ ТОЛЬКО ГОЛОВУ (индекс 0)
            if (train.Count > 0) train[0] = (headX, headY);

            // Проверка на самопоедание
            for (int i = 1; i < train.Count; i++)
                if (train[i].x == headX && train[i].y == headY) gameOver = true;
        }

        static bool ShowGameOverScreen()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\n   === ТЫ ПРОИГРАЛ, БРО ===");
            Console.WriteLine($"   {mgeQuotes[rand.Next(mgeQuotes.Length)]}");
            Console.ResetColor();
            Console.WriteLine($"\n   ИТОГОВЫЙ СЧЕТ: {score}");
            Console.WriteLine("\n   'R' - РЕСТАРТ | ESC - ВЫХОД");

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.R) { Console.Clear(); return true; }
                if (key == ConsoleKey.Escape) { exitRequested = true; return false; }
            }
        }
    }
}
