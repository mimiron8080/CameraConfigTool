using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace CameraConfigTool
{
    public class CameraSettings
    {
        private CameraFtpUser _cameraFtpUser;
        private CameraConfigReader cameraConfigReader = new CameraConfigReader();
        private ConfigMethod configMethod;

        public CameraSettings(string path ,CameraFtpUser cameraFtpUser)
        {
            if (cameraFtpUser == null)
                throw new Exception("Argument error: cameraFtpUser is null");
            if (string.IsNullOrEmpty(cameraFtpUser.FtpHost) || string.IsNullOrEmpty(cameraFtpUser.PortNum))
                throw new Exception("Argument error: cameraFtpUser is invalid");
            _cameraFtpUser = cameraFtpUser;
            cameraConfigReader.OpenConfiguration(path);
        }

        public void CameraConfig(IPCamera ipCamera)
        {
            if (GetMethod(ipCamera.ModelName) == true)
            {
                PostData(ipCamera);
            }
        }

        private void PostData(IPCamera ipCamera)
        {
            
            if (string.IsNullOrEmpty(_cameraFtpUser.FilePath))
                _cameraFtpUser.FilePath = ipCamera.ModelName + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
            if (configMethod.AuthenticationType == AuthenticationType.Url)
            {
                try
                {
                    ipCamera.Percent = "70%";
                    configMethod.Url = string.Format(configMethod.Url, _cameraFtpUser.FtpHost +"\\"+ _cameraFtpUser.FilePath, _cameraFtpUser.PortNum, _cameraFtpUser.UserName, _cameraFtpUser.Password, configMethod.AdminUserName, configMethod.AdminPassword);
                    HttpWebResponse webResponse = (HttpWebResponse)WebRequest.Create("http://" + ipCamera.PresentantionUrl + configMethod.Url).GetResponse();
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    { 
                        ///statusCode maybe right ,according to response result
                        ipCamera.IsConfigureWorkSuccess = true;
                    }
                    ipCamera.IsConfigureWorkDone = true; 
                }
                catch (Exception ex)
                {
                    ipCamera.IsConfigureWorkSuccess = false;
                    throw ex;
                }
                ipCamera.Percent = "100%";
            }
            if (configMethod.AuthenticationType == AuthenticationType.Basic)
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(ipCamera.PresentantionUrl + configMethod.Url);
                    webRequest.ServicePoint.Expect100Continue = false;
                    webRequest.Method = "POST";
                    webRequest.KeepAlive = true;
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    webRequest.Accept = "text/html, application/xhtml+xml, */*";
                    webRequest.Referer = ipCamera.PresentantionUrl + configMethod.Url;

                    webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(configMethod.AdminUserName + ":" + configMethod.AdminPassword));
                    //if (ipCamera.ModelName == "DCS-942L")
                    //    _cameraFtpUser.Password = "*a%21_-%2F6%5EP%24";
                    configMethod.Data = string.Format(configMethod.Data, _cameraFtpUser.FtpHost, _cameraFtpUser.PortNum, _cameraFtpUser.UserName, _cameraFtpUser.Password, _cameraFtpUser.FilePath);
                    Stream requestStream = webRequest.GetRequestStream();
                    requestStream.Write(Encoding.UTF8.GetBytes(configMethod.Data), 0, Encoding.UTF8.GetBytes(configMethod.Data).Length);
                    requestStream.Close();
                    ipCamera.Percent = "75%";
                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        ///ok
                        ipCamera.IsConfigureWorkSuccess = true;
                    }
                    ipCamera.IsConfigureWorkDone = true; 
                }
                catch (Exception ex)
                {
                    ipCamera.IsConfigureWorkSuccess = false;
                    throw ex;
                }
                ipCamera.Percent = "100%";
            }
        }

        private bool GetMethod(string deviceType)
        {
            try
            {
                configMethod.Url = cameraConfigReader.GetUrl(deviceType);
                configMethod.AuthenticationType = cameraConfigReader.GetAuthenticationType(deviceType);
                configMethod.Data = cameraConfigReader.GetData(deviceType);
                configMethod.AdminUserName = cameraConfigReader.GetUserName(deviceType);
                configMethod.AdminPassword = cameraConfigReader.GetPassword(deviceType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (configMethod.Url == string.Empty)
                return false;
            if (configMethod.AuthenticationType == AuthenticationType.error)
                return false;
            if (configMethod.AuthenticationType == AuthenticationType.Basic && string.IsNullOrEmpty(configMethod.Data))
                return false;

            return true;
        }
    }



    public struct ConfigMethod
    {
        public string Url
        {
            get;
            set;
        }

        public AuthenticationType AuthenticationType
        {
            get;
            set;
        }

        public string Data
        {
            get;
            set;
        }

        public string AdminUserName
        {
            get;
            set;
        }

        public string AdminPassword
        {
            get;
            set;
        }
    }
}
