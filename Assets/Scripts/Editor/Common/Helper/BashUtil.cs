using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace TaoTie
{
    public static class BashUtil
    {
        public static int RunCommand(string workingDir, string program, params string[] args)
        {
            using (Process p = new Process())
            {
                p.StartInfo.WorkingDirectory = workingDir;
                p.StartInfo.FileName = program;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                string argsStr = string.Join(" ", args.Select(arg => "\"" + arg + "\""));
                p.StartInfo.Arguments = argsStr;
                
                UnityEngine.Debug.Log($"[BashUtil] run => {program} {argsStr}");
                
                p.Start();
                p.WaitForExit();
                return p.ExitCode;
            }
        }


        public static (int ExitCode, string StdOut, string StdErr) RunCommand2(string workingDir, string program, string[] args)
        {
            using (Process p = new Process())
            {
                p.StartInfo.WorkingDirectory = workingDir;
                p.StartInfo.FileName = program;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                string argsStr = string.Join(" ", args);
                p.StartInfo.Arguments = argsStr;
                
                UnityEngine.Debug.Log($"[BashUtil] run => {program} {argsStr}");
                
                p.Start();
                p.WaitForExit();

                string stdOut = p.StandardOutput.ReadToEnd();
                string stdErr = p.StandardError.ReadToEnd();
                return (p.ExitCode, stdOut, stdErr);
            }
        }
        
        public static (int ExitCode, string StdOut, string StdErr) RunCommand3(string workingDir, string program, string[] args)
        {
            var psiNpmRunDist = new ProcessStartInfo
            {
                FileName = program,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = workingDir,
                CreateNoWindow = true,
            };
            var p = Process.Start(psiNpmRunDist);
            string argsStr = string.Join(" ", args);
            p.StandardInput.WriteLine(argsStr+" & exit");
            p.WaitForExit();
            string stdOut = p.StandardOutput.ReadToEnd();
            string stdErr = p.StandardError.ReadToEnd();
            return (p.ExitCode, stdOut, stdErr);
        }
    }
}