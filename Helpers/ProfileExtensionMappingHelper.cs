using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.AudioEncoderConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.ProfileExtensions.Audio;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.ProfileExtensions;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 4:28:20 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*--------------------------- MAPPING HELPERS -----------------------*/
public static partial class OnvifMappingHelper
{
    /*-------------------------------------------------------------*\
    |  ProfileExtension  →  ProfileExtensionDto                    |
    \*-------------------------------------------------------------*/
    public static ProfileExtensionDto ToDto(this ProfileExtension src)
        => new()
        {
            Any = src.Any?.Select(x => x.OuterXml).ToList(),
            AudioOutputConfiguration = src.AudioOutputConfiguration?.ToDto(),
            AudioDecoderConfiguration = src.AudioDecoderConfiguration?.ToDto(),
            Extension = src.Extension?.ToDto()
        };

    /*-------------------------------------------------------------*\
    |  AudioOutputConfiguration → AudioOutputConfigDto             |
    \*-------------------------------------------------------------*/
    public static AudioOutputConfigDto ToDto(this AudioOutputConfiguration src)
        => new()
        {
            /* ConfigEntityDto 공통 속성 */
            Name = src.Name,
            UseCount = src.UseCount,
            Token = src.token,

            /* 전용 필드 */
            OutputToken = src.OutputToken,
            SendPrimacy = src.SendPrimacy,
            OutputLevel = src.OutputLevel,
            Any = src.Any?.Select(e => e.OuterXml).ToList()
        };

    /*-------------------------------------------------------------*\
    |  ProfileExtension2 → ProfileExtension2Dto                    |
    \*-------------------------------------------------------------*/
    public static ProfileExtension2Dto ToDto(this ProfileExtension2 src)
    => new()
    {
            Any = src.Any?.Select(x => x.OuterXml).ToList()
        };

    /*-------------------------------------------------------------*\
    |  AudioDecoderConfiguration → AudioDecoderConfigDto                    |
    \*-------------------------------------------------------------*/
    public static AudioDecoderConfigDto ToDto(this AudioDecoderConfiguration src)
    => new()
    {
        Any = src.Any?.Select(x => x.OuterXml).ToList()
    };
}