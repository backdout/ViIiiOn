using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;
using System;

public class SkillTreeItem : MonoBehaviour
{


    public class SkillTreeItemData
    {
        /// <summary>
        /// 구매 가능한 상태
        /// </summary>
        public bool isOn { get; private set; }

        public SkillTreeDataScript.SKILL_TREE_TYPE type { get; private set; }
        public int id { get; private set; }
        public int buffId { get; private set; }
        public int price { get; private set; }
        public int value { get; private set; }
        public string desc { get; private set; }

        public SkillTreeView skillTreeView { get; private set; }    

        public SkillTreeItemData(bool _isOn, SkillTreeDataScript.SKILL_TREE_TYPE _type, int _Id, int _price, int _value, string _desc, int buffId)
        {
            isOn = _isOn;
            type = _type;
            id = _Id;
            price = _price;
            value = _value;
            desc = _desc;
            this.buffId = buffId;
        }

        public SkillTreeItemData(SkillTreeView view, SkillTreeDataScript script, bool IslangKr, bool _isOn)
        {
            skillTreeView = view;
            isOn = _isOn;
            type = script.Type;
            id = script.Id;
            buffId = script.BuffId;
            price = script.Price;
            value = script.Value;
            desc = IslangKr? script.desc_kor : script.desc_en;
        }


        public void SetIsOn(bool _isOn)
        {
            isOn = _isOn;
        }


    }

    [SerializeField]
    private Image Icon;

    [SerializeField]
    private GameObject PriceGroup;
    [SerializeField]
    private TextMeshProUGUI Price;

    [SerializeField]
    private GameObject GetIcon;

    [SerializeField]
    private Image CardIcon;

    [SerializeField]
    private GameObject DescPopup;
    [SerializeField]
    private TextMeshProUGUI Desc;

    [SerializeField]
    private Sprite[] CardIconSprites;

    private Button button;
    private SkillTreeItemData data;

    private bool isClick;
    public void SetData(SkillTreeItemData _data)
    {
        data = _data;
        isClick = false;

        if (data.isOn)
            Price.text = data.price.ToString();
        CardIcon.sprite = CardIconSprites[(int)_data.type];
        Desc.text = string.Format(data.desc, data.value);
        SetUI();
        DescPopup.SetActive(false);


    }

    private void SetUI()
    {
        Icon.sprite = Resources.Load<Sprite>("UISprite/Theme2/07_Resources/Skill/SkillTree_" + data.buffId);
        // ui 작업 후 구매 여부 표현 추가 작업 필요
        GetIcon.SetActive(!data.isOn);
        PriceGroup.SetActive(data.isOn);
       
        if (button == null)
            button = gameObject.GetComponent<Button>();
        button.interactable = data.isOn;
    }



    public void OnClickBuy()
    {
        if (isClick)
        {
            if (IsBuyPossible())
            {
                NoticePopup.SHOW(NoticePopup.ButtonType.YesAndNo, Lang.UI_MSG("NOTICE_BUY_SKILLTREE"), BuySkill);
            }
            else
            {
                // 구매 불가 
                NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG("NOTICE_NOT_ENOUGHT_BUY_SKILLTREE"), null);
            }

            SetIsClick(false);
        }
        else
        {
            data.skillTreeView.SetClickItem(this);
            SetIsClick(true);
        }
    }
    public void SetIsClick(bool _isClick)
    {
        isClick = _isClick;      
        DescPopup.SetActive(isClick);
    }


    private void BuySkill()
    {

        var itemdata = ItemData.Instance;
        int card = itemdata.GetSkillCard(data.type);
        //구매 가능 여부 
        if (card >= data.price)
        {
            UserData.Instance.SetSkillTree(data.type, data.id);
            itemdata.SubSkillCardValue(data.type, data.price);
            data.SetIsOn(false);
            SetUI();
            data.skillTreeView.ShowAddEff(data.type, data.price);
            SoundManager.Instance.PlayEffectAudioClip(SoundManager.EffSoundKind.Cash);
            data.skillTreeView.isBuy = true;
        }
        else//금액 부족 
            NoticePopup.SHOW(NoticePopup.ButtonType.Ok, Lang.UI_MSG("NOTICE_NOT_ENOUGHT_CARD"), null);
    }

    private bool IsBuyPossible()
    {
        var user = UserData.Instance;
        int point = data.type == SkillTreeDataScript.SKILL_TREE_TYPE.RED ? user.SkillRedPoint : data.type == SkillTreeDataScript.SKILL_TREE_TYPE.BLUE ? user.SkillBluePoint : user.SkillGreenPoint;
        if (point == 0 && data.id % 100 == 1)
            return true;

        return point >= data.id - 1;

    }

   
}
