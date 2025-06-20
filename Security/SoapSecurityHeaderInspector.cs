using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Security
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 12/14/2023 11:48:16 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SoapSecurityHeaderInspector : IClientMessageInspector
    {

        #region - Ctors -
        public SoapSecurityHeaderInspector(string username, string password, TimeSpan timeShift)
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
        #endregion
        #region - Processes -
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {

        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            request.Headers.Add(new SoapSecurityHeader(username, password, time_shift));

            return null;
        }
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
