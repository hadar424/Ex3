using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Ex3.Models;

namespace Ex3.Controllers
{
    public class FirstController : Controller
    {
        #region Singleton
        private static FirstController m_Instance = null;
        public static FirstController Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new FirstController();
                }
                return m_Instance;
            }
        }
        #endregion

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        private int arrayIndex;
        public int ArrayIndex
        {
            get { return arrayIndex; }
            set { arrayIndex = value; }
        }


        public ActionResult Default()
        {
            return View();

        }

        public ActionResult Save(string ip, int port, int time, int saveTime, string file)
        {
            Debug.WriteLine("save");
            Debug.WriteLine(file);
            
            ViewBag.ip = ip;
            ViewBag.port = port;
            ViewBag.time = time;
            ViewBag.saveTime = saveTime;
            ViewBag.file = file;
            FirstController.Instance.FileName = file;
            Debug.WriteLine("!!!!!!!!!!" + FileName);

            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;
            CommandChannel.Instance.Time = time;
            CommandChannel.Instance.Start();

            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            float throttle = getData(data, 2);
            float rudder = getData(data, 3);

            ViewBag.lon = lon;
            ViewBag.lat = lat;
            ViewBag.throttle = throttle;
            ViewBag.rudder = rudder;

            // read from file
            Session["time"] = time;
            Session["saveTime"] = saveTime;

            return View();

        }

        public string saveToFile()
        {
            Debug.WriteLine("saveToFile");
            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            float throttle = getData(data, 2);
            float rudder = getData(data, 3);


            string fName = FirstController.Instance.FileName;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fName + ".txt";
            using (StreamWriter streamWriter = System.IO.File.AppendText(filePath))
            {
                streamWriter.WriteLine(lon.ToString() + ' ' + lat.ToString() + ' ' + throttle.ToString() +
                    ' ' + rudder.ToString());
            }

            return ToXml(data);

            
        }

        public ActionResult Load(string file, int time)
        {
            FirstController.Instance.FileName = file;
            FirstController.Instance.ArrayIndex = 0;
            Session["time"] = time;

            Debug.WriteLine("doneLoad");

            return View("Load");
        }

        // GET: First
        public ActionResult Index(string ip, int port)
        {
            Debug.WriteLine("Index");

            if (ip.IndexOf(".") == -1)
            {
                Debug.Write("in if");
                return Load(ip, port);

            }

            ViewBag.ip = ip;
            ViewBag.port = port;
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;
            CommandChannel.Instance.Start();
            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);

            ViewBag.lon = lon;
            ViewBag.lat = lat;

            return View();
        }

        public float getData(string line, int index)
        {
            string parseString = "";
            string[] values = line.Split(' ');
            parseString = values[index];
            Debug.WriteLine(parseString);

            return float.Parse(parseString);
        }

        public ActionResult Display(string ip, int port, int time)
        {
            Debug.WriteLine("Display");

            ViewBag.ip = ip;
            ViewBag.port = port;
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;
            CommandChannel.Instance.Time = time;
            CommandChannel.Instance.Start();


            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);

            ViewBag.lon = lon;
            ViewBag.lat = lat;
            ViewBag.lat = lat;

            // read from file
            Session["time"] = time;

            return View();
        }

        public string GetLonLatFile()
        {
            Debug.WriteLine("GetLonLatFile");
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + FirstController.Instance.FileName + ".txt";
            string data = "";
            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (FirstController.Instance.ArrayIndex < lines.Length)
            {
                data = lines[FirstController.Instance.ArrayIndex];
                FirstController.Instance.ArrayIndex += 1;

            }
            return ToXml(data);
        }

        [HttpPost]
        public string GetLonLat()
        {
            Debug.WriteLine("GetLonLat");
             string data = CommandChannel.Instance.GetInfo();
            

            return ToXml(data);
        }

        [HttpPost]
        public void CloseServer()
        {


            Debug.WriteLine("closeServer");
            CommandChannel.Instance.Disconnect();
        }

        private string ToXml(string data)
        {
            Debug.WriteLine("ToXml");

            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            // parse data string
            if (data == "")
            {
                return data;
            }
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            float throttle = getData(data, 2);
            float rudder = getData(data, 3);

            writer.WriteStartDocument();
            writer.WriteStartElement("Sampling");
            writer.WriteElementString("Lon", lon.ToString());
            writer.WriteElementString("Lat", lat.ToString());
            writer.WriteElementString("Throttle", throttle.ToString());
            writer.WriteElementString("Rudder", rudder.ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
        public string Search(string name)
        {
            return "";
        }
    }
}