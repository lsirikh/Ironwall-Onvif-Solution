using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Text;
using System.Net;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Factories;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/13/2025 7:37:34 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/// <summary>
/// SOAP/WCF 바인딩 생성을 하나로 묶어 재사용·테스트 가능하게 함
/// </summary>
public interface IBindingFactory
{
    Binding Create();
}

/// <remarks>
/// CustomBinding 은 매 호출 시 동일 설정이므로
/// 싱글턴 캐싱을 적용 (thread-safe).
/// </remarks>
internal sealed class DefaultBindingFactory : IBindingFactory
{
    private readonly Lazy<Binding> _binding = new(CreateBindingCore);

    public Binding Create() => _binding.Value;

    private static Binding CreateBindingCore()
    {
        var binding = new CustomBinding
        {
            Elements =
            {
                new TextMessageEncodingBindingElement
                {
                    MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12,
                                                                  AddressingVersion.None),
                    WriteEncoding  = Encoding.UTF8
                },
                new HttpTransportBindingElement
                {
                    AllowCookies           = true,
                    MaxBufferSize          = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,

                    /* Digest 인증 스킴 추가 */
                    AuthenticationScheme   = AuthenticationSchemes.Digest
                }
            }
        };
        return binding;
    }
}
