using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace LuaFramework {
    /// <summary>
    /// 对象池管理器，分普通类对象池+资源游戏对象池
    /// </summary>
    public class ObjectPoolManager : Manager {
        private Transform m_PoolRootObject = null;
        private Dictionary<string, PoolObject> m_GameObjectPools = new Dictionary<string, PoolObject>();

        Transform PoolRootObject {
            get {
                if (m_PoolRootObject == null) {
                    var objectPool = new GameObject("ObjectPool");
                    objectPool.transform.SetParent(transform);
                    objectPool.transform.localScale = Vector3.one;
                    objectPool.transform.localPosition = Vector3.zero;
                    m_PoolRootObject = objectPool.transform;
                }
                return m_PoolRootObject;
            }
        }

        public void CreatePool(string poolName, int initSize, int maxSize, GameObject prefab) {
            PoolObject pool = new PoolObject(poolName, prefab, initSize, maxSize, PoolRootObject);
            m_GameObjectPools[poolName] = pool;
        }

        public GameObject GetGameObject(string path) {
            if (!m_GameObjectPools.ContainsKey(path))
            {
                GameObject fatherGameobject = App.ResourceManager.LoadPrefab(path);
                CreatePool(path, 1, 5, fatherGameobject);
            }
            return m_GameObjectPools[path].GetObjectFromPool();
        }

        public void ReturnObjectToPool(string poolName, GameObject go) {
            if (m_GameObjectPools.ContainsKey(poolName)) {
                PoolObject pool = m_GameObjectPools[poolName];
                pool.ReturnObjectToPool(go);
            } else {
                Debug.LogWarning("No pool available with name: " + poolName);
            }
        }

        public void DestoryPool(string poolName, GameObject go)
        {
            if (m_GameObjectPools.ContainsKey(poolName))
            {
                PoolObject pool = m_GameObjectPools[poolName];
                pool.Destory(go);
                m_GameObjectPools[poolName] = null;
            }
            else
            {
                Debug.LogWarning("No pool destory with name: " + poolName);
            }
        }
    }

    public class PoolObject
    {
        private int maxSize;
        private string poolName;
        private Transform poolRoot;
        private GameObject poolObjectPrefab;
        private List<GameObject> unUseList;
        private List<GameObject> useList;
        public PoolObject(string poolName, GameObject poolObjectPrefab, int initCount, int maxSize, Transform pool)
        {
            unUseList = new List<GameObject>();
            useList = new List<GameObject>();
            this.poolName = poolName;
            this.maxSize = maxSize;
            this.poolRoot = pool;
            this.poolObjectPrefab = poolObjectPrefab;
            unUseList = new List<GameObject>();
            //populate the pool
            for (int i = 0; i < initCount; i++)
            {
                AddObjectToPool();
            }
        }

        private void AddObjectToPool()
        {
            GameObject temp = GameObject.Instantiate(poolObjectPrefab);
            temp.transform.SetParent(this.poolRoot);
            unUseList.Add(temp);
        }

        public void RemoveObjectFromPool(GameObject go)
        {
            if (unUseList.Contains(go))
            {
                unUseList.Remove(go);
            }
            else
            {
                useList.Remove(go);
            }
        }

        public GameObject GetObjectFromPool()
        {
            if(unUseList.Count <= 0)
            {
                AddObjectToPool();
            }
            GameObject temp = unUseList[0];
            unUseList.RemoveAt(0);
            useList.Add(temp);
            return temp;
        }

        public void ReturnObjectToPool(GameObject go)
        {
            if (useList.Contains(go))
            {
                useList.Remove(go);
            }
            go.transform.SetParent(this.poolRoot);
            unUseList.Add(go);
        }

        public void Destory(GameObject go)
        {
            useList.Clear();
            unUseList.Clear();
            useList = null;
            unUseList = null;
            poolObjectPrefab = null;
            poolRoot = null;
        }
    }
}