/*
 *  OnvifClientFactory.cs  –  최종 수정본
 *  ------------------------------------
 *  • BindingFactory : 싱글턴/DI 로 전달           (IBindingFactory)
 *  • SoapSecurityHeaderBehavior : 보안헤더 삽입   (Helpers namespace)
 *  • dynamic 사용 → ClientBase<TChannel>.ChannelFactory 접근 컴파일 오류 해결
 *  • 모든 공통 로직은 CreateClientAsync<T>() 제네릭 한 곳에서 처리
 */

using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;          // SoapSecurityHeaderBehavior
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using Ironwall.Dotnet.Libraries.OnvifSolution.Security;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Factories
{
    /****************************************************************************
        Purpose      : ONVIF WCF 클라이언트 생성 전담
        Created By   : GHLee
        Created On   : 2025-06-13
        Department   : SW Team
        Company      : Sensorway Co., Ltd.
        Email        : lsirikh@naver.com
    ****************************************************************************/
    internal sealed class OnvifClientFactory : IOnvifClientFactory
    {
        #region ── ctor ─────────────────────────────────────────────────────────
        private readonly IBindingFactory _bindingFactory;
        public OnvifClientFactory(IBindingFactory bindingFactory)
            => _bindingFactory = bindingFactory;
        #endregion

        #region ── Generic core ────────────────────────────────────────────────
        /*
         *  모든 Media / PTZ / Imaging 클라이언트 생성 로직을 하나로 통합.
         *  - conn        : 접속 정보
         *  - capsRequired: GetCapabilitiesAsync 에 전달할 카테고리
         *  - activator   : (xAddr, binding) → 실제 클라이언트 생성 람다
         */
        private async Task<T> CreateClientAsync<T>(IOnvifConnectionModel conn, CapabilityCategory[] capsRequired,
            Func<Uri, Binding, T> activator) where T : class
        {
            /* 1) DeviceClient 선행 확보 (보안 헤더 포함) */
            var device = await CreateDeviceAsync(conn);

            /* 2) Capabilities 조회 → 해당 서비스 XAddr 결정 */
            var caps = await device.GetCapabilitiesAsync(capsRequired);
            var xAddr = caps.Capabilities switch
            {
                { Media: not null } when typeof(T) == typeof(MediaClient) => caps.Capabilities.Media.XAddr,
                { PTZ: not null } when typeof(T) == typeof(PTZClient) => caps.Capabilities.PTZ.XAddr,
                { Imaging: not null } when typeof(T) == typeof(ImagingPortClient) => caps.Capabilities.Imaging.XAddr,
                _ => throw new InvalidOperationException($"Unsupported capability for {typeof(T).Name}")
            };

            /* 3) 실제 클라이언트 인스턴스화 */
            var binding = _bindingFactory.Create();
            var client = activator(new Uri(xAddr), binding);

            /* 4) 보안헤더 삽입 – dynamic 으로 ChannelFactory 접근 (컴파일 오류 방지) */
            var timeShift = await GetDeviceTimeShiftAsync(device);

            dynamic dynClient = client;  // 런타임에 ClientBase<TChannel>
            dynClient.ChannelFactory.Endpoint.EndpointBehaviors.Clear();
            dynClient.ChannelFactory.Endpoint.EndpointBehaviors
                     .Add(new SoapSecurityHeaderBehavior(conn.Username, conn.Password, timeShift));

            return client;
        }
        #endregion

        #region ── 공개 API (Device / Media / PTZ / Imaging) ───────────────────
        /// <summary>
        /// DeviceClient - 기본 진입점
        /// </summary>
        public async Task<DeviceClient> CreateDeviceAsync(IOnvifConnectionModel conn)
        {
            var ep = new EndpointAddress($"http://{conn.Host}/onvif/device_service");
            var dev = new DeviceClient(_bindingFactory.Create(), ep);

            /* A. HTTP-Digest 자격증명 추가 */
            dev.ClientCredentials.HttpDigest.ClientCredential =
                new NetworkCredential(conn.Username, conn.Password);

            /* B. WS-UsernameToken (기존) */
            dev.ClientCredentials.UserName.UserName = conn.Username;
            dev.ClientCredentials.UserName.Password = conn.Password;

            /* C. WS-Security 헤더 삽입 */
            var ts = await GetDeviceTimeShiftAsync(dev);
            dynamic d = dev;                         // ClientBase 파생 Proxy
            d.ChannelFactory.Endpoint.EndpointBehaviors.Clear();
            d.ChannelFactory.Endpoint.EndpointBehaviors
                .Add(new SoapSecurityHeaderBehavior(conn.Username, conn.Password, ts));

            return dev;
        }

        public Task<MediaClient> CreateMediaAsync(IOnvifConnectionModel conn) =>
            CreateClientAsync(conn,
                              new[] { CapabilityCategory.Media },
                              (uri, b) => new MediaClient(b, new EndpointAddress(uri)));

        public Task<PTZClient> CreatePtzAsync(IOnvifConnectionModel conn) =>
            CreateClientAsync(conn,
                              new[] { CapabilityCategory.PTZ },
                              (uri, b) => new PTZClient(b, new EndpointAddress(uri)));

        public Task<ImagingPortClient> CreateImagingAsync(IOnvifConnectionModel conn) =>
            CreateClientAsync(conn,
                              new[] { CapabilityCategory.Imaging },
                              (uri, b) => new ImagingPortClient(b, new EndpointAddress(uri)));
        #endregion

        #region ── Utility ─────────────────────────────────────────────────────
        /// <summary>카메라-로컬 시간과 PC UTC 차이 계산</summary>
        private static async Task<TimeSpan> GetDeviceTimeShiftAsync(DeviceClient dev)
        {
            var utc = (await dev.GetSystemDateAndTimeAsync()).UTCDateTime;
            var dt = new System.DateTime(utc.Date.Year, utc.Date.Month, utc.Date.Day,
                                   utc.Time.Hour, utc.Time.Minute, utc.Time.Second);
            return dt - System.DateTime.UtcNow;
        }
        #endregion
    }
}
