using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using LuaInterface;
using UObject = UnityEngine.Object;

public class AssetBundleInfo {
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount;

    public AssetBundleInfo(AssetBundle assetBundle) {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 0;
    }
}

namespace LuaFramework {

    public class ResourceManager : Manager {
        string m_BaseDownloadingURL = "";
        string[] m_AllManifest = null;
        public bool LoadFinish = false;
        AssetBundleManifest m_AssetBundleManifest = null;
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
        Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();
        Dictionary<string, UObject> m_LoadUObject = new Dictionary<string, UObject>();
        class LoadAssetRequest {
            public Type assetType;
            public string assetNames;
            public LuaFunction luaFunc;
            public Action<UObject> sharpFunc;
        }

        public override void Init()
        {

            Initialize("StreamingAssets", delegate ()
            {

            });
        }


        // Load AssetBundleManifest.
        public void Initialize(string manifestName, Action initOK) {
            m_BaseDownloadingURL = Util.GetRelativePath();
            if (AppConst.BundleMode)
            {
                LoadAssetAsync<AssetBundleManifest>("StreamingAssets", delegate (UObject objs) {
                    m_AssetBundleManifest = objs as AssetBundleManifest;
                    m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
                    LoadFinish = true;
                    if (initOK != null) initOK();
                });
            }
            else
            {
                LoadFinish = true;
            }
           
        }
        [NoToLua]
        public void LoadPrefabAsync(string path, Action<UObject> func) {
            LoadAssetAsync<GameObject>(path, func);
        }

        [NoToLua]
        public GameObject LoadPrefab(string path)
        {
            return LoadAsset<GameObject>(path) as GameObject;
        }

        [NoToLua]
        public void LoadMaterialAsync(string path, Action<UObject> func)
        {
            LoadAssetAsync<Material>(path, func);
        }

        [NoToLua]
        public void LoadTextureAsync(string path, Action<UObject> func)
        {
            LoadAssetAsync<Texture>(path, func);
        }

        [NoToLua]
        public void LoadShaderAsync(string path, Action<UObject> func)
        {
            LoadAssetAsync<Shader>(path, func);
        }

        //public void LoadPrefabAsync(string abName, string assetNames, LuaFunction func) {
        //    LoadAsset<GameObject>(abName, assetNames, null, func);
        //}

        string GetRealAssetPath(string abName) {
            if (abName.Equals(AppConst.AssetDir)) {
                return abName;
            }
            abName = abName.ToLower();
            if (!abName.EndsWith(AppConst.ExtName)) {
                abName += AppConst.ExtName;
            }
            if (abName.Contains("/")) {
                return abName;
            }
            //string[] paths = m_AssetBundleManifest.GetAllAssetBundles();  产生GC，需要缓存结果
            for (int i = 0; i < m_AllManifest.Length; i++) {
                int index = m_AllManifest[i].LastIndexOf('/');  
                string path = m_AllManifest[i].Remove(0, index + 1);    //字符串操作函数都会产生GC
                if (path.Equals(abName)) {
                    return m_AllManifest[i];
                }
            }
            Debug.LogError("GetRealAssetPath Error:>>" + abName);
            return null;
        }

        /// <summary>
        /// 同步载入素材
        /// </summary>
        UObject LoadAsset<T>(string path) where T : UObject
        {
            int index = path.LastIndexOf("\\");
            string abName = Util.GetABNameByPath(path.Substring(0,index));
            string assetNames = Util.GetAssetNameByPath(path);
            UObject tempObj = null;
            if (m_LoadUObject.TryGetValue(abName, out tempObj))
            {
                return tempObj;
            }

            if (AppConst.BundleMode)
            {
                abName = GetRealAssetPath(abName);
                AssetBundleInfo temp = null;
                if (m_LoadedAssetBundles.TryGetValue(abName, out temp))
                {
                    return temp.m_AssetBundle.LoadAsset(assetNames) as T;
                }
                else
                {
                    OnLoadAssetBundle<T>(abName);
                    if (m_LoadedAssetBundles.TryGetValue(abName, out temp))
                    {
                        return temp.m_AssetBundle.LoadAsset(assetNames) as T;
                    }
                    return null;
                }
            }
            else
            {
                Debug.Log(path);
                return Resources.Load(path);
            }
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        void LoadAssetAsync<T>(string path, Action<UObject> action = null, LuaFunction func = null) where T : UObject {
            UObject temp = null;
            string abName = Util.GetABNameByPath(path);
            string assetNames = Util.GetAssetNameByPath(path);
            if (m_LoadUObject.TryGetValue(abName,out temp))
            {
                if (action != null)
                {
                    action(temp);
                }
                if (func != null)
                {
                    func.Call((object)temp);
                    func.Dispose();
                }
            }
            else
            {
                abName = GetRealAssetPath(abName);
                LoadAssetRequest request = new LoadAssetRequest();
                request.assetType = typeof(T);
                request.assetNames = assetNames;
                request.luaFunc = func;
                request.sharpFunc = action;

                List<LoadAssetRequest> requests = null;
                if (!m_LoadRequests.TryGetValue(abName, out requests))
                {
                    requests = new List<LoadAssetRequest>();
                    requests.Add(request);
                    m_LoadRequests.Add(abName, requests);
                    StartCoroutine(OnLoadAssetAsync<T>(abName));
                }
                else
                {
                    requests.Add(request);
                }
            }
        }

        public void OnLoadAssetBundle<T>(string abName) where T : UObject
        {
            Type type = typeof(T);
            AssetBundle assetObj = null;
            AssetBundleCreateRequest download = null;
            if (type == typeof(AssetBundleManifest))
                download = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + abName);// App.UpdateManager.abPathDic[abName]);
            else
            {
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
                if (dependencies.Length > 0)
                {
                    m_Dependencies.Add(abName, dependencies);
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        string depName = dependencies[i];
                        AssetBundleInfo bundleInfo = null;
                        if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
                        {
                            bundleInfo.m_ReferencedCount++;
                        }
                        else if (!m_LoadRequests.ContainsKey(depName))
                        {
                           OnLoadAssetBundle<T>(depName);
                        }
                    }
                }
                assetObj = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abName); // App.UpdateManager.abPathDic[abName]);
            }

            if (assetObj != null)
            {
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        IEnumerator OnLoadAssetAsync<T>(string abName) where T : UObject {
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null) {
                yield return StartCoroutine(OnLoadAssetBundleAsync(abName, typeof(T)));

                bundleInfo = GetLoadedAssetBundle(abName);
                if (bundleInfo == null) {
                    m_LoadRequests.Remove(abName);
                    Debug.LogError("OnLoadAsset--->>>" + abName);
                    yield break;
                }
            }
            List<LoadAssetRequest> list = null;
            if (!m_LoadRequests.TryGetValue(abName, out list)) {
                m_LoadRequests.Remove(abName);
                yield break;
            }
            for (int i = 0; i < list.Count; i++) {
                string assetNames = list[i].assetNames;

                AssetBundle ab = bundleInfo.m_AssetBundle;
                string assetPath = assetNames;
                AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
                yield return request;
                UObject result = request.asset;
                m_LoadUObject.Add(assetNames, result);
                    //T assetObj = ab.LoadAsset<T>(assetPath);
                    //result.Add(assetObj);
                if (list[i].sharpFunc != null) {
                    list[i].sharpFunc(result);
                    list[i].sharpFunc = null;
                }
                if (list[i].luaFunc != null) {
                    list[i].luaFunc.Call((object)result);
                    list[i].luaFunc.Dispose();
                    list[i].luaFunc = null;
                }
                bundleInfo.m_ReferencedCount++;
            }
            m_LoadRequests.Remove(abName);
        }

        IEnumerator OnLoadAssetBundleAsync(string abName, Type type) {

            AssetBundleCreateRequest download = null;
            if (type == typeof(AssetBundleManifest))
                download = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + abName);  //App.UpdateManager.abPathDic[abName]);
            else
            {
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
                if (dependencies.Length > 0)
                {
                    m_Dependencies.Add(abName, dependencies);
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        string depName = dependencies[i];
                        AssetBundleInfo bundleInfo = null;
                        if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
                        {
                            bundleInfo.m_ReferencedCount++;
                        }
                        else if (!m_LoadRequests.ContainsKey(depName))
                        {
                            yield return StartCoroutine(OnLoadAssetBundleAsync(depName, type));
                        }
                    }
                }
                download = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + abName);  //App.UpdateManager.abPathDic[abName]);
            }

            AssetBundle assetObj = download.assetBundle;
            if (assetObj != null) {
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName) {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies) {
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null) return null;
            }
            return bundle;
        }

        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false) {
            abName = GetRealAssetPath(abName);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        void UnloadDependencies(string abName, bool isThorough) {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies) {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        void UnloadAssetBundleInternal(string abName, bool isThorough) {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;

            if (--bundle.m_ReferencedCount <= 0) {
                if (m_LoadRequests.ContainsKey(abName)) {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.m_AssetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                Debug.Log(abName + " has been unloaded successfully");
            }
        }

        public AssetBundle LoadLuaAssetBundleSync<T>(string abName)
        {
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null)
            {
                OnLoadAssetBundle<AssetBundle>(abName);
                if (m_LoadedAssetBundles.TryGetValue(abName, out bundleInfo))
                {
                    return bundleInfo.m_AssetBundle;
                }
                return null;
            }
            else
            {
                return bundleInfo.m_AssetBundle;
            }
        }

    }
}
