using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using System.Net;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 2/25/2024 10:26:35 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OnvifConnectionModel : IOnvifConnectionModel
    {

        #region - Ctors -
        public OnvifConnectionModel(IConnectionModel model)
        {
            Host = $"{model?.IpAddress}:{model?.PortOnvif}";
            Username = model.Username;
            Password = model.Password;
        }

        public OnvifConnectionModel(ICameraModel model)
        {
            Host = $"{model?.IpAddress}:{model?.PortOnvif}";
            Username = model.Username;
            Password = model.Password;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        #endregion
        #region - Attributes -
        #endregion
    }
}
