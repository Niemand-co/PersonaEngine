using System;
using System.Diagnostics;

namespace Persona
{

    public static class CommandLine
    {
        public static void RunAndLog(string Command)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;
            process.StartInfo = startInfo;

            process.Start();

            string commandWithArgs = Command;

            process.StandardInput.WriteLine(commandWithArgs);
            process.StandardInput.Flush();
            process.StandardInput.Close();

            process.WaitForExit();

            Console.WriteLine(process.StandardOutput.ReadToEnd());
        }
    }

}