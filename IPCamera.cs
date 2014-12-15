using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraConfigTool
{
    public class WorkMessage : EventArgs
    {
        public bool IsSuccess
        {
            get;
            set;
        }
    }
    public class IPCamera
    {
        public string UserSettingName
        {
            get;
            set;
        }
        public string Percent
        {
            get;
            set;
        }
        public string ModelName
        {
            get;
            set;
        }
        public string PresentantionUrl
        {
            get;
            set;
        }
        public string UUID
        {
            get;
            set;
        }
        public string Username
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
        public bool IsConfigureWorkDone
        {
            get;
            set;
        }
        public bool IsConfigureWorkSuccess
        {
            get;
            set;
        }
        public string ResultMessage
        {
            get;
            set;
        }
    }
}
