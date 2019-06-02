using System;
using System.IO;
using System.Net;
using System.Text;
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
            // send the parameters to the model
            FirstController.Instance.FileName = file;
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;

            CommandChannel.Instance.Disconnect();
            CommandChannel.Instance.Start();
            // read from file
            Session["time"] = time;
            Session["saveTime"] = saveTime;

            return View();
        }

        public string SaveToFile()
        {
            // get the data from the server
            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            float throttle = getData(data, 2);
            float rudder = getData(data, 3);

            // save the data in the file
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
            // save the parameters for loading
            FirstController.Instance.FileName = file;
            FirstController.Instance.ArrayIndex = 0;
            Session["time"] = time;

            return View("Load");
        }

        // GET: First
        public ActionResult Index(string ip, int port)
        {
            // check if the string is IP or file name (if file name - goto Load function)
            System.Net.IPAddress IP = null;
            // check which kind of argument we received 
            bool check = IPAddress.TryParse(ip, out IP);
            if (check == false)
            {
                return Load(ip, port);
            }

            // save the parameters and send to the model
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;
            CommandChannel.Instance.Disconnect();
            CommandChannel.Instance.Start();

            // get the data from  the server and save in the view bag
            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            ViewBag.lon = lon;
            ViewBag.lat = lat;

            return View();
        }

        public float getData(string line, int index)
        {
            // get the data according to the index - split by ' '
            string parseString = "";
            string[] values = line.Split(' ');
            parseString = values[index];
            float value;
            if (!float.TryParse(parseString, out value))
            {
                throw new System.FormatException();
            }
            return value;
        }

        public ActionResult Display(string ip, int port, int time)
        {
            // save the parameters and send to the model
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;

            CommandChannel.Instance.Disconnect();
            CommandChannel.Instance.Start();

            // sand the time to view
            Session["time"] = time;

            return View();
        }

        [HttpPost]
        public string GetLonLat()
        {
            // send the parameters to the xml and send to the view
            string data = CommandChannel.Instance.GetInfo();
            return ToXml(data);
        }

        public string GetLonLatFile()
        {
            // get the lon and lat values from the file
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + FirstController.Instance.FileName + ".txt";
            string data = "";
            // check if file exists
            if (!System.IO.File.Exists(filePath)) { 
                throw new FileNotFoundException();
             }
            // read all the lines from the file and split them
            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (FirstController.Instance.ArrayIndex < lines.Length)
            {
                // get the data from index row on the file
                data = lines[FirstController.Instance.ArrayIndex];
                FirstController.Instance.ArrayIndex += 1;
            }
            return ToXml(data);
        }

        [HttpPost]
        public void CloseServer()
        {
            // disconnect from the server
            CommandChannel.Instance.Disconnect();
        }

        private string ToXml(string data)
        {
            // Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            // validation data
            if (data == "")
            {
                return data;
            }
            // get the values
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            float throttle = getData(data, 2);
            float rudder = getData(data, 3);

            // save the values in xml
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
    }
}