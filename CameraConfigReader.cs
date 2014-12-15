using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CameraConfigTool
{
    public enum AuthenticationType
    {
        Basic,
        Url,
        error
    }

    public class CameraConfigReader
    {
        enum ValueType
        {
            URL,
            AUTHENTICATIONTYPE,
            DATA,
            RESULT,
            USERNAME,
            PASSWORD
        };

        private XmlDocument _configXml;
        private XmlNamespaceManager _nsmgr;

        public CameraConfigReader()
        {
            _configXml = new XmlDocument();
        }

        /// <summary>
        /// Open camera configuration xml file
        /// </summary>
        /// <param name="path">File path</param>
        public void OpenConfiguration(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Argument error: Path is null or empty");
            try
            {
                _configXml.Load(path);
                _nsmgr = new XmlNamespaceManager(_configXml.NameTable);
            }
            catch (Exception ex)
            {
                throw ex; 
            }
        }

        /// <summary>
        /// Get request url 
        /// </summary>
        /// <param name="deviceType">Camera model</param>
        /// <returns>Suffix of request url to configure camera</returns>
        public string GetUrl(string deviceType)
        {
            return GetMethodStepValue(deviceType, ValueType.URL);
        }

        /// <summary>
        /// Get authentication type
        /// </summary>
        /// <param name="deviceType">Camera model</param>
        /// <returns>Authentication type when login camera web interface</returns>
        public AuthenticationType GetAuthenticationType(string deviceType)
        {
            string authenticationType = GetMethodStepValue(deviceType, ValueType.AUTHENTICATIONTYPE);
            if (authenticationType == "Basic")
                return AuthenticationType.Basic;
            if (authenticationType == "Url")
                return AuthenticationType.Url;
            return AuthenticationType.error;
        }

        /// <summary>
        /// Get data to post
        /// </summary>
        /// <param name="deviceType">Camera model</param>
        /// <returns>Data to post when configure camera settings </returns>
        public string GetData(string deviceType)
        {
            return GetMethodStepValue(deviceType, ValueType.DATA);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public string GetResult(string deviceType)
        {
            return GetMethodStepValue(deviceType, ValueType.RESULT);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public string GetUserName(string deviceType)
        {
            return GetMethodStepValue(deviceType, ValueType.USERNAME);
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public string GetPassword(string deviceType)
        {
            return GetMethodStepValue(deviceType, ValueType.PASSWORD);
        }

        private XmlNode GetDeviceNode(string deviceType)
        {
            if (string.IsNullOrEmpty(deviceType))
            {
                throw new Exception("Agument error: Device type is null or empty");
            }

            if (_configXml == null)
                throw new Exception("Abused error:The configuration xml have not loaded successfully, please use  void OpenConfiguration(string path)");
            if (_nsmgr == null)
                throw new Exception("Abused error:The configuration xml have not loaded successfully, please use  void OpenConfiguration(string path)");

            _nsmgr.AddNamespace("tts", "camera-configuration-DHQ-1.0");
            XmlNodeList deviceNodeList = _configXml.SelectNodes("//tts:deviceList/tts:device", _nsmgr);
            if (deviceNodeList == null)
                throw new Exception("Configuration xml error: Can not find 'device' node in specified root node");
            foreach (var node in deviceNodeList)
            {
                if (deviceType.CompareTo((node as XmlNode).ChildNodes.Item(0).FirstChild.Value) == 0)
                {
                    XmlNode deviceNode = node as XmlNode;
                    if (deviceNode.ChildNodes.Count != 6)
                        throw new Exception("Code error: Get the wrong node, this is not 'device' node");
                    return deviceNode;
                }
            }
            return null;
        }

        private XmlNode GetMethodNode(string deviceType)
        {
            XmlNode node = GetDeviceNode(deviceType);
            XmlNodeList methodNodeList = _configXml.SelectNodes("//tts:methodList/tts:method", _nsmgr);
            if (methodNodeList == null)
                throw new Exception("Configuration xml error: Can not find 'method' node in specified root node");
            if (node != null)
            {
                int index = int.Parse(node.ChildNodes.Item(3).FirstChild.Value);
                XmlNode methodNode = methodNodeList.Item(index - 1);
                if (methodNode.ChildNodes.Count != 3)
                    throw new Exception("Code error: Get the wrong node, this is not 'method' node");
                if (methodNode.ChildNodes.Item(2).ChildNodes.Count < 1)
                    throw new Exception("Configuration xml error: 'steps' node is null");
                return methodNode;
            }
            else
                return null;
        }

        private string GetMethodStepValue(string deviceType, ValueType valueType)
        {
            //if (deviceType == string.Empty)
            //    throw new Exception("Argument error: The deviceType is empty string");
            int index = -1;
            if (valueType == ValueType.URL)
                index = 0;
            if (valueType == ValueType.AUTHENTICATIONTYPE)
                index = 1;
            if (valueType == ValueType.DATA)
                index = 2;
            if (valueType == ValueType.RESULT)
                index = 3;
            if (valueType == ValueType.USERNAME)
                index = -2;
            if (valueType == ValueType.PASSWORD)
                index = -3;
            if (index == -1)
                throw new Exception("Code error: The specified ValueType is invalid"); 

            if (index < 0)
            {
                XmlNode deviceNode = GetDeviceNode(deviceType);
                if (deviceNode != null)
                {
                    XmlNode valueNode = deviceNode.ChildNodes.Item(2 - index);
                    if (valueNode.HasChildNodes)
                    {
                        return valueNode.FirstChild.Value;
                    }
                    else
                        throw new Exception("Configuration xml error: The node value is null");
                }
                else
                    return string.Empty;
            }

            XmlNode methodNode = GetMethodNode(deviceType);
            if (methodNode != null)
            {
             
                if (methodNode.ChildNodes.Item(2).ChildNodes.Count > 1)
                {
                    ///
                    /// pending to accomplish
                    ///
                    return string.Empty;
                }
                else
                {
                    XmlNode valueNode = methodNode.ChildNodes.Item(2).ChildNodes.Item(0).ChildNodes.Item(index);
                    if (valueNode.HasChildNodes)
                    {
                        return valueNode.FirstChild.Value;
                    }
                    else
                    {
                        if (index != 2 && index != 3)
                            throw new Exception("Configuration xml error: 'step' child node value is null");
                        else
                            return string.Empty;
                    }
                }
            }
            else
                return string.Empty;
        }
    }
}
