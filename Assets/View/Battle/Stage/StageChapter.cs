using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class StageChapter : MonoBehaviour
{
    /// <summary>
    ///  �������� ���� �������� é�� ��� ������
    /// </summary>

    public class StageChapterData
    {
        public int _chapterNum;
        public List<bool> hasClearList;
        public bool hasBeforeChapterClear;
    }

    [Serializable]
    public class StageBtn
    {
        bool hasClear;
        bool canPlay;
        int stageNum;
        public TextMeshProUGUI StageNum;
        public GameObject ClearIcon;
        public Button button;
        public void SetClear(bool clear)
        {
            hasClear = clear;
            ClearIcon.SetActive(clear);
          
        }

        public void SetData(int _stageNum, bool clear, bool hasBeforeChapterClear)
        {
            stageNum = _stageNum;
            hasClear = clear;
            ClearIcon.SetActive(clear);

            canPlay = stageNum == 1 || hasBeforeChapterClear;
            StageNum.text = stageNum.ToString();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }


        public void OnClick()
        {

            if (canPlay || stageNum == 1)
            {
                //  ����ȯ
                UserData.Instance.LastEnterStage = stageNum;
                NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG("NOTICE_ENTER_STAGE"), () => ChangeScene());

            }
            else
            {
                /// �ȳ� �˾� 

                NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG("NOTICE_NOT_ENOUGHT_STAGE_CLEAR"), null);
            }
        }


        private void ChangeScene()
        {
            if (UserData.Instance.Ticket > 0)
            {
                LoadingScenePrefab.Instance.LoadScene("Battle");
                UserData.Instance.Ticket -= 1;
            }
            else
                NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG("NOTICE_NOT_ENOUGHT_TICKET"), null);

            


        }
    }


    private int chapterNum;
    public List<StageBtn> buttons;
    const string backgroundName = "StageBack";
    public Image Background;
    public void SetData(StageChapterData data)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == 0)
                buttons[i].SetData((data._chapterNum - 1) * 5 + i + 1, data.hasClearList[i], data.hasBeforeChapterClear);
            else
                buttons[i].SetData((data._chapterNum - 1) * 5 + i + 1, data.hasClearList[i], data.hasClearList[i - 1]);
        }

        chapterNum = data._chapterNum;
        Background.sprite = Resources.Load<Sprite>("Background/" + backgroundName + chapterNum);
    }
}
