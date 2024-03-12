using UnityEngine;
using UnityEngine.UI;

public class DamageFontHandler : MonoBehaviour
{

    public RectTransform[] TextGroup; //   NormalDamage = 0, CriticalDamage = 1, Heal = 2,
    public Text[] NumText; //   NormalDamage = 0, CriticalDamage = 1 , Heal = 2,
    public CanvasGroup canvasGroup;
    Vector3 randomPos;

    GameObject target;

    bool isShow = false;
    float startTime;


    float startAlpha = 1f;
    private bool isDamage;
    float endAlpha = 0;
    float nowAlpha;
    float AddPosY = 135;
    private Vector3 AddPos;

    int index;
    private Canvas Canvas;
    GameObject parent;
    Vector3 maxScale;
    Vector3 minScale;


    bool isFirst;
    void init()
    {
 
        startTime = 0;
        isShow = false;
        parent = BattleSceneManager.Instance.battleUI.DamageGroup;
        gameObject.SetParent(parent);
        AddPosY = 0;
    }

    float beforeTime;


    float playTime;
    float midPlayTime;
    float multiValue;
    float addPosValue;

    private void LateUpdate()
    {

        if (isShow)
        {
            // 일시정지 예외처리
            if (beforeTime != Time.time)
                AddPosY += addPosValue;

            if (isFirst)
            {
                isFirst = false;
            }

           if (Time.time - startTime > midPlayTime)
            {
                nowAlpha = Mathf.Lerp(startAlpha, endAlpha, ((Time.time - startTime - midPlayTime) * multiValue));
                canvasGroup.alpha = nowAlpha;

                if (!isDamage && addPosValue != 0.6f)
                    addPosValue = 0.6f;

                if (Time.time - startTime > playTime)
                {
                    isShow = false;
                    ObjectPoolManager.Instance.doDestroy(gameObject);
                    return;
                }
            }
            else
            {
                GetComponent<RectTransform>().localScale = Vector3.Lerp(minScale, maxScale, ((Time.time - startTime) * multiValue));
            }

            if (target == null)
            {
                isShow = false;
                ObjectPoolManager.Instance.doDestroy(gameObject);
                return;
            }

            //transform.localPosition = GetPos();
            transform.position = GetPos();

            beforeTime = Time.time;
        }
    }

    void setRandomPos()
    {
        randomPos = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0f, 0.3f));
    }


    private Vector2 GetPos()
    {
     
     
        Vector2 uiPos;
        if (isDamage)
        {
            // 몹 위치에 따라 출력
            uiPos = Camera.main.WorldToScreenPoint(target.transform.position + randomPos);        
        }
        else
        {
            // HP 바의 위치는 고정임으로, 힐텍스트 위치는 고정된 위치에서 이동
            uiPos = new Vector3(0, 10);
        }
        uiPos.y += AddPosY;
       
        return uiPos;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="number_">대미지</param>
    /// <param name="_target">출력될 위치</param>
    /// <param name="isDamage">힐/ 데미지 구분</param>
    /// <param name="isCri"> 크리티컬 여부 구분-> 즉사의 경우, 크리티컬</param>
    public void Show(int number_, GameObject _target, bool isDamage, bool isCri = false)
    {
        this.isDamage = isDamage;

        target = _target;

        init();
        //gameObject.SetParent(FactoryManager.Instance.battleUI.DamageTextGroup);
        setRandomPos();

        index = number_ == 0? 3: isDamage == false ? 2 : isCri ? 1 : 0;
        if (index < 3)
            NumText[index].text = isDamage ? UIDefine.POINT_WITH_K_M(number_, false) : "+" + UIDefine.POINT_WITH_K_M(number_, false);

        for (int i = 0; i < NumText.Length; i++)
            TextGroup[i].gameObject.SetActive(i == index);


        maxScale = isDamage == false ? new Vector3(1f, 1f, 1f) : new Vector3(0.89f, 0.89f, 0.89f);
        minScale = isDamage ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0f, 0f, 0f);

        playTime = isDamage ? 0.8f : 0.7f;
        midPlayTime = isDamage ? 0.4f : 0.35f;
        multiValue = isDamage ? 2.5f : 2.857f;
        addPosValue = isDamage ? 0.8f : 0.05f;

        var rect = GetComponent<RectTransform>();
        rect.localScale = minScale;
        rect.pivot = isDamage ? new Vector2(0.5f, 0.5f) : new Vector2(0.5f, 0f);

        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        canvasGroup.alpha = 1;

       // transform.localPosition = GetPos();

        startTime = Time.time;
        isFirst = true;
        isShow = true;

        gameObject.SetActive(true);


    }
}
