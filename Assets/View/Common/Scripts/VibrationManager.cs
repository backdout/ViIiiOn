
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : Singleton<VibrationManager>
{
    enum DeviceKind
    {
        ANDROID, IOS, UNITY_EDITOR
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif
    private DeviceKind deviceKind = DeviceKind.UNITY_EDITOR;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        deviceKind = DeviceKind.ANDROID;
#endif
    }
    public void SetVibrate(long millisecconds)
    {
        Cancel();
        Vibrate(millisecconds);
    }
    public void SetVibrate(long FirMillisecconds, long SecMillisecconds)
    {
        long[] patternCont = { FirMillisecconds, 80, SecMillisecconds };
        Cancel();
        Vibrate(patternCont, -1);
    }
    private void Vibrate(long milliseconds)
    {
        if (deviceKind == DeviceKind.ANDROID)
            vibrator.Call("vibrate", milliseconds);
        else if (deviceKind == DeviceKind.IOS)
        {
            Handheld.Vibrate();
        }
    }
    private void Vibrate(long[] pattern, int repeat)
    {
        long[] initzero = { 0 };
        var list = new List<long>();
        list.AddRange(initzero);
        list.AddRange(pattern);
        long[] temp = list.ToArray();
        if (deviceKind == DeviceKind.ANDROID)
            vibrator.Call("vibrate", temp, repeat);
        else if (deviceKind == DeviceKind.IOS)
        {
            Handheld.Vibrate();
        }
    }
    private void Cancel()
    {
        if (deviceKind == DeviceKind.ANDROID)
            vibrator.Call("cancel");
    }
}