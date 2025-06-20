using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.AudioSourceConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 1:57:37 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*--------------------------- Mapping Helper ---------------------------*/
public static partial class OnvifMappingHelper
{
    /*--------------------------- Audio-Source → DTO ---------------------------*/
    public static AudioSourceConfigDto? ToDto(this AudioSourceConfiguration src)
    {
        if (src is null) return null;

        return new AudioSourceConfigDto
        {
            Name = src.Name,          /* base-class field (ConfigurationEntity)   */
            UseCount = src.UseCount,      /* base-class field                         */
            Token = src.token,         /* base-class attribute                     */
            SourceToken = src.SourceToken,
            Any = src.Any?.Select(x => x.OuterXml).ToList()
        };
    }
}