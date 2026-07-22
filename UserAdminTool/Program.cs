using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EnterpriseSaaS.UserManagement
{
    // --- 1. DOMAIN MODELS ---
    public record CommandResult(bool Success, string Message, string ErrorCode = "SUCCESS");
    // Added 'Status' field here
    public record UserReport(string Username, string Created, string LastLogin, string Status);

    // --- 2. ABSTRACTIONS ---
    public interface IUserEngine
    {
        CommandResult CreateUser(string username, string password);
        CommandResult DeleteUser(string username);
        CommandResult ResetPassword(string username, string password);
        CommandResult LockUser(string username);
        CommandResult UnlockUser(string username);
        IEnumerable<UserReport> GetUserList();
    }

    public interface IAuditService { void LogAction(string action, string target, CommandResult result); }

    // --- 3. INFRASTRUCTURE ---
    public class LinuxUserEngine : IUserEngine
    {
        private readonly HashSet<string> _excludedUsers = new(StringComparer.OrdinalIgnoreCase) 
        { "root", "nobody", "bin", "daemon", "sys", "sync", "games", "man", "lp", "mail", "news", "uucp", "proxy", "www-data", "backup", "list", "irc", "gnats" };

        public CommandResult CreateUser(string username, string password)
        {
            if (RunShell("id", username) == 0) return new(false, "User already exists.", "ERR_USER_EXISTS");
            var exit = RunShell("useradd", $"-m -s /bin/bash {username}");
            return exit == 0 ? ResetPassword(username, password) : new(false, "System failure.", "ERR_KERNEL");
        }

        public CommandResult DeleteUser(string username)
        {
            if (RunShell("id", username) != 0) return new(false, "User not found.", "ERR_NOT_FOUND");
            var exit = RunShell("userdel", $"-r {username}");
            return exit == 0 ? new(true, "Purged.") : new(false, "OS IO failure.", "ERR_OS_IO");
        }

        public CommandResult ResetPassword(string username, string password)
        {
            var p = new Process { StartInfo = new ProcessStartInfo("chpasswd") { UseShellExecute = false, RedirectStandardInput = true } };
            p.Start();
            p.StandardInput.WriteLine($"{username}:{password}");
            p.StandardInput.Close();
            p.WaitForExit();
            return p.ExitCode == 0 ? new(true, "Password updated.") : new(false, "Auth failure.", "ERR_AUTH");
        }

        public CommandResult LockUser(string username)
        {
            var exit = RunShell("usermod", $"-L {username}");
            return exit == 0 ? new(true, "Account locked.") : new(false, "Failed to lock.", "ERR_LOCK");
        }

        public CommandResult UnlockUser(string username)
        {
            var exit = RunShell("usermod", $"-U {username}");
            return exit == 0 ? new(true, "Account unlocked.") : new(false, "Failed to unlock.", "ERR_UNLOCK");
        }

        public IEnumerable<UserReport> GetUserList()
        {
            var reports = new List<UserReport>();
            foreach (var line in File.ReadLines("/etc/passwd"))
            {
                var parts = line.Split(':');
                if (parts.Length < 3) continue;
                string name = parts[0];
                int uid = int.Parse(parts[2]);

                if (!_excludedUsers.Contains(name) && uid >= 1000 && uid < 60000)
                {
                    string home = $"/home/{name}";
                    string created = Directory.Exists(home) ? CaptureShell("stat", $"-c %y {home}").Split('.')[0] : "N/A";
                    string last = CaptureShell("lastlog", $"-u {name}").Split('\n').LastOrDefault()?.Trim() ?? "Never";
                    
                    // Logic to check lock status via passwd -S
                    string pStatus = CaptureShell("passwd", $"-S {name}");
                    string status = (pStatus.Contains(" L ") || pStatus.StartsWith(name + " L")) ? "LOCKED" : "ACTIVE";
                    
                    reports.Add(new UserReport(name, created, last, status));
                }
            }
            return reports;
        }

        private int RunShell(string cmd, string args)
        {
            try { using var p = Process.Start(new ProcessStartInfo(cmd, args) { RedirectStandardError = true, UseShellExecute = false }); p?.WaitForExit(); return p?.ExitCode ?? -1; } 
            catch { return -1; }
        }

        private string CaptureShell(string cmd, string args)
        {
            try { using var p = Process.Start(new ProcessStartInfo(cmd, args) { RedirectStandardOutput = true, UseShellExecute = false }); string output = p?.StandardOutput.ReadToEnd() ?? ""; p?.WaitForExit(); return output; } 
            catch { return "Error"; }
        }
    }

    // --- 4. DECORATOR ---
    public class AuditDecorator : IUserEngine
    {
        private readonly IUserEngine _inner;
        private readonly IAuditService _audit;
        public AuditDecorator(IUserEngine inner, IAuditService audit) { _inner = inner; _audit = audit; }
        public CommandResult CreateUser(string u, string p) { var r = _inner.CreateUser(u, p); _audit.LogAction("CREATE", u, r); return r; }
        public CommandResult DeleteUser(string u) { var r = _inner.DeleteUser(u); _audit.LogAction("DELETE", u, r); return r; }
        public CommandResult ResetPassword(string u, string p) { var r = _inner.ResetPassword(u, p); _audit.LogAction("PWD", u, r); return r; }
        public CommandResult LockUser(string u) { var r = _inner.LockUser(u); _audit.LogAction("LOCK", u, r); return r; }
        public CommandResult UnlockUser(string u) { var r = _inner.UnlockUser(u); _audit.LogAction("UNLOCK", u, r); return r; }
        public IEnumerable<UserReport> GetUserList() => _inner.GetUserList();
    }

    public class FileAuditService : IAuditService
    {
        public void LogAction(string action, string target, CommandResult res) =>
            File.AppendAllText("audit.log", $"{DateTime.UtcNow:O} | {action} | {target} | {res.Success} | {res.ErrorCode}\n");
    }

    // --- 5. APPLICATION LAYER ---
    class Program
    {
        private static readonly string[] Commands = { "create", "delete", "reset", "lock", "unlock", "list", "help", "exit" };

        static void Main()
        {
            IUserEngine engine = new AuditDecorator(new LinuxUserEngine(), new FileAuditService());
            Console.WriteLine("=== ENTERPRISE CLOUD ADMIN v2.2.1 ===");
            
            while (true)
            {
                Console.Write($"\n[Options: {string.Join(", ", Commands)}]\nAdmin> ");
                var raw = Console.ReadLine()?.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(raw)) continue;
                
                var input = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var cmd = input[0];

                if (cmd == "exit") break;

                switch (cmd)
                {
                    case "create": Handle(engine.CreateUser(Prompt("Username: "), Prompt("Password: "))); break;
                    case "delete": Handle(engine.DeleteUser(Prompt("Username: "))); break;
                    case "reset":  Handle(engine.ResetPassword(Prompt("Username: "), Prompt("New Password: "))); break;
                    case "lock":   Handle(engine.LockUser(Prompt("Username: "))); break;
                    case "unlock": Handle(engine.UnlockUser(Prompt("Username: "))); break;
                    case "list":   PrintTable(engine.GetUserList()); break;
                    case "help":   Console.WriteLine($"Commands: {string.Join(", ", Commands)}"); break;
                    default:       Console.WriteLine($"Unknown command: '{cmd}'"); break;
                }
            }
        }

        static void Handle(CommandResult res) => Console.WriteLine(res.Success ? $"[SUCCESS] {res.Message}" : $"[ERROR {res.ErrorCode}] {res.Message}");
        static string Prompt(string msg) { 
            string input = "";
            while (string.IsNullOrWhiteSpace(input)) {
                Console.Write(msg);
                input = Regex.Replace(Console.ReadLine() ?? "", @"[^a-zA-Z0-9-!@#$]", "");
            }
            return input;
        }
        
        static void PrintTable(IEnumerable<UserReport> list)
        {
            var reports = list.ToList();
            if (!reports.Any()) { Console.WriteLine("No user records found."); return; }
            Console.WriteLine($"{"NAME".PadRight(15)} | {"CREATED".PadRight(20)} | {"LAST LOGIN".PadRight(15)} | {"STATUS"}");
            foreach (var r in reports) Console.WriteLine($"{r.Username.PadRight(15)} | {r.Created.PadRight(20)} | {r.LastLogin.PadRight(15)} | {r.Status}");
        }
    }
}