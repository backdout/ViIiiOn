using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageView : MonoBehaviour
{

    // [SerializeField] private  StageChaters;

    public LoopVerticalScrollRect VerticalScrollRect;

    public LayoutElement StageLayoutElement;
    public SafeArea safeArea;
    // 마지막 플레이한 스테이지 체크해서 스크롤 이동 시켜 놓기
    // 스크롤 렉트 일단 일반으로 함. 추후 생각해보고 루프로 변경(왜냐면, 현재는 3챕터만 구현되어 있음.
    public void Awake()
    {
       // StageLayoutElement.preferredHeight = SafeArea.SAFE_AREA_HEIGHT;
    }

    public void Init()
    {
        List<StageChapter.StageChapterData> list = new List<StageChapter.StageChapterData>();
      
        var data = UserData.Instance.StageClearData;
       
       
       foreach(var item in data)
       {
            var stageChapterData = new StageChapter.StageChapterData();
            stageChapterData._chapterNum = item.Key;
            stageChapterData.hasClearList = item.Value;
            stageChapterData.hasBeforeChapterClear = item.Key <= 1 ? true : data[item.Key - 1].LastOrDefault();
          
            list.Add(stageChapterData);
       }
       // 역순으로 가야 함. 
        list.Sort((x, y) => y._chapterNum.CompareTo(x._chapterNum));
        VerticalScrollRect.init(list.ToArray());
        //최초 진입은 맨마지막, 클리어 스테이지 있으면, 해당 챕터로 이동 
        VerticalScrollRect.verticalNormalizedPosition = 1f;
    }


}
