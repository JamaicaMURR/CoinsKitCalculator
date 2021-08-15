using System;
using Bycicles;
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
                kitSize = RequestIntFromConsoleInput($"\nHow many nominals of coins in coins kit? (1 < Kit size < {unitSize}): ", defaultValue: 4);

                if(kitSize <= 1)
                    WriteErrorMessage("\nError! Kit size must be > 1");
                else if(kitSize >= unitSize)
                    WriteErrorMessage($"\nError! Kit size must be < {unitSize}");
                else
                    break;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            int breakingValueOf2ndNominal;

            while(true) // breakingValueOf2ndNominal input
            {
                breakingValueOf2ndNominal = RequestIntFromConsoleInput("\nInput critical value for 2nd nominal : ", defaultValue: 10);

                if(breakingValueOf2ndNominal < 2)
                    WriteErrorMessage("\nError! Critical value for 2nd nominal must be >=2");
                else
                    break;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            double bestAverage = unitSize;

            int[] coinsKit = new int[kitSize];

            for(int i = 1; i <= coinsKit.Length; i++)
                coinsKit[i - 1] = i;

            int[] bestKit = new int[coinsKit.Length];

            coinsKit.CopyTo(bestKit, 0);

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            DateTime startTime = DateTime.Now;

            bool nextKitExists = true;

            while(nextKitExists)
            {

                Average averageCoins = new();

                SummChangingRun(1, unitSize, averageCoins);

                if(averageCoins.Val < bestAverage)
                {
                    bestAverage = averageCoins.Val;
                    coinsKit.CopyTo(bestKit, 0);

                    Console.WriteLine($"{MakeKitString(bestKit)} : {bestAverage.ToString().FormToLengthRight(8)}");
                }

                nextKitExists = TryChangetoNextKit(coinsKit, unitSize - 1);

                if(coinsKit[1] > breakingValueOf2ndNominal)
                    break;
            }

            TimeSpan timeLeft = DateTime.Now - startTime;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"\nCalculations completed at {timeLeft.Hours} h : {timeLeft.Minutes} m : {timeLeft.Seconds} s : {timeLeft.Milliseconds} ms");

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            Console.WriteLine($"\nBest coins kit is {MakeKitString(bestKit)}");
            Console.WriteLine($"\n{bestAverage.ToString().FormToLengthRight(8)} - average coins needed to find change");
            Console.WriteLine($"\nAt unit on {unitSize}");

            Console.ResetColor();

            Console.WriteLine("\nRestart? (Y/N) :");

            string answer = Console.ReadLine();

            if(answer.ToLower() == "y")
                goto start;

            // Inner methods
            void SummChangingRun(int startValue, int finishValue, Average result)
            {
                for(int currentSumm = startValue; currentSumm < finishValue; currentSumm++)
                    result.Add(CalcLowestCoinsNeed(coinsKit, currentSumm));
            }

            string MakeKitString(int[] array)
            {
                string result = "1";

                for(int i = 1; i < array.Length; i++)
                    result += $", {array[i]}";

                return result;
            }
        }

        static int RequestIntFromConsoleInput(string requestMessage = "\nInput value: ", string errorMessage = "\nError! Please enter int value", string defaultValueKey = "", int defaultValue = 0)
        {
            bool success = false;
            int result = defaultValue;

            while(!success)
            {
                Console.Write(requestMessage);

                string inputValue = Console.ReadLine();

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

        static int CalcLowestCoinsNeed(int[] kit, int summ)
        {
            int lowestCoinsNeed = int.MaxValue;

            for(int currentKitSize = 2; currentKitSize <= kit.Length; currentKitSize++)
            {
                int coinsNeed = 0;
                int change = summ;

                for(int i = currentKitSize - 1; i >= 0; i--)
                {
                    int numberOfCoins = change / kit[i];

                    coinsNeed += numberOfCoins;
                    change -= numberOfCoins * kit[i];
                }

                if(coinsNeed < lowestCoinsNeed)
                    lowestCoinsNeed = coinsNeed;
            }

            return lowestCoinsNeed;
        }
    }
}
