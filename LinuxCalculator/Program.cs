using System;

namespace LinuxCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            bool keepRunning = true;
            Console.Clear();
            
            while (keepRunning)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- Linux Native Calculator v2.0 ---");
                Console.ResetColor();
                Console.WriteLine("Available operations: +, -, *, /, ^ (power), exit");
                
                Console.Write("Enter operator (or 'exit'): ");
                string op = Console.ReadLine()?.ToLower() ?? "";

                if (op == "exit")
                {
                    keepRunning = false;
                    continue;
                }

                try
                {
                    Console.Write("Enter first number: ");
                    double num1 = Convert.ToDouble(Console.ReadLine());

                    Console.Write("Enter second number: ");
                    double num2 = Convert.ToDouble(Console.ReadLine());

                    double result = op switch
                    {
                        "+" => num1 + num2,
                        "-" => num1 - num2,
                        "*" => num1 * num2,
                        "/" => num2 != 0 ? num1 / num2 : throw new DivideByZeroException(),
                        "^" => Math.Pow(num1, num2),
                        _ => throw new Exception("Invalid operator")
                    };

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Result: {result}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
            
            Console.WriteLine("Calculator closed. Happy hacking!");
        }
    }
}