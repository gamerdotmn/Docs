using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        public static string host = "gamer.mn";
        public static int port_servertoclient = 40001;
        public static int port_servertomonitor = 40002;
        public static int port_monitortoserver = 40003;
        public static int port_broadcast = 40004;
        public static int port_clienttoserver = 40005;
        public static int port_clienttoserver1 = 40005;
        public static int port_clienttoserver2 = 40006;
        public static int port_clienttoserver3 = 40007;
        public static int port_clienttoserver4 = 40008;
        public static int port_clienttoserver5 = 40009;
        public static int port_clienttoserver6 = 40010;
        public static int port_clienttoserver7 = 40011;
        public static int port_clienttoserver8 = 40012;
        public static int port_clienttoserver9 = 40013;
        public static int port_clienttoserver10 = 40014;

        public class PacketMonitorServerTest
        {
            public string command = "test";
            public string text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum";
        }

        public static string Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        public static string Decompress(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        public void Send(string clientip, int port, string message)
        {
            string[] param = new string[3];
            param[0] = clientip;
            param[1] = port.ToString();
            param[2] = message;
            Task.Factory.StartNew(() => _Send(param));
        }

        public void _Send(object obj)
        {
            int step = 60000;
            int sleep = 10;
            string[] param = (string[])obj;
            string ip = param[0];
            int port = Convert.ToInt32(param[1]);
            string data = param[2];
            string id = Guid.NewGuid().ToString();
            data = Program.Compress(data);
            int cnt = data.Length / step;
            while (step * cnt < data.Length)
            {
                cnt = cnt + 1;
            }
            string[] packets = new string[cnt];
            for (int i = 0; i < cnt; i++)
            {
                if (i + 1 == cnt)
                {
                    step = data.Length - i * step;
                }
                packets[i] = data.Substring(i * step, step);
                byte[] message = System.Text.Encoding.UTF8.GetBytes("<rc><id>" + id + "</id><part>" + i + "</part><count>" + cnt + "</count><data>" + packets[i] + "</data></rc>");
                try
                {
                    UdpClient sock = new UdpClient(ip, port);
                    sock.Send(message, message.Length);
                    sock.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int c = 0;
            for (int i = 0; i < 500; i++)
            {
                c++;
                if (c == 1)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver1));
                }
                else if (c == 2)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver2));
                }
                else if (c == 3)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver3));
                }
                else if (c == 4)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver4));
                }
                else if (c == 5)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver5));
                }
                else if (c == 6)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver6));
                }
                else if (c == 7)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver7));
                }
                else if (c == 8)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver8));
                }
                else if (c == 9)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver9));
                }
                else if (c == 10)
                {
                    Task.Factory.StartNew(() => sendtest(Program.port_clienttoserver10));
                }
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        public void sendtest(int port)
        {
            PacketMonitorServerTest packet = new PacketMonitorServerTest();
            Send("127.0.0.1", port, Newtonsoft.Json.JsonConvert.SerializeObject(packet));
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.run();
            Console.ReadKey();
        }
    }
}
