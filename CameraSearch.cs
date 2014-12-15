using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraConfigTool
{
    public class CameraSearch
    {
        public CameraSearch()
        {
        }

        public List<IPCamera> GetAvailableCameraList()
        {
            List<IPCamera> serviceList = new List<IPCamera>();
            UdpCamSearch udpInstance = new UdpCamSearch();
            udpInstance.SendCamData();
            if (udpInstance.GetServiceList() != null)
                serviceList=udpInstance.GetServiceList();
            UpnpCamSearch upnpInstance = new UpnpCamSearch();
            upnpInstance.SendCamData();
            serviceList.AddRange(upnpInstance.GetServiceList());
            
            return serviceList;
        }
    }
}
