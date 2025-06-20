using Autofac;
using Ironwall.Dotnet.Libraries.Base.Services;
using Ironwall.Dotnet.Libraries.OnvifSolution.Factories;
using Ironwall.Dotnet.Libraries.OnvifSolution.Services;
using System.ServiceModel.Channels;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Modules
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 2/7/2024 4:37:16 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OnvifServiceModule : Module
    {
        #region - Ctors -
        public OnvifServiceModule(ILogService? log = default) => _log = log;
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                _log?.Info($"{nameof(OnvifServiceModule)} is trying to create a {nameof(OnvifService)} instance as InstancePerDependency.");
                builder.RegisterType<DefaultBindingFactory>().As<IBindingFactory>().SingleInstance();          // 한 번만 생성
                /* 2)  “Binding” 을 직접 필요로 하는 컴포넌트가 있다면 … */
                builder.Register(c => c.Resolve<IBindingFactory>().Create()).As<Binding>().SingleInstance();          // 내부 Lazy<Binding> 덕분에 동일 인스턴스
                builder.RegisterType<OnvifClientFactory>().As<IOnvifClientFactory>().SingleInstance();          // 또는 InstancePerDependency()
                builder.RegisterType<OnvifService>().As<IOnvifService>().SingleInstance();
            }
            catch
            {
                throw;
            }
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService? _log;
        #endregion
    }
}
