using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.AudioEncoderConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/16/2025 4:26:07 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public static partial class OnvifMappingHelper
{
    /*───────────────────────────────────────────────────────────────*/
    /* 1. Audio-Encoder                                             */
    /*───────────────────────────────────────────────────────────────*/
    public static AudioEncoderConfigDto? ToDto(this AudioEncoderConfiguration src)
    {
        if (src is null) return null;

        return new AudioEncoderConfigDto
        {
            /* ── ConfigEntityDto 필드 ───────────────────────────*/
            Name = src.Name,
            UseCount = src.UseCount,
            Token = src.token,

            /* ── Audio-Encoder 고유 필드 ────────────────────────*/
            Encoding = src.Encoding.ToDto(),
            Bitrate = src.Bitrate,
            SampleRate = src.SampleRate,
            SessionTimeout = src.SessionTimeout,
            Multicast = src.Multicast.ToDto()
        };
    }

    /*───────────────────────────────────────────────────────────────*/
    /* 2. Multicast                                                 */
    /*───────────────────────────────────────────────────────────────*/
    public static MulticastConfigDto? ToDto(this MulticastConfiguration src)
    {
        if (src is null) return null;

        return new MulticastConfigDto
        {
            Address = src.Address.ToDto(),
            Port = src.Port,
            TTL = src.TTL,
            AutoStart = src.AutoStart
        };
    }

    /*───────────────────────────────────────────────────────────────*/
    /* 3. IP-Address                                                */
    /*───────────────────────────────────────────────────────────────*/
    public static IpAddressDto? ToDto(this IPAddress src)
    {
        if (src is null) return null;

        return new IpAddressDto
        {
            Type = src.Type.ToDto(),
            IPv4Address = src.IPv4Address,
            IPv6Address = src.IPv6Address
        };
    }

    /*───────────────────────────────────────────────────────────────*/
    /* 4. Enum 변환                                                  */
    /*───────────────────────────────────────────────────────────────*/
    private static EnumAudioEncoding ToDto(this AudioEncoding enc) => enc switch
    {
        AudioEncoding.G711 => EnumAudioEncoding.G711,
        AudioEncoding.G726 => EnumAudioEncoding.G726,
        AudioEncoding.AAC => EnumAudioEncoding.AAC,
        _ => EnumAudioEncoding.Unknown
    };

    private static EnumIpType ToDto(this IPType t) => t switch
    {
        IPType.IPv6 => EnumIpType.IPv6,
        _ => EnumIpType.IPv4        // 기본값
    };
}