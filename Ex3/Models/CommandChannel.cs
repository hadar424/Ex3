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


namespace Ex3.Models
{
    class CommandChannel
    {
        private int port;
        private string ip;
        private NetworkStream stream;
        private Socket client;
        private TcpListener listener;
        private StreamReader reader;
        private volatile bool closeThread = false;
        private Thread infoThread;
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
            listener = new TcpListener(ep);

            listener.Start();
            // create new thread for the information channel
            infoThread = new Thread(() =>
            {
                while (!closeThread)
                {
                    try
                    {
                        client = listener.AcceptSocket();
                        while (!client.Connected)
                        {
                            try
                            {
                                // try to connect to the simulator as client
                                client.Connect(ep);
                                getInfo(getLon);
                                getInfo(getLan);
                            }
                            catch (SocketException)
                            {
                            }

                        }

                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }
                listener.Stop();
            });
            infoThread.Start();


        }

        public void ReadInfo()
        {
            string info = "";
            if (client.Connected)
            {
                stream = new NetworkStream(client);
                reader = new StreamReader(stream);
                while (!closeThread)
                {
                    // read one line
                    info = reader.ReadLine();
                    //HandleInfo(info);
                    System.Console.WriteLine(info);
                    // initialize info buffer
                    info = "";

                }
            }
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

        public void getInfo(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }
            Byte[] buffer = Encoding.ASCII.GetBytes(command + "\r\n");
            // send the massage to the simulator
            stream.Write(buffer, 0, buffer.Length);

            ReadInfo();
        }





    }
}