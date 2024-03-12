using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization; 
using UnityEngine.Localization.Settings;
using TMPro;

/// <summary>
/// ���ö������̼����� �ؽ�Ʈ ����Ҷ�, �߰� �۾��� �ʿ��Ѱ��, �ش� ��ũ��Ʈ�� �߰� 
/// </summary>
public class TextManager : MonoBehaviour
{
   // [SerializeField] private LocalizedString localizedString;
    [SerializeField] private TextMeshProUGUI text;

    private string keyName;
    private string values;
    private void Start()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnChangedLocale;
    }

    private void OnDisable ()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangedLocale;
    }

    public static string GetUIText(string tableName, string value)
    {
        if (string.IsNullOrEmpty(value))
            return LocalizationSettings.StringDatabase.GetLocalizedString("UI", tableName, LocalizationSettings.SelectedLocale);
        else
            return string.Format(LocalizationSettings.StringDatabase.GetLocalizedString("UI", tableName,LocalizationSettings.SelectedLocale), value);

        //GetComponent<LocalizeStringEvent>().StringReference
        //    .SetReference(tableName, value);
    }


    void OnChangedLocale(Locale locale)
    {
        SetText();
    }

    private void SetText ()
    {
        if (text!=null)
            text.text = GetUIText(keyName, values);
    }



}
