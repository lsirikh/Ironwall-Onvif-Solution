using Autofac.Features.Metadata;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.PTZPresets;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceMgmt;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using DeviceClient = Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo.DeviceClient;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 12/15/2023 5:31:34 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraOnvifModel : CameraModel, ICameraOnvifModel
    {
        #region - Ctors -
        public CameraOnvifModel()
        {
        }

        public CameraOnvifModel(IConnectionModel model) : base(model)
        {
        }

        public CameraOnvifModel(ICameraModel model) : base(model)
        {
        }

        public CameraOnvifModel(ICameraModel model, DeviceClient device, MediaClient media, PTZClient ptz, ImagingPortClient imaging, List<Profile> profiles) : base(model)
        {
            DeviceClient = device;
            MediaClient = media;
            PtzClient = ptz;
            ImagingClient = imaging;
            Profiles = profiles;
        }

        public CameraOnvifModel(CameraOnvifModel model) : base(model)
        {
            IsDevicePossible = model.IsDevicePossible;
            IsMediaPossible = model.IsMediaPossible;
            IsEventPossible = model.IsEventPossible;
            ImagingClient = model.ImagingClient;
            IsPtzPossible = model.IsPtzPossible;

            DeviceClient = model.DeviceClient;
            MediaClient = model.MediaClient;
            PtzClient = model.PtzClient;
            ImagingClient = model.ImagingClient;
            Profiles = model.Profiles;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        [JsonIgnore]
        public bool IsDevicePossible { get; set; }
        [JsonIgnore]
        public bool IsMediaPossible { get; set; }
        [JsonIgnore]
        public bool IsEventPossible { get; set; }
        [JsonIgnore]
        public bool IsImagingPossible { get; set; }
        [JsonIgnore]
        public bool IsPtzPossible { get; set; }

        /// <summary>
        /// Onvif Class deviceClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifDeviceIo.Device
        /// </summary>
        [JsonIgnore]
        public DeviceClient? DeviceClient { get; set; }

        /// <summary>
        /// Onvif Class mediaClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifMedia.Media
        /// </summary>
        [JsonIgnore]
        public MediaClient? MediaClient { get; set; }

        /// <summary>
        /// Onvif Class ptzClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifPtz.PTZ
        /// </summary>
        [JsonIgnore]
        public PTZClient? PtzClient { get; set; }

        /// <summary>
        /// Onvif Class imagingClient
        /// From Ironwall.Dotnet.Libraries.OnvifSolution.OnvifImaging.ImagingPort
        /// </summary>
        [JsonIgnore]
        public ImagingPortClient? ImagingClient { get; set; }

        /// <summary>
        /// Onvif Class profiles
        /// From http://www.onvif.org/ver10/schema
        /// </summary>
        [JsonIgnore]
        public List<Profile>? Profiles { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
