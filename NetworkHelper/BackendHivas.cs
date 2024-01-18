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
    /// <summary>
    /// Hálózati hívás módjait definiáló felsorolás.
    /// </summary>
    public enum Methods
    {
        GET, POST, PUT, DELETE
    }

    /// <summary>
    /// A backend válaszához férünk hozzá általa
    /// </summary>
    public class BackendValasz
    {
        /// <summary>
        /// Hibát jelzi, ha a kérés során probléma merül fel.
        /// </summary>
        /// <remarks>
        /// Hiba képződhet, ha:
        /// <br/>- nem létezik a backend útvonal
        /// <br/>- egy HTTP protokolt helyett HTTPS protokollal van megadva az útvonal
        /// <br/>- a backend útvonal létezik, de az nem támogatja a példányosítás során megadott (GET, PUT, POST, DELETE) protokolt
        /// <br/>- a GET hívásba body tartalom van helyezve
        /// <br/>- a body.Count 0-val egyenlő
        /// </remarks>

        public bool Error { get; set; } = false;
        private string json = "";
        /// <summary>
        /// A backend válaszát JSON formában adja vissza.
        /// </summary>
        public string Json => json;

        public BackendValasz(string json, bool error)
        {
            Error = error;
            this.json = json;
        }


        /// <summary>
        /// JSON választ deszerializáló metódus. 
        /// Az átadott osztálynak megfelelő listával tér vissza.
        /// </summary>
        public List<T> List<T>()
        {
            return JsonConvert.DeserializeObject<List<T>>(Json);
        }

    }

    /// <summary>
    /// A háttérhívásokat kezelő osztály.
    /// </summary>
    public static class BackendHivas
    {
        /// <summary>
        /// Backend-re történő egyszerű kérést indító függvény.
        /// </summary>
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

        /// <summary>
        /// Backend-re adatokkal történő kérést indító függvény.
        /// </summary>
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

        /// <summary>
        /// Backend-re adatokkal történő kérést indító függvény.
        /// </summary>
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


        /// <summary>
        /// Backend-re adatokkal történő kérést indító függvény.
        /// </summary>
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


