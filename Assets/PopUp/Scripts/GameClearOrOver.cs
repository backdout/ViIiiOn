using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameClearOrOver : MonoBehaviour
{
    [SerializeField]
    GameObject ClearText;
    [SerializeField]
    GameObject OverText;

    [SerializeField]
    GameObject NextBtn;
    [SerializeField]
    GameObject RetryBtn;
    [SerializeField]
    GameObject[] GemGroups;
    [SerializeField]
    TextMeshProUGUI[] GemTexts;
    public void SetData(bool isClear)
    {
        BattleSceneManager.Instance.battleManager.IsPush = true;
        ClearText.SetActive(isClear);
        NextBtn.SetActive(isClear);

        OverText.SetActive(!isClear);
        RetryBtn.SetActive(!isClear);
        if(isClear)
        {
            var GemCounts = BattleSceneManager.Instance.battleManager.GetGemCounts();
            for(int i = 0; i < GemCounts.Length; i++)
            {
                if(GemCounts[i] > 0)
                {
                    GemGroups[i].SetActive(true);
                    GemTexts[i].text = "X " + GemCounts[i].ToString();
                }
                else
                {
                    GemGroups[i].SetActive(false);
                }
            }
        }
    }


    public void OnClickNextStage()
    {
      
        BattleSceneManager.Instance.ReInit(UserData.Instance.LastEnterStage);
    }


    public void OnClickRetry()
    {
        BattleSceneManager.Instance.ReInit();
    }

    public void OnClickReturnHome()
    {
        BattleSceneManager.Instance.battleManager.IsPush = false;
        NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG("NOTICE_EXIT_STAGE"), () => ChangeScene());

    }

    private void ChangeScene()
    {
        LoadingScenePrefab.Instance.LoadScene("Main");
    }




}
