using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization; 
using UnityEngine.Localization.Settings;
using TMPro;

/// <summary>
/// 로컬라이제이션으로 텍스트 출력할때, 추가 작업이 필요한경우, 해당 스크립트에 추가 
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
