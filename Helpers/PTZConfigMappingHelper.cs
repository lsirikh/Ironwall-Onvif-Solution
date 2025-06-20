using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.PtzConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 12:56:56 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*-----------------------------------------------------------------------
|  Onvif → DTO 변환 유틸리티
|  • 모든 메서드는 null-safe 확장 메서드 형태
|  • 필요 타입만 구현 : PTZConfiguration 계열
|-----------------------------------------------------------------------*/
public static partial class OnvifMappingHelper
{
    /*--------------------------- 기본 Range ---------------------------*/
    public static FloatRangeDto? ToDto(this FloatRange? src) =>
        src is null ? null :
        new FloatRangeDto { Min = src.Min, Max = src.Max };

    /*--------------------------- Vector ---------------------------*/
    public static Vector2DDto? ToDto(this Vector2D? v) =>
        v is null ? null :
        new Vector2DDto { X = v.x, Y = v.y, Space = v.space };

    public static Vector1DDto? ToDto(this Vector1D? v) =>
        v is null ? null :
        new Vector1DDto { X = v.x, Space = v.space };

    /*--------------------------- Space-Desc ---------------------------*/
    public static Space2DDescriptionDto? ToDto(this Space2DDescription? s) =>
        s is null ? null :
        new Space2DDescriptionDto
        {
            URI = s.URI,
            XRange = s.XRange.ToDto()!,
            YRange = s.YRange.ToDto()!
        };

    public static Space1DDescriptionDto? ToDto(this Space1DDescription? s) =>
        s is null ? null :
        new Space1DDescriptionDto
        {
            URI = s.URI,
            XRange = s.XRange.ToDto()!
        };

    /*--------------------------- Limits ---------------------------*/
    public static PanTiltLimitsDto? ToDto(this PanTiltLimits? p) =>
        p is null ? null : new PanTiltLimitsDto { Range = p.Range.ToDto()! };

    public static ZoomLimitsDto? ToDto(this ZoomLimits? z) =>
        z is null ? null : new ZoomLimitsDto { Range = z.Range.ToDto()! };

    /*--------------------------- Speed ---------------------------*/
    public static PtzSpeedDto? ToDto(this PTZSpeed? s) =>
        s is null ? null :
        new PtzSpeedDto
        {
            PanTilt = s.PanTilt.ToDto(),
            Zoom = s.Zoom.ToDto()
        };

    /*--------------------------- EFlip / Reverse ---------------------------*/
    private static EnumEFlipMode Map(this EFlipMode m) => m switch
    {
        EFlipMode.ON => EnumEFlipMode.On,
        EFlipMode.Extended => EnumEFlipMode.Extended,
        _ => EnumEFlipMode.Off
    };

    private static EnumReverseMode Map(this ReverseMode m) => m switch
    {
        ReverseMode.ON => EnumReverseMode.On,
        ReverseMode.AUTO => EnumReverseMode.Auto,
        ReverseMode.Extended => EnumReverseMode.Extended,
        _ => EnumReverseMode.Off
    };

    public static EFlipDto? ToDto(this EFlip? e) => e is null ? null :
        new EFlipDto { Mode = e.Mode.Map(), Any = e.Any?.Select(x => x.OuterXml).ToList() };

    public static ReverseDto? ToDto(this Reverse? r) => r is null ? null :
        new ReverseDto { Mode = r.Mode.Map(), Any = r.Any?.Select(x => x.OuterXml).ToList() };

    /*--------------------------- PT-Control-Direction ---------------------------*/
    public static PtControlDirExtDto? ToDto(this PTControlDirectionExtension? ext) =>
        ext is null ? null :
        new PtControlDirExtDto { Any = ext.Any?.Select(x => x.OuterXml).ToList() };

    public static PtControlDirectionDto? ToDto(this PTControlDirection? dir) =>
        dir is null ? null :
        new PtControlDirectionDto
        {
            EFlip = dir.EFlip.ToDto(),
            Reverse = dir.Reverse.ToDto(),
            Extension = dir.Extension.ToDto()
        };

    /*--------------------------- Extension ---------------------------*/
    public static PtzConfigExt2Dto? ToDto(this PTZConfigurationExtension2? ext) =>
        ext is null ? null :
        new PtzConfigExt2Dto { Any = ext.Any?.Select(x => x.OuterXml).ToList() };

    public static PtzConfigExtDto? ToDto(this PTZConfigurationExtension? ext) =>
        ext is null ? null :
        new PtzConfigExtDto
        {
            Any = ext.Any?.Select(x => x.OuterXml).ToList(),
            PTControlDirection = ext.PTControlDirection.ToDto(),
            Extension = ext.Extension.ToDto()
        };

    /*--------------------------- PTZ-Configuration ---------------------------*/
    public static PTZConfigDto? ToDto(this PTZConfiguration? cfg)
    {
        if (cfg is null) return null;

        return new PTZConfigDto
        {
            /* ConfigEntity 공통 */
            Name = cfg.Name,
            UseCount = cfg.UseCount,
            Token = cfg.token,

            /* 고유 필드 */
            NodeToken = cfg.NodeToken,
            AbsPanTiltPosSpace = cfg.DefaultAbsolutePantTiltPositionSpace,
            AbsZoomPosSpace = cfg.DefaultAbsoluteZoomPositionSpace,
            RelPanTiltTransSpace = cfg.DefaultRelativePanTiltTranslationSpace,
            RelZoomTransSpace = cfg.DefaultRelativeZoomTranslationSpace,
            ContPanTiltVelSpace = cfg.DefaultContinuousPanTiltVelocitySpace,
            ContZoomVelSpace = cfg.DefaultContinuousZoomVelocitySpace,
            DefaultSpeed = cfg.DefaultPTZSpeed.ToDto(),
            DefaultTimeout = cfg.DefaultPTZTimeout,
            PanTiltLimits = cfg.PanTiltLimits.ToDto(),
            ZoomLimits = cfg.ZoomLimits.ToDto(),
            Extension = cfg.Extension.ToDto(),
            MoveRamp = cfg.MoveRampSpecified ? cfg.MoveRamp : (int?)null,
            PresetRamp = cfg.PresetRampSpecified ? cfg.PresetRamp : (int?)null,
            PresetTourRamp = cfg.PresetTourRampSpecified ? cfg.PresetTourRamp : (int?)null
        };
    }
}