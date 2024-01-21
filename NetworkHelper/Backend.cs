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
            if (StatusCode != StatusCode.OK)
            {
                return new List<T>(); // hiba esetén üres listával tér vissza
            }
            else
            {
                return JsonConvert.DeserializeObject<List<T>>(Message);
            }
        }
    }

    public class RequestBuilder
    {
        WebRequest request;

        MultipartFormDataContent formData;
        HttpClient httpClient;
        FileStream fileStream;
        string url;
        string method;
        public RequestBuilder(string method, string from)
        {
            var request = WebRequest.Create(from);
            request.Method = method;
            this.request = request;
            this.url = from;
            this.method = method;
        }

        public RequestBuilder Body<T>(T body, Expression<Func<T, object>> fileProperty = null)
        {
            if (fileProperty == null)
            {
                request.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
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
                    }
                    streamWriter.Write(JsonConvert.SerializeObject(dictionary));
                }
            }
            else
            {
                var compiledFileProperty = fileProperty.Compile();
                MemberExpression memberExpression = (MemberExpression)fileProperty.Body;
                string filePath = compiledFileProperty(body).ToString();
                httpClient = new HttpClient();
                {
                    formData = new MultipartFormDataContent();
                    {
                        fileStream = File.OpenRead(filePath);
                        {
                            var streamContent = new StreamContent(fileStream);

                            formData.Add(streamContent, memberExpression.Member.Name, filePath.Split('\\')[filePath.Split('\\').Length - 1]);

                            PropertyInfo[] properties = body.GetType().GetProperties();
                            foreach (PropertyInfo property in properties)
                            {
                                string propertyName = property.Name;
                                object propertyValue = property.GetValue(body);
                                if (propertyValue != null && propertyName != memberExpression.Member.Name)
                                {
                                    formData.Add(new StringContent(propertyValue.ToString()), propertyName);
                                }
                            }
                        }

                    }
                }

            }
            return this;
        }

        public Response Send()
        {
            if (formData != null)
            {

                var response = (method == "POST") ? httpClient.PostAsync(url, formData).Result : (method == "PUT") ? httpClient.PutAsync(url, formData).Result : null;

                if (response != null && response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result;
                    return new Response(message: json, statusCode: (StatusCode)response.StatusCode);
                }
                else
                {
                    return new Response(message: ((StatusCode)response.StatusCode).ToString(), statusCode: (StatusCode)response.StatusCode);
                }
            }
            else
            {
                var response = request.GetResponse();
                if (response != null && ((HttpWebResponse)response).StatusCode == HttpStatusCode.OK)
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = streamReader.ReadToEnd();
                        return new Response(message: json, statusCode: (StatusCode)((HttpWebResponse)response).StatusCode);
                    }
                }
                else
                {
                    return new Response(message: ((HttpWebResponse)response).StatusCode.ToString(), statusCode: (StatusCode)((HttpWebResponse)response).StatusCode);
                }
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


