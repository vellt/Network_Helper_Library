using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetworkHelper
{
    public class Response
    {
        /// <summary>
        /// Inicializálja a <see cref="Response"/> osztály új példányát a megadott JSON adat alapján.
        /// </summary>
        /// <param name="jsonData">A válasz JSON formátumú adata.</param>
        private Response(string jsonData)
        {
            JsonData = jsonData;
            SelectedData = jsonData;
        }

        internal static Response Create(string jsonData)
        {
            return new Response(jsonData);
        }

        /// <summary>
        /// A válasz teljes JSON adata.
        /// </summary>
        private string JsonData { get; }
        // <summary>
        /// Az éppen kiválasztott JSON (részleges) adat, amely az aktuális feldolgozás eredménye.
        /// </summary>
        private string SelectedData { get; set; }

        /// <summary>
        /// Kiválasztja a JSON adatban található értéket az adott index alapján.
        /// </summary>
        /// <param name="index">Az index, amely alapján az értéket ki szeretnénk választani.</param>
        /// <returns>A <see cref="Response"/> példány, amely lehetővé teszi a további feldolgozást.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Ha az index érvénytelen.</exception>
        /// <exception cref="InvalidOperationException">Ha a JSON adat nem olvasható.</exception>
        public Response ValueAt(int index)
        {
            try
            {
                JObject response = JObject.Parse(SelectedData);
                var keys = response.Properties().Select(p => p.Name).ToList();

                if (index < 0 || index >= keys.Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

                string key = keys[index];
                SelectedData = response[key]?.ToString();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse JSON data.", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException("Invalid index provided.", ex);
            }

            return this;
        }

        /// <summary>
        /// Kiválasztja a JSON adatban található értéket a megadott név alapján.
        /// </summary>
        /// <param name="name">A név, amely alapján az értéket ki szeretnénk választani.</param>
        /// <returns>A <see cref="Response"/> példány, amely lehetővé teszi a további feldolgozást.</returns>
        /// <exception cref="InvalidOperationException">Ha a JSON adat nem olvasható.</exception>
        public Response ValueOf(string name)
        {
            try
            {
                JObject response = JObject.Parse(SelectedData);
                SelectedData = response[name]?.ToString();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse JSON data.", ex);
            }

            return this;
        }

        /// <summary>
        /// A kiválasztott JSON adatot deszerializálja a megadott típusra.
        /// </summary>
        /// <typeparam name="T">A típus, amire az adatot deszerializálni szeretnénk.</typeparam>
        /// <returns>A deszerializált objektum.</returns>
        /// <exception cref="InvalidOperationException">Ha a deszerializálás során hiba történik.</exception>
        public T As<T>()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SelectedData))
                {
                    throw new InvalidOperationException("SelectedData is null or empty.");
                }

                if (typeof(T) == typeof(string))
                {
                    var result = (T)(object)SelectedData;
                    SelectedData = JsonData;
                    return result;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    // Lehetséges dátumformátumok
                    var dateFormats = new[]
                    {
                        "yyyy. MM. dd. H:mm:ss",         // Pl. "2007. 05. 12. 0:00:00"
                        "yyyy-MM-dd",                    // Pl. "2007-05-12"
                        "yyyy-MM-ddTHH:mm:ssZ",          // Pl. "2007-05-12T00:00:00Z"
                        "yyyy-MM-dd HH:mm:ss",           // Pl. "2007-05-12 00:00:00"
                        "dd/MM/yyyy",                    // Pl. "12/05/2007"
                        "MM/dd/yyyy",                    // Pl. "05/12/2007"
                        "MM/dd/yyyy HH:mm:ss",           // Pl. "05/12/2007 00:00:00"
                        "dd-MM-yyyy",                    // Pl. "12-05-2007"
                        "yyyy/MM/dd",                    // Pl. "2007/05/12"
                        "yyyy.MM.dd",                    // Pl. "2007.05.12"
                        "yyyy-MM-ddTHH:mm:ss",           // Pl. "2007-05-12T00:00:00"
                        "yyyy-MM-ddTHH:mm:ss.fffZ",      // Pl. "2007-05-12T00:00:00.000Z"
                        "M/d/yyyy",                      // Pl. "5/12/2007"
                        "M/d/yyyy HH:mm:ss",             // Pl. "5/12/2007 00:00:00"
                        "d/M/yyyy",                      // Pl. "12/5/2007"
                        "d/M/yyyy HH:mm:ss",             // Pl. "12/5/2007 00:00:00"
                        "yyyyMMddTHHmmss",               // Pl. "20070512T000000"
                        "yyyyMMddTHHmmssZ",              // Pl. "20070512T000000Z"
                        "yyyyMMdd",                      // Pl. "20070512"
                        "MM/dd/yyyy hh:mm:ss tt",        // Pl. "05/12/2007 12:00:00 PM"
                        "dd/MM/yyyy hh:mm:ss tt",        // Pl. "12/05/2007 12:00:00 PM"
                        "yyyy-MM-ddTHH:mm:ss.fff"        // Pl. "2007-05-12T00:00:00.000"
                    };

                    if (DateTime.TryParseExact(SelectedData, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        SelectedData = JsonData;
                        return (T)(object)parsedDate;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to parse DateTime from the provided string.");
                    }
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<T>(SelectedData);
                    if (result == null)
                    {
                        throw new InvalidOperationException("Deserialization resulted in a null value.");
                    }
                    SelectedData = JsonData;
                    return result;
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to deserialize JSON data.", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during deserialization.", ex);
            }
        }
    }

    /// <summary>
    /// HTTP kérések építéséhez és küldéséhez használt osztály.
    /// </summary>
    public class RequestBuilder
    {
        private WebRequest request;

        // Privát konstruktor, hogy külső példányosítás ne legyen lehetséges
        private RequestBuilder(string method, string url)
        {
            request = WebRequest.Create(url);
            request.Method = method;
        }

        /// <summary>
        /// Létrehozza a <see cref="RequestBuilder"/> új példányát a megadott HTTP metódussal és URL-lel.
        /// </summary>
        /// <param name="method">Az HTTP metódus (pl. GET, POST).</param>
        /// <param name="url">A kérés URL-je.</param>
        /// <returns>A létrehozott <see cref="RequestBuilder"/> példány.</returns>
        internal static RequestBuilder Create(string method, string url)
        {
            return new RequestBuilder(method, url);
        }

        /// <summary>
        /// Beállítja a kérés body-ját JSON formátumra.
        /// </summary>
        /// <typeparam name="T">A body típusa, amely JSON formába lesz alakítva.</typeparam>
        /// <param name="body">A JSON-re alakítandó body.</param>
        /// <returns>A jelenlegi <see cref="RequestBuilder"/> példány, lehetővé téve a további láncolást.</returns>
        public RequestBuilder Body<T>(T body)
        {
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(body));
            }
            return this;
        }

        /// <summary>
        /// Szinkron módon elküldi a kérést és visszaadja a választ.
        /// </summary>
        /// <returns>A választ tartalmazó <see cref="Response"/> objektum.</returns>
        /// <exception cref="InvalidOperationException">Ha a válasz nem nyerhető ki.</exception>
        public Response Send()
        {
            try
            {
                var response = request.GetResponse();
                if (response != null)
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = streamReader.ReadToEnd();
                        response.Close();
                        return Response.Create(jsonData: json);
                    }
                }
                throw new Exception("Response is null.");
            }
            catch (WebException ex)
            {
                throw new InvalidOperationException("Failed to get response from server.", ex);
            }
        }

        /// <summary>
        /// Aszinkron módon elküldi a kérést és visszaadja a választ.
        /// </summary>
        /// <returns>A választ tartalmazó <see cref="Response"/> objektum.</returns>
        /// <exception cref="InvalidOperationException">Ha a válasz nem nyerhető ki.</exception>
        public async Task<Response> SendAsync()
        {
            try
            {
                // Átkonvertáljuk a WebRequest-t HttpWebRequest-re aszinkron műveletekhez
                var httpRequest = (HttpWebRequest)request;

                using (var response = await httpRequest.GetResponseAsync())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string json = await streamReader.ReadToEndAsync();
                    return Response.Create(jsonData: json);
                }
            }
            catch (WebException ex)
            {
                throw new InvalidOperationException("Failed to get response from server.", ex);
            }
        }
    }

    /// <summary>
    /// Statikus osztály, amely segít HTTP kérést készíteni különböző HTTP metódusokhoz.
    /// </summary>
    public static class Backend
    {
        /// <summary>
        /// Létrehozza a GET kéréshez használható <see cref="RequestBuilder"/> példányt.
        /// </summary>
        /// <param name="url">Az URL a kéréshez.</param>
        /// <returns>A GET kéréshez használható <see cref="RequestBuilder"/> példány.</returns>
        public static RequestBuilder GET(string from) => RequestBuilder.Create(MethodBase.GetCurrentMethod().Name, from);

        /// <summary>
        /// Létrehozza a POST kéréshez használható <see cref="RequestBuilder"/> példányt.
        /// </summary>
        /// <param name="url">Az URL a kéréshez.</param>
        /// <returns>A POST kéréshez használható <see cref="RequestBuilder"/> példány.</returns>
        public static RequestBuilder POST(string from) => RequestBuilder.Create(MethodBase.GetCurrentMethod().Name, from);

        /// <summary>
        /// Létrehozza a PUT kéréshez használható <see cref="RequestBuilder"/> példányt.
        /// </summary>
        /// <param name="url">Az URL a kéréshez.</param>
        /// <returns>A PUT kéréshez használható <see cref="RequestBuilder"/> példány.</returns>
        public static RequestBuilder PUT(string from) => RequestBuilder.Create(MethodBase.GetCurrentMethod().Name, from);

        /// <summary>
        /// Létrehozza a DELETE kéréshez használható <see cref="RequestBuilder"/> példányt.
        /// </summary>
        /// <param name="url">Az URL a kéréshez.</param>
        /// <returns>A DELETE kéréshez használható <see cref="RequestBuilder"/> példány.</returns>
        public static RequestBuilder DELETE(string from) => RequestBuilder.Create(MethodBase.GetCurrentMethod().Name, from);
    }
}


