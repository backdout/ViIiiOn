using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

using System;
using System.Reflection;
using System.Collections.Generic;

public static class GameObjectExtensions
{


    static public void ApplyComponent<T>(this GameObject obj, ref T componentValue) where T : Component
    {

        if (null != componentValue) return;

        T component = obj.GetComponent<T>();
        if (null != component) componentValue = component;

    }

    static public void ApplyComponentFromChildren<T>(this GameObject obj, ref T componentValue_, bool deepSearch_ = false) where T : Component
    {

        if (null != componentValue_) return;

        if (false == deepSearch_)
        {

            foreach (Transform childTransform in obj.transform)
            {
                if (!childTransform.gameObject.IsExistComponent<T>()) continue;
                componentValue_ = childTransform.gameObject.GetComponent<T>();
            }
        }
        else
        {
            componentValue_ = obj.FindInChildren<T>();
        }

    }

    static public bool IsExistComponent<T>(this GameObject obj)
    {
        T component = obj.GetComponent<T>();
        return (null != component);
    }

    static public void SetParent(this GameObject obj_, GameObject parent_, bool worldPositionStays_ = true)
    {
        obj_.transform.SetParent(parent_.transform, worldPositionStays_);
    }

    static public GameObject InstantiatePrefab(this GameObject destObj, GameObject prefab)
    {
        GameObject newObj = UnityEngine.Object.Instantiate(prefab, destObj.transform.position, destObj.transform.rotation) as GameObject;
        newObj.ApplyTransform(prefab);
        return newObj;
    }




    static public Vector3 GetMeshSize(this GameObject go)
    {
        if (null == go) return Vector3.zero;

        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (null == mf) return Vector3.zero;

        Mesh mesh = mf.sharedMesh;
        if (null == mesh) return Vector3.zero;

        return new Vector3(
            mesh.bounds.size.x * go.transform.localScale.x
            , mesh.bounds.size.y * go.transform.localScale.y
            , mesh.bounds.size.z * go.transform.localScale.z
        );
    }

    static public void ApplyRectTransform(this GameObject destObj, GameObject sourceObj)
    {

        RectTransform destRectTransform = destObj.GetComponent<RectTransform>();
        RectTransform sourceRectTransform = sourceObj.GetComponent<RectTransform>();

        destRectTransform.localPosition = sourceRectTransform.localPosition;
        destRectTransform.localScale = sourceRectTransform.localScale;
        destRectTransform.localRotation = sourceRectTransform.localRotation;

        destRectTransform.anchorMin = sourceRectTransform.anchorMin;
        destRectTransform.anchorMax = sourceRectTransform.anchorMax;
        destRectTransform.anchoredPosition = sourceRectTransform.anchoredPosition;
        destRectTransform.sizeDelta = sourceRectTransform.sizeDelta;
        destRectTransform.pivot = sourceRectTransform.pivot;
    }

    static public void ApplyTransform(this GameObject destObj, GameObject sourceObj)
    {
        destObj.transform.localPosition = sourceObj.transform.localPosition;
        destObj.transform.localRotation = sourceObj.transform.localRotation;
        destObj.transform.localScale = sourceObj.transform.localScale;
    }

    static public void SetTransform(this GameObject destObj, GameObject sourceObj)
    {
        destObj.transform.position = sourceObj.transform.position;
        destObj.transform.rotation = sourceObj.transform.rotation;
    }

    static public void AddTransform(this GameObject destObj, GameObject sourceObj)
    {
        destObj.transform.localPosition += sourceObj.transform.localPosition;
        destObj.transform.localRotation = destObj.transform.localRotation
        * Quaternion.Euler(sourceObj.transform.localRotation.x, sourceObj.transform.localRotation.y, sourceObj.transform.localRotation.z);
        destObj.transform.localScale += sourceObj.transform.localScale;
    }

    static public void ResetTransform(this GameObject go)
    {
        if (null == go) return;

        go.transform.localRotation = Quaternion.identity;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
    }

    static public T FindInParents<T>(this GameObject go) where T : Component
    {
        if (null == go) return null;
        var comp = go.GetComponent<T>();

        if (null != comp) return comp;

        Transform t = go.transform.parent;

        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }

        return comp;
    }

    static public T FindInParentsWithName<T>(this GameObject go, string withName) where T : Component
    {

        if (null == go) return null;
        var comp = go.GetComponent<T>();

        if (null != comp) return comp;

        Transform t = go.transform.parent;

        while (t != null && comp == null && comp.gameObject.name == withName)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }

        return comp;
    }

    static public T FindInChildren<T>(this GameObject go) where T : Component
    {

        if (null == go) return null;

        var comp = go.GetComponent<T>();

        if (null != comp) return comp;

        foreach (Transform trans in go.transform)
        {
            T t = trans.gameObject.FindInChildren<T>();
            if (null != t) return t;
        }

        return null;
    }

    static public T FindInChildrenWithName<T>(this GameObject go, string withName) where T : Component
    {

        GameObject child = go.FindInChildrenWithName(withName);
        return child.GetComponent<T>();
    }

    static public GameObject FindInChildrenWithName(this GameObject go, string withName)
    {

        if (null == go) return null;

        //Debug.Log( string.Format( "{0}, {1}", go.name, withName ) );			
        if (go.name.Equals(withName)) return go;

        foreach (Transform trans in go.transform)
        {
            GameObject findObj = trans.gameObject.FindInChildrenWithName(withName);
            if (null != findObj) return findObj;
        }

        return null;
    }
    /*
			static public GameObject FindInChildrenWithName( this GameObject go, string withName, bool deepSearch_=false ) {

				if (!deepSearch_) {

					Transform[] ts = go.GetComponentsInChildren<Transform>();

					foreach (Transform t in go) {
						if (t.gameObject.name != withName) continue;
						return t.gameObject;
					}
				}
				else {
					go.FindInChildrenWithName( withName );
				}

				return null;
			}
	*/
}