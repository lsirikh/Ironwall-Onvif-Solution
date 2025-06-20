using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.PTZPresets;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 7:55:07 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*============================================================
|  ONVIF  →  DTO  Mapping Helper (partial)                   |
============================================================*/
public static partial class OnvifMappingHelper
{
    /*---------------------------  PTZ-Preset  ---------------------------*/
    public static PTZPresetDto ToDto(this PTZPreset src)
    {
        if (src == null) throw new ArgumentNullException(nameof(src));

        return new PTZPresetDto
        {
            Name = src.Name,
            Token = src.token,
            Position = src.PTZPosition?.ToDto()
        };
    }

    /*---------------------------  PTZ-Vector  ---------------------------*/
    private static PtzVectorDto ToDto(this PTZVector vec)
        => vec == null ? null! : new PtzVectorDto
        {
            PanTilt = vec.PanTilt?.ToDto(),
            Zoom = vec.Zoom?.ToDto()
        };
}