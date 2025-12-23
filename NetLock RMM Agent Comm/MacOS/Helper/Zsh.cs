using Global.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacOS.Helper
{
    internal class Zsh
    {
        public static string Execute_Script(string type, bool decode, string script, int timeout = 0) // 0 = 60 minutes, timeout in minutes
        {
            Process process = null;

            try
            {
                Logging.Debug("MacOS.Helper.Zsh.Execute_Script", "Executing script", $"type: {type}, script length: {script.Length}");

                // Set timeout to 60 minutes if no timeout is set, otherwise convert minutes to milliseconds
                if (timeout == 0)
                    timeout = 3600000; // 60 minutes in milliseconds
                else
                    timeout = timeout * 60 * 1000; // Convert minutes to milliseconds

                if (String.IsNullOrEmpty(script))
                {
                    Logging.Error("MacOS.Helper.Zsh.Execute_Script", "Script is empty", "");
                    return "Error: Script is empty";
                }

                // Decode the script from Base64
                if (decode)
                {
                    byte[] script_data;
                    string decoded_script;

                    try
                    {
                        script_data = Convert.FromBase64String(script);
                        decoded_script = Encoding.UTF8.GetString(script_data);
                    }
                    catch (FormatException ex)
                    {
                        Logging.Error("MacOS.Helper.Zsh.Execute_Script", "Invalid Base64 script", ex.Message);
                        return "Error: Invalid Base64 encoding";
                    }

                    if (String.IsNullOrWhiteSpace(decoded_script))
                    {
                        Logging.Error("MacOS.Helper.Zsh.Execute_Script", "Decoded script is empty", String.Empty);
                        return "Error: Decoded script is empty";
                    }

                    // Convert Windows line endings (\r\n) to Unix line endings (\n)
                    script = decoded_script.Replace("\r\n", "\n");

                    Logging.Debug("MacOS.Helper.Zsh.Execute_Script", "Decoded script", script);
                }

                // Create a new process
                process = new Process();
                process.StartInfo.FileName = "/bin/zsh";
                process.StartInfo.Arguments = "-s"; // Read script from standard input
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                // Use StringBuilder for better performance when reading large outputs
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                // Asynchronous output reading to avoid deadlocks
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        output.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                        error.AppendLine(e.Data);
                };

                // Start the process
                process.Start();

                // Write the script to the process's standard input
                using (StreamWriter writer = process.StandardInput)
                {
                    writer.Write(script);
                }

                // Begin asynchronous reading
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for the process to exit, with a timeout
                bool exited = process.WaitForExit(timeout);

                if (!exited)
                {
                    // Timeout occurred - kill the process
                    process.Kill(true);
                    process.WaitForExit(); // Ensure async streams are flushed
                    string timeoutMessage = $"Error: Script execution timed out after {timeout / 60000} minutes.";
                    Logging.Error("MacOS.Helper.Zsh.Execute_Script", "Script execution timed out", $"Timeout: {timeout}ms");
                    return timeoutMessage;
                }

                // Wait for async output reading to complete
                process.WaitForExit();

                string result = output.ToString();
                string errorOutput = error.ToString();

                if (!String.IsNullOrWhiteSpace(errorOutput))
                {
                    Logging.Error("MacOS.Helper.Zsh.Execute_Script", "Script produced error output", errorOutput);
                    result += Environment.NewLine + "STDERR: " + errorOutput;
                }

                int exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    Logging.Error("MacOS.Helper.Zsh.Execute_Script", $"Script exited with code {exitCode}", errorOutput);
                    result += Environment.NewLine + $"Exit Code: {exitCode}";
                }
                else
                {
                    Logging.Debug("MacOS.Helper.Zsh.Execute_Script", "Script execution successful", $"Exit code: {exitCode}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Logging.Error("MacOS.Helper.Zsh.Execute_Script", "Error executing script", ex.ToString());
                return "Error: " + ex.Message;
            }
            finally
            {
                // Ensure process is disposed
                if (process != null)
                {
                    try
                    {
                        if (!process.HasExited)
                            process.Kill(true);

                        process.Dispose();
                    }
                    catch
                    {
                        // Ignore disposal errors
                    }
                }
            }
        }
    }
}
