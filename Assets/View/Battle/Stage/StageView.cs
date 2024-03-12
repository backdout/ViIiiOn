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
    // ������ �÷����� �������� üũ�ؼ� ��ũ�� �̵� ���� ����
    // ��ũ�� ��Ʈ �ϴ� �Ϲ����� ��. ���� �����غ��� ������ ����(�ֳĸ�, ����� 3é�͸� �����Ǿ� ����.
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
       // �������� ���� ��. 
        list.Sort((x, y) => y._chapterNum.CompareTo(x._chapterNum));
        VerticalScrollRect.init(list.ToArray());
        //���� ������ �Ǹ�����, Ŭ���� �������� ������, �ش� é�ͷ� �̵� 
        VerticalScrollRect.verticalNormalizedPosition = 1f;
    }


}
