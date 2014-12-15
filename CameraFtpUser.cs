using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraConfigTool
{
	public class CameraFtpUser
	{
        private string userName;
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }

        private string ftpHost;
        public string FtpHost
        {
            get
            {
                return ftpHost;
            }
            set
            {
                ftpHost = value;
            }
        }

        private string portNum;
        public string PortNum
        {
            get
            {
                return portNum;
            }
            set
            {
                portNum = value;
            }
        }

        private string filePath;
        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }
	}
}
