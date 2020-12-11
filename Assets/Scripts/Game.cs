using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// 게임의 시작 부분
    /// </summary>
    void Start()
    {
        
    }
}
