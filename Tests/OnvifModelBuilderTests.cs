using Ironwall.Dotnet.Libraries.Base.Services;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Enums;
using Ironwall.Dotnet.Libraries.OnvifSolution.Base.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.DeviceIo;
using Ironwall.Dotnet.Libraries.OnvifSolution.Factories;
using Ironwall.Dotnet.Libraries.OnvifSolution.Imaging;
using Ironwall.Dotnet.Libraries.OnvifSolution.Media;
using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using Ironwall.Dotnet.Libraries.OnvifSolution.Ptz;
using Moq;
using System;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Xunit;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Tests;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/16/2025 11:22:49 AM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/

// ────────────────────────────────────────────────────────────────────────
// 테스트 클래스: 파일-범위 네임스페이스 안에 단일 public 테스트 클래스를 선언
// Syntax: public class <Name> { … }         클래스 정의
// Semantics: xUnit가 이 형식의 public 메서드를 테스트 엔트리로 인식
public class OnvifModelBuilderTests
{
    /*───────────────────────────────────────────────────────────────────*\
     | 2. 공통 빌더 헬퍼                                                   |
    \*───────────────────────────────────────────────────────────────────*/
    private static OnvifModelBuilder CreateBuilder()
    {
        // 테스트용 ConnectionModel + LogService
        var connInfo = new ConnectionModel
        {
            IpAddress = "192.168.1.119",
            Port = 80,
            PortOnvif = 80,
            Username = "admin",
            Password = "sensorway1"
        };
        IOnvifConnectionModel conn = new OnvifConnectionModel(connInfo);
        IOnvifClientFactory fac = new OnvifClientFactory(new DefaultBindingFactory());
        ILogService log = new LogService();      // 실 로그 구현 사용

        return new OnvifModelBuilder(conn, fac, connInfo, log);
    }

    /*───────────────────────────────────────────────────────────────────*\
     | 3. “빌드 성공” 테스트                                                |
    \*───────────────────────────────────────────────────────────────────*/
    [Fact(DisplayName = "Build 성공 → CameraStatus == AVAILABLE")]
    public async Task Build_SetsStatusAvailable()
    {
        // arrange
        var builder = CreateBuilder();

        // act
        // Syntax: await expr;      비동기 메서드 완료까지 호출자 일시 중단
        builder = await builder.WithDeviceInfoAsync();
        builder = await builder.WithCreateMediaAsync();
        builder = await builder.WithCreatePtzAsync();
        builder = await builder.WithCreateImageingAsync();
        builder = await builder.WithProfilesAsync();
        builder = await builder.WithPTZPresetsAsync();
        var model = builder.Build();      // 동기 Build

        // assert
        // Syntax: Assert.Equal(expected, actual);
        // Semantics: 두 값이 다르면 테스트 실패(예외)
        Assert.Equal(EnumCameraStatus.AVAILABLE, model.CameraStatus);
        Assert.True(model.IsDevicePossible);
        Assert.True(model.IsMediaPossible);
        Assert.True(model.IsPtzPossible);
    }

    /*───────────────────────────────────────────────────────────────────*\
     | 3. “관련 헬퍼” 테스트                                                |
    \*───────────────────────────────────────────────────────────────────*/
    [Fact(DisplayName = "Build 성공 → CameraStatus == AVAILABLE")]
    public async Task HelperTest()
    {
        // arrange
        var builder = CreateBuilder();

        // act
        // Syntax: await expr;      비동기 메서드 완료까지 호출자 일시 중단
        builder = await builder.WithDeviceInfoAsync();
        builder = await builder.WithCreateMediaAsync();
        var model = builder.Build();      // 동기 Build

        //builder.
    }

}