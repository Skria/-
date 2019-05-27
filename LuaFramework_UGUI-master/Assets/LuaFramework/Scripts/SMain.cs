using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMain : MonoBehaviour
{

    public bool BundleMode = false;
    // Start is called before the first frame update
    void Awake()
    {
        AppConst.BundleMode = BundleMode;
        AddManager();
    }

    void AddManager()
    {
        App.Instance.AddManager<UpdateManager>(ManagerName.Update);
        App.Instance.AddManager<ResourceManager>(ManagerName.Resource);
        App.Instance.AddManager<ObjectPoolManager>(ManagerName.ObjectPool);
        StartCoroutine(TempC());

        //AppFacade.Instance.AddManager<SoundManager>(ManagerName.Sound);
        //AppFacade.Instance.AddManager<TimerManager>(ManagerName.Timer);
        //AppFacade.Instance.AddManager<NetworkManager>(ManagerName.Network);
        //AppFacade.Instance.AddManager<ResourceManager>(ManagerName.Resource);
        //AppFacade.Instance.AddManager<ThreadManager>(ManagerName.Thread);
        //AppFacade.Instance.AddManager<ObjectPoolManager>(ManagerName.ObjectPool);
        //AppFacade.Instance.AddManager<GameManager>(ManagerName.Game);
    }

    IEnumerator TempC()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Wait 5 s");
            if (App.ResourceManager.LoadFinish)
            {
                break;
            }
        }
        App.Instance.AddManager<UIManager>(ManagerName.UI);
        App.Instance.AddManager<LuaManager>(ManagerName.Lua);
    }
}
