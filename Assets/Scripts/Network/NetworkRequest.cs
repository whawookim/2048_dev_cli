using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using UnityEngine;

public class NetworkRequest : CustomYieldInstruction
{
    public bool IsDone { get; private set; }
    public bool Ok { get; private set; }
    public ApiResponse Response { get; private set; }
    public Dictionary<string, object> Result => Response?.data;

    public NetworkRequest(Task<ApiResponse> task)
    {
        Run(task);
    }

    private async void Run(Task<ApiResponse> task)
    {
        try
        {
            Response = await task;
            Ok = Response?.ok == true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"API 호출 실패: {e.Message}");
            Ok = false;
        }
        IsDone = true;
    }

    public override bool keepWaiting => !IsDone;
}
