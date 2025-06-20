using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 11:06:01 AM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*---------------------------  Mapping Helper  ---------------------------*/
public static partial class OnvifMappingHelper
{
    /*--------------------------------------------------------------------
    MediaUri  ➜  MediaUriDto 변환
    --------------------------------------------------------------------*/
    public static MediaUriDto ToDto(this MediaUri src)
    {
        if (src is null) throw new ArgumentNullException(nameof(src));

        return new MediaUriDto
        {
            Uri = src.Uri,
            InvalidAfterConnect = src.InvalidAfterConnect,
            InvalidAfterReboot = src.InvalidAfterReboot,
            Timeout = src.Timeout
        };
    }
}