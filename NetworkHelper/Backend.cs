using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public static class Extensions
    {
        public static T Modify<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
    public class Response
    {
        public Response(string message, StatusCode statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
        public StatusCode StatusCode { get; }
        public string Message { get; }
        public List<T> ToList<T>()
        {
            if (StatusCode != StatusCode.OK) return new List<T>(); // hiba esetén üres listával tér vissza
            else return JsonConvert.DeserializeObject<List<T>>(Message);
        }
    }

    public class RequestBuilder
    {
        WebRequest request;
        public RequestBuilder(string method, string from)
        {
            var request = WebRequest.Create(from);
            request.Method = method;
            this.request = request;
        }

        public RequestBuilder Body<T>(T body)
        {
            request.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(body));
            }
            return this;
        }

        public Response Send()
        {
            var response = request.GetResponse();
            if (response != null && ((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string json = streamReader.ReadToEnd();
                    response.Close();
                    return new Response(message: json, statusCode: (StatusCode)((HttpWebResponse)response).StatusCode);
                }
            }
            else
            {
                return new Response(message: ((HttpWebResponse)response).StatusCode.ToString(), statusCode: (StatusCode)((HttpWebResponse)response).StatusCode);
            }
        }
    }

    public static class Backend
    {
        public static RequestBuilder GET(string from) => new RequestBuilder(MethodBase.GetCurrentMethod().Name, from);

        public static RequestBuilder POST(string from) => new RequestBuilder(MethodBase.GetCurrentMethod().Name, from);

        public static RequestBuilder PUT(string from) => new RequestBuilder(MethodBase.GetCurrentMethod().Name, from);

        public static RequestBuilder DELETE(string from) => new RequestBuilder(MethodBase.GetCurrentMethod().Name, from);
    }
}


