using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CameraConfigTool;
using System.IO;
using System.Reflection;
using System.Windows;
using CameraFTPAutoConfigureTool.Control;
using System.Collections;

namespace CameraFTPAutoConfigureTool
{
    public class CameraConfigureWork
    {
        private Hashtable _ipCameraList;
        private CameraFtpUser _cameraFtpUser;
        private CameraConfigureWorkThread[] _workThreads;
        public bool IsDone = false;

        public CameraConfigureWork(Hashtable ipCameraList,CameraFtpUser cameraFtpUser)
        {
            if (ipCameraList == null)
                throw new Exception("Argument error: ipCameraList is null");
            if (cameraFtpUser == null)
                throw new Exception("Argument error: cameraFtpuser is null");
            if (ipCameraList.Count == 0)
                throw new Exception("Argument error: ipCameraList count is zero");
            _cameraFtpUser = cameraFtpUser;
            _workThreads = new CameraConfigureWorkThread[ipCameraList.Count];
            _ipCameraList = ipCameraList;
        }

        public void Work(WorkListener workListener)
        {
            int index = 0;
            foreach (DictionaryEntry de in _ipCameraList)
            {
                (de.Value as IPCamera).Percent = "40%";
                _workThreads[index] = new CameraConfigureWorkThread(this, (de.Value as IPCamera), _cameraFtpUser);
                _workThreads[index].Key = (int)de.Key;
                _workThreads[index].ThreadRun();
                index += 1;
            }

            bool notFinish = true;
            int[] isShow = new int[_workThreads.Length];
            for (int i = 0; i < _workThreads.Length; i++)
            {
                isShow[i] = 0;
            }
            while (notFinish)
            {
                notFinish = false;
                for (int i = 0; i < _workThreads.Length; i++)
                {
                    if ((_workThreads[i].IsFinish == false) && (_workThreads[i].IsSuccess = true))
                    {
                        if (workListener != null)
                        {
                            //workListener.Listen(_ipCameraList[i].ModelName + " : " + _ipCameraList[i].Percent + DateTime.Now.ToString("ss.fff") + " : ");
                        }
                        notFinish = true;
                    }
                    else
                    {
                        if (workListener != null)
                        {
                            if (isShow[i] == 0)
                            {
                                isShow[i] = 1;
                                workListener.Listen(_workThreads[i].Key.ToString(), _workThreads[i].IsSuccess);
                            }
                        }
                    }
                }
                Thread.Sleep(50);
                IsDone = true;
            }
        }
    }
}
