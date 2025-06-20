using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using System.Collections.Generic;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Models
{
    public interface IOnvifClientModel
    {
        /// <summary>
        /// Onvif Class deviceClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifDeviceIo.Device
        /// </summary>
        DeviceClient DeviceClient { get; }
        /// <summary>
        /// Onvif Class mediaClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifMedia.Media
        /// </summary>
        MediaClient MediaClient { get; }
        /// <summary>
        /// Onvif Class ptzClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifPtz.PTZ
        /// </summary>
        PTZClient PtzClient { get;  }
        /// <summary>
        /// Onvif Class imagingClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifImaging.ImagingPort
        /// </summary>
        ImagingPortClient ImagingClient { get;  }
        /// <summary>
        /// Onvif Class profiles
        /// From http://www.onvif.org/ver10/schema
        /// </summary>
        List<Profile> Profiles { get;}


        bool IsDevicePossible { get; set; }
        bool IsMediaPossible { get; set; }
        bool IsEventPossible { get; set; }
        bool IsImagingPossible { get; set; }
        bool IsPtzPossible { get; set; }
    }
}
