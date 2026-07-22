using System;
using System.IO;
using System.Threading;

namespace HealthMonitor
{
    class Program
    {
        // Define the log file name
        private const string LogFile = "system_health.log";

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("--- Linux System Health Monitor ---");
            Console.WriteLine("Logging to: " + LogFile);
            Console.WriteLine("Press Ctrl+C to stop.");

            while (true)
            {
                // 1. Get CPU Load
                string loadAvg = File.ReadAllText("/proc/loadavg").Split(' ')[0];

                // 2. Get Memory Info
                string[] memLines = File.ReadAllLines("/proc/meminfo");
                long totalMem = ParseMemLine(memLines[0]);
                long availMem = ParseMemLine(memLines[2]);
                long usedMem = totalMem - availMem;

                // 3. Format strings
                string statusLine = $"CPU: {loadAvg} | Mem: {usedMem / 1024}MB / {totalMem / 1024}MB";
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {statusLine}";

                // 4. Output to Console
                Console.SetCursorPosition(0, 3);
                Console.WriteLine($"{statusLine}           ");
                Console.WriteLine($"Timestamp: {DateTime.Now:HH:mm:ss}        ");

                // 5. Append to Log File
                File.AppendAllText(LogFile, logEntry + Environment.NewLine);

                Thread.Sleep(5000); // Poll every 5 seconds
            }
        }

        static long ParseMemLine(string line)
        {
            var parts = line.Split(':');
            var valParts = parts[1].Trim().Split(' ');
            return long.Parse(valParts[0]);
        }
    }
}