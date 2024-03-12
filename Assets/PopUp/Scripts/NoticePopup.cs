using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;



public class NoticePopup : UIPopup
{
    public static void SHOW(ButtonType type, string title, UnityAction yesAction, UnityAction noAction = null)
    {
        UIPopup popup = PopupManager.Show(PopupKind.Notice, null);
        if (popup != null)
        {
            ((NoticePopup)popup).SetData(type, title, yesAction, noAction);
        }
    }

    public void SetData(ButtonType type, string title, UnityAction yesAction, UnityAction noAction = null)
    {
        make(title);

        SetButtonType(type);

        YesFunc = yesAction;
        NoFunc = noAction;

    }

    protected void make(string title)
    {

        if (Content != null)
            Content.text = title;

        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        gameObject.SetActive(true);

        if (Board != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(Board);

    }


    public enum ButtonType : int
    {
        None,
        Ok,
        YesAndNo,
    }

    private ButtonType buttonType;


    public Image Back;
    public Button YesButton;
    public Button NoButton;
    public Button OkButton;


    public TextMeshProUGUI Content;
    public RectTransform Board;


    UnityAction YesFunc;
    UnityAction NoFunc;

    private void ResetButtons()
    {
        if (OkButton != null)
            OkButton.gameObject.SetActive(false);

        if (YesButton != null)
            YesButton.gameObject.SetActive(false);

        if (NoButton != null)
            NoButton.gameObject.SetActive(false);

    }

    public NoticePopup SetButtonType(ButtonType type)
    {
        ResetButtons();

        buttonType = type;


        switch (buttonType)
        {
            case ButtonType.Ok:
                OkButton.gameObject.SetActive(true);
                break;

            case ButtonType.YesAndNo:
                YesButton.gameObject.SetActive(true);
                NoButton.gameObject.SetActive(true);
                break;


            default:
                break;
        }

        return this;
    }


    public void OnClickYes()
    {
        YesFunc?.Invoke();
        base.Close();

    }
    public void OnClickNo()
    {
        NoFunc?.Invoke();
        base.Close();
    }


}
