using System;
using System.Diagnostics;

using Renci.SshNet;
using Renci.SshNet.Common;

namespace Siemplify.Integrations.WebSense.SSH
{
    public class ContentGatewayShell : SshClient
    {
        private const string DEFAULT_SHELL = "sshclient";
        private const uint COLUMNS = 80;
        private const uint ROWS = 24;
        private const uint WIDTH = 800;
        private const uint HEIGHT = 600;
        private const int BUFFER_SIZE = 4 * 1024;

        public static readonly string SHELL_READY_INDICATOR = string.Format("[{0}@{1} ~]#",
            Settings.GatewayUser, Settings.GatewayHostname);

        private static readonly Tuple<string, string> ReplacableShellString =
            new Tuple<string, string>("\r\r\n", string.Empty);

        private static readonly Tuple<string, string> ReplacableBreakLine =
            new Tuple<string, string>("\r\n\r\n", "\n");

        private const string UNIX_LINEBREAK = "\n";

        public TimeSpan CommandTimeout { get; private set; }
        private ShellStream _shell;
        private object _syncObj;
        private bool _isShellReady;

        public ContentGatewayShell(string host, string username, string pass)
            : base(host, username, pass)
        {
            ErrorOccurred += ContentGatewayShell_ErrorOccurred;
            _syncObj = new object();
            _isShellReady = false;
            CommandTimeout = new TimeSpan(0, 1, 0); // Timeout is 1 minute
        }

        public ContentGatewayShell(ConnectionInfo conn)
            : base(conn)
        {
            ErrorOccurred += ContentGatewayShell_ErrorOccurred;
            _syncObj = new object();
            _isShellReady = false;
            CommandTimeout = new TimeSpan(0, 1, 0); // Timeout is 1 minute
        }

        public void StartShell(string terminalName = DEFAULT_SHELL, uint columns = COLUMNS, uint rows = ROWS, uint width = WIDTH, uint height = HEIGHT, int bufferSize = BUFFER_SIZE)
        {
            if (!IsConnected)
                Connect();

            _shell = CreateShellStream(terminalName, columns, rows, width, height, bufferSize);

            // Wait for the shell to be ready by waiting for the console indicator to appear on the stream
            var result = _shell.Expect(SHELL_READY_INDICATOR, CommandTimeout);
            _isShellReady = true;
        }

        public string RunShellCommand(string commandText)
        {
            if (!_isShellReady)
                return null;

            lock (_syncObj)
            {
                _shell.Write(commandText + UNIX_LINEBREAK);

                var result = _shell.Expect(SHELL_READY_INDICATOR, CommandTimeout);

                // Replace the string \r\r\n which returns from the shell with empty string
                result = result.Replace(ReplacableShellString.Item1, ReplacableShellString.Item2);
                // Replace the breakline which returns from the shell with single \n
                result = result.Replace(ReplacableBreakLine.Item1, ReplacableBreakLine.Item2);

                Debug.WriteLine(string.Format("Shell command {0} returned with {1}",
                    commandText,
                    result));

                return result;
            }
        }

        public void CloseShell()
        {
            _shell.Close();
            _isShellReady = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (null != _shell)
            {
                CloseShell();
                _shell.Dispose();
            }

            Disconnect();
            base.Dispose(disposing);
        }

        public void ContentGatewayShell_ErrorOccurred(object sender, ExceptionEventArgs e)
        {
            Debug.WriteLine(
                string.Format("Error occurred in ssh client, source: {0} hresult: {1} message: {2}"),
                e.Exception.Source,
                e.Exception.HResult,
                e.Exception.Message);

            Console.WriteLine("Error occurred in ssh client, source: {0} hresult: {1} message: {2}",
                e.Exception.Source,
                e.Exception.HResult,
                e.Exception.Message);
        }
    }
}
