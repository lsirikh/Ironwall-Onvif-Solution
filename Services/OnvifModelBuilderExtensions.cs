using Ironwall.Dotnet.Libraries.OnvifSolution.Models;
using System;

namespace Ironwall.Dotnet.Libraries.OnvifSolution.Services;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 6/20/2025 10:37:53 AM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
/// <summary>
/// OnvifModelBuilder 체이닝을 간결하게 해 주는 async-fluent 헬퍼.
/// </summary>
public static class OnvifModelBuilderExtensions
{
    /*───────────────────────────────────────────
     * 단계 1) Task<Builder>  → 다음 async 단계
     *──────────────────────────────────────────*/
    public static async Task<OnvifModelBuilder> Do(
        this Task<OnvifModelBuilder> task,
        Func<OnvifModelBuilder, Task<OnvifModelBuilder>> next)
        => await next(await task);

    /*───────────────────────────────────────────
     * 단계 2) “이미 완료된” Builder → 다음 async 단계
     *──────────────────────────────────────────*/
    public static Task<OnvifModelBuilder> Do(
        this OnvifModelBuilder builder,
        Func<OnvifModelBuilder, Task<OnvifModelBuilder>> next)
        => next(builder);

    /*───────────────────────────────────────────
     * 단계 3)  Builder → 모델 꺼내기
     *──────────────────────────────────────────*/
    public static async Task<ICameraOnvifModel> BuildAsync(
        this Task<OnvifModelBuilder> task)
        => (await task).Build();          // Build() 호출 후 ICameraOnvifModel 반환
}