using Ironwall.Dotnet.Libraries.OnvifSolution.Security;
using System;
using System.ServiceModel;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/13/2025 7:38:16 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public static class SoapSecurityHelper
{
    /// <summary>
    /// 클라이언트에 UsernameToken 보안 헤더 부착
    /// </summary>
    public static void Apply(ChannelFactory factory,
                             string user, string pass, TimeSpan timeShift)
    {
        factory.Endpoint.EndpointBehaviors.Clear();
        factory.Endpoint.EndpointBehaviors
               .Add(new SoapSecurityHeaderBehavior(user, pass, timeShift));
    }
}
