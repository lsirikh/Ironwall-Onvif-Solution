using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.PTZPresets;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Services;
/// <summary>
/// ONVIF 서비스 진입점  
/// ‣ 카메라 초기화(Prepare/Full) 및 PTZ · Imaging 제어용 고수준 API 집합  
/// ‣ 각 메서드는 <b>await</b> 기반 비동기 실행을 권장한다.
/// </summary>
public interface IOnvifService
{
    /*────────────────────  INITIALIZE  ────────────────────*/

    /// <summary>
    /// 카메라의 모든 ONVIF 클라이언트를 생성하고
    /// <see cref="ICameraOnvifModel"/> 을 완전 채운다.<br/>
    /// ‣ Device / Media / PTZ / Imaging / Profiles / Presets 포함<br/>
    /// ‣ 내부 캐시(동일 IP·ID) 존재 시 재사용
    /// </summary>
    /// <param name="conn">
    ///     연결 정보(IP · 포트 · 계정)를 담은 <see cref="IConnectionModel"/>
    /// </param>
    /// <returns>
    ///     초기화가 끝난 모델, 실패하면 <c>null</c>
    /// </returns>
    Task<ICameraOnvifModel?> InitializeFullAsync(IConnectionModel conn, CancellationToken ct = default);

    /// <summary>
    /// 빠른 준비 단계.<br/>
    /// ‣ <b>DeviceClient</b> 로 기본 메타 정보만 수집(제조사·펌웨어 등)<br/>
    /// ‣ Media / PTZ / Imaging 클라이언트는 생성하지 않는다.
    /// </summary>
    /// <param name="conn">카메라 연결 정보</param>
    /// <returns>준비된 모델, 실패 시 <c>null</c></returns>
    Task<ICameraOnvifModel?> InitializeDeviceAsync(IConnectionModel conn, CancellationToken ct = default);

    /// <summary>
    /// Device·Media 까지만 연결하여 <b>Profiles·스트림 URL</b> 을 채운다.<br/>
    /// ‣ PTZ / Imaging은 포함하지 않음<br/>
    /// ‣ 스트리밍 재생, 화질 정보만 필요할 때 사용
    /// </summary>
    /// <param name="conn">카메라 연결 정보</param>
    /// <returns>프로파일까지 채워진 모델, 실패 시 <c>null</c></returns>
    Task<ICameraOnvifModel?> InitializeProfileAsync(IConnectionModel conn, CancellationToken ct = default);

    /*────────────────────  PTZ PRESET  ────────────────────*/

    /// <summary>
    /// 카메라에 설정된 PTZ 프리셋 목록을 조회한다.
    /// </summary>
    /// <param name="client">이미 연결된 <see cref="PTZClient"/> 인스턴스</param>
    /// <param name="token">프로파일 토큰</param>
    /// <returns>프리셋 DTO 리스트(없으면 <c>null</c>)</returns>
    Task<List<PTZPresetDto>?> GetPTZPreset(PTZClient client, string token);

    /// <summary>
    /// 지정한 프리셋을 삭제한다.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="profileToken">대상 프로파일 토큰</param>
    /// <param name="presetToken">삭제할 프리셋 토큰</param>
    /// <returns>성공 여부</returns>
    Task<bool> DeletePTZPreset(PTZClient ptzClient, string profileToken, string presetToken);

    /// <summary>
    /// 카메라의 ‘Home’ 프리셋으로 이동한다.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="ptzSpeed">이동 속도(Null = 디폴트)</param>
    /// <param name="profileToken">프로파일 토큰</param>
    /// <returns>성공 여부</returns>
    Task<bool> GoHomePreset(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken);

    /// <summary>
    /// 지정한 프리셋으로 이동한다.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="ptzSpeed">이동 속도(Null = 디폴트)</param>
    /// <param name="profileToken">프로파일 토큰</param>
    /// <param name="presetToken">목표 프리셋 토큰</param>
    /// <returns>성공 여부</returns>
    Task<bool> GoPTZPreset(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken, string presetToken);

    /// <summary>
    /// ‘Home’ 프리셋을 현재 위치로 설정한다.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="profileToken">프로파일 토큰</param>
    /// <returns>성공 여부</returns>
    Task<bool> SetHomePreset(PTZClient ptzClient, string profileToken);

    /// <summary>
    /// 새로운 프리셋을 저장한다.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="profileToken">프로파일 토큰</param>
    /// <param name="presetName">프리셋 명칭</param>
    /// <param name="presetToken">저장될 토큰(Null = 자동 할당)</param>
    /// <returns>성공 여부</returns>
    Task<bool> SetPTZPreset(PTZClient ptzClient, string profileToken, string presetName, string presetToken);


    /*────────────────────  PTZ MOVE  ────────────────────*/

    /// <summary>
    /// PTZ ContinuousMove 명령 – <b>타임아웃 기반</b>.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="ptzSpeed">이동 속도 벡터</param>
    /// <param name="profileToken">프로파일 토큰</param>
    /// <param name="timeout">P60 형식 타임아웃 문자열(예: "PT5S")</param>
    /// <returns>성공 여부</returns>
    Task<bool> MovePTZ(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken, string timeout = null);

    /// <summary>
    /// PTZ ContinuousMove 명령 – <b>CancellationToken 기반</b>  
    /// 토큰이 취소되면 즉시 <see cref="StopPTZ"/> 가 호출된다.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="ptzSpeed">이동 속도 벡터</param>
    /// <param name="profileToken">프로파일 토큰</param>
    /// <param name="token">취소 토큰</param>
    /// <param name="timeout">P60 형식 타임아웃(옵션)</param>
    /// <returns>성공 여부</returns>
    Task<bool> MovePTZ(PTZClient ptzClient, PtzSpeedDto ptzSpeed, string profileToken, CancellationToken token, string timeout = null);

    /// <summary>
    /// PTZ 동작을 중지한다.
    /// </summary>
    /// <param name="ptzClient">PTZ 클라이언트</param>
    /// <param name="profileToken">프로파일 토큰</param>
    /// <param name="panTilt">Pan/Tilt 중지 여부</param>
    /// <param name="zoom">Zoom 중지 여부</param>
    /// <returns>성공 여부</returns>
    Task<bool> StopPTZ(PTZClient ptzClient, string profileToken, bool panTilt, bool zoom);


    /*────────────────────  IMAGING MOVE  ────────────────────*/

    /// <summary>
    /// Imaging Move 명령 – 토큰 없이 단순 호출.
    /// </summary>
    /// <param name="imagingPortClient">Imaging 클라이언트</param>
    /// <param name="vsourceToken">비디오 소스 토큰</param>
    /// <param name="focusMove">초점 이동 DTO</param>
    /// <returns>성공 여부</returns>
    Task<bool> MoveImaging(ImagingPortClient imagingPortClient, string vsourceToken, FocusMoveDto focusMove);

    /// <summary>
    /// Imaging Move 명령 – <see cref="CancellationToken"/> 지원.  
    /// 취소 시 내부적으로 StopAsync 호출.
    /// </summary>
    /// <param name="imagingPortClient">Imaging 클라이언트</param>
    /// <param name="vsourceToken">비디오 소스 토큰</param>
    /// <param name="focusMove">초점 이동 DTO</param>
    /// <param name="token">취소 토큰(선택)</param>
    /// <returns>성공 여부</returns>
    Task<bool> MoveImaging(ImagingPortClient imagingPortClient, string vsourceToken, FocusMoveDto focusMove, CancellationToken token = default);

    /// <summary>
    /// Imaging 동작을 중지한다.
    /// </summary>
    /// <param name="imagingPortClient">Imaging 클라이언트</param>
    /// <param name="vsourceToken">비디오 소스 토큰</param>
    /// <returns>성공 여부</returns>
    Task<bool> StopImaging(ImagingPortClient imagingPortClient, string vsourceToken);
}