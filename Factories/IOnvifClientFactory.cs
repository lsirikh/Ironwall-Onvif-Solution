using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Factories;
/// <summary>
/// ONVIF WCF 클라이언트 생성 책임 추상화
/// </summary>
public interface IOnvifClientFactory
{
    Task<DeviceClient> CreateDeviceAsync(IOnvifConnectionModel c);
    Task<MediaClient> CreateMediaAsync(IOnvifConnectionModel c);
    Task<PTZClient> CreatePtzAsync(IOnvifConnectionModel c);
    Task<ImagingPortClient> CreateImagingAsync(IOnvifConnectionModel c);
}
