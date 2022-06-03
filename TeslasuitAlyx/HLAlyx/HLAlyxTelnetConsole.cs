using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telnet = PrimS.Telnet;

namespace TeslasuitAlyx
{
    public class HLAlyxTelnetConsole
    {
        public bool IsConnected => m_client!=null ? m_client.IsConnected : false;
        private Telnet.Client m_client = null;
        private string m_hostname;
        private int m_port;
        

        public HLAlyxTelnetConsole(string hostname, int port)
        {
            m_hostname = hostname;
            m_port = port;
        }

        public HLAlyxTelnetConsole(int port) : this("127.0.0.1", port) {}

        public void Connect()
        { 
            m_client = new Telnet.Client(m_hostname, m_port, new CancellationToken());
        }

        public string ReadLine()
        {
            int length = 0;
            string res = string.Empty;
            while (!res.Contains("\n") && m_client.IsConnected)
            {
                res += m_client.ReadAsync().Result;
            }

            return res;
        }

        public void Write(string line)
        {
            if(IsConnected)
            {
                m_client.WriteLine(line);
            }
        }

        public async Task<string> TerminatedReadAsync(string terminator, TimeSpan timeout, int millisecondSpin)
        {
            Func<string, bool> isTerminated = (x) => IsTerminatorLocated(terminator, x);
            var s = await this.TerminatedReadAsync(isTerminated, timeout, millisecondSpin).ConfigureAwait(false);
            if (!isTerminated(s))
            {
                System.Diagnostics.Debug.WriteLine("Failed to terminate '{0}' with '{1}'", s, terminator);
            }

            return s;
        }

        protected static bool IsTerminatorLocated(string terminator, string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }

            return s.Contains(terminator);
        }

        private async Task<string> TerminatedReadAsync(Func<string, bool> isTerminated, TimeSpan timeout, int millisecondSpin)
        {
            var endTimeout = DateTime.Now.Add(timeout);
            var s = string.Empty;
            while (!isTerminated(s) && endTimeout >= DateTime.Now)
            {
                var read = await m_client.ReadAsync(TimeSpan.FromMilliseconds(millisecondSpin)).ConfigureAwait(false);
                s += read;
            }

            return s;
        }

        public void Disconnect()
        {
            m_client.Dispose();
            m_client = null;
        }

    }
}
