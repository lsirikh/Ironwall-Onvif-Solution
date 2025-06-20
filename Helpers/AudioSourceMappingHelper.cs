using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.AudioSourceConfigs.AudioSource;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 1:57:03 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*--------------------------- Mapping Helper ---------------------------*/
public static partial class OnvifMappingHelper
{
    /*--------------------------- Audio-Source → DTO ---------------------------*/
    public static AudioSourceDto? ToDto(this AudioSource src)
    {
        if (src is null) return null;

        return new AudioSourceDto
        {
            Token = src.token,                 /* DeviceEntity.token (base) */
            Channels = src.Channels,
            Any = src.Any?.Select(e => e.OuterXml).ToList()
        };
    }
}