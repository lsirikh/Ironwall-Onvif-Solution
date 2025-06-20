using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.VideoSourceConfigs.VideoSource;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 5:37:40 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*============================================================
|  ONVIF ↔ DTO  Mapping Helper                               |
|  - VideoSource → VideoSourceDto                            |
|  - 하위 Imaging / Exposure … 전부 포함                      |
============================================================*/
public static partial class OnvifMappingHelper
{
    /*---------------------------  Video-Source  ----------------------------*/
    public static VideoSourceDto ToDto(this VideoSource src)
    {
        if (src == null) throw new ArgumentNullException(nameof(src));

        return new VideoSourceDto
        {
            /* DeviceEntity */
            Token = src.token,

            /* VideoSource */
            Framerate = src.Framerate,
            Resolution = src.Resolution == null
                       ? null!
                       : new VideoResolutionDto
                       {
                           Width = src.Resolution.Width,
                           Height = src.Resolution.Height
                       },

            Imaging = src.Imaging?.ToDto(),

            Extension = src.Extension?.ToDto()
        };
    }

    /*---------------------------  Video-Source-Ext  ------------------------*/
    private static VideoSourceExtensionDto ToDto(this VideoSourceExtension ext)
    {
        if (ext == null) return null!;

        return new VideoSourceExtensionDto
        {
            Any = ext.Any?.Select(x => x.OuterXml).ToList(),
            Imaging = ext.Imaging?.ToDto(),          /* ImagingSettings20 */
            Extension = ext.Extension?.ToDto()         /* VS-Ext2 */
        };
    }

    private static VideoSourceExtension2Dto ToDto(this VideoSourceExtension2 ext2)
        => ext2 == null
           ? null!
           : new VideoSourceExtension2Dto
           {
               Any = ext2.Any?.Select(x => x.OuterXml).ToList()
           };

    /*---------------------------  Imaging-Settings 1.0  --------------------*/
    private static ImagingSettingsDto ToDto(this ImagingSettings img)
    {
        if (img == null) return null!;

        return new ImagingSettingsDto
        {
            BacklightCompensation = img.BacklightCompensation?.ToDto(),
            Brightness = img.BrightnessSpecified ? img.Brightness : (float?)null,
            ColorSaturation = img.ColorSaturationSpecified ? img.ColorSaturation : (float?)null,
            Contrast = img.ContrastSpecified ? img.Contrast : (float?)null,
            Exposure = img.Exposure?.ToDto(),
            Focus = img.Focus?.ToDto(),
            IrCutFilter = img.IrCutFilterSpecified ? (EnumIrCutFilterMode?)img.IrCutFilter : null,
            Sharpness = img.SharpnessSpecified ? img.Sharpness : (float?)null,
            WideDynamicRange = img.WideDynamicRange?.ToDto(),
            WhiteBalance = img.WhiteBalance?.ToDto(),
            Extension = img.Extension?.ToDto()
        };
    }


    /*---------------- Imaging-Settings 2.0 ------------------------*/
    private static ImagingSettings20Dto ToDto(this ImagingSettings20 src)
    {
        if (src == null) return null!;

        /* 1.0 과 공통인 필드 수동 매핑 */
        var dto = new ImagingSettings20Dto
        {
            BacklightCompensation = src.BacklightCompensation?.ToDto(),
            Brightness = src.BrightnessSpecified ? src.Brightness : (float?)null,
            ColorSaturation = src.ColorSaturationSpecified ? src.ColorSaturation : (float?)null,
            Contrast = src.ContrastSpecified ? src.Contrast : (float?)null,
            Exposure = src.Exposure?.ToDto(),
            Focus = src.Focus?.ToDto(),
            IrCutFilter = src.IrCutFilterSpecified ? (EnumIrCutFilterMode?)src.IrCutFilter : null,
            Sharpness = src.SharpnessSpecified ? src.Sharpness : (float?)null,
            WideDynamicRange = src.WideDynamicRange?.ToDto(),
            WhiteBalance = src.WhiteBalance?.ToDto(),
            Extension = null,                   /* 1.0 전용 Ext 는 없음 */

            /* 2.0 전용 --------------------------*/
            ImageStabilization = src.Extension?.ImageStabilization?.ToDto(),
            Extension20 = src.Extension?.ToDto()   // ImagingSettingsExtension20 → DTO
        };

        return dto;
    }


    /*--------------------------- Sub-structures ----------------------------*/
    private static BacklightCompDto ToDto(this BacklightCompensation bc)
        => bc == null ? null! : new BacklightCompDto
        {
            Mode = (EnumBacklightCompensationMode)bc.Mode,
            Level = bc.Level
        };

    private static ExposureDto ToDto(this Exposure ex)
        => ex == null ? null! : new ExposureDto
        {
            Mode = (EnumExposureMode)ex.Mode,
            Priority = (EnumExposurePriority)ex.Priority,
            Window = ex.Window?.ToDto(),
            MinExposureTime = ex.MinExposureTime,
            MaxExposureTime = ex.MaxExposureTime,
            MinGain = ex.MinGain,
            MaxGain = ex.MaxGain,
            MinIris = ex.MinIris,
            MaxIris = ex.MaxIris,
            ExposureTime = ex.ExposureTime,
            Gain = ex.Gain,
            Iris = ex.Iris
        };

    private static FocusConfigDto ToDto(this FocusConfiguration fc)
        => fc == null ? null! : new FocusConfigDto
        {
            Mode = (EnumAutoFocusMode)fc.AutoFocusMode,
            DefaultSpeed = fc.DefaultSpeed,
            NearLimit = fc.NearLimit,
            FarLimit = fc.FarLimit
        };

    private static WideDynamicRangeDto ToDto(this WideDynamicRange wdr)
        => wdr == null ? null! : new WideDynamicRangeDto
        {
            Mode = (EnumWideDynamicMode)wdr.Mode,
            Level = wdr.Level
        };

    private static WhiteBalanceDto ToDto(this WhiteBalance wb)
        => wb == null ? null! : new WhiteBalanceDto
        {
            Mode = (EnumWhiteBalanceMode)wb.Mode,
            CrGain = wb.CrGain,
            CbGain = wb.CbGain
        };

    private static BacklightCompDto ToDto(this BacklightCompensation20 bc)
        => bc == null ? null! : new BacklightCompDto
        {
            Mode = (EnumBacklightCompensationMode)bc.Mode,
            Level = bc.Level
        };

    private static ExposureDto ToDto(this Exposure20 ex)
        => ex == null ? null! : new ExposureDto
        {
            Mode = (EnumExposureMode)ex.Mode,
            Priority = (EnumExposurePriority)ex.Priority,
            Window = ex.Window?.ToDto(),
            MinExposureTime = ex.MinExposureTime,
            MaxExposureTime = ex.MaxExposureTime,
            MinGain = ex.MinGain,
            MaxGain = ex.MaxGain,
            MinIris = ex.MinIris,
            MaxIris = ex.MaxIris,
            ExposureTime = ex.ExposureTime,
            Gain = ex.Gain,
            Iris = ex.Iris
        };

    private static FocusConfigDto ToDto(this FocusConfiguration20 fc)
        => fc == null ? null! : new FocusConfigDto
        {
            Mode = (EnumAutoFocusMode)fc.AutoFocusMode,
            DefaultSpeed = fc.DefaultSpeed,
            NearLimit = fc.NearLimit,
            FarLimit = fc.FarLimit
        };

    private static WideDynamicRangeDto ToDto(this WideDynamicRange20 wdr)
        => wdr == null ? null! : new WideDynamicRangeDto
        {
            Mode = (EnumWideDynamicMode)wdr.Mode,
            Level = wdr.Level
        };

    private static WhiteBalanceDto ToDto(this WhiteBalance20 wb)
        => wb == null ? null! : new WhiteBalanceDto
        {
            Mode = (EnumWhiteBalanceMode)wb.Mode,
            CrGain = wb.CrGain,
            CbGain = wb.CbGain
        };

    private static ImagingSettingsExtDto ToDto(this ImagingSettingsExtension ext)
        => ext == null ? null! : new ImagingSettingsExtDto
        {
            Any = ext.Any?.Select(x => x.OuterXml).ToList()
        };

    /*---------------- Imaging 2.0 Extension layers -------------------------*/
    private static ImageStabilizationDto ToDto(this ImageStabilization isb)
        => isb == null ? null! : new ImageStabilizationDto
        {
            Mode = (EnumImageStabilizationMode?)isb.Mode,
            Level = isb.LevelSpecified ? isb.Level : (float?)null
        };

    private static ImagingSettingsExt20Dto ToDto(this ImagingSettingsExtension20 ext20)
    {
        if (ext20 == null) return null!;

        return new ImagingSettingsExt20Dto
        {
            Any = ext20.Any?.Select(x => x.OuterXml).ToList(),
            ImageStabilization = ext20.ImageStabilization?.ToDto(),
            Extension = ext20.Extension?.ToDto()   // → Extension202 DTO
        };
    }

    private static ImagingSettingsExt202Dto ToDto(this ImagingSettingsExtension202 ext202)
    {
        if (ext202 == null) return null!;

        return new ImagingSettingsExt202Dto
        {
            IrCutFilterAutoAdjustments = ext202.IrCutFilterAutoAdjustment?
                                         .Select(ToDto).ToList() ?? new(),
            Extension = ext202.Extension?.ToDto()
        };
    }

    private static IrCutFilterAutoAdjustmentDto ToDto(this IrCutFilterAutoAdjustment adj)
        => adj == null ? null! : new IrCutFilterAutoAdjustmentDto
        {
            BoundaryType = adj.BoundaryType,
            BoundaryOffset = adj.BoundaryOffsetSpecified ? adj.BoundaryOffset : (float?)null,
            ResponseTime = adj.ResponseTime,
            Extension = adj.Extension == null
                           ? null
                           : new IrCutAutoAdjExtDto
                           {
                               Any = adj.Extension.Any?.Select(x => x.OuterXml).ToList()
                           }
        };

    private static ImagingSettingsExt203Dto ToDto(this ImagingSettingsExtension203 ext203)
    {
        if (ext203 == null) return null!;

        return new ImagingSettingsExt203Dto
        {
            ToneCompensation = ext203.ToneCompensation?.ToDto(),
            Defogging = ext203.Defogging?.ToDto(),
            NoiseReduction = ext203.NoiseReduction?.ToDto(),
            Extension = ext203.Extension?.ToDto()
        };
    }

    private static ToneCompensationDto ToDto(this ToneCompensation tc)
        => tc == null ? null! : new ToneCompensationDto
        {
            Mode = tc.Mode,
            Level = tc.LevelSpecified ? tc.Level : (float?)null,
            Extension = tc.Extension == null
                      ? null
                      : new ToneCompensationExtDto
                      {
                          Any = tc.Extension.Any?.Select(x => x.OuterXml).ToList()
                      }
        };

    private static DefoggingDto ToDto(this Defogging df)
        => df == null ? null! : new DefoggingDto
        {
            Mode = df.Mode,
            Level = df.LevelSpecified ? df.Level : (float?)null,
            Extension = df.Extension == null
                      ? null
                      : new DefoggingExtDto
                      {
                          Any = df.Extension.Any?.Select(x => x.OuterXml).ToList()
                      }
        };

    private static NoiseReductionDto ToDto(this NoiseReduction nr)
        => nr == null ? null! : new NoiseReductionDto
        {
            Level = nr.Level,
            Any = nr.Any?.Select(x => x.OuterXml).ToList()
        };

    private static ImagingSettingsExt204Dto ToDto(this ImagingSettingsExtension204 ext204)
        => ext204 == null ? null! : new ImagingSettingsExt204Dto
        {
            Any = ext204.Any?.Select(x => x.OuterXml).ToList()
        };

    /*--------------  WindowRectDto ---------------------------*/
    private static WindowRectDto? ToDto(this Rectangle? src)
    {
        if (src == null) return null;

        return new WindowRectDto
        {
            Left = src.leftSpecified ? src.left : (float?)null,
            Top = src.topSpecified ? src.top : (float?)null,
            Right = src.rightSpecified ? src.right : (float?)null,
            Bottom = src.bottomSpecified ? src.bottom : (float?)null
        };
    }
}