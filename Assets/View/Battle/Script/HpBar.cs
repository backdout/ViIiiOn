using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{

    [SerializeField]
    private Sprite[] hpbarSprites;
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private Image hpBarAlpha;
    [SerializeField]
    private CanvasGroup alpha;

    Coroutine CoHPSize;
    Coroutine CoHpAlpha;
    private float beforefillAmount;

    private RectTransform rect;
    private Vector3 AddPos = Vector2.up* 135;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        rect.position = Camera.main.WorldToScreenPoint(BattleSceneManager.Instance.battleManager.Player.transform.position) + AddPos;
  
    }


    public void SetHPBar(float curhp, float maxHp)
    {
        int index = GetHpColorIndex(curhp, maxHp);
        hpBar.sprite = hpbarSprites[index];
      
        hpBar.fillAmount = curhp / maxHp;
        beforefillAmount = hpBar.fillAmount;
        alpha.alpha = 1;
    }


    public void UpdateHP(float curhp, float maxHp)
    {
        int index = GetHpColorIndex(curhp, maxHp);
        hpBar.sprite = hpbarSprites[index];

        hpBar.fillAmount = curhp / maxHp;


        if (CoHPSize != null)
            StopCoroutine(CoHPSize);

        CoHPSize = StartCoroutine(PlayChangeSize(beforefillAmount));



        if(IsWarnnig())
        {
            if (CoHpAlpha != null)
                StopCoroutine(CoHpAlpha);

            CoHpAlpha = StartCoroutine(PlayWarnnig());
        }
        else
        {
            if (CoHpAlpha != null)
                StopCoroutine(CoHpAlpha);

            alpha.alpha = 1;
        }
   
    }

    private IEnumerator PlayChangeSize(float startSize)
    {
        hpBarAlpha.fillAmount = startSize;

        float size = startSize;

        while (hpBar.fillAmount <= hpBarAlpha.fillAmount)
        {
            size -= Time.deltaTime * 0.2f;
            hpBarAlpha.fillAmount = size;

            yield return null;
        }

        CoHPSize = null;
        beforefillAmount = hpBar.fillAmount;
    }




    private int GetHpColorIndex(float curhp, float basehp)
    {
        float per = (curhp / basehp) * 100;

        if (per >= 30.0f)
            return 0;
 
        return 1;
    }


    private bool IsWarnnig()
    {
        return hpBar.fillAmount <= 0.1f;
    }

    private IEnumerator PlayWarnnig()
    {
      
        float time = 0; ;
        bool isOn = false;
        while (hpBar.fillAmount <= 0.1f)
        {
            if (isOn)
            {              
                time += Time.deltaTime;
             
                if (time >= 1.0)
                    isOn = false;
            }
            else 
            {
               
                time -= Time.deltaTime;
               
                if (time <= 0.0)
                    isOn = true;
            }

            alpha.alpha = time;

            yield return null;
        }

    }


}
