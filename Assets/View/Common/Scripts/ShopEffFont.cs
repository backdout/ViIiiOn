using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System;


public class ShopEffFont : MonoBehaviour
{

    [SerializeField]
    GameObject[] TextGroup;
    [SerializeField]
    Text[] effText;
    [SerializeField]
    private CanvasGroup canvasGroup;
    private string number;
    int index;

    Vector2 startPos;
    Vector2 endPos;

    float startAlpha = 1f;
    float endAlpha = 0;
    float nowAlpha;
  
  

    GameObject Parent;

    bool isShow = false;

    float startTime;
   

    void init()
    {
        startTime = 0;
        isShow = false;
    }

    private void LateUpdate()
    {
        if (isShow)
        {
            if (Time.time - startTime > 0.15f)
            {

                if (effText != null)
                {
                    nowAlpha = Mathf.Lerp(startAlpha, endAlpha, ((Time.time - startTime) * 2.5f));
                    canvasGroup.alpha = nowAlpha;
                    
                }

            }


            if (Time.time - startTime > 0.4f)
            {
                isShow = false;
                ObjectPoolManager.Instance.doDestroy(gameObject);
                return;
            }

            if (Parent == null)
            {
                isShow = false;
                ObjectPoolManager.Instance.doDestroy(gameObject);
                return;
            }

            transform.localPosition = Vector3.Lerp(startPos, endPos, ((Time.time - startTime) * 2.5f));
        }
    }



    public void Show(bool isAdd, int _number, GameObject parent, float startX = 100f, float scale =1f )
    {
        init();

        Parent = parent;

        if (Parent != null)
        {
            gameObject.SetParent(Parent);
        }


        if (_number <= 0)
        {
            isShow = false;
            ObjectPoolManager.Instance.doDestroy(gameObject);
            return;
        }


        
        startPos = new Vector3(startX, -10f, 0);
        endPos = new Vector3(startX, 35f, 0);

        
        number = isAdd ? "+" + UIDefine.POINT_WITH_K_M(_number) : "-" + UIDefine.POINT_WITH_K_M(_number);
        index = isAdd ? 0 : 1;
        TextGroup[0].SetActive(isAdd);
        TextGroup[1].SetActive(!isAdd);
        effText[index].text = number;

  
        canvasGroup.alpha = 1;
        startAlpha = 1f;
        endAlpha = 0;


        transform.localScale = Vector3.one * scale;

        transform.localPosition = startPos;

        gameObject.SetActive(true);


        startTime = Time.time;

        isShow = true;
    }
  
   


}
