using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.VideoSourceConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/16/2025 3:04:34 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/// <summary>
/// WSDL 코드-제너레이티드 타입  ➜  ❶ DTO 로 변환
///            (Device SDK)        (View/API 공유)
/// </summary>
public static partial class OnvifMappingHelper
{
    /*───────────────────────────────*
     * 0)  Util : null-safe ToList() *
     *───────────────────────────────*/
    private static IReadOnlyList<T>? AsReadOnly<T>(this IEnumerable<T>? src)
        => src == null ? null : src.ToList();

    /*───────────────────────────────*
     * 1)  가장 작은 단위 매핑       *
     *───────────────────────────────*/
    private static RectangleDto ToDto(this IntRectangle r) => new()
    {
        X = r.x,
        Y = r.y,
        Width = r.width,
        Height = r.height
    };

    private static RotateExtensionDto? ToDto(this RotateExtension? ext)
        => ext == null ? null : new RotateExtensionDto
        {
            Any = ext.Any?.Select(x => x.OuterXml).AsReadOnly()
        };

    private static RotateDto? ToDto(this Rotate? r)
        => r == null ? null : new RotateDto
        {
            Mode = Enum.TryParse(r.Mode.ToString(), out RotateModeDto m) ? m : RotateModeDto.OFF,
            Degree = r.DegreeSpecified ? r.Degree : null,
            Extension = r.Extension.ToDto()
        };

    private static SceneOrientationDto? ToDto(this SceneOrientation? s)
        => s == null ? null : new SceneOrientationDto
        {
            Mode = Enum.TryParse(s.Mode.ToString(), out SceneOrientationModeDto m) ? m : SceneOrientationModeDto.Off,
            Orientation = s.Orientation
        };

    private static LensOffsetDto? ToDto(this LensOffset? o)
        => o == null ? null : new LensOffsetDto
        {
            X = o.xSpecified ? o.x : null,
            Y = o.ySpecified ? o.y : null
        };

    private static LensProjectionDto ToDto(this LensProjection p) => new()
    {
        Angle = p.Angle,
        Radius = p.Radius,
        Transmittance = p.TransmittanceSpecified ? p.Transmittance : null,
        Any = p.Any?.Select(x => x.OuterXml).AsReadOnly()
    };

    private static LensDescriptionDto ToDto(this LensDescription l) => new()
    {
        Offset = l.Offset.ToDto(),
        Projection = l.Projection?.Select(ToDto).AsReadOnly(),
        XFactor = l.XFactor,
        Any = l.Any?.Select(x => x.OuterXml).AsReadOnly(),
        FocalLength = l.FocalLengthSpecified ? l.FocalLength : null
    };

    /*───────────────────────────────*
     * 2)  Extension 블록 매핑       *
     *───────────────────────────────*/
    private static VideoSourceConfigExt2Dto? ToDto(this VideoSourceConfigurationExtension2? e2)
        => e2 == null ? null : new VideoSourceConfigExt2Dto
        {
            Lenses = e2.LensDescription?.Select(ToDto).AsReadOnly(),
            SceneOrientation = e2.SceneOrientation.ToDto(),
            Any = e2.Any?.Select(x => x.OuterXml).AsReadOnly()
        };

    private static VideoSourceConfigExtDto? ToDto(this VideoSourceConfigurationExtension? e)
        => e == null ? null : new VideoSourceConfigExtDto
        {
            Rotate = e.Rotate.ToDto(),
            Extension2 = e.Extension.ToDto()
        };

    /*───────────────────────────────*
     * 3)  최상위 Video-Source 매핑  *
     *───────────────────────────────*/
    public static VideoSourceConfigDto ToDto(this VideoSourceConfiguration v)
    {
        if (v == null) throw new ArgumentNullException(nameof(v));

        return new VideoSourceConfigDto
        {
            /* ConfigurationEntity 공통 */
            Name = v.Name,
            UseCount = v.UseCount,
            Token = v.token,

            /* VideoSourceConfiguration 고유 */
            SourceToken = v.SourceToken,
            Bounds = v.Bounds.ToDto(),
            AnyElements = v.Any?.Select(x => x.OuterXml).AsReadOnly(),
            ViewMode = v.ViewMode,

            /* 확장 */
            Extension = v.Extension.ToDto()
        };
    }
}