using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateFrameManager : MonoBehaviour, IDisposable
{
    private static UpdateFrameManager instance;

    public static UpdateFrameManager Instance => instance;

    private List<IUpdatable> updatableList;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Update()
    {
        if (updatableList == null) return;

        if (updatableList.Count > 0)
        {
            updatableList.ForEach(u =>
            {
                if (u == null) return;

                u.UpdateFrame();
            });
        }
    }

    public void AddUpdatable(IUpdatable updatable)
    {
        updatableList ??= new List<IUpdatable>();
        updatableList.Add(updatable);
    }

    public void RemoveUpdatable(IUpdatable updatable)
    {
        updatableList?.Remove(updatable);
    }

    void IDisposable.Dispose()
    {
        updatableList.Clear();
        updatableList = null;
    }
}
