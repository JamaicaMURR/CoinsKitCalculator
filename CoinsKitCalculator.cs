using System;
using Bycicles;
using Bycicles.Ranges;
using Bycicles.StringExtensions;

namespace Coins
{
    class CoinsKitCalculator
    {
        static void Main(string[] args)
        {
            start:

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            int unitSize;

            while(true) // unitSize input
            {
                unitSize = RequestIntFromConsoleInput("How many coins in one unit? (default 100 in 1): ", defaultValue: 100);

                if(unitSize <= 1)
                    WriteErrorMessage("\nError! Unit size must be > 1");
                else
                    break;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            int kitSize;

            while(true) // kitSize input
            {
                kitSize = RequestIntFromConsoleInput($"\nHow many nominals of coins in coins kit? (1 < Kit size < {unitSize}): ", defaultValue: 4.NotAbove(unitSize - 1));

                if(kitSize <= 1)
                    WriteErrorMessage("\nError! Kit size must be > 1");
                else if(kitSize >= unitSize)
                    WriteErrorMessage($"\nError! Kit size must be < {unitSize}");
                else
                    break;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            int podiumSize;

            while(true) // podiumSize input
            {
                podiumSize = RequestIntFromConsoleInput($"\nHow many of best results you want? (default 3) : ", defaultValue: 3);

                if(podiumSize < 1)
                    WriteErrorMessage("\nError! Number of best results must be >= 1");
                else
                    break;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            double bestAverage = unitSize;

            int[] coinsKit = new int[kitSize];

            for(int i = 1; i <= coinsKit.Length; i++)
                coinsKit[i - 1] = i;

            ScoreBoard<int[], double> scoreBoard = new(podiumSize, ScoreBoardMode.LowerBest);

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            DateTime startTime = DateTime.Now;

            bool nextKitExists = true;

            while(nextKitExists)
            {
                double average = CalcAverageCoinsForSummsRange(coinsKit, unitSize);

                scoreBoard.TryToInsert((int[])coinsKit.Clone(), average);

                nextKitExists = TryChangetoNextKit(coinsKit, unitSize - 1);
            }

            TimeSpan timeLeft = DateTime.Now - startTime;

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"\nCalculations completed at {timeLeft.Hours} h : {timeLeft.Minutes} m : {timeLeft.Seconds} s : {timeLeft.Milliseconds} ms");

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            if(scoreBoard.Count > 1)
                Console.WriteLine($"\nBest coin kits is : ");
            else
                Console.WriteLine($"\nBest coin kit is : ");

            for(int i = 0; i < scoreBoard.Count; i++)
                Console.WriteLine($"\n{MakeKitString(scoreBoard[i].Item1).FormToLengthRight(kitSize * 4)} | Average : {scoreBoard[i].Item2.ToString().FormToLengthRight(9)}");

            Console.WriteLine($"\nAt unit on {unitSize}");

            Console.ResetColor();

            Console.Write("\nRestart? (Y/N) : ");

            Console.ForegroundColor = ConsoleColor.Yellow;

            string answer = Console.ReadLine();

            Console.ResetColor();

            if(answer.ToLower() == "y")
            {
                Console.Clear();
                goto start;
            }
        }

        static int RequestIntFromConsoleInput(string requestMessage = "\nInput value: ", string errorMessage = "\nError! Please enter int value", string defaultValueKey = "", int defaultValue = 0)
        {
            bool success = false;
            int result = defaultValue;

            while(!success)
            {
                Console.Write(requestMessage);

                Console.ForegroundColor = ConsoleColor.Yellow;

                string inputValue = Console.ReadLine();

                Console.ResetColor();

                if(inputValue == defaultValueKey)
                {
                    success = true;

                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine($"\nSet default value: {result}");

                    Console.ResetColor();
                }
                else
                    success = int.TryParse(inputValue, out result);

                if(!success)
                {
                    WriteErrorMessage(errorMessage);
                    result = defaultValue;
                }
            }

            return result;
        }

        static void WriteErrorMessage(string message = "\nError!")
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(message);

            Console.ResetColor();
        }

        static bool TryChangetoNextKit(int[] kit, int maximalValue)
        {
            bool success = false;

            for(int i = kit.Length - 1; i > 0; i--)
            {
                if(kit[i] < maximalValue - (kit.Length - 1 - i))
                {
                    kit[i]++;
                    i++;

                    for(; i < kit.Length; i++)
                        kit[i] = kit[i - 1] + 1;

                    success = true;

                    break;
                }
            }

            return success;
        }

        static double CalcAverageCoinsForSummsRange(int[] coinsKit, int maxSumm)
        {
            Average result = new Average();

            int[] minCoinNumbers = new int[maxSumm];

            minCoinNumbers[0] = 0;

            for(int targetSumm = 1; targetSumm < maxSumm; targetSumm++)
            {
                minCoinNumbers[targetSumm] = int.MaxValue;

                for(int coinNominalIndex = coinsKit.Length - 1; coinNominalIndex >= 0; coinNominalIndex--)
                {
                    int diff = targetSumm - coinsKit[coinNominalIndex];

                    if(diff >= 0 && minCoinNumbers[diff] + 1 < minCoinNumbers[targetSumm])
                        minCoinNumbers[targetSumm] = minCoinNumbers[diff] + 1;
                }

                result.Add(minCoinNumbers[targetSumm]);
            }

            return result.Val;
        }

        static string MakeKitString(int[] array)
        {
            string result = "1";

            for(int i = 1; i < array.Length; i++)
                result += $", {array[i]}";

            return result;
        }
    }
}
