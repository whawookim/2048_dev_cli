using System;
using UnityEngine;

/// <summary>
/// 게임의 로비 메인을 관리하는 메인 UI
/// </summary>
public class LobbyMain : MonoBehaviour
{
    public static LobbyMain Instance { get; set; }
	
	void Awake()
	{
		Debug.Assert(Instance != null);
		
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
		
		Debug.Assert(Instance == null);
	}

	// Start is called before the first frame update
    void Start()
    {
        Debug.Log("Enter Lobby");
    }

	private void OnDisable()
	{
		Debug.Log("Close Lobby");
	}
}
