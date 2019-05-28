using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace Ex3.Models
{
    class CommandChannel
    {
        private int port;
        private string ip;
        private int time;
        private NetworkStream stream;
        private Socket client;
        private StreamReader reader;
        private Thread commandThread;
        private string getLon = "get /position/longitude-deg";
        private string getLan = "get /position/latitude-deg";
        private string getThrottle = "get /controls/engines/current-engine/throttle";
        private string getRudder = "get /controls/flight/rudder";
            

        #region Singleton
        private static CommandChannel m_Instance = null;
        public static CommandChannel Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new CommandChannel();
                }
                return m_Instance;
            }
        }
        #endregion

        public int CommandPort
        {
            get { return port; }
            set { port = value; }
        }

        public string ServerIP
        {
            get { return ip; }
            set { ip = value; }
        }

        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // create new thread
            commandThread = new Thread(() =>
            {
                while (!client.Connected)
                {
                    try
                    {
                        // try to connect to the simulator as client
                        client.Connect(ep);
                        Debug.WriteLine("connected");
                        stream = new NetworkStream(client);
                        GetInfo();
                    }
                    catch (SocketException)
                    {
                    }

                }
            });
            commandThread.Start();
        }

        public double HandleInfo(string info)
        {
            string parseString = "";
            string[] values = info.Split('\'');
            parseString = values[1];
            return double.Parse(parseString);
        }

        public string GetInfo()
        {
            string lon = "";
            string lan = "";
            string throttle = "";
            string rudder = "";
            reader = new StreamReader(stream);
            while (client.Connected)
            {
                Byte[] bufferLon = Encoding.ASCII.GetBytes(getLon + "\r\n");
                Byte[] bufferLan = Encoding.ASCII.GetBytes(getLan + "\r\n");
                Byte[] bufferThrottle = Encoding.ASCII.GetBytes(getThrottle + "\r\n");
                Byte[] bufferRudder = Encoding.ASCII.GetBytes(getRudder + "\r\n");

                // send the massage to the simulator 
                stream.Write(bufferLon, 0, bufferLon.Length);
                // get lon
                lon = reader.ReadLine();
             
                // send the massage to the simulator 
                stream.Write(bufferLan, 0, bufferLan.Length);
                // get lan
                lan = reader.ReadLine();

                // send the massage to the simulator 
                stream.Write(bufferThrottle, 0, bufferThrottle.Length);
                // get throttle
                throttle = reader.ReadLine();

                // send the massage to the simulator 
                stream.Write(bufferRudder, 0, bufferRudder.Length);
                // get rudder
                rudder = reader.ReadLine();

                string data = lon + " " + lan + " " + throttle + " " + rudder;
                Debug.WriteLine(data);

                return data;
            }
            return "";
        }

        private void ToXml(Double info)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Lon");
            writer.WriteString(info.ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
        }




    }
}