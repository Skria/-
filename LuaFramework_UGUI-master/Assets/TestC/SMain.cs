using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        AddManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddManager()
    {
        App.Instance.AddManager<LuaManager>(ManagerName.Lua);
        //AppFacade.Instance.AddManager<PanelManager>(ManagerName.Panel);
        //AppFacade.Instance.AddManager<SoundManager>(ManagerName.Sound);
        //AppFacade.Instance.AddManager<TimerManager>(ManagerName.Timer);
        //AppFacade.Instance.AddManager<NetworkManager>(ManagerName.Network);
        //AppFacade.Instance.AddManager<ResourceManager>(ManagerName.Resource);
        //AppFacade.Instance.AddManager<ThreadManager>(ManagerName.Thread);
        //AppFacade.Instance.AddManager<ObjectPoolManager>(ManagerName.ObjectPool);
        //AppFacade.Instance.AddManager<GameManager>(ManagerName.Game);
    }
}
