using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using UnityEngine;

public class NetworkRequest : CustomYieldInstruction
{
    /// <summary>
    /// 실제 서버 응답 객체
    /// </summary>
    public ApiResponse Response { get; private set; }
    /// <summary>
    /// 서버 응답 데이터(Dictionary), null 가능성 있음
    /// </summary>
    public Dictionary<string, object> Result => Response?.data;
    /// <summary>
    /// 요청이 성공했는지 여부 (ApiResponse.ok 기준)
    /// </summary>
    public bool Ok { get; private set; }
    /// <summary>
    /// 요청 완료 여부 (true면 응답 수신 완료)
    /// </summary>
    public bool IsDone { get; private set; }

    /// <summary>
    /// 생성자에서 ApiClient의 Task를 받아 실행
    /// </summary>
    public NetworkRequest(Task<ApiResponse> task)
    {
        Run(task);
    }

    /// <summary>
    /// 내부 Task 실행 및 결과 수신 처리
    /// </summary>
    private async void Run(Task<ApiResponse> task)
    {
        try
        {
            Response = await task;
            Ok = Response?.ok == true;
        }
        catch (System.Exception e)
        {
            MyDebug.LogError($"API Call Failed: {e.Message}");
            Ok = false;
        }
        IsDone = true;
    }

    /// <summary>
    /// Unity 코루틴에서 대기 조건으로 사용
    /// </summary>
    public override bool keepWaiting => !IsDone;
}
