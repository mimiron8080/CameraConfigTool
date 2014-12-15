using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraConfigTool;
using System.Threading;
using System.IO;
using System.Reflection;
using DHQ.Common.Util;

namespace CameraFTPAutoConfigureTool
{
    public class CameraConfigureWorkThread
    {
        private IPCamera _ipCamera;
        private CameraFtpUser _cameraFtpUser;
        private CameraConfigureWork _cameraConfigureWork;
        public int Key;

        public bool IsFinish = false;
        public bool IsSuccess = true;

        public CameraConfigureWorkThread(CameraConfigureWork cameraConfigureWork,IPCamera ipCamera, CameraFtpUser cameraFtpUser)
        {
            _cameraConfigureWork = cameraConfigureWork;
            _ipCamera = ipCamera;
            if (cameraFtpUser == null)
                throw new Exception("Argument error: cameraFtpuser is null");
            _cameraFtpUser = cameraFtpUser;
        }

        public void ThreadRun()
        {
            Thread t = new Thread(new ThreadStart(ThreadWork));
            t.Start();
        }

        private void ThreadWork()
        {
            try
            {
                ///
                Thread t = new Thread(new ThreadStart(ConfigureWork));
                t.Start();
                while (true)
                {
                    if (_ipCamera.IsConfigureWorkSuccess == false)
                        break;
                    if (_ipCamera.IsConfigureWorkDone == true)
                        break;
                }

                if (_ipCamera.IsConfigureWorkSuccess)
                    this.IsSuccess = true;
                else
                    this.IsSuccess = false;
                this.IsFinish = true;
            }
            catch (Exception ex)
            {
                this.IsSuccess = false;
                if (!string.IsNullOrEmpty(_ipCamera.ResultMessage))
                    _ipCamera.ResultMessage += "\n";
                _ipCamera.ResultMessage += ex.Message;
                LogUtil.LogError(ex);
            }
        }

        private void ConfigureWork()
        {
            try
            {
                string xmlPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\configuration.xml";
                CameraSettings cameraSettings = new CameraSettings(xmlPath, _cameraFtpUser);
                _ipCamera.Percent = "50%";
                cameraSettings.CameraConfig(_ipCamera);
            }
            catch (Exception ex)
            {
                this.IsSuccess = false;
                if (!string.IsNullOrEmpty(_ipCamera.ResultMessage))
                    _ipCamera.ResultMessage += "\n";
                _ipCamera.ResultMessage += ex.Message;
                LogUtil.LogError(ex);
            }
        }
    }
}
