﻿using System;
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

        public double HandleInfo(string info)
        {
            string parseString = "";
            string[] values = info.Split('\'');
            parseString = values[1];
            return double.Parse(parseString);
        }

        public void GetInfo()
        {
            string lon = "";
            string lan = "";
            reader = new StreamReader(stream);
            while (client.Connected)
            {
                Byte[] bufferLon = Encoding.ASCII.GetBytes(getLon + "\r\n");
                Byte[] bufferLan = Encoding.ASCII.GetBytes(getLan + "\r\n");

                // send the massage to the simulator 
                stream.Write(bufferLon, 0, bufferLon.Length);
                // get lon
                lon = reader.ReadLine();
                // initialize info buffer
                lon = "";

                // send the massage to the simulator 
                stream.Write(bufferLan, 0, bufferLan.Length);
                // get lan
                lan = reader.ReadLine();
                // initialize info buffer
                lan = "";

                double lonValue = HandleInfo(lon);
                double lanValue = HandleInfo(lan);

            }
        }





    }
}