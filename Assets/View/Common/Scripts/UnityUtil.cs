using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public static class UnityUtil
{

    public static string GetRoutineName<T>(string event_name, T state)
    {
        return string.Format("{0}{1}", event_name, state.ToString());
    }

    public static void GoCoroutine<T1, T2, T3>(T1 obj, string event_name, T2 state, T3 param)
    {
        string methodName = GetRoutineName(event_name, state);


        MethodInfo methodInfo = typeof(T1).GetMethod(methodName);
        if (null == methodInfo) return;

        MonoBehaviour mono = obj as MonoBehaviour;
        mono.StartCoroutine(methodName, param);
    }

    public static Quaternion LookRotation(GameObject owner, GameObject target)
    {
        return LookRotation(owner.transform.position, target.transform.position);
    }

    public static Quaternion LookRotation(Vector3 ownerPos, Vector3 targetPos)
    {
        return Quaternion.LookRotation(targetPos - ownerPos);
    }


    public static Transform FindChildren(this Transform transform, string name)
    {
        Transform[] list = transform.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray();
        if (list.Length > 0)
        {
            return list[0];
        }

        return null;
    }


    /*
        public static bool isBackKey()
        {
    #if UNITY_EDITOR

            if (Keyboard.current[Key.Escape].wasReleasedThisFrame)
    #else
                if (Application.platform == RuntimePlatform.Android && Keyboard.current[Key.Escape].wasReleasedThisFrame)
    #endif
            {
                return true;
            }

            return false;
        }

        public static bool isUKey()
        {
    #if UNITY_STANDALONE
            return Keyboard.current[Key.U].wasReleasedThisFrame;
    #else
                 return false;         
    #endif
        }

        public static bool isAKey()
        {
    #if UNITY_STANDALONE
            return Keyboard.current[Key.A].wasReleasedThisFrame;
    #else
                return false;

    #endif
        }

        public static bool isSpaceKey()
        {
    #if UNITY_STANDALONE
            return Keyboard.current[Key.Space].wasReleasedThisFrame;
    #else
                return false;
    #endif
        }

        public static bool isRKey()
        {
    #if UNITY_STANDALONE
            return Keyboard.current[Key.R].wasReleasedThisFrame;
    #else
                return false;
    #endif
        }

    */

    public static void Quit()
    {


        // https://forum.unity.com/threads/solved-android-2018-3-13-unitypurchasing-dont-initialize-after-application-quit.665497/

        //#if UNITY_ANDROID
        //            AndroidJavaClass ajc = new AndroidJavaClass("com.lancekun.quit_helper.AN_QuitHelper");
        //            AndroidJavaObject UnityInstance = ajc.CallStatic<AndroidJavaObject>("Instance");
        //            UnityInstance.Call("AN_Exit");
        //#else
        //            Application.Quit();
        //#endif

        Application.Quit();

    }
}