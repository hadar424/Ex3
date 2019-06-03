using System;
using System.Diagnostics;
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

        /*
       * Name: Default
       * Input: -
       * Output: ActionResult view
       * Operation: stage 0 - dafault, no parameters
       */
        public ActionResult Default()
        {
            return View();

        }

        /*
         * Name: Save
         * Input: string ip, int port, int time, int saveTime, string file
         * Output: ActionResult view
         * Operation: stage 3 - animation and save the data in txt file
         */
        public ActionResult Save(string ip, int port, int time, int saveTime, string file)
        {
            // send the parameters to the model
            Instance.FileName = file;
            CommandChannel.Instance.ServerIP = ip;
            CommandChannel.Instance.CommandPort = port;

            CommandChannel.Instance.Disconnect();
            CommandChannel.Instance.Start();
            // read from file
            Session["time"] = time;
            Session["saveTime"] = saveTime;

            return View();
        }

        /*
      * Name: SaveToFile
      * Input: -
      * Output: string
      * Operation: save the data in txt file and goto xml function - return the values to the view
      */
        public string SaveToFile()
        {
            // get the data from the server
            string data = CommandChannel.Instance.GetInfo();
            float lon = getData(data, 0);
            float lat = getData(data, 1);
            float throttle = getData(data, 2);
            float rudder = getData(data, 3);

            // save the data in the file
            string fName = Instance.FileName;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fName + ".txt";
            using (StreamWriter streamWriter = System.IO.File.AppendText(filePath))
            {
                streamWriter.WriteLine(lon.ToString() + ' ' + lat.ToString() + ' ' + throttle.ToString() +
                    ' ' + rudder.ToString());
            }

            return ToXml(data);
        }

            /*
        * Name: Load
        * Input: string file, int time
        * Output: ActionResult view
        * Operation: stage 4 - load the data from txt file and do the animation according to that
        */
        public ActionResult Load(string file, int time)
        {
            // save the parameters for loading
            Instance.FileName = file;
            Instance.ArrayIndex = 0;
            Session["time"] = time;

            return View("Load");
        }

            /*
        * Name: Index
        * Input: string ip, int port
        * Output: ActionResult view
        * Operation: stage 1 - connect to server and draw the start point
        */
        public ActionResult Index(string ip, int port)
        {
            // check if the string is IP or file name (if file name - goto Load function)
            System.Net.IPAddress IP = null;
            // check which kind of argument we received 
            if (!IPAddress.TryParse(ip, out IP))
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


            /*
        * Name: getData
        * Input: string line, int index
        * Output: float
        * Operation: parse the data - get the value according to the index
        */
        public float getData(string line, int index)
        {
            // get the data according to the index - split by ' '
            string parseString = "";
            string[] values = line.Split(' ');
            parseString = values[index];
            float value;
            // convert from string to float
            if (!float.TryParse(parseString, out value))
            {
                return 200;
            }
            return value;
        }

        /*
      * Name: Display
      * Input: string ip, int port, int time
      * Output: ActionResult view
      * Operation: stage 2 - play the animation according to the simulator
      */
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

            /*
      * Name: GetLonLat
      * Input: -
      * Output: string
      * Operation: get the info from the simulator
      */
        public string GetLonLat()
        {
            // send the parameters to the xml and send to the view
            string data = CommandChannel.Instance.GetInfo();
            return ToXml(data);
        }

        /*
       * Name: GetLonLatFile
       * Input: -
       * Output: string
       * Operation: get the info from the file
       */
        public string GetLonLatFile()
        {
            // get the lon and lat values from the file
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + Instance.FileName + ".txt";
            string data = "";
            // check if file exists
            if (!System.IO.File.Exists(filePath)) { 
                throw new FileNotFoundException();
             }
            // read all the lines from the file and split them
            string[] lines = System.IO.File.ReadAllLines(filePath);
            if (Instance.ArrayIndex < lines.Length)
            {
                // get the data from index row on the file
                data = lines[FirstController.Instance.ArrayIndex];
                Instance.ArrayIndex += 1;
            }
            return ToXml(data);
        }

        /*
      * Name: CloseServer
      * Input: -
      * Output: -
      * Operation: disconnect from the server
      */
        public void CloseServer()
        {
            // disconnect from the server
            CommandChannel.Instance.Disconnect();
        }


        /*
      * Name: ToXml
      * Input: string data
      * Output: string
      * Operation: save the data in the xml (for using in the view)
      */
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
            Random rnd = new Random();
            float lon = getData(data, 0) + rnd.Next(50);
            float lat = getData(data, 1) + rnd.Next(50);
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