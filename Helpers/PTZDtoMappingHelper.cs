using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 9:18:42 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/

/*--------------------------- Window (left-right-top-bottom) ----------*/
public static partial class OnvifMappingHelper
{
    /*------------------------------------------------------------------
     |  DTO → WSDL 변환 계층
     |  - 기존 ToDto( WSDL → DTO ) 와 대칭되도록 ‘ToWsdl’ 네이밍 채택
     |  - 필요 시 별도 ReverseHelper 로 분리해도 무방하지만
     |    한 클래스로 두 방향 매핑을 모아두면 DI·유지보수 간편
     ------------------------------------------------------------------*/

    /*---------------------------  PTZ-Speed DTO → WSDL  ---------------------------*/
    public static Ptz.PTZSpeed ToWsdl(this PtzSpeedDto src)
    {
        if (src == null) return null!;

        return new Ptz.PTZSpeed
        {
            PanTilt = src.PanTilt == null
                ? null
                : new Ptz.Vector2D
                {
                    x = src.PanTilt.X,
                    y = src.PanTilt.Y,
                    space = src.PanTilt.Space
                },

            Zoom = src.Zoom == null
                ? null
                : new Ptz.Vector1D
                {
                    x = src.Zoom.X,
                    space = src.Zoom.Space
                }
        };
    }

    /*---------------------------  Continuous-Focus DTO → WSDL  ---------------------------*/
    private static ContinuousFocus? ToWsdl(this ContinuousFocusDto? src)
        => src == null ? null : new ContinuousFocus { Speed = src.Speed };

    /*---------------------------  Relative-Focus DTO → WSDL  ---------------------------*/
    private static RelativeFocus? ToWsdl(this RelativeFocusDto? src)
        => src == null
            ? null
            : new RelativeFocus
            {
                Distance = src.Distance,
                Speed = src.Speed.GetValueOrDefault(),
                SpeedSpecified = src.Speed.HasValue
            };

    /*---------------------------  Absolute-Focus DTO → WSDL  ---------------------------*/
    private static AbsoluteFocus? ToWsdl(this AbsoluteFocusDto? src)
        => src == null
            ? null
            : new AbsoluteFocus
            {
                Position = src.Position,
                Speed = src.Speed.GetValueOrDefault(),
                SpeedSpecified = src.Speed.HasValue
            };

    /*---------------------------  Focus-Move DTO → WSDL  ---------------------------*/
    public static Imaging.FocusMove ToWsdl(this FocusMoveDto src)
    {
        if (src == null) return null!;

        return new Imaging.FocusMove
        {
            Absolute = src.Absolute.ToWsdl(),
            Relative = src.Relative.ToWsdl(),
            Continuous = src.Continuous.ToWsdl()
        };
    }
}
