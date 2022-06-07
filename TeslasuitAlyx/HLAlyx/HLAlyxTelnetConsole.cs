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
        public void Release()
        {
            m_client.Dispose();
            m_client = null;
        }

    }
}
