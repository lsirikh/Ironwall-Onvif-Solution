using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Models;
public interface ICameraOnvifModel : IOnvifClientModel, ICameraModel
{
    bool IsDevicePossible { get; set; }
    bool IsEventPossible { get; set; }
    bool IsImagingPossible { get; set; }
    bool IsMediaPossible { get; set; }
    bool IsPtzPossible { get; set; }
    DeviceClient? DeviceClient { get; set; }
    ImagingPortClient? ImagingClient { get; set; }
    MediaClient? MediaClient { get; set; }
    List<Profile>? Profiles { get; set; }
    PTZClient? PtzClient { get; set; }
}