
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;



public class ShopNoticePopup : UIPopup
{
    public static void SHOW(string title, int maxCount, UnityAction<int> yesAction, UnityAction noAction = null)
    {
        UIPopup popup = PopupManager.Show(PopupKind.ShopNotice, null);
        if (popup != null)
        {
            ((ShopNoticePopup)popup).SetData(title, maxCount, yesAction, noAction);
        }
    }

    public void SetData( string title, int _maxCount, UnityAction<int> yesAction, UnityAction noAction = null)
    {
        make(title);

        maxCount = _maxCount;
        YesFunc = yesAction;
        NoFunc = noAction;

    }

    protected void make(string title)
    {

        if (Content != null)
            Content.text = title;


        CountText.text = "1";
        count = 1;
        transform.localPosition = new Vector3(0f, 0f, 0f);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        gameObject.SetActive(true);

        if (Board != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(Board);

    }



    public Image Back;
    public Button YesButton;
    public Button NoButton;


    
    public TextMeshProUGUI Content;

    public TextMeshProUGUI CountText;

    public RectTransform Board;

    bool isBuy;
    private int count;
    private int maxCount;
    UnityAction<int> YesFunc;
    UnityAction NoFunc;

    public void OnclickPlus()
    {
        if (count < maxCount)
            count++;

        CountText.text = count.ToString();
    }
    public void OnclickMinus()
    {
        if (count > 1)
            count--;

        CountText.text = count.ToString();
    }
    public void OnclickMax()
    {
        count = maxCount;
        CountText.text = maxCount.ToString();
    }


    public void OnClickYes()
    {
        YesFunc?.Invoke(count);
        base.Close();

    }
    public void OnClickNo()
    {
        NoFunc?.Invoke();
        base.Close();
    }


}
