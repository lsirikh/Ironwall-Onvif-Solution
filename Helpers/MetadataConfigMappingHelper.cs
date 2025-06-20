using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.MetadataConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 10:16:07 AM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*---------------------------  Metadata-Mapping Helper  ---------------------------*/
public static partial class OnvifMappingHelper
{
    /*---------------------------------------------------------------------------
    여러 ONVIF 원본 객체를 MetadataConfigDto 로 변환한다.
    필요 시 하위 객체(PTZFilter · Multicast · AnalyticsEngine 등)도
    재귀적으로 DTO 로 매핑한다.
    ---------------------------------------------------------------------------*/
    public static MetadataConfigDto ToDto(this MetadataConfiguration src)
    {
        if (src is null) throw new ArgumentNullException(nameof(src));

        return new MetadataConfigDto
        {
            /* ConfigurationEntity 공통 */
            Name = src.Name,
            UseCount = src.UseCount,
            Token = src.token,

            /* 고유 필드 */
            PTZStatus = src.PTZStatus?.ToDto(),
            Events = src.Events?.ToDto(),
            Analytics = src.AnalyticsSpecified ? src.Analytics : null,
            Multicast = src.Multicast?.ToDto(),
            SessionTimeout = src.SessionTimeout,
            CompressionType = src.CompressionType,
            GeoLocation = src.GeoLocationSpecified ? src.GeoLocation : null,
            ShapePolygon = src.ShapePolygonSpecified ? src.ShapePolygon : null,
            AnalyticsEngineConfiguration = src.AnalyticsEngineConfiguration?.ToDto()
        };
    }

    /*---------------------------  PTZ-Filter ---------------------------*/
    private static PtzFilterDto ToDto(this PTZFilter src) => new()
    {
        Status = src.Status,
        Position = src.Position,
        FieldOfView = src.FieldOfViewSpecified ? src.FieldOfView : false
    };

    /*---------------------------  Event-Subscription ---------------------------*/
    private static EventSubscriptionDto ToDto(this EventSubscription src) => new()
    {
        Filter = src.Filter?.ToDto(),
        SubscriptionPolicy = src.SubscriptionPolicy?.ToDto()
    };

    private static FilterTypeDto ToDto(this FilterType src) => new()
    {
        /* <Any/> XML 들을 string 로 변환하여 보존 */
        Topics = src.Any?.Select(n => n.OuterXml).ToList()
    };

    private static SubscriptionPolicyDto ToDto(this EventSubscriptionSubscriptionPolicy src) => new()
    {
        AnyElements = src.Any?.Select(n => n.OuterXml).ToList()
    };
}
