using System;
using System.Collections.Generic;
using System.Text;

namespace Horserace
{
    internal class Program
    {

        private const int amountOfRounds = 5;
        private const int totalDistance = 30;
        private static int round;
        private static int playersCount;
        private static int[] playersNumber;
        private static int[] playersPoints;
        private static Horse[] horses;
        private static List<int> winner;
        private static readonly Random random = new();

        struct Horse
        {
            public char Symbol;
            public int DistancePassed;
        }

        static void Main()
        {
            Console.CursorVisible = false;
            ShowRules();
            ChoosePlayers();
            StartGame();
            EndGame();
        }

        private static void ShowRules()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Игра \"Скачки\"");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("В игре соревнуются 10 наездников. Поэтому играет до 10 игроков, где каждый ставит на одного из наездников.");
            Console.WriteLine("Каждый ход игрок бросает кубик и надеется на удачу - его наездник с лощадью продвинутся на соответствующее количество километров.");
            Console.WriteLine("Одна игра состоит из 5 заездов, а в каждом заезде есть победитель. Он получает 5 победных очков.");
            Console.WriteLine("Главным победителем является тот, кто набрал больше всех очков за 5 заездов.");
            Console.WriteLine("Перед началом игры игроки выбирают свой номер, либо он присвоится случайно.");
            Console.WriteLine("В каждом туре игрок может упасть с вероятностью 10%. В этом случае он остаётся на месте.");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Хорошей игры!");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.Write("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void ChoosePlayers()
        {
            do
            {
                Console.Clear();
                Console.Write("Введите количество игроков (от 2 до 10): ");
            } while (!int.TryParse(Console.ReadLine(), out playersCount)
                    || playersCount < 2
                    || playersCount > 10);

            playersNumber = new int[playersCount];
            playersPoints = new int[playersCount];
            var availableNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Console.WriteLine("Игроки! Сегодня в заезде участвуют наездники с номерами от 1 до 10.");
            for (int i = 0; i < playersCount; i++)
            {
                var busy = true;
                do
                {
                    var cursor = Console.GetCursorPosition();
                    var answer = string.Empty;
                    var offer = $"Игрок {i + 1}, выберите номер вашего наездника (0 для случайного выбора): ";
                    do
                    {
                        Console.SetCursorPosition(cursor.Left, cursor.Top);
                        Console.Write(new string(' ', answer.Length + offer.Length) + '\r');
                        Console.Write(offer);
                        answer = Console.ReadLine();
                    } while (!int.TryParse(answer, out playersNumber[i])
                             || !(playersNumber[i] >= 0 && playersNumber[i] < 11));

                    if (playersNumber[i] == 0)
                    {
                        var index = random.Next(availableNumbers.Count);
                        playersNumber[i] = availableNumbers[index];
                        busy = false;
                        availableNumbers.RemoveAt(index);
                        Console.WriteLine("Ваш наездник под номером {0}", playersNumber[i]);
                    }
                    else
                    {
                        foreach (var item in availableNumbers)
                        {
                            if (item == playersNumber[i])
                            {
                                busy = false;
                                availableNumbers.Remove(playersNumber[i]);
                                break;
                            }
                        }
                    }

                    if (busy)
                    {
                        Console.Write("Этот номер уже занят игроком. Доступные номера: ");
                        foreach (var item in availableNumbers)
                        {
                            Console.Write("{0} ", item);
                        }
                        Console.WriteLine();
                    }

                } while (busy);

            }
        }

        #region Start Game
        private static void StartGame()
        {
            round = 0;
            winner = new List<int>();
            InitializeHorses();

            while (round != amountOfRounds)
            {
                Console.Clear();
                Console.WriteLine($"Заезд №{round + 1}");
                ShowPlayerTable();
                ShowTracks();
                MoveHorses();

                if (winner.Count != 0)
                {
                    Console.Clear();
                    Console.WriteLine($"Заезд №{round + 1}");
                    AddPoints();
                    ShowPlayerTable();
                    ShowTracks();
                    ShowWinner(round + 1);
                    ConfigureNewRound(ref round);
                }
                Console.WriteLine("Для продолжения нажмите любую клавишу...");
                Console.ReadKey();
            }
        }

        private static void ConfigureNewRound(ref int round)
        {
            round++;
            winner.Clear();
            for (int i = 0; i < horses.Length; i++)
            {
                horses[i].DistancePassed = 0;
            }
        }

        private static void InitializeHorses()
        {
            horses = new Horse[10];
            for (int i = 0; i < horses.Length; i++)
            {
                horses[i].Symbol = (char)(0x2460 + i);
                horses[i].DistancePassed = 0;
            }
        }

        private static void ShowTracks()
        {
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < horses.Length; i++)
            {
                var track = new StringBuilder("0 км [○" + new string('.', totalDistance) + $" ] {totalDistance} км");
                track[6 + horses[i].DistancePassed] = horses[i].Symbol;
                Console.WriteLine(track);
            }
        }

        private static void MoveHorses()
        {
            for (int i = 0; i < horses.Length; i++)
            {
                var fail = false;

                if (random.NextDouble() < 0.1d)
                {
                    fail = true;
                }

                if (!fail)
                {
                    var progress = random.Next(1, 6);
                    horses[i].DistancePassed += progress;
                    if (horses[i].DistancePassed >= totalDistance)
                    {
                        horses[i].DistancePassed = totalDistance;
                        winner.Add(i + 1);
                    }
                }
            }
        }

        private static void ShowWinner(int round)
        {
            Console.WriteLine();
            foreach (var item in winner)
            {
                Console.WriteLine($"Победителем заезда №{round} становится лошадь №{item}");
            }
        }

        private static void AddPoints()
        {
            for (int i = 0; i < playersCount; i++)
            {
                foreach (var item in winner)
                {
                    if (playersNumber[i] == item)
                    {
                        playersPoints[i] += 5;
                    }
                }

            }
        }

        #endregion

        private static void EndGame()
        {
            Console.Clear();
            ShowPlayerTable();

            var maximum = 0;
            var number = new List<int>();
            for (int i = 0; i < playersCount; i++)
            {
                if (maximum == playersPoints[i])
                {
                    number.Add(i);
                }
                if (maximum < playersPoints[i])
                {
                    maximum = playersPoints[i];
                    number.Clear();
                    number.Add(i);
                }
            }
            if (maximum == 0)
            {
                Console.WriteLine("Победителей нет.");
            }
            else
            {
                Console.WriteLine("Список победителей: ");
                foreach (var item in number)
                {
                    Console.WriteLine($"Игрок №{item + 1}");
                }
            }
            Console.ReadKey();
        }

        private static void ShowPlayerTable()
        {
            var line = new string('-', 8 * 8 + 1);
            var header = $"{line}\n" +
                         "|\tИгроки\t\t|\tОчки\t|\tСтавка\t\t|\n" +
                         $"{line}";

            Console.WriteLine(header);
            for (int i = 0; i < playersCount; i++)
            {
                Console.WriteLine($"|\tИгрок №{i + 1}\t|\t{playersPoints[i]}\t|\tЛошадь №{playersNumber[i]}\t|");
            }
            Console.WriteLine(line);
        }
    }
}
