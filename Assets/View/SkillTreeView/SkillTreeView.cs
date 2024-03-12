using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeView : MonoBehaviour
{

    // [SerializeField] private  StageChaters;
    [SerializeField]
    private LoopVerticalScrollRect VerticalScrollRect;


    [SerializeField]
    private TextMeshProUGUI RedValue;
    [SerializeField]
    private TextMeshProUGUI BlueValue;
    [SerializeField]
    private TextMeshProUGUI GreenValue;

    public SafeArea safeArea;
  
    public void Awake()
    {
        
    }

    private SkillTreeItem clickItem;
    public bool isBuy; 

    public void Init()
    {

        // ���� ���� ī�� ���
        RedValue.text = UIDefine.POINT_WITH_K_M(ItemData.Instance.GetSkillCard(SkillTreeDataScript.SKILL_TREE_TYPE.RED));
        BlueValue.text = UIDefine.POINT_WITH_K_M(ItemData.Instance.GetSkillCard(SkillTreeDataScript.SKILL_TREE_TYPE.BLUE));
        GreenValue.text = UIDefine.POINT_WITH_K_M(ItemData.Instance.GetSkillCard(SkillTreeDataScript.SKILL_TREE_TYPE.GREEN));


        //��ų Ʈ�� ����Ʈ ���� 
        List<SkillTreeItem.SkillTreeItemData> list = new List<SkillTreeItem.SkillTreeItemData>();

        var redData = SkillTreeDataScript.GET_DATA_LIST(SkillTreeDataScript.SKILL_TREE_TYPE.RED);
        var blueData = SkillTreeDataScript.GET_DATA_LIST(SkillTreeDataScript.SKILL_TREE_TYPE.BLUE);
        var greenData = SkillTreeDataScript.GET_DATA_LIST(SkillTreeDataScript.SKILL_TREE_TYPE.GREEN);

        //  Ÿ�Ժ� ID ������ ����
        redData.Sort((x, y) => x.Id.CompareTo(y.Id));
        blueData.Sort((x, y) => x.Id.CompareTo(y.Id));
        greenData.Sort((x, y) => x.Id.CompareTo(y.Id));

        bool isLangKr = OptionDataManager.Instance.Language == UIDefine.Lang_Kind.Kor;

        var user = UserData.Instance;

        //�� Ÿ���� ������ ���� ��/ �� ���پ� �־�� ��
        for(int i = 0; i < redData.Count; i++)
        {
            list.Add(new SkillTreeItem.SkillTreeItemData(this, redData[i], isLangKr, redData[i].Id > user.SkillRedPoint));
            list.Add(new SkillTreeItem.SkillTreeItemData(this, blueData[i], isLangKr, blueData[i].Id > user.SkillBluePoint));
            list.Add(new SkillTreeItem.SkillTreeItemData(this, greenData[i], isLangKr, greenData[i].Id > user.SkillGreenPoint));
        }

        VerticalScrollRect.init(list.ToArray());
        isBuy = false; 
    }

    public void SetClickItem(SkillTreeItem _clickItem)
    {
        if(clickItem!=null)
            clickItem.SetIsClick(false);

        clickItem = _clickItem;

    }

    public void ShowAddEff(SkillTreeDataScript.SKILL_TREE_TYPE type, int price)
    {
        GameObject parent = null;
        switch (type)
        {
            case SkillTreeDataScript.SKILL_TREE_TYPE.RED:
                RedValue.text = UIDefine.POINT_WITH_K_M(ItemData.Instance.GetSkillCard(SkillTreeDataScript.SKILL_TREE_TYPE.RED));
                parent = RedValue.gameObject;
                break;

            case SkillTreeDataScript.SKILL_TREE_TYPE.BLUE:              
                BlueValue.text = UIDefine.POINT_WITH_K_M(ItemData.Instance.GetSkillCard(SkillTreeDataScript.SKILL_TREE_TYPE.BLUE));
                parent = BlueValue.gameObject;
                break;
                

            case SkillTreeDataScript.SKILL_TREE_TYPE.GREEN:
                GreenValue.text = UIDefine.POINT_WITH_K_M(ItemData.Instance.GetSkillCard(SkillTreeDataScript.SKILL_TREE_TYPE.GREEN));
                parent = GreenValue.gameObject;
                break;
        }


        GameObject AddTextGo = InstantiateShopEffText();

        var AddText = AddTextGo.GetComponent<ShopEffFont>();

        if (AddText != null)
        {
            AddText.Show(false, price, parent, 140f, 1.4f);
        }

    }

    GameObject ShopEffText;
    private GameObject InstantiateShopEffText()
    {
        if (ShopEffText == null)
            ShopEffText = Resources.Load<GameObject>("ShopEffFont");

        GameObject g = ObjectPoolManager.Instance.doInstantiate(ShopEffText);

        g.SetActive(false);

        return g;
    }

}
