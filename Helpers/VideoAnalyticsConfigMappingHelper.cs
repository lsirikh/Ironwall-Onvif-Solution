using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.VideoAnalyticConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Newtonsoft.Json.Linq;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/16/2025 3:30:42 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public static partial class OnvifMappingHelper
{
    /*────────────────── 0. TOP LEVEL ──────────────────*/
    public static VideoAnalyticsConfigDto? ToDto(this VideoAnalyticsConfiguration? src)
        => src == null
            ? null
            : new VideoAnalyticsConfigDto
            {
                Name = src.Name,
                UseCount = src.UseCount,
                Token = src.token,
                AnalyticsEngineConfiguration = src.AnalyticsEngineConfiguration.ToDto(),
                RuleEngineConfiguration = src.RuleEngineConfiguration.ToDto()
            };

    /*────────────────── 1-A. ANALYTICS ENGINE ─────────*/
    public static AnalyticsEngineConfigDto? ToDto(this AnalyticsEngineConfiguration? src)
        => src == null
            ? null
            : new AnalyticsEngineConfigDto
            {
                Modules = src.AnalyticsModule?
                                .Select(m => m.ToDto())
                                .Where(x => x != null)!
                                .ToList()
                         ?? new()
            };

    /*────────────────── 1-B. RULE ENGINE ─────────────*/
    public static RuleEngineConfigDto? ToDto(this RuleEngineConfiguration? src)
        => src == null
            ? null
            : new RuleEngineConfigDto
            {
                Rules = src.Rule?
                              .Select(r => r.ToDto())
                              .Where(x => x != null)!
                              .ToList()
                          ?? new()
            };

    /*────────────────── 2. CONFIG & PARAM LIST ───────*/
    public static ConfigDto? ToDto(this Config? src)
        => src == null
            ? null
            : new ConfigDto
            {
                Name = src.Name ?? string.Empty,
                TypeQualifiedName = src.Type?.ToString(),
                Parameters = src.Parameters.ToDto()
            };

    public static ItemListDto? ToDto(this ItemList? src)
        => src == null
            ? null
            : new ItemListDto
            {
                SimpleItems = src.SimpleItem?
                                   .Select(s => s.ToDto())
                                   .Where(x => x != null)!
                                   .ToList()
                               ?? new(),
                ElementItems = src.ElementItem?
                                   .Select(e => e.ToDto())
                                   .Where(x => x != null)!
                                   .ToList()
                               ?? new()
            };

    public static SimpleItemDto? ToDto(this ItemListSimpleItem? src)
        => src == null
            ? null
            : new SimpleItemDto
            {
                Name = src.Name ?? string.Empty,
                Value = src.Value ?? string.Empty
            };

    public static ElementItemDto? ToDto(this ItemListElementItem? src)
        => src == null
            ? null
            : new ElementItemDto
            {
                Name = src.Name ?? string.Empty,
                RawXml = src.Any is null ? string.Empty : src.Any.OuterXml
            };

    /*────────────────── 3. ENGINE-INPUT ───────────────*/
    public static AnalyticsEngineInputDto? ToDto(this AnalyticsEngineInput? src)
        => src == null
            ? null
            : new AnalyticsEngineInputDto
            {
                Name = src.Name,
                UseCount = src.UseCount,
                Token = src.token,
                SourceIdentification = src.SourceIdentification.ToDto()
                // 필요 시 VideoInput / MetadataInput 매핑 확장
            };

    public static SourceIdentificationDto? ToDto(this SourceIdentification? src)
        => src == null
            ? null
            : new SourceIdentificationDto
            {
                Name = src.Name,
                Tokens = src.Token?.ToList() ?? new List<string>()
            };

    /*──────────────────────────────────────────────────*/
    /*  - 확장이 필요한 경우 다음과 같은 패턴을 그대로  */
    /*    유지하여 *.ToDto() 메서드를 추가하면 된다.    */
    /*──────────────────────────────────────────────────*/
}