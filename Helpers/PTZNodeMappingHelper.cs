using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.PtzConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 1:27:31 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/*--------------------------- Mapping-Helper Root ---------------------------*/
public static partial class OnvifMappingHelper
{
    /*--------------------------- PTZ-Node -> DTO ---------------------------*/
    public static PTZNodeDto ToDto(this PTZNode src)
    {
        if (src is null) return null!;

        return new PTZNodeDto
        {
            Token = src.token,
            Name = src.Name,
            Spaces = src.SupportedPTZSpaces?.ToDto(),
            MaximumNumberOfPresets = src.MaximumNumberOfPresets,
            HomeSupported = src.HomeSupported,
            AuxiliaryCommands = src.AuxiliaryCommands?.ToList(),
            Extension = src.Extension?.ToDto(),
            FixedHomePosition = src.FixedHomePositionSpecified ? src.FixedHomePosition : null,
            GeoMove = src.GeoMoveSpecified ? src.GeoMove : null
        };
    }

    /*--------------------------- PTZ-Spaces ---------------------------*/
    private static PtzSpacesDto ToDto(this PTZSpaces src)
    {
        if (src is null) return null!;

        return new PtzSpacesDto
        {
            AbsPanTiltPos = src.AbsolutePanTiltPositionSpace?.Select(ToDto).ToList(),
            AbsZoomPos = src.AbsoluteZoomPositionSpace?.Select(ToDto).ToList(),
            RelPanTiltTrans = src.RelativePanTiltTranslationSpace?.Select(ToDto).ToList(),
            RelZoomTrans = src.RelativeZoomTranslationSpace?.Select(ToDto).ToList(),
            ContPanTiltVel = src.ContinuousPanTiltVelocitySpace?.Select(ToDto).ToList(),
            ContZoomVel = src.ContinuousZoomVelocitySpace?.Select(ToDto).ToList(),
            PanTiltSpeed = src.PanTiltSpeedSpace?.Select(ToDto).ToList(),
            ZoomSpeed = src.ZoomSpeedSpace?.Select(ToDto).ToList(),
            Extension = src.Extension?.ToDto()
        };
    }

   

    /*--------------------------- PTZ-Spaces-Ext ---------------------------*/
    private static PtzSpacesExtDto ToDto(this PTZSpacesExtension src)
    {
        if (src is null) return null!;

        return new PtzSpacesExtDto
        {
            Any = src.Any?.Select(x => x.OuterXml).ToList()
        };
    }

    /*--------------------------- PTZ-Node-Ext ---------------------------*/
    private static PTZNodeExtDto ToDto(this PTZNodeExtension src)
    {
        if (src is null) return null!;

        return new PTZNodeExtDto
        {
            Any = src.Any?.Select(x => x.OuterXml).ToList(),
            SupportedPresetTour = src.SupportedPresetTour?.ToDto(),
            Extension = src.Extension?.ToDto()
        };
    }

    private static PTZNodeExt2Dto ToDto(this PTZNodeExtension2 src)
    {
        if (src is null) return null!;

        return new PTZNodeExt2Dto
        {
            Any = src.Any?.Select(x => x.OuterXml).ToList()
        };
    }

    /*--------------------------- Preset-Tour-Supported ---------------------------*/
    private static PtzPresetTourSupportedDto ToDto(this PTZPresetTourSupported src)
    {
        if (src is null) return null!;

        return new PtzPresetTourSupportedDto
        {
            MaximumNumberOfPresetTours = src.MaximumNumberOfPresetTours,
            Operations = src.PTZPresetTourOperation?
                                             .Select(o => (EnumPtzPresetTourOperation)Enum.Parse(
                                                             typeof(EnumPtzPresetTourOperation), o.ToString()))
                                             .ToList(),
            Extension = src.Extension?.ToDto()
        };
    }

    private static PtzPresetTourSupportedExtDto ToDto(this PTZPresetTourSupportedExtension src)
    {
        if (src is null) return null!;

        return new PtzPresetTourSupportedExtDto
        {
            Any = src.Any?.Select(x => x.OuterXml).ToList()
        };
    }
}
