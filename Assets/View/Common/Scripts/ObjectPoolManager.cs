

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Pool;


public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public bool isPooling = true;
    public bool IsInit = false;

    private Dictionary<int, ObjectPool<GameObject>> pools;
    private Dictionary<int, int> prefabs; // gameobject 에서 , prefab 찾기용


    private GameObject poolRootObject;

    ObjectPoolManager()
    {
        pools = new Dictionary<int, ObjectPool<GameObject>>();
        prefabs = new Dictionary<int, int>();
    }

    public void Init()
    {
        Dispose();

        if (poolRootObject == null)
        {
            poolRootObject = new GameObject("poolRootObject");
            poolRootObject.transform.SetParent(this.transform);
        }
        poolRootObject.SetActive(false);

        IsInit = true;
    }

    public GameObject doInstantiate(GameObject prefab)
    {
        if (pools.TryGetValue(prefab.GetInstanceID(), out ObjectPool<GameObject> op) == false)
        {
            op = new ObjectPool<GameObject>(() => ActionOnCreate(prefab), ActionOnGet, ActionOnRelease, ActionOnDestroy);

            pools.Add(prefab.GetInstanceID(), op);
        }

        GameObject go = op.Get();

        if (prefabs.ContainsKey(go.GetInstanceID()) == false)
            prefabs.Add(go.GetInstanceID(), prefab.GetInstanceID());

        return go;
    }

    public void doDestroy(GameObject go)
    {
        if (prefabs.TryGetValue(go.GetInstanceID(), out int prefab) && pools.TryGetValue(prefab, out ObjectPool<GameObject> op))
        {
            prefabs.Remove(go.GetInstanceID());

            op.Release(go);
        }
        else
        {
            Destroy(go);
        }
    }

    public void doDestroyImmediate(GameObject go)
    {
        doDestroy(go);
    }

    private GameObject ActionOnCreate(GameObject prefab)
    {
        return GameObject.Instantiate<GameObject>(prefab);
    }

    private void ActionOnGet(GameObject go)
    {
        go.transform.SetParent(null);
        go.SetActive(true);
    }

    private void ActionOnRelease(GameObject go)
    {
        if (poolRootObject == null && IsInit == false)
            Init();

        go.transform.SetParent(poolRootObject.transform);

        go.SetActive(false);
    }

    private void ActionOnDestroy(GameObject go)
    {
        Destroy(go);
    }

    public void Dispose()
    {
   
        foreach (var pool in pools)
        {
            pool.Value.Dispose();
        }

        pools.Clear();
        prefabs.Clear();
    }
}

