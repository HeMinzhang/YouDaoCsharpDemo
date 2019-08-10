using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleYouDao
{
    //by - Shijiazhuang,HeMinzhang
    //github https://github.com/HeMinzhang
    class Program
    {
        public static void Main()
        {
            Dictionary<String, String> dic = new Dictionary<String, String>();
            string url = "https://openapi.youdao.com/api";
            string q = "test";
            string appKey = "";
            string appSecret = "";
            string salt = DateTime.Now.Millisecond.ToString();
            dic.Add("from", "EN");
            dic.Add("to", "zh-CHS");
            dic.Add("signType", "v3");

            var curtime = ((DateTime.UtcNow.Ticks - 621355968000000000) / 10000000).ToString();
            dic.Add("curtime", curtime);
            string signStr = appKey + Truncate(q) + salt + curtime + appSecret; ;

            byte[] bytes = Encoding.UTF8.GetBytes(signStr);
            SHA256Managed hashManaged = new SHA256Managed();
            byte[] hash = hashManaged.ComputeHash(bytes);
            string sign = string.Empty;
            foreach (byte x in hash)
            {
                sign += String.Format("{0:x2}", x);
            }

            dic.Add("q", System.Web.HttpUtility.UrlEncode(q));
            dic.Add("appKey", appKey);
            dic.Add("salt", salt);
            dic.Add("sign", sign);
            AsyncPost(url, dic).Wait();
        }
        protected static async Task AsyncPost(string url, Dictionary<String, String> dic)
        {
            HttpClient client = new HttpClient();
            var formContent = new FormUrlEncodedContent(dic);
            var ret = await client.PostAsync(url, formContent);
            var content = await ret.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
        protected static string Truncate(string q)
        {
            if (q == null)
            {
                return null;
            }
            int len = q.Length;
            return len <= 20 ? q : (q.Substring(0, 10) + len + q.Substring(len - 10, 10));
        }

        private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            string FilePath = FileName + DateTime.Now.Millisecond.ToString() + ".mp3";
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FilePath))
                    File.Delete(FilePath);
                Stream outStream = System.IO.File.Create(FilePath);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
    }
}
