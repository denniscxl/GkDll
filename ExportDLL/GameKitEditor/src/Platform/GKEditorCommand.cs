using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GKPlatform
{
    public class GKEditorCommand
    {
        public static void ProcessCommand(string command, string argument)
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
            info.Arguments = argument;
            info.CreateNoWindow = false;
            info.ErrorDialog = true;
            info.UseShellExecute = true;

            if (info.UseShellExecute)
            {
                info.RedirectStandardOutput = false;
                info.RedirectStandardError = false;
                info.RedirectStandardInput = false;
            }
            else
            {
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.RedirectStandardInput = true;
                info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
                info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
            }

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

            if (!info.UseShellExecute)
            {
                UnityEngine.Debug.Log(process.StandardOutput);
                UnityEngine.Debug.Log(process.StandardError);
            }

            process.WaitForExit();
            process.Close();
        }

    }
}