using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Xml;

namespace CameraConfigTool
{
    public class UpnpCamSearch
    {
        private List<string> CameraDescriptionUrlList = new List<string>();
        private const string MULTCASTADDRESS = "255.255.255.255";
        private const int PORTNUM = 1900;
        private const string MESSFORCAM = "M-SEARCH * HTTP/1.1\r\n" +
                              "HOST: 239.255.255.250:1900\r\n" +
                              "ST: urn:schemas-upnp-org:device:Basic:1.0\r\n" +
                              "MAN: \"ssdp:discover\"\r\n" +
                              "MX: 3\r\n\r\n";
        private UdpClient udpC;
        private Thread listener;
        private Byte[] receiveBytes;

        /// <summary>
        /// Send camera research packet and receive camera respones packet
        /// </summary>
        public void SendCamData()
        {
            SendCamSearch();
            ReceiveCam();
        }

        /// <summary>
        /// Get camera web interface address
        /// </summary>
        /// <returns>Camera web interface address list</returns>
        public List<IPCamera> GetServiceList()
        {
            if (CameraDescriptionUrlList != null)
            {
                List<IPCamera> cameraList = new List<IPCamera>();
                foreach (var serviceUrl in CameraDescriptionUrlList)
                {
                    IPCamera ipCamera=new IPCamera();
                    XmlDocument responesXml = new XmlDocument();
                    responesXml.Load(WebRequest.Create(serviceUrl).GetResponse().GetResponseStream());
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(responesXml.NameTable);
                    nsmgr.AddNamespace("tts", "urn:schemas-upnp-org:device-1-0");
                    XmlNode modelNode = responesXml.SelectSingleNode("//tts:device/tts:modelName/text()", nsmgr);
                    if (String.IsNullOrEmpty(modelNode.Value))
                        return null;
                    ipCamera.ModelName = modelNode.Value;
                    XmlNode urlNode = responesXml.SelectSingleNode("//tts:device/tts:presentationURL/text()", nsmgr);
                    if (String.IsNullOrEmpty(urlNode.Value))
                        return null;
                    ipCamera.PresentantionUrl = urlNode.Value;
                    XmlNode uuidNode = responesXml.SelectSingleNode("//tts:device/tts:UDN/text()", nsmgr);
                    if (string.IsNullOrEmpty(uuidNode.Value))
                        return null;
                    ipCamera.UUID = uuidNode.Value;
                    ipCamera.IsConfigureWorkDone = false;
                    ipCamera.IsConfigureWorkSuccess = true;
                    cameraList.Add(ipCamera);
                }
                return cameraList;
            }
            else
                return null;
        }

        /// <summary>
        /// Send search packet
        /// </summary>
        private void SendCamSearch()
        {
            udpC = new UdpClient(1200);
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(MULTCASTADDRESS), PORTNUM);
            try
            {
                udpC.Send(Encoding.ASCII.GetBytes(MESSFORCAM), Encoding.ASCII.GetBytes(MESSFORCAM).Length, iep);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Receive respones packet
        /// </summary>
        private void ReceiveCam()
        {
            if (udpC != null)
            {
                listener = new Thread(UpnpReceiveThread);
                listener.IsBackground = true;
                listener.Start();
                listener.Join(2000);
                udpC.Close();
            }
        }

        /// <summary>
        /// Receive thread
        /// </summary>
        private void UpnpReceiveThread()
        {
            string result;
            const string splitStr = "location:";
            while (true)
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(MULTCASTADDRESS), PORTNUM);
                iep = null;
                try
                {
                    receiveBytes = udpC.Receive(ref iep);
                    result = Encoding.ASCII.GetString(receiveBytes);
                    result = result.Substring(result.ToLower().IndexOf(splitStr)+splitStr.Length);
                    result = result.Substring(0,result.IndexOf("\r")).Trim();
                    CameraDescriptionUrlList.Add(result);
                }
                catch
                {
                    break;
                }
            }
        }
    }
}
