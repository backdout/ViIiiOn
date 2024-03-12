using UnityEngine;
using System.Collections;
namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        public GameObject prefab;
        public int poolSize = 5;

        public bool inited = false;

        private Transform DefaultParent;
        public void setPool(Transform _DefaultParent)
        {
            DefaultParent = _DefaultParent;
            inited = true;
        }

        public virtual GameObject GetObject()
        {
            var item = ObjectPoolManager.Instance.doInstantiate(prefab);
            item.gameObject.transform.SetParent(DefaultParent);
            item.gameObject.SetActive(false);
            return item;
        }

        public virtual void ReturnObject(Transform go)
        {
            ObjectPoolManager.Instance.doDestroy(go.gameObject);
            go.SetParent(DefaultParent);
            go.gameObject.ResetTransform();
        }
    }
}
