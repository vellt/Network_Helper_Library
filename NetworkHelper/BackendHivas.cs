using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetworkHelper
{
    public enum Methods
    {
        GET, POST, PUT, DELETE
    }
    public class BackendValasz
    {
        /// <summary>
        /// Hiba képződik:
        /// - Ha nem létezik a backend útvonal
        /// - Ha egy http prtotkolt helyett https protokollal van megadva az útvonal
        /// - Ha a backend útvonal létezik, de az nem támogatja a példányosítás sorám megadott (GET, PUT, POST, DELETE) protokolt
        /// - Ha a GET hivásba body tartalom van helyezve
        /// - Ha a body.count az 0-val egyenlő
        /// </summary>
        public bool Error { get; set; } = false;
        private string Json = "";

        public BackendValasz(string json, bool error)
        {
            Error = error;
            Json = json;
        }

        public string ResponseAsJson()
        {
            return Json;
        }

        public List<T> ResponseAsObject<T>()
        {
            return JsonConvert.DeserializeObject<List<T>>(Json);
        }

    }
    public static class BackendHivas
    {
        public static BackendValasz Kuldese(string url, Methods method)
        {
            var request = WebRequest.Create(url);
            request.Method = method.ToString();

            // kérés elküldése és fogadása
            try
            {
                var response = request.GetResponse();
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                bool error = false;
                return new BackendValasz(json, error);
            }
            catch (WebException)
            {
                //string code= new string(e.Message.Where(x => ((int)x) >= 48 && ((int)x) <= 57).ToArray());
                string json = "";
                bool error = true;
                return new BackendValasz(json, error);
                //Json = code;
            }
        }

        public static BackendValasz Kuldese(string url, Methods method, List<string> body)
        {
            // kérés elkészítése
            var request = WebRequest.Create(url);
            request.Method = method.ToString();
            request.ContentType = "application/json";
            bool error = true;

            // a kérés body-jába adatot helyezünk
            if (method != Methods.GET && body.Count != 0)
            {
                var streamWriter = new StreamWriter(request.GetRequestStream());
                var kulcsErtekParok = new Dictionary<string, string>();
                body.ForEach(x => kulcsErtekParok.Add($"bevitel{kulcsErtekParok.Count() + 1}", x));
                streamWriter.Write(JsonConvert.SerializeObject(kulcsErtekParok));
                streamWriter.Close();
            }

            // kérés elküldése és fogadása
            try
            {
                var response = request.GetResponse();
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                error = false;
                return new BackendValasz(json, error);
            }
            catch (WebException)
            {
                //string code= new string(e.Message.Where(x => ((int)x) >= 48 && ((int)x) <= 57).ToArray());
                string json = "";
                return new BackendValasz(json, error);
                //Json = code;
            }
        }

        public static BackendValasz Kuldese(string url, Methods method, Dictionary<string, string> body)
        {
            // kérés elkészítése
            var request = WebRequest.Create(url);
            request.Method = method.ToString();
            request.ContentType = "application/json";
            bool error = true;

            // a kérés body-jába adatot helyezünk
            if (method != Methods.GET && body.Count != 0)
            {
                var streamWriter = new StreamWriter(request.GetRequestStream());
                streamWriter.Write(JsonConvert.SerializeObject(body));
                streamWriter.Close();
            }

            // kérés elküldése és fogadása
            try
            {
                var response = request.GetResponse();
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                error = false;
                return new BackendValasz(json, error);
            }
            catch (WebException)
            {
                //string code= new string(e.Message.Where(x => ((int)x) >= 48 && ((int)x) <= 57).ToArray());
                string json = "";
                return new BackendValasz(json, error);
                //Json = code;
            }
        }


        public static BackendValasz Kuldese<T>(string url, Methods method, T body)
        {
            // kérés elkészítése
            var request = WebRequest.Create(url);
            request.Method = method.ToString();
            request.ContentType = "application/json";
            bool error = true;

            // a kérés body-jába adatot helyezünk
            if (method != Methods.GET)
            {
                var streamWriter = new StreamWriter(request.GetRequestStream());
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                PropertyInfo[] properties = body.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    string propertyName = property.Name;
                    object propertyValue = property.GetValue(body);

                    if (propertyValue != null)
                    {
                        dictionary.Add(propertyName, propertyValue.ToString());
                    }
                    else
                    {
                        dictionary.Add(propertyName, "null");
                    }
                }
                streamWriter.Write(JsonConvert.SerializeObject(body));
                streamWriter.Close();
            }

            // kérés elküldése és fogadása
            try
            {
                var response = request.GetResponse();
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                error = false;
                return new BackendValasz(json, error);
            }
            catch (WebException)
            {
                //string code= new string(e.Message.Where(x => ((int)x) >= 48 && ((int)x) <= 57).ToArray());
                string json = "";
                return new BackendValasz(json, error);
                //Json = code;
            }
        }
    }
}


