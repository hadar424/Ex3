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

namespace Ex3.Models
{
    class CommandChannel
    {
        private int port;
        private string ip;
        private NetworkStream stream;
        private Socket client;
        private StreamReader reader;
        private Thread commandThread;
        private string getLon = "get /position/longitude-deg";
        private string getLan = "get /position/latitude-deg";

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

        public void HandleInfo(string info)
        {
            if (info != null)
            {
                int first = info.IndexOf(",");
                int second = info.IndexOf(",", info.IndexOf(",") + 1);
                // get the substring of the lon value
                string lon = info.Substring(0, first - 1);
                // get the substring of the lat value
                string lat = info.Substring(first + 1, second - first - 1);

                // send lon and lat
                /*
                // convert lon from string to float
                FlightBoardViewModel.Instance.Lon = float.Parse(lon);
                // convert lat from string to float
                FlightBoardViewModel.Instance.Lat = float.Parse(lat);
                */
            }
        }

        public void GetInfo()
        {
            string info = "";
            reader = new StreamReader(stream);
            while (client.Connected)
            {
                Byte[] bufferLon = Encoding.ASCII.GetBytes(getLon + "\r\n");
                Byte[] bufferLan = Encoding.ASCII.GetBytes(getLan + "\r\n");

                // send the massage to the simulator 
                stream.Write(bufferLon, 0, bufferLon.Length);
                // get lon
                info = reader.ReadLine();
                Debug.WriteLine(info);
                // initialize info buffer
                info = "";

                // send the massage to the simulator 
                stream.Write(bufferLan, 0, bufferLan.Length);
                // get lan
                info = reader.ReadLine();
                //HandleInfo(info);
                Debug.WriteLine(info);
                // initialize info buffer
                info = "";

            }
        }





    }
}