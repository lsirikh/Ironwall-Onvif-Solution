using Autofac;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.PTZPresets;
using Ironwall.Dotnet.Libraries.OnvifSolution.Factories;
using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceMgmt;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ironwall.Dotnet.Libraries.Base.Services;
using System.Collections.Concurrent;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 12/14/2023 10:58:17 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    internal class OnvifService : IOnvifService
    {
        #region - Ctors -
        public OnvifService(IOnvifClientFactory factory, ILogService? log = null)
        {
            _factory = factory;
            _log = log;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        /*======================================================================*\
         * 1.  InitializePreparing(준비 단계)                                   *
         *     - 장치 연결 및 기본 정보만 필요할 때 사용                        *
        \*======================================================================*/
        public async Task<ICameraOnvifModel?> InitializeDeviceAsync(IConnectionModel conn, CancellationToken ct = default)
        {
            try
            {
                var onvifConn = new OnvifConnectionModel(conn);

                /* 이미 캐시에 있으면 즉시 반환 */
                var key = string.IsNullOrWhiteSpace(conn.IpAddress) ? onvifConn.Username : onvifConn.Host;
                if (_camMap.TryGetValue(key, out var cached))
                    _log.Info($"{cached.IpAddress}:{cached.Port} was already registered...");

                // 빌더 인스턴스 생성
                var model = await new OnvifModelBuilder(onvifConn, _factory, conn, _log)
                                   .WithDeviceInfoAsync(ct)
                                   .BuildAsync(); // 첫 단계는 그냥 호출
                if (ct.IsCancellationRequested) throw new TaskCanceledException();

                if (model?.CameraStatus == EnumCameraStatus.AVAILABLE)
                    _camMap[key] = model;   // 캐시에 저장

                return model;
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        /*======================================================================*\
         * 2.  InitializeOnvif(풀 초기화)                                       *
         *     - Prepare → Clients → Profiles → PTZPresets 전부 수행            *
        \*======================================================================*/
        public async Task<ICameraOnvifModel?> InitializeProfileAsync(IConnectionModel conn, CancellationToken ct = default)
        {
            try
            {
                var onvifConn = new OnvifConnectionModel(conn);

                /* 이미 캐시에 있으면 즉시 반환 */
                var key = string.IsNullOrWhiteSpace(conn.IpAddress) ? onvifConn.Username : onvifConn.Host;
                key = $"{key}_{System.DateTime.Now.Ticks}";
                /* 이미 풀 초기화된 경우 재활용 */
                if (_camMap.TryGetValue(key, out var cached) &&
                    cached.CameraStatus == EnumCameraStatus.AVAILABLE)// MediaClient 존재 여부로 ‘풀 초기화’ 판별
                {
                    _log.Info($"{cached.IpAddress}:{cached.Port} was already registered...");
                }

                // 빌더 인스턴스 생성

                var model = await new OnvifModelBuilder(onvifConn, _factory, conn, _log)
                                     .WithDeviceInfoAsync(ct)          // 첫 단계는 그냥 호출
                                     .Do(b => b.WithCreateMediaAsync(ct))
                                     .Do(b => b.WithCreatePtzAsync(ct))
                                     .Do(b => b.WithCreateImageingAsync(ct))
                                     .Do(b => b.WithProfilesAsync(ct))
                                     .BuildAsync();


                if (model?.CameraStatus == EnumCameraStatus.AVAILABLE)
                    _camMap[key] = model;   // 캐시 갱신

                return model;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<ICameraOnvifModel?> InitializeFullAsync(IConnectionModel conn, CancellationToken ct = default)
        {
            try
            {
                var onvifConn = new OnvifConnectionModel(conn);

                /* 이미 캐시에 있으면 즉시 반환 */
                var key = string.IsNullOrWhiteSpace(conn.IpAddress) ? onvifConn.Username : onvifConn.Host;
                key = $"{key}_{System.DateTime.Now.Ticks}";
                /* 이미 풀 초기화된 경우 재활용 */
                if (_camMap.TryGetValue(key, out var cached) &&
                    cached.CameraStatus == EnumCameraStatus.AVAILABLE)// MediaClient 존재 여부로 ‘풀 초기화’ 판별
                    _log.Info($"{cached.IpAddress}:{cached.Port} was already registered...");

                // 빌더 인스턴스 생성
                var model = await new OnvifModelBuilder(onvifConn, _factory, conn, _log)
                                     .WithDeviceInfoAsync(ct)          // 첫 단계는 그냥 호출
                                     .Do(b => b.WithCreateMediaAsync(ct))
                                     .Do(b => b.WithCreatePtzAsync(ct))
                                     .Do(b => b.WithCreateImageingAsync(ct))
                                     .Do(b => b.WithProfilesAsync(ct))
                                     .Do(b => b.WithPTZPresetsAsync(ct))
                                     .BuildAsync();

                if (model?.CameraStatus == EnumCameraStatus.AVAILABLE)
                    _camMap[key] = model;   // 캐시 갱신

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<List<PTZPresetDto>?> GetPTZPreset(PTZClient client, string token)
        {
            try
            {
                if (client == null) throw new NullReferenceException($"{nameof(PTZClient)} is not exist.");


                var presets = await client.GetPresetsAsync(token);
                if (presets == null || presets.Preset == null) return null;

                var pps = new List<PTZPresetDto>();
                foreach (var item in presets.Preset)
                {
                    var preset = OnvifMappingHelper.ToDto(item);
                    if (preset == null) continue;
                    pps.Add(preset);
                }

                return pps;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(GetPTZPreset)} of {nameof(OnvifService)} : {ex.Message}");
                return null;
            }
        }

        public async Task<bool> GoPTZPreset(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken, string presetToken)
        {
            try
            {
                var velocity = OnvifMappingHelper.ToWsdl(ptzSpeed);
                await ptzClient.GotoPresetAsync(profileToken, presetToken, velocity);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(GoPTZPreset)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SetPTZPreset(PTZClient ptzClient, string profileToken, string presetName, string presetToken)
        {
            try
            {
                var presetRequeset = new SetPresetRequest(profileToken, presetName, presetToken);
                var presetResponse = await ptzClient.SetPresetAsync(presetRequeset);

                if (presetResponse != null)
                {
                    _log?.Info($"Preset Token : {presetResponse.PresetToken}");
                }
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(SetPTZPreset)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> DeletePTZPreset(PTZClient ptzClient, string profileToken, string presetToken)
        {
            try
            {
                await ptzClient.RemovePresetAsync(profileToken, presetToken);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(DeletePTZPreset)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> SetHomePreset(PTZClient ptzClient, string profileToken)
        {
            try
            {
                await ptzClient.SetHomePositionAsync(profileToken);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(SetHomePreset)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> GoHomePreset(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken)
        {
            try
            {
                var velocity = OnvifMappingHelper.ToWsdl(ptzSpeed);
                await ptzClient.GotoHomePositionAsync(profileToken, velocity);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(GoHomePreset)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> MovePTZ(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken, CancellationToken token, string timeout = null)
        {
            try
            {
                var velocity = OnvifMappingHelper.ToWsdl(ptzSpeed);
                var response = await ptzClient.ContinuousMoveAsync(profileToken, velocity, timeout);
                _log?.Info($"Response ContinuousMoveAsync : {response}");
                await Task.Delay(-1, token);
                return true;
            }
            catch (TaskCanceledException)
            {
                await StopPTZ(ptzClient, profileToken, true, true);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(MovePTZ)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> MovePTZ(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken, string timeout = null)
        {
            try
            {
                var velocity = OnvifMappingHelper.ToWsdl(ptzSpeed);
                var response = await ptzClient.ContinuousMoveAsync(profileToken, velocity, timeout);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(MovePTZ)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }

        public async Task<bool> StopPTZ(PTZClient ptzClient, string profileToken, bool panTilt, bool zoom)
        {
            try
            {
                await ptzClient.StopAsync(profileToken, panTilt, zoom);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(StopPTZ)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MoveImaging(ImagingPortClient imagingPortClient, string vsourceToken, FocusMoveDto focusMove, CancellationToken token = default)
        {
            try
            {
                var focusMoveInstance = OnvifMappingHelper.ToWsdl(focusMove);
                await imagingPortClient.MoveAsync(vsourceToken, focusMoveInstance);
                await Task.Delay(-1, token);
                return true;
            }
            catch (TaskCanceledException)
            {
                await imagingPortClient.StopAsync(vsourceToken);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(MoveImaging)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MoveImaging(ImagingPortClient imagingPortClient, string vsourceToken, FocusMoveDto focusMove)
        {
            try
            {
                var focusMoveInstance = OnvifMappingHelper.ToWsdl(focusMove);
                await imagingPortClient.MoveAsync(vsourceToken, focusMoveInstance);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(MoveImaging)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> StopImaging(ImagingPortClient imagingPortClient, string vsourceToken)
        {
            try
            {
                await imagingPortClient.StopAsync(vsourceToken);
                return true;
            }
            catch (Exception ex)
            {
                _log?.Error($"Raised Exception in {nameof(StopPTZ)} of {nameof(OnvifService)} : {ex.Message}");
                return false;
            }
        }

        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private readonly IOnvifClientFactory _factory;
        private readonly ILogService? _log;

        private readonly ConcurrentDictionary<string, ICameraModel> _camMap = new();
        #endregion
    }
}
