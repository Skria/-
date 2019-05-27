using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : Manager
{
    string filePath = "StreamingAssets/files";
    public Dictionary<string, string> abPathDic;
    public override void Init()
    {
        abPathDic = new Dictionary<string, string>();
        abPathDic["uiprefeb_ingame.unity3d"] = "uiprefeb_ingame.unity3d";
        abPathDic["uiprefeb_lobby.unity3d"] = "uiprefeb_lobby.unity3d";
        abPathDic["StreamingAssets"] = "StreamingAssets";
        abPathDic["uiprefeb_login.unity3d"] = "uiprefeb_login.unity3d";
        abPathDic["art_heroicon.unity3d"] = "art_heroicon.unity3d";
        
    }
}
