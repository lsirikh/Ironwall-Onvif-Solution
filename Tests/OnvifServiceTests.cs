using Ironwall.Dotnet.Libraries.Base.Services;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models.Commons;
using Ironwall.Dotnet.Libraries.OnvifSolution.Factories;
using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using Xunit;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Tests;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/17/2025 9:32:46 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public class OnvifServiceTests
{

    private static List<string> ips =new List<string>
    { 
        "192.168.1.119",
        "192.168.1.66",
        "192.168.1.184",
        "192.168.1.170",
        "192.168.1.108",
        "192.168.1.175",
        "192.168.1.176",
        "192.168.1.65",
        "192.168.1.177",
        //"192.168.1.119",
        //"192.168.1.66",
        //"192.168.1.184",
        //"192.168.1.170",
        //"192.168.1.108",
        //"192.168.1.175",
        //"192.168.1.176",
        //"192.168.1.65",
        //"192.168.1.177",
        //"192.168.1.119",
        //"192.168.1.66",
        //"192.168.1.184",
        //"192.168.1.170",
        //"192.168.1.108",
        //"192.168.1.175",
        //"192.168.1.176",
        //"192.168.1.65",
        //"192.168.1.177",
        //"192.168.1.119",
        //"192.168.1.66",
        //"192.168.1.184",
        //"192.168.1.170",
        //"192.168.1.108",
        //"192.168.1.175",
        //"192.168.1.176",
        //"192.168.1.65",
        //"192.168.1.177",
        //"192.168.1.119",
        //"192.168.1.66",
        //"192.168.1.184",
        //"192.168.1.170",
        //"192.168.1.108",
        //"192.168.1.175",
        //"192.168.1.176",
        //"192.168.1.65",
        //"192.168.1.177",
        //"192.168.1.119",
        //"192.168.1.66",
        //"192.168.1.184",
        //"192.168.1.170",
        //"192.168.1.108",
        //"192.168.1.175",
        //"192.168.1.176",
        //"192.168.1.65",
        //"192.168.1.177",
        //"192.168.1.119",
        //"192.168.1.66",
        //"192.168.1.184",
        //"192.168.1.170",
        //"192.168.1.108",
        //"192.168.1.175",
        //"192.168.1.176",
        //"192.168.1.65",
        //"192.168.1.177"
    };

    private static IEnumerable<IConnectionModel> CamConnections
    {
        get
        {
            foreach (var item in ips)
            {
                yield return new ConnectionModel
                {
                    IpAddress = item,
                    Port = 80,
                    PortOnvif = 80,
                    Username = "admin",
                    Password = "sensorway1"
                };
            }
        }
    }

    /*------------------------------------------------------------------
     |  2)  OnvifService 생성 (싱글턴)
     |      ‣ 내부 ConcurrentDictionary 로 다중 스레드 안전
     ------------------------------------------------------------------*/
    private static readonly IOnvifService Svc = new OnvifService(
        new OnvifClientFactory(new DefaultBindingFactory()),
        new LogService());

    /*---------------- 3) 5대를 병렬로 Prepare 단계 테스트 --------------*/
    //[Fact(DisplayName = "5대 실카메라 – Prepare 병렬 성공")]
    //public async Task Prepare_AllCameras_Parallel()
    //{
    //    /* arrange */
    //    var tasks = CamConnections
    //                .Select(async cm =>
    //                {
    //                    var model = await Svc.InitializeDeviceAsync(cm);
    //                    return (cm.IpAddress, model);
    //                })
    //                .ToList();

    //    if (tasks.Count == 0)
    //        return;          // 환경변수 미설정 → 전체 Skip

    //    /* act  – 병렬 실행 */
    //    var results = await Task.WhenAll(tasks);

    //    /* assert */
    //    foreach (var (ip, cam) in results)
    //    {
    //        Assert.NotNull(cam);                             // 모델 null 여부
    //        Assert.Equal(EnumCameraStatus.AVAILABLE, cam!.CameraStatus);
    //        Assert.True(cam.IsDevicePossible);               // 기본 연결 OK
    //    }
    //}
    [Fact(DisplayName = "실카메라 – Prepare 병렬(Streaming) 테스트")]
    public async Task Prepare_AllCameras_Parallel()
    {
        /*--------------------------------------------------------------
         * 1) 결과를 모아둘 스레드-세이프 컬렉션
         *------------------------------------------------------------*/
        var bag = new ConcurrentBag<(string Ip, ICameraOnvifModel? Model)>();

        /*--------------------------------------------------------------
         * 2) CamConnections 를 병렬로 순회
         *    ‣ Parallel.ForEachAsync 는 .NET 6+ 의 IAsyncEnumerable helper
         *    ‣ await 내부에서 I/O 작업을 처리하면서도 워커 수를 조절
         *------------------------------------------------------------*/
        await Parallel.ForEachAsync(
            CamConnections,
            /* 옵셔널: 최대 동시 카메라 수 제한 예) degreeOfParallelism: 8 */
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (cm, ct) =>
            {
                var model = await Svc.InitializeDeviceAsync(cm);   // 비동기 I/O
                bag.Add(($"{cm.IpAddress}_{DateTime.Now.Ticks}", model));
            });

        /*--------------------------------------------------------------
         * 3) 결과 검증
         *------------------------------------------------------------*/
        foreach (var (ip, cam) in bag)
        {
            Assert.NotNull(cam);
            Assert.Equal(EnumCameraStatus.AVAILABLE, cam!.CameraStatus);
            Assert.True(cam.IsDevicePossible);
        }
    }

    /*------------- 4) 병렬로 Full 초기화(MEDIA 포함) -------------*/
    [Fact(DisplayName = "실카메라 – PtzInit 병렬 테스트")]
    public async Task FullInit_PtzCameras_Parallel()
    {

        /*--------------------------------------------------------------
         * 1) 결과를 모아둘 스레드-세이프 컬렉션
         *------------------------------------------------------------*/
        var bag = new ConcurrentBag<(string Ip, ICameraOnvifModel? Model)>();

        /*--------------------------------------------------------------
         * 2) CamConnections 를 병렬로 순회
         *    ‣ Parallel.ForEachAsync 는 .NET 6+ 의 IAsyncEnumerable helper
         *    ‣ await 내부에서 I/O 작업을 처리하면서도 워커 수를 조절
         *------------------------------------------------------------*/
        await Parallel.ForEachAsync(
            CamConnections,
            /* 옵셔널: 최대 동시 카메라 수 제한 예) degreeOfParallelism: 8 */
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (cm, ct) =>
            {
                var model = await Svc.InitializeFullAsync(cm);   // 비동기 I/O
                bag.Add(($"{cm.IpAddress}_{DateTime.Now.Ticks}", model));
            });

        /*--------------------------------------------------------------
         * 3) 결과 검증
         *------------------------------------------------------------*/
        foreach (var (ip, cam) in bag)
        {
            Assert.NotNull(cam);
            Assert.Equal(EnumCameraStatus.AVAILABLE, cam!.CameraStatus);
            Assert.True(cam.IsMediaPossible);                // MediaClient 확보
            switch (cam.Type)
            {
                case EnumCameraType.NONE:
                    break;
                case EnumCameraType.FIXED_CAMERA:
                    break;
                case EnumCameraType.PTZ_CAMERA:
                    Assert.NotEmpty(cam.CameraMedia.PTZPresets);       // 최소 1개 프로파일
                    break;
                default:
                    break;
            }
            
        }
    }


    /*------------- 4) 병렬로 Full 초기화(MEDIA 포함) -------------*/
    [Fact(DisplayName = "실카메라 – FullInit 병렬 성공")]
    public async Task FullInit_AllCameras_Parallel()
    {

        /*--------------------------------------------------------------
         * 1) 결과를 모아둘 스레드-세이프 컬렉션
         *------------------------------------------------------------*/
        var bag = new ConcurrentBag<(string Ip, ICameraOnvifModel? Model)>();

        /*--------------------------------------------------------------
         * 2) CamConnections 를 병렬로 순회
         *    ‣ Parallel.ForEachAsync 는 .NET 6+ 의 IAsyncEnumerable helper
         *    ‣ await 내부에서 I/O 작업을 처리하면서도 워커 수를 조절
         *------------------------------------------------------------*/
        await Parallel.ForEachAsync(
            CamConnections,
            /* 옵셔널: 최대 동시 카메라 수 제한 예) degreeOfParallelism: 8 */
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            async (cm, ct) =>
            {
                var model = await Svc.InitializeFullAsync(cm);   // 비동기 I/O
                bag.Add(($"{cm.IpAddress}_{DateTime.Now.Ticks}", model));
            });

        /*--------------------------------------------------------------
         * 3) 결과 검증
         *------------------------------------------------------------*/
        foreach (var (ip, cam) in bag)
        {
            Assert.NotNull(cam);
            Assert.Equal(EnumCameraStatus.AVAILABLE, cam!.CameraStatus);
            Assert.True(cam.IsMediaPossible);                // MediaClient 확보
            Assert.NotEmpty(cam.CameraMedia.Profiles);       // 최소 1개 프로파일
        }
    }

    /*------------- 4) 병렬로 Full 초기화(MEDIA 포함) -------------*/
    [Fact(DisplayName = "5대 실카메라 – 프리셋 이동 병렬 성공")]
    public async Task MovePreset_AllCameras_Parallel()
    {
        /* arrange */
        var initTasks = CamConnections
                    .Select(async cm =>
                    {
                        var model = await Svc.InitializeFullAsync(cm);
                        return (cm.IpAddress, model);
                    })
                    .ToList();

        if (initTasks.Count == 0)
            return;          // 환경변수 미설정 → 전체 Skip

        var ready = await Task.WhenAll(initTasks);

        /* PTZ 가능한 카메라만 추출 */
        var ptzCams = ready.Where(r => r.model != null && r.model.IsPtzPossible).ToList();
        if (!ptzCams.Any()) return;

        /* act : 카메라별 Preset 1→2→3 순차, 카메라 간 병렬 */
        var camTasks = ptzCams.Select(async item =>
        {
            var ptz = item.model!.PtzClient!;
            var profile = item.model.CameraMedia.Token;        // 기본 프로파일 토큰
            var presets = item.model.CameraMedia.PTZPresets.Take(5);

            foreach (var p in presets)
            {
                bool ok = await Svc.GoPTZPreset(ptz, new PtzSpeedDto(), profile, p.Token);
                Assert.True(ok);                               // 각 이동 성공 여부
                await Task.Delay(2_000);                       // 2초 대기(움직임 완료)
            }
        });

        await Task.WhenAll(camTasks);                          // 병렬 완료
        Assert.True(true);
    }

    /*------------- 4) 1대를 카메라 Preset 설정(MEDIA 포함) -------------*/
    [Fact(DisplayName = "1대 실카메라 – 프리셋 가져오기 이동 설정 삭제  성공")]
    public async Task MovePreset_OneCamera_Parallel()
    {
        /* arrange */
        var initTasks = CamConnections
                    .Select(async cm =>
                    {
                        var model = await Svc.InitializeFullAsync(cm);
                        return (cm.IpAddress, model);
                    })
                    .ToList();

        var ready = await Task.WhenAll(initTasks);

        /* PTZ 가능한 카메라만 추출 */
        var ptzCam = ready.Where(r => r.model != null && r.model.IsPtzPossible).FirstOrDefault();

        var ptz = ptzCam.model!.PtzClient!;
        var profile = ptzCam.model.CameraMedia.Token;        // 기본 프로파일 토큰
        var presets = ptzCam.model.CameraMedia.PTZPresets.Take(5);


        /* Preset 추출 */
        var presetDto = await Svc.GetPTZPreset(ptz, profile);
        Assert.True(presetDto != null && presetDto.Count > 0);

        await Task.Delay(2_000);                       // 2초 대기(움직임 완료)

        /* Ptz 이동 */

        /* Preset 설정 */

        /* Ptz 이동 */
        
        /* Preset 이동 */
       
        /* Preset 삭제 */
        //foreach (var p in presets)
        //{
        //    bool ok = await Svc.GoPTZPreset(ptz, new PtzSpeedDto(), profile, p.Token);
        //    Assert.True(ok);                               // 각 이동 성공 여부
        //}

        Assert.True(true);
    }
}