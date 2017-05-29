﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using PatchKit.Unity.Patcher.Debug;
using PatchKit.Unity.Utilities;
using UnityEngine;

namespace PatchKit.Unity.Patcher.AppData.Remote.Downloaders
{
    /// <summary>
    /// Provides an easy access for torrent client program.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TorrentClient : IDisposable
    {
        private const string TorrentClientWinPath = "torrent-client/win/torrent-client.exe";
        private const string TorrentClientOsx64Path = "torrent-client/osx64/torrent-client";
        private const string TorrentClientLinux64Path = "torrent-client/linux64/torrent-client";
        private static readonly DebugLogger DebugLogger = new DebugLogger(typeof(TorrentClient));

        private string _streamingAssetsPath;

        private readonly Process _process;

        private readonly StreamReader _stdOutput;

        private readonly StreamWriter _stdInput;

        private bool _disposed;

        public TorrentClient()
        {
            DebugLogger.LogConstructor();

            Dispatcher.Invoke(() =>
            {
                _streamingAssetsPath = Application.streamingAssetsPath;
            }).WaitOne();

            _process = StartProcess();
            _stdOutput = CreateStdOutputStream();
            _stdInput = CreateStdInputStream();
        }

        /// <summary>
        /// Executes the command and returns the result.
        /// </summary>
        public JToken ExecuteCommand(string command)
        {
            Checks.ArgumentNotNull(command, "command");

            DebugLogger.Log(string.Format("Executing command {0}", command));

            WriteCommand(command);
            string resultStr = ReadCommandResult();
            return ParseCommandResult(resultStr);
        }

        private void WriteCommand(string command)
        {
            _stdInput.WriteLine(command);
            _stdInput.Flush();
        }

        private JToken ParseCommandResult(string resultStr)
        {
            return JToken.Parse(resultStr);
        }

        private string ReadCommandResult()
        {
            var str = new StringBuilder();

            while (!str.ToString().EndsWith("#=end"))
            {
                ThrowIfProcessExited();

                str.Append((char)_stdOutput.Read());
            }

            return str.ToString().Substring(0, str.Length - 5);
        }

        private void ThrowIfProcessExited()
        {
            if (_process.HasExited)
            {
                throw new TorrentClientException("torrent-client process has exited.");
            }
        }

        private StreamReader CreateStdOutputStream()
        {
            return new StreamReader(_process.StandardOutput.BaseStream, CreateStdEncoding());
        }

        private StreamWriter CreateStdInputStream()
        {
            return new StreamWriter(_process.StandardInput.BaseStream, CreateStdEncoding());
        }

        private Encoding CreateStdEncoding()
        {
            return new UTF8Encoding(false);
        }

        private Process StartProcess()
        {
            var processStartInfo = GetProcessStartInfo();
            
            return Process.Start(processStartInfo);
        }

        private ProcessStartInfo GetProcessStartInfo()
        {
            if (Platform.IsWindows())
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = _streamingAssetsPath.PathCombine(TorrentClientWinPath),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                return processStartInfo;
            }

            if (Platform.IsOSX())
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = _streamingAssetsPath.PathCombine(TorrentClientOsx64Path),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // make sure that binary can be executed
                Chmod.SetExecutableFlag(processStartInfo.FileName);

                processStartInfo.EnvironmentVariables["DYLD_LIBRARY_PATH"] = Path.Combine(_streamingAssetsPath, "torrent-client/osx64");

                return processStartInfo;
            }

            if (Platform.IsLinux() && IntPtr.Size == 8) // Linux 64 bit
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = _streamingAssetsPath.PathCombine(TorrentClientLinux64Path),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // make sure that binary can be executed
                Chmod.SetExecutableFlag(processStartInfo.FileName);

                processStartInfo.EnvironmentVariables["LD_LIBRARY_PATH"] = Path.Combine(_streamingAssetsPath, "torrent-client/linux64");

                return processStartInfo;
            }

            throw new TorrentClientException("Unsupported platform by torrent-client.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TorrentClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed)
            {
                return;
            }

            DebugLogger.LogDispose();

            if(disposing)
            {
                _stdOutput.Dispose();
                _stdInput.Dispose();

                if (!_process.HasExited)
                {
                    _process.Kill();
                }
            }

            _disposed = true;
        }
    }
}
