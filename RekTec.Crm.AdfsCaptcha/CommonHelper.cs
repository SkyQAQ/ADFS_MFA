using System;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace RekTec.Crm.AdfsCaptcha
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public static class CommonHelper
    {
        /// <summary>
        /// 请求方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="requestString"></param>
        /// <param name="contentType"></param>
        /// <param name="encoding"></param>
        /// <param name="seconds"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string Request(string method, string url, string requestString = "", string contentType = "application/x-www-form-urlencoded", Encoding encoding = null, int seconds = 30, IDictionary<string, string> headers = null)
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(method))
                    throw new Exception("请求方式不能为空！");
                if (string.IsNullOrEmpty(url))
                    throw new Exception("请求地址不能为空！");
                if (url.ToLower().StartsWith("https"))
                {
                    ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                if (encoding == null)
                    encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ReadWriteTimeout = seconds * 1000;
                request.Timeout = seconds * 1000;
                request.UserAgent = "WUYAO";

                if (method.ToLower() == "post")
                    request.ServicePoint.Expect100Continue = false;
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        if (header.Key.Replace("-", "").ToLower() == "contenttype")
                            request.ContentType = header.Value;
                        else
                            request.Headers.Add(header.Key, header.Value);
                    }
                }
                if (!string.IsNullOrEmpty(requestString))
                {
                    byte[] data = encoding.GetBytes(requestString);
                    request.ContentLength = data.Length;
                    if (!string.IsNullOrWhiteSpace(contentType))
                        request.ContentType = contentType.Contains("charset=utf8") ? contentType : contentType + ";charset=utf8";
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Close();
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), encoding);
                    result = reader.ReadToEnd();
                }
                return result;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="info"></param>
        public static void WriteLog(string info, string type = "")
        {
            try
            {
                byte[] myByte = System.Text.Encoding.UTF8.GetBytes("记录时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "内容：" + info + "\r\n\r\n");
                string strPath = "C:\\CRMWeb\\Log\\";
                if (!System.IO.Directory.Exists(strPath))
                {
                    System.IO.Directory.CreateDirectory(strPath);
                }
                string strPathLog = strPath + "MFA_" + type + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt";
                using (System.IO.FileStream fsWrite = new System.IO.FileStream(strPathLog, System.IO.FileMode.Append))
                {
                    fsWrite.Write(myByte, 0, myByte.Length);
                };
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(obj.GetType());
            MemoryStream msObj = new MemoryStream();
            js.WriteObject(msObj, obj);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj, Encoding.UTF8);
            string json = sr.ReadToEnd();
            sr.Close();
            msObj.Close();
            return json;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deseralize<T>(string json)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(T));
                T model = (T)deseralizer.ReadObject(ms);
                return model;
            }
        }
    }

    /// <summary>
    /// 接口地址信息
    /// </summary>
    public class API_UrlInfo
    {
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        public string API_SendVerifyCode { get; set; }
        /// <summary>
        /// 验证手机验证码
        /// </summary>
        public string API_CheckVerifyCode { get; set; }
    }
}
