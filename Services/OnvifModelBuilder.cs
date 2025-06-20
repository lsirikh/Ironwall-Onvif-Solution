using Ironwall.Dotnet.Libraries.Base.Services;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.AudioSourceConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.AudioSourceConfigs.AudioSource;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.PtzConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.VideoSourceConfigs;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Profiles.VideoSourceConfigs.VideoSource;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.PTZPresets;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.Factories;
using Ironwall.Dotnet.Libraries.OnvifSolution.Helpers;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using Ironwall.Dotnet.Libraries.OnvifSolution.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Media.Media3D;
using Xunit.Abstractions;
using AudioEncoderConfiguration = Ironwall.Dotnet.Libraries.OnvifSolution.Media.AudioEncoderConfiguration;
using AudioSourceConfiguration = Ironwall.Dotnet.Libraries.OnvifSolution.Media.AudioSourceConfiguration;
using MetadataConfiguration = Ironwall.Dotnet.Libraries.OnvifSolution.Media.MetadataConfiguration;
using VideoAnalyticsConfiguration = Ironwall.Dotnet.Libraries.OnvifSolution.Media.VideoAnalyticsConfiguration;
using VideoEncoderConfiguration = Ironwall.Dotnet.Libraries.OnvifSolution.Media.VideoEncoderConfiguration;
using VideoSourceConfiguration = Ironwall.Dotnet.Libraries.OnvifSolution.Media.VideoSourceConfiguration;

/*──────────────────────────────---───────*\
 |  OnvifModelBuilder                                                          |
 |  ───────────────────────────────────── |
 |  • 목적 : 한 대의 ONVIF 카메라에서 모든 초기 데이터를 수집하여             |
 |          CameraOnvifModel(POCO) 을 완성한다.                                |
 |  • 호출 순서 (플루언트)                                                    |
 |      1) WithDeviceInfoAsync   : 장치 기본 정보, DeviceClient 확보           |
 |      2) WithClientsAsync      : Media / PTZ / Imaging 클라이언트 생성       |
 |      3) WithProfilesAsync     : 스트림·Encoder·Metadata·Source 파싱      |
 |      4) WithPTZPresetsAsync   : 프리셋 조회                                 |
 |      5) Build()/BuildAsync()  : 최종 모델 반환                              |
 |  • 실패 허용 설계                                                          |
 |      - 각 단계는 try/catch 로 자체 오류 처리 후 다음 단계로 진행            |
 |      - 치명적 오류일 때만 CameraStatus = NOT_AVAILABLE 설정                 |
 *──────────────────────────────---───────*/
public sealed class OnvifModelBuilder
{
    /*────────────────── 필드 ──────────────────*/
    private readonly IOnvifConnectionModel _conn;     // 접속 정보(IP, Port, 계정)
    private readonly IOnvifClientFactory _factory;  // WCF 클라이언트 팩토리(DI)
    private readonly ILogService? _log;      // 선택형 로거(DI)

    private readonly CameraOnvifModel _model;    // 빌드 대상 POCO

    /*────────────────── ctor ──────────────────*/
    public OnvifModelBuilder(IOnvifConnectionModel conn,
                             IOnvifClientFactory factory,
                             IConnectionModel connectionModel,
                             ILogService? log = null)
    {
        _conn = conn ?? throw new ArgumentNullException(nameof(conn));
        _factory = factory;
        _log = log;
        _model = new CameraOnvifModel(connectionModel);         // 빈 POCO 생성
    }

    /*───────────────────────────────────────────
     *  STEP 1 : 장치 기본 정보 + DeviceClient
     *──────────────────────────────────────────*/
    public async Task<OnvifModelBuilder> WithDeviceInfoAsync(CancellationToken ct = default)
    {
        try
        {

            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            // 1-1) DeviceClient 인스턴스 확보
            _model.DeviceClient = await _factory.CreateDeviceAsync(_conn);

            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            // 1-2) 카메라 제조사·펌웨어 등 메타데이터 조회
            var info = await _model.DeviceClient
                                   .GetDeviceInformationAsync(new GetDeviceInformationRequest());

            // 1-3) 모델 필드 채움
            _model.Manufacturer = info.Manufacturer;
            _model.DeviceModel = info.Model;
            _model.FirmwareVersion = info.FirmwareVersion;
            _model.SerialNumber = info.SerialNumber;
            //MAC Address
            _model.HardwareId = info.HardwareId;


            /* 2) Name / Location → GetScopes */
            var scopes = await _model.DeviceClient.GetScopesAsync();
            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            foreach (var s in scopes.Scopes ?? Array.Empty<Scope>())
            {
                if (s.ScopeItem.StartsWith("onvif://www.onvif.org/name/"))
                    _model.Name =
                        Uri.UnescapeDataString(s.ScopeItem["onvif://www.onvif.org/name/".Length..]);

                if (s.ScopeItem.StartsWith("onvif://www.onvif.org/location/"))
                    _model.Location =
                        Uri.UnescapeDataString(s.ScopeItem["onvif://www.onvif.org/location/".Length..]);
            }

            /* 3) MAC Address / IP → GetNetworkInterfaces */
            var nics = await _model.DeviceClient.GetNetworkInterfacesAsync();
            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            var nic = nics?.NetworkInterfaces?.FirstOrDefault();
            if (nic != null)
            {
                _model.MacAddress = nic.Info.HwAddress;
            }

            /* 4) ONVIF Version → GetServices(false) */
            var svcs = await _model.DeviceClient.GetServicesAsync(false);
            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            var devSvc = svcs?.Service?.FirstOrDefault(
                             s => s.Namespace == "http://www.onvif.org/ver10/device/wsdl");
            if (devSvc != null)
            {
                _model.ServiceUri = devSvc.XAddr;
                _model.OnvifVersion =
                    $"{devSvc.Version.Major}.{devSvc.Version.Minor}";
            }
            _model.IsDevicePossible = true;
        }
        catch (Exception ex)
        {
            // DeviceClient 자체가 실패하면 이후 단계 진행해도 의미 없음
            _model.CameraStatus = EnumCameraStatus.NOT_AVAILABLE;
            _log?.Error($"[Builder] DeviceInfo 단계 실패 : {ex.Message}");
        }
        return this; 
    }

    /*───────────────────────────────────────────
     *  STEP 2 : Media / PTZ / Imaging 클라이언트
     *──────────────────────────────────────────*/
    public async Task<OnvifModelBuilder> WithCreateMediaAsync(CancellationToken ct = default)
    {
        // 이전 단계(Device Info)에서 이미 “사용 불가”로 판정되면 더 이상 진행하지 않는다.
        if (_model.CameraStatus == EnumCameraStatus.NOT_AVAILABLE)
            return this;

        /* Media 클라이언트 (필수) */
        try
        {
            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            _model.MediaClient = await _factory.CreateMediaAsync(_conn);
            _model.IsMediaPossible = _model.MediaClient is not null;
        }
        catch (Exception ex)
        {
            // MediaClient 없으면 프로파일을 못 가져오므로 모델을 NOT_AVAILABLE 로 표시
            _model.CameraStatus = EnumCameraStatus.NOT_AVAILABLE;
            _log?.Error($"[Builder] MediaClient 생성 실패 : {ex.Message}");
        }

        return this;
    }


    public async Task<OnvifModelBuilder> WithCreatePtzAsync(CancellationToken ct = default)
    {
        // 이전 단계(Device Info)에서 이미 “사용 불가”로 판정되면 더 이상 진행하지 않는다.
        if (_model.CameraStatus == EnumCameraStatus.NOT_AVAILABLE)
            return this;

        try
        {
            /* 1) 취소 토큰 확인 -> 외부에서 빌드 과정을 중단하고 싶을 때 */
            if (ct.IsCancellationRequested) throw new TaskCanceledException();

            /* 2) PTZ 클라이언트 생성 -> PTZ 미지원 카메라도 있으므로 실패는 예외 아님 */
            _model.PtzClient = await _factory.CreatePtzAsync(_conn);
            _model.IsPtzPossible = _model.PtzClient is not null;

            /* 3) 카메라 유형 결정 */
            _model.Type = _model.IsPtzPossible
                            ? EnumCameraType.PTZ_CAMERA
                            : EnumCameraType.FIXED_CAMERA;
        }
        catch
        {
            /* PTZ 미지원일 수 있으므로 조용히 패스 */
        }
        return this;   // 다음 체인으로
    }

    public async Task<OnvifModelBuilder> WithCreateImageingAsync(CancellationToken ct = default)
    {
        if (_model.CameraStatus == EnumCameraStatus.NOT_AVAILABLE)
            return this;

        try
        {
            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            _model.ImagingClient = await _factory.CreateImagingAsync(_conn);
            _model.IsImagingPossible = _model.ImagingClient is not null;
        }
        catch { /* Imaging 미지원 → 무시 */ }
        return this;
    }

   
    /*───────────────────────────────────────────
     *  STEP 3 : 프로파일 + 스트림 URL + 구성 파싱
     *──────────────────────────────────────────*/
    public async Task<OnvifModelBuilder> WithProfilesAsync(CancellationToken ct = default)
    {
        if (!_model.IsMediaPossible)           // MediaClient 없으면 스킵
            return this;

        // 1) 모든 프로파일 조회
        if (ct.IsCancellationRequested) throw new TaskCanceledException();
        var profiles = (await _model.MediaClient!.GetProfilesAsync())?.Profiles?.ToList();
        if (profiles == null || !(profiles.Count > 0)) return this;
        _model.Profiles = profiles;                    // 원본 저장
        _model.CameraMedia!.Token = profiles!.FirstOrDefault()!.token;

        if (ct.IsCancellationRequested) throw new TaskCanceledException();
        // 2) 각 프로파일 → CameraProfileDto 로 변환
        foreach (Profile p in profiles ?? Enumerable.Empty<Profile>())
        {
            var cp = await BuildCameraProfileAsync(p);          // 상세 파싱
            if (cp != null)
                _model.CameraMedia.Profiles.Add((CameraProfileDto)cp);            // 리스트에 추가
        }
       
        return this;
    }


    /*───────────────────────────────────────────
     *  STEP 4 : PTZ Preset 목록
     *──────────────────────────────────────────*/
    public async Task<OnvifModelBuilder> WithPTZPresetsAsync(CancellationToken ct = default)
    {
        // Fixed 카메라면 프리셋 무의미
        if (_model.Type != EnumCameraType.PTZ_CAMERA)
            return this;

        try
        {
            if (ct.IsCancellationRequested) throw new TaskCanceledException();
            var presets = await _model.PtzClient.GetPresetsAsync(_model.CameraMedia.Token);

            _model.CameraMedia.PTZPresets = presets.Preset
                                            .Select(p => OnvifMappingHelper.ToDto(p))
                                            .ToList();
        }
        catch 
        { }
        return this;
    }
    

    /*───────────────────────────────────────────
     *  STEP 5 : 최종 모델 반환
     *──────────────────────────────────────────*/
    public ICameraOnvifModel Build()
    {
        // Device 연결 OK + 아직 상태 설정 안 됨 ⇒ AVAILABLE 로 변경
        if (_model.CameraStatus == EnumCameraStatus.NONE)
            _model.CameraStatus = EnumCameraStatus.AVAILABLE;
        

        return _model;     // 완성된 모델 반환
    }

    public Task<ICameraOnvifModel> BuildAsync() => Task.FromResult(Build());

    /*───────────────────────────────────────────
     *  내부 헬퍼 : ONVIF Profile → CameraProfileDto
     *──────────────────────────────────────────*/
    private async Task<ICameraProfileDto?> BuildCameraProfileAsync (Profile p)
    {
        try
        {
            /* 1) 기본 필드 채우기 */
            ICameraProfileDto cp = new CameraProfileDto
            {
                Name = p.Name,
                Token = p.token,
                Fixed = p.@fixed
            };

            /* 2) 스트림 URL 얻기 */
            var setup = new StreamSetup
            {
                Stream = StreamType.RTPUnicast,      // 단일 송출
                Transport = new Transport { Protocol = TransportProtocol.RTSP }
            };

            cp = await GetProfileStreamUrl(cp, p.token, _model);
            /* 3) 나머지 구성 파싱 (기존 메서드 재활용) */
            cp = GetVideoSourceConfig(cp, p.VideoSourceConfiguration);
            cp = GetAudioSourceConfig(cp, p.AudioSourceConfiguration);
            cp = GetVideoEncoderConfig(cp, p.VideoEncoderConfiguration);
            cp = GetAudioEncoderConfig(cp, p.AudioEncoderConfiguration);
            cp = GetVideoAnalyticsConfig(cp, p.VideoAnalyticsConfiguration);
            cp = GetMetadataConfig(cp, p.MetadataConfiguration);
            cp = GetExtension(cp, p.Extension);

            return cp == null? throw new NullReferenceException() : cp;
        }
        catch (Exception ex)
        {
            // 프로파일 하나 실패해도 전체 빌드는 계속된다.
            _log?.Error($"[Builder] Profile({p.Name}) 파싱 실패 : {ex.Message}");
            return null;
        }
    }

    #region 기존 OnvifService 의 private 헬퍼 이동
    /// <summary>
    /// VideoSource를 가져오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    private ICameraProfileDto GetVideoSourceConfig(ICameraProfileDto cp, VideoSourceConfiguration vsc)
    {
        try
        {
            if (vsc == null) throw new NullReferenceException($"{nameof(VideoSourceConfiguration)} is not exist.");
            
            var vcsDto = OnvifMappingHelper.ToDto(vsc);
            if (vcsDto == null) throw new NullReferenceException();
            cp.VideoSourceConfig = vcsDto;
        }
        catch (NullReferenceException)
        {

        }
        catch (TaskCanceledException ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetVideoSourceConfig)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;
    }


    /// <summary>
    /// 메타데이터 가져오는 Config
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="mc"></param>
    /// <returns></returns>
    private ICameraProfileDto GetMetadataConfig(ICameraProfileDto cp, MetadataConfiguration mc)
    {
        try
        {
            if (mc == null) throw new NullReferenceException($"{nameof(Ironwall.Dotnet.Libraries.OnvifSolution.Media.MetadataConfiguration)} is not exist.");

            cp.MetadataConfig = OnvifMappingHelper.ToDto(mc);

        }
        catch (NullReferenceException)
        {

        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetMetadataConfig)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;
    }


    /// <summary>
    /// 프로파일 별로 URL을 가져오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="token"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    private async Task<ICameraProfileDto> GetProfileStreamUrl(ICameraProfileDto cp, string token, ICameraOnvifModel model)
    {
        try
        {
            if (model == null || model.MediaClient == null) throw new NullReferenceException($"{nameof(MediaUri)} is not exist.");

            var streamSetup = new StreamSetup
            {
                Stream = StreamType.RTPUnicast,  // 또는 RTPMulticast
                Transport = new Transport
                {
                    Protocol = TransportProtocol.RTSP
                }
            };
            if (model!.MediaClient == null) return null;

            MediaUri stream = await model!.MediaClient!.GetStreamUriAsync(streamSetup, token);
            
            if (stream != null)
                cp.MediaUri = OnvifMappingHelper.ToDto(stream);
        }
        catch (NullReferenceException)
        {
        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetProfileStreamUrl)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;

    }

    /// <summary>
    /// VideoAnalyticsConfig 가져오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="videoAnalyticsConfiguration"></param>
    /// <returns></returns>
    private ICameraProfileDto GetVideoAnalyticsConfig(ICameraProfileDto cp, VideoAnalyticsConfiguration videoAnalyticsConfiguration)
    {
        try
        {
            if (videoAnalyticsConfiguration == null) throw new NullReferenceException($"{nameof(VideoAnalyticsConfiguration)} is not exist.");

            cp.VideoAnalyticsConfig = OnvifMappingHelper.ToDto(videoAnalyticsConfiguration);
        }
        catch (NullReferenceException)
        {
        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetVideoAnalyticsConfig)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;
    }

    /// <summary>
    /// AudioEncodingConfig를 가져오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="audioEncoderConfiguration"></param>
    /// <returns></returns>
    private ICameraProfileDto GetAudioEncoderConfig(ICameraProfileDto cp, AudioEncoderConfiguration audioEncoderConfiguration)
    {
        try
        {
            if (audioEncoderConfiguration == null) throw new NullReferenceException($"{nameof(AudioEncoderConfiguration)} is not exist.");

            cp.AudioEncoderConfig = OnvifMappingHelper.ToDto(audioEncoderConfiguration);
        }
        catch (NullReferenceException)
        {
        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetAudioEncoderConfig)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;
    }

    /// <summary>
    /// AudioSourceConfig를 가져오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="audioSourceConfiguration"></param>
    /// <returns></returns>
    private ICameraProfileDto GetAudioSourceConfig(ICameraProfileDto cp, AudioSourceConfiguration audioSourceConfiguration) 
    {
        try
        {
            if (audioSourceConfiguration == null) throw new NullReferenceException($"{nameof(AudioSourceConfiguration)} is not exist.");

            cp.AudioSourceConfig = OnvifMappingHelper.ToDto(audioSourceConfiguration);
        }
        catch (NullReferenceException)
        {
        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetAudioSourceConfig)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;
    }

    /// <summary>
    /// VideoEncodingConfig를 갖고오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="videoEncoderConfiguration"></param>
    /// <returns></returns>
    private ICameraProfileDto GetVideoEncoderConfig(ICameraProfileDto cp, VideoEncoderConfiguration videoEncoderConfiguration)
    {
        try
        {
            if (videoEncoderConfiguration == null) throw new NullReferenceException($"{nameof(VideoEncoderConfiguration)} is not exist.");

            cp.VideoEncoderConfig = OnvifMappingHelper.ToDto(videoEncoderConfiguration);
        }
        catch (NullReferenceException)
        {

        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetVideoEncoderConfig)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;
    }

    /// <summary>
    /// ProfileExtension을 가져오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="pe"></param>
    /// <returns></returns>
    private ICameraProfileDto GetExtension(ICameraProfileDto cp, ProfileExtension pe)
    {
        try
        {
            if (pe == null) throw new NullReferenceException($"{nameof(ProfileExtension)} is not exist.");

            var vcsDto = OnvifMappingHelper.ToDto(pe);
            if (vcsDto == null) throw new NullReferenceException();
            cp.ProfileExtension = vcsDto;
        }
        catch (NullReferenceException)
        {

        }
        catch (TaskCanceledException ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetExtension)} of {nameof(OnvifService)} : {ex.Message}");
        }
        return cp;
    }

    /// <summary>
    /// AudioSource 리스트를 가져오는 메소드
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private async Task<List<AudioSourceDto>?> GetAudioSource(MediaClient client)
    {
        try
        {

            if (client == null) throw new NullReferenceException($"{nameof(MediaClient)} is not exist.");

            var audioSources = await client.GetAudioSourcesAsync();
            if (audioSources == null) return null;

            var ass = new List<AudioSourceDto>();
            foreach (var item in audioSources.AudioSources)
            {
                var asDto = OnvifMappingHelper.ToDto(item);
                if (asDto == null) continue;
                ass.Add(asDto);
            }
            return ass;
        }
        catch (NullReferenceException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetAudioSource)} of {nameof(OnvifService)} : {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    private async Task<List<VideoSourceDto>?> GetVideoSource(MediaClient client)
    {
        try
        {
            if (client == null) throw new NullReferenceException($"{nameof(MediaClient)} is not exist.");

            var vsrcResp = await client.GetVideoSourcesAsync();
            if (vsrcResp == null || vsrcResp.VideoSources == null) return null;
            
            var vss = new List<VideoSourceDto>();
            foreach (var item in vsrcResp.VideoSources)
            {
                var vsDto = OnvifMappingHelper.ToDto(item);
                if(vsDto == null) continue;
                vss.Add(vsDto);
            }
            return vss;
        }
        catch (NullReferenceException)
        {
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetVideoSource)} of {nameof(OnvifService)} : {ex.Message}");
            return null;
        }
    }
    

    /// <summary>
    /// PTZConfig 정보를 갖고 오는 메소드
    /// </summary>
    /// <param name="cp"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    private async Task<List<PTZConfigDto>?> GetPTZConfig(PTZClient client)
    {
        try
        {
            if (client == null) throw new NullReferenceException($"{nameof(PTZClient)} is not exist.");

            var configs = await client.GetConfigurationsAsync();
            if (configs == null) return null;

            var cps = new List<PTZConfigDto>();

            foreach (var item in configs.PTZConfiguration)
            {
                if (item == null) continue;
                var ptzConfig = OnvifMappingHelper.ToDto(item);
                if (ptzConfig == null) continue;
                var ptzNode = await client.GetNodeAsync(item.NodeToken);
                ptzConfig.PTZNode = OnvifMappingHelper.ToDto(ptzNode);
                cps.Add(ptzConfig);            
            }

            return cps;
        }
        catch (Exception ex)
        {
            _log?.Error($"Raised Exception in {nameof(GetPTZConfig)} of {nameof(OnvifService)} : {ex.Message}");
            return null;
        }
    }

    #endregion
}