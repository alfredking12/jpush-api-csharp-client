#if COREFX
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace cn.jpush.api
{
    static public class CapabilityExtension
    {
        class HttpAsyncState
        {
            public HttpWebRequest Request { get; set; }

            public Stream Stream { get; set; }

            public HttpWebResponse Response { get; set; }

            public ManualResetEvent Event = new ManualResetEvent(false);
        }

        #region Http

        public static void Add(this WebHeaderCollection collection, NameValueCollection headers)
        {
            foreach (var key in headers.AllKeys)
            {
                collection.Add(key, headers[key]);
            }
        }

        public static void Add(this WebHeaderCollection collection, string key, string value)
        {
            collection[key] = value;
        }

        public static string CharacterSet(this HttpWebResponse response)
        {
            var contentType = response.Headers["Content-Type"];

            string charset = "";

            do
            {
                if (string.IsNullOrEmpty(contentType))
                    break;

                contentType = contentType.Replace(" ", "");

                Regex reg = new Regex(@"charset=(.+)[;]", RegexOptions.IgnoreCase);
                var result = reg.Match(contentType).Groups;
                if (result == null || result.Count != 2)
                    break;

                charset = result[1].Value.Trim().ToLower();

            } while (false);

            return charset;
        }

        public static Stream GetRequestStream(this HttpWebRequest request)
        {
            HttpAsyncState state = new HttpAsyncState { Request = request };
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), state);
            state.Event.WaitOne();
            return state.Stream;
        }

        public static HttpWebResponse GetResponse(this HttpWebRequest request)
        {
            HttpAsyncState state = new HttpAsyncState { Request = request };
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), state);
            state.Event.WaitOne();
            return state.Response;
        }

        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpAsyncState state = (HttpAsyncState)asynchronousResult.AsyncState;
            HttpWebRequest request = state.Request;
            state.Stream = request.EndGetRequestStream(asynchronousResult);
            state.Event.Set();
        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpAsyncState state = (HttpAsyncState)asynchronousResult.AsyncState;
            HttpWebRequest request = state.Request;
            state.Response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            state.Event.Set();
        }

        #endregion
    }
}
#endif