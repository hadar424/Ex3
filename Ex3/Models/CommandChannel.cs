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
        //private Socket client;
        private TcpClient client;
        private StreamReader reader;
        //private Thread commandThread;
        private string getLon = "get /position/longitude-deg";
        private string getLat = "get /position/latitude-deg";
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
            client = new TcpClient();
            //client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (!client.Connected)
            {
                try
                {
                    // try to connect to the simulator as client
                    client.Connect(ep);
                    Debug.WriteLine("connected");
                        
                }
                catch (SocketException)
                {
                }

            }
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
            string lat = "";
            string throttle = "";
            string rudder = "";
            if (client.Connected)
            {
                stream =client.GetStream();
                reader = new StreamReader(stream);
          
                Byte[] bufferLon = Encoding.ASCII.GetBytes(getLon + "\r\n");
                Byte[] bufferLat = Encoding.ASCII.GetBytes(getLat + "\r\n");
                Byte[] bufferThrottle = Encoding.ASCII.GetBytes(getThrottle + "\r\n");
                Byte[] bufferRudder = Encoding.ASCII.GetBytes(getRudder + "\r\n");

                // send the massage to the simulator 
                stream.Write(bufferLon, 0, bufferLon.Length);
                // get lon
                lon = reader.ReadLine();
             
                // send the massage to the simulator 
                stream.Write(bufferLat, 0, bufferLat.Length);
                // get lat
                lat = reader.ReadLine();

                // send the massage to the simulator 
                stream.Write(bufferThrottle, 0, bufferThrottle.Length);
                // get throttle
                throttle = reader.ReadLine();

                // send the massage to the simulator 
                stream.Write(bufferRudder, 0, bufferRudder.Length);
                // get rudder
                rudder = reader.ReadLine();

                string data = HandleInfo(lon).ToString() + " " + HandleInfo(lat).ToString() + " " +
                     HandleInfo(throttle).ToString() + " " + HandleInfo(rudder).ToString();
      
                return data;
            }
            return "";
        }

        public void Disconnect()
        {
            Debug.WriteLine("Command channel disconnect");
            if (client.Connected)
            {

                // close socket
                client.Close();
            }

        }




    }
}