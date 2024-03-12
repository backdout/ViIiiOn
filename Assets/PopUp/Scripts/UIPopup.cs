using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIPopup : MonoBehaviour
{
    [Header("Event")]
    public UnityAction CloseAction = null;

    [Header("Variable")]
    protected PopupManager.popupData popupData;

    

    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {
        PopupManager.Instance.Close();
    }


    public void InitPopupData(PopupKind kind, string data)
    {
        popupData = new PopupManager.popupData(kind, data);
    }
    public void Close()
    {
        CloseAction?.Invoke();
        gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
