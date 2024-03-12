
using Google.Impl;
using UnityEngine;


public class DropItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer GemImg;
    [SerializeField]
    private Sprite[] GemSprites;

    public CanvasGroup canvasGroup;


    GameObject target;

    bool isPossibleMove = false;


    public int GemIndex { get; private set; }   

    private Monster mob;
    public void Show(Monster _mob)
    {
        mob = _mob;

     
        init();      
        gameObject.SetActive(true);

    }


    void init()
    {
        var battleManager = BattleSceneManager.Instance.battleManager;
        target = battleManager.Player.gameObject;

    
        isPossibleMove = false;      
        gameObject.SetParent(battleManager.dropItemGroup);
        GemIndex = GetGemIndex();
        GemImg.sprite = GemSprites[GemIndex];
        transform.localPosition = Vector3.zero;


        //몹 죽은 곳에 드랍 
        transform.SetPositionAndRotation(mob.transform.position, Quaternion.identity);
      
      
        isPossibleMove = false;
    }


    private void LateUpdate()
    {
        if (isPossibleMove)
        {
            // 일시정지 예외처리
            if (Time.timeScale == 0f)
                return;
            //startTime += Time.fixedDeltaTime;

            //if (startTime < 1)
            //    return;

          
            Vector2 dir = target.transform.position - transform.position;
            if (dir.sqrMagnitude <= 0.1)
            {
                BattleSceneManager.Instance.battleManager.AddGemCount(GemIndex);
                BattleSceneManager.Instance.battleManager.RemoveDropItemList(this);
                SoundManager.Instance.PlayEffectAudioClip(SoundManager.EffSoundKind.GetGem);
                isPossibleMove = false;
                ObjectPoolManager.Instance.doDestroy(gameObject);
                return;
            }

            Vector2 moveVec = dir.normalized * 8.0f * Time.fixedDeltaTime;
            transform.position = new Vector3(transform.position.x + moveVec.x, transform.position.y + moveVec.y);
        }
    }


    public void SetDropItemMove(bool isMove)
    {

        isPossibleMove = isMove;

    }


    public void DeleteDropItem()
    {
        gameObject.SetActive(false);
        ObjectPoolManager.Instance.doDestroy(gameObject);
    }


    private int GetGemIndex()
    {
        //아이템 별로 각기 확률이 다른게 아니라 단순 젬만 희소성에 따라 확률을 정한거라 단순하게 처리 
        int Max = 0;
        for (int i = 0; i < mob.DropRateGem.Length; i++)
            Max += mob.DropRateGem[i];

        int value = Random.Range(0, Max+1);

        if (mob.DropRateGem[2] > value)
            return 2;

        if (mob.DropRateGem[1] > value)
            return 1;

            return 0;
    }




}
