using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.AudioEncoderConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.VideoEncoderConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/16/2025 5:00:17 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public static partial class OnvifMappingHelper
{
    /*---------------------------  VideoEncoder → DTO  ---------------------------*/
    public static VideoEncoderConfigDto ToDto(this VideoEncoderConfiguration src)
    {
        if (src == null) return null!;

        return new VideoEncoderConfigDto
        {
            /*---------------------------  공통(ConfigEntity)  ---------------------------*/
            Name = src.Name,
            UseCount = src.UseCount,
            Token = src.token,

            /*---------------------------  기본 파라미터  ---------------------------*/
            Encoding = (EnumVideoEncoding)src.Encoding,
            Resolution = src.Resolution.ToDto(),
            Quality = src.Quality,

            /*---------------------------  Rate-Control  ---------------------------*/
            RateControl = src.RateControl.ToDto(),

            /*---------------------------  Codec 별 설정  ---------------------------*/
            Mpeg4 = src.MPEG4?.ToDto(),
            H264 = src.H264?.ToDto(),

            /*---------------------------  네트워크 / 세션  ---------------------------*/
            Multicast = src.Multicast?.ToDto(),
            SessionTimeout = src.SessionTimeout,
            GuaranteedFrameRate = src.GuaranteedFrameRate
        };
    }

    /*---------------------------  Resolution  ---------------------------*/
    public static VideoResolutionDto ToDto(this VideoResolution src) => src == null
        ? null!
        : new VideoResolutionDto { Width = src.Width, Height = src.Height };

    /*---------------------------  RateControl  ---------------------------*/
    public static VideoRateControlDto ToDto(this VideoRateControl src) => src == null
        ? null!
        : new VideoRateControlDto
        {
            FrameRateLimit = src.FrameRateLimit,
            EncodingInterval = src.EncodingInterval,
            BitrateLimit = src.BitrateLimit
        };

    /*---------------------------  MPEG-4  ---------------------------*/
    public static Mpeg4ConfigDto ToDto(this Mpeg4Configuration src) => src == null
        ? null!
        : new Mpeg4ConfigDto
        {
            GovLength = src.GovLength,
            Profile = (EnumMpeg4Profile)src.Mpeg4Profile
        };

    /*---------------------------  H-264  ---------------------------*/
    public static H264ConfigDto ToDto(this H264Configuration src) => src == null
        ? null!
        : new H264ConfigDto
        {
            GovLength = src.GovLength,
            Profile = (EnumH264Profile)src.H264Profile
        };

    /*---------------------------  펼친 Rate 값용 Property  ---------------------------*/
    private static int? FrameRate(this VideoRateControl? rc) => rc?.FrameRateLimit;
    private static int? Bitrate(this VideoRateControl? rc) => rc?.BitrateLimit;
    private static int? EncodingInterval(this VideoRateControl? rc) => rc?.EncodingInterval;
}