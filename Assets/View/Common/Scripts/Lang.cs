using UnityEngine;
using System;
using System.Text;
using System.Linq;
using UnityEngine.Localization.Settings;
#if UNITY_EDITOR
using UnityEditor.Localization.Plugins.Google;
using UnityEditor.Localization;

using UnityEditor.Localization.Reporting;
using UnityEditor;
#endif


public class Lang : Singleton<Lang>
{
    

    private readonly string UI_MSG_NAME = "UI";
    private readonly string SCRIPT_MSG_NAME = "desc";

    public bool IsInit { get; private set; } = false;


    public static string UI_MSG(string key)
    {
        return Instance.getUIMsgText(key);
    }
    public static string DESC_MSG(string key)
    {
        return Instance.getScriptMsgText(key);
    }
    public static string ERROR_UI_MSG(int ErrorNo)
    {
        return Instance.getUIMsgText("ERROR_" + ErrorNo);
    }
 
    public static string DESC_MSG(string scriptName, string propertyName, int id, int index)
    {
        return Instance.getScriptMsgText(string.Format("{0}_{1}_{2}", scriptName, propertyName, id));
    }
    public static string DESC_MSG(string scriptName, string propertyName, string id)
    {
        return Instance.getScriptMsgText(string.Format("{0}_{1}_{2}", scriptName, propertyName, id));
    }
 


    public void Init()
    {
#if UNITY_EDITOR
        UIPULLGOOGLE();
#endif
        InitTag();
        IsInit = true;
    }

#if UNITY_EDITOR
    [MenuItem("Localization/Google Sheets/UIPull")]

    public static void UIPULLGOOGLE()
    {
        // You should provide your String Table Collection name here
        var tableCollection = LocalizationEditorSettings.GetStringTableCollection("UI");
        var googleExtension = tableCollection.Extensions.FirstOrDefault(e => e is GoogleSheetsExtension) as GoogleSheetsExtension;
        if (googleExtension == null)
        {
            Debug.LogError($"String Table Collection {tableCollection.TableCollectionName} Does not contain a Google Sheets Extension.");
            
            return;
        }

        PullExtension(googleExtension);
    }

    static void PullExtension(GoogleSheetsExtension googleExtension)
    {
        // Setup the connection to Google
        var googleSheets = new GoogleSheets(googleExtension.SheetsServiceProvider);
        googleSheets.SpreadSheetId = googleExtension.SpreadsheetId;

        // Now update the collection. We can pass in an optional ProgressBarReporter so that we can updates in the Editor.
        googleSheets.PullIntoStringTableCollection(googleExtension.SheetId, googleExtension.TargetCollection as StringTableCollection, googleExtension.Columns, reporter: new ProgressBarReporter());


    }
#endif

    public static string SCRIPT_MSG(string scriptName, string propertyName, int id, int index)
    {
        return Instance.getScriptMsgText(string.Format("{0}_{1}_{2}", scriptName, propertyName, id));
    }

    public static string MSG_PATCH(string scriptName, string propertyName, int id, int index)
    {
        return Instance.getUIMsgText(string.Format("{0}_{1}_{2}", scriptName, propertyName, id));
    }

    // 한글자모
    private string tag1;      //TAG1,<은는>
    private string tag2;      //TAG2,<이가>
    private string tag3;      //TAG3,<을를>
    private string tag4;      //TAG4,<와과>
    private string tag5;      //TAG5,<으로로
    private string tag1_1;    //TAG1_1,은
    private string tag1_2;    //TAG1_2, 는
    private string tag2_1;    //TAG2_1,이
    private string tag2_2;    //TAG2_2, 가
    private string tag3_1;    //TAG3_1,을
    private string tag3_2;    //TAG3_2, 를
    private string tag4_1;    //TAG4_1,와
    private string tag4_2;    //TAG4_2, 과
    private string tag5_1;    //TAG5_1,으로
    private string tag5_2;    //TAG5_2, 로

    private void InitTag()
    {
        tag1 = getUIMsgText("TAG1");
        tag2 = getUIMsgText("TAG2");
        tag3 = getUIMsgText("TAG3");
        tag4 = getUIMsgText("TAG4");
        tag5 = getUIMsgText("TAG5");

        tag1_1 = getUIMsgText("TAG1_1");
        tag1_2 = getUIMsgText("TAG1_2");

        tag2_1 = getUIMsgText("TAG2_1");
        tag2_2 = getUIMsgText("TAG2_2");

        tag3_1 = getUIMsgText("TAG3_1");
        tag3_2 = getUIMsgText("TAG3_2");

        tag4_1 = getUIMsgText("TAG4_1");
        tag4_2 = getUIMsgText("TAG4_2");

        tag5_1 = getUIMsgText("TAG5_1");
        tag5_2 = getUIMsgText("TAG5_2");
    }
    public string getUIMsgText(string key)
    {
        string values = LocalizationSettings.StringDatabase.GetLocalizedString(UI_MSG_NAME, key, LocalizationSettings.SelectedLocale);


        if (string.IsNullOrEmpty(values))
        {
            Debug.LogWarning("uiMsg key: " + key + " get value fail");
            return "";
        }

        return values;
    }
    public string getScriptMsgText(string key)
    {
        string values = LocalizationSettings.StringDatabase.GetLocalizedString(SCRIPT_MSG_NAME, key, LocalizationSettings.SelectedLocale);

        if (string.IsNullOrEmpty(values))
        {
            Debug.LogWarning("SCRIPT_MSG_NAME key: " + key + " get value fail");
            return "";
        }


        return values;
    }
  

    private bool IsBadChim(char ch)
    {
        ushort unicode = Convert.ToUInt16(ch);
        if (unicode < 0xAC00 || unicode > 0xD79F)
            return false;

        if (((unicode - 44032) % (21 * 28)) % 28 == 0)
            return false;

        return true;
    }
    private string checkHangle(string msg, string tag, string yesbadchim, string nobadchim)
    {
        int index = msg.IndexOf(tag);

        if (index > 0)
        {
            char c = msg.ToCharArray()[index - 1];

            if (IsBadChim(c) == true)
            {
                msg = msg.Replace(tag, yesbadchim);
            }
            else
            {
                msg = msg.Replace(tag, nobadchim);
            }
        }

        return msg;
    }
    private string checkHangleConsonant(string msg, string yesbadchim, string nobadchim)
    {
        if (string.IsNullOrEmpty(msg))
            return "";

        char c = msg[msg.Length - 1];

        if (IsBadChim(c) == true)
        {
            return yesbadchim;
        }
        else
        {
            return nobadchim;
        }
    }
    public string ToHangle(string msg)
    {
        if (msg == null || msg.Length == 0)
            return "";

        msg = checkHangle(msg, tag1, tag1_1, tag1_2);
        msg = checkHangle(msg, tag2, tag2_1, tag2_2);
        msg = checkHangle(msg, tag3, tag3_1, tag3_2);
        msg = checkHangle(msg, tag4, tag4_1, tag4_2);
        msg = checkHangle(msg, tag5, tag5_1, tag5_2);

        return msg;
    }




    /// <summary>
    /// str에 맞는 조사를 리턴(은/는)
    /// </summary>
    /// <param name="str"></param>
    /// <returns>은/는</returns>
    public string ToHangleConsonant1(string str)
    {
        return checkHangleConsonant(str, tag1_1, tag1_2);
    }

    /// <summary>
    /// str에 맞는 조사를 리턴(이/가)
    /// </summary>
    /// <param name="str"></param>
    /// <returns>이/가</returns>
    public string ToHangleConsonant2(string str)
    {
        return checkHangleConsonant(str, tag2_1, tag2_2);
    }

    /// <summary>
    /// str에 맞는 조사를 리턴(을/를)
    /// </summary>
    /// <param name="str"></param>
    /// <returns>을/를</returns>
    public string ToHangleConsonant3(string str)
    {
        return checkHangleConsonant(str, tag3_1, tag3_2);
    }

    /// <summary>
    /// str에 맞는 조사를 리턴(와/과)
    /// </summary>
    /// <param name="str"></param>
    /// <returns>와/과</returns>
    public string ToHangleConsonant4(string str)
    {
        return checkHangleConsonant(str, tag4_1, tag4_2);
    }

    /// <summary>
    /// str에 맞는 조사를 리턴(으로/로)
    /// </summary>
    /// <param name="str"></param>
    /// <returns>으로/로</returns>
    public string ToHangleConsonant5(string str)
    {
        return checkHangleConsonant(str, tag5_1, tag5_2);
    }


    public string Encode(string text)
    {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytesToEncode);
    }
}



