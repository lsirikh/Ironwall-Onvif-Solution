using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Security
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 12/14/2023 11:45:35 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SoapSecurityHeaderBehavior : IEndpointBehavior
    {

        #region - Ctors -
        public SoapSecurityHeaderBehavior(string username, string password) : this(username, password, TimeSpan.FromMilliseconds(0))
        {
        }

        public SoapSecurityHeaderBehavior(string username, string password, TimeSpan timeShift)
        {
            this.username = username;
            this.password = password;
            time_shift = timeShift;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new SoapSecurityHeaderInspector(username, password, time_shift));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {

        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        readonly string username;
        readonly string password;
        readonly TimeSpan time_shift;
        #endregion
    }
}
