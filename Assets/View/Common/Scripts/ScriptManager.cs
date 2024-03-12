using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static GoogleSheetsManager;

public class ScriptManager : Singleton<ScriptManager>
{

    public bool IsInit { get; private set; }
    Coroutine coLoadAllScript;
    public void Init()
    {
        if (coLoadAllScript == null)
            coLoadAllScript = StartCoroutine(LOAD_ALL_SCRIPT());

        IsInit = true;
    }

  


    public IEnumerator LOAD_ALL_SCRIPT()
    {
        StartCoroutine(GoogleSheetsManager.Instance.ALL_REQUEST());

        yield return new WaitUntil(() => GoogleSheetsManager.Instance.isGoogleSheetsLoad() == true);

        LoadAllScripts();
    }


    public static void LoadAllScripts(bool isForGame = false)
    {

      //  LoadUnitScripts();

        if (StageMobDataScript.INIT() == false)
        {
            Debug.LogError("StageMobDataScript NOT INIT");
        }

        if (MobStatDataScript.INIT() == false)
        {
            Debug.LogError("MobStatDataScript NOT INIT");
        }

        if (SkillTreeDataScript.INIT() == false)
        {
            Debug.LogError("SkillTreeDataScript NOT INIT");
        }

        if (ItemDataScript.INIT() == false)
        {
            Debug.LogError("ItemDataScript NOT INIT");
        }
        
    }

    static Regex regexFloat = new Regex(@"^[+-]?\d*(\.?\d*)$");
#if UNITY_EDITOR

    public static float ParseFloat(FieldInfo fi, string s, float min = float.MinValue, float max = float.MaxValue)
    {
        s = s.Replace("\r", "");

        float result;

        if (float.TryParse(regexFloat.Match(s).Value, out result) == false)
            ThrowException(fi, ERROR_TYPE.ERROR_PARSE_FLOAT, regexFloat.Match(s).Value, "NO FLOAT");

        if (result < min)
            ThrowException(fi, ERROR_TYPE.ERROR_FLOAT_RANGE, result.ToString(), "UNDER " + min);

        if (result > max)
            ThrowException(fi, ERROR_TYPE.ERROR_FLOAT_RANGE, result.ToString(), "OVER " + max);

        return result;
    }

    public static double ParseDouble(FieldInfo fi, string s, double min = double.MinValue, double max = double.MaxValue)
    {
        s = s.Replace("\r", "");

        double result;

        if (double.TryParse(regexFloat.Match(s).Value, out result) == false)
            ThrowException(fi, ERROR_TYPE.ERROR_PARSE_FLOAT, regexFloat.Match(s).Value, "NO double");

        if (result < min)
            ThrowException(fi, ERROR_TYPE.ERROR_FLOAT_RANGE, result.ToString(), "UNDER " + min);

        if (result > max)
            ThrowException(fi, ERROR_TYPE.ERROR_FLOAT_RANGE, result.ToString(), "OVER " + max);

        return result;
    }


    public static int ParseInt(FieldInfo fi, string s, int min = int.MinValue, int max = int.MaxValue)
    {
        int result;

        if (int.TryParse(s.Replace("\r", ""), out result) == false)
            ThrowException(fi, ERROR_TYPE.ERROR_PARSE_INT, s.Replace("\r", ""), "NO INT");

        if (result < min)
            ThrowException(fi, ERROR_TYPE.ERROR_INT_RANGE, result.ToString(), "UNDER " + min);

        if (result > max)
            ThrowException(fi, ERROR_TYPE.ERROR_INT_RANGE, result.ToString(), "OVER " + max);

        return result;
    }

    public static string ParseString(FieldInfo fi, string s, int length)
    {
        s = s.Replace("\r", "");

        if (s.Length > length)
            ThrowException(fi, ERROR_TYPE.ERROR_STRING_RANGE, s, "OVER " + length);

        return s;
    }

    public static int ParseEnumInt(FieldInfo fi, string s, Type type)
    {
        int value = ParseInt(fi, s);

        if (Enum.IsDefined(type, value) == false)
            ThrowException(fi, ERROR_TYPE.ERROR_NO_ENUM_KIND);

        return value;
    }

    public static object CheckReferenceID(FieldInfo fi, object o, int id, string reference)
    {
        if (o == null)
            ThrowException(fi, ERROR_TYPE.ERROR_NO_REFERNCE_ID, id.ToString(), "NO ID IN " + reference);

        return o;
    }

    [UnityEditor.MenuItem("Custom/CheckFile")]
    public void GetErrorFile()
    {
        errorLog.Remove(0, errorLog.Length);

        isFiling = true;

        StartCoroutine(LOAD_ALL_SCRIPT());
        //LoadAllScripts();

        File.WriteAllText(Path.Combine(Application.dataPath, "error.csv"), errorLog.ToString());
    }

#else
    public static float ParseFloat(FieldInfo fi, string s, float min = float.MinValue, float max = float.MaxValue)
    {
        return float.Parse(regexFloat.Match(s.Replace("\r", "")).Value);
    }

    public static double ParseDouble(FieldInfo fi, string s, double min = double.MinValue, double max = double.MaxValue)
    {
        return double.Parse(regexFloat.Match(s.Replace("\r", "")).Value);
    }

    public static int ParseInt(FieldInfo fi, string s, int min = int.MinValue, int max = int.MaxValue, Type type = null)
    {
        int result;

        if (int.TryParse(s.Replace("\r", ""), out result) == false)
            ThrowException(fi, ERROR_TYPE.ERROR_PARSE_INT, s.Replace("\r", ""), "NO INT");

        if (result < min)
            ThrowException(fi, ERROR_TYPE.ERROR_INT_RANGE, result.ToString(), "UNDER " + min);

        if (result > max)
            ThrowException(fi, ERROR_TYPE.ERROR_INT_RANGE, result.ToString(), "OVER " + max);

        return result;
    }

    public static string ParseString(FieldInfo fi, string s, int byteCount)
    {
        return s.Replace("\r", "");
    }

    public static int ParseEnumInt(FieldInfo fi, string s, Type type)
    {
        return ParseInt(fi, s);
    }

    public static object CheckReferenceID(FieldInfo fi, object o, int id, string reference)
    {
        return o;
    }
#endif

    public enum ERROR_TYPE
    {
        ERROR_OTHER = -1,
        ERROR_PARSE_INT = 0,
        ERROR_PARSE_FLOAT = 1,
        ERROR_INT_RANGE = 2,
        ERROR_FLOAT_RANGE = 3,
        ERROR_STRING_RANGE = 4,
        ERROR_NO_REFERNCE_ID = 5,
        ERROR_NO_ENUM_KIND = 6,
        ERROR_EXIST_ID = 7,
    }

    static string GetErrorMsg(FieldInfo fi, ERROR_TYPE e, params string[] ps)
    {
        StringBuilder sb = new StringBuilder();

        if (fi != null)
        {
            sb.Append(fi.classname);

            sb.Append(" RAW: ");
            sb.Append(fi.raw + 1);

            sb.Append(" COLUMN: ");
            sb.Append(fi.GetColumnAlphabet());
        }

        if (e != ERROR_TYPE.ERROR_OTHER)
        {
            sb.Append(" ERROR: ");
            sb.Append(e.ToString());
        }

        for (int i = 0; i < ps.Length; i++)
        {
            sb.Append(" ");
            sb.Append(ps[i]);
        }

        return sb.ToString();
    }

    public class FieldInfo
    {
        public string classname { get; private set; }
        public int raw { get; private set; }
        public int column { get; private set; }

        public FieldInfo(string cn, int r, int c)
        {
            classname = cn;
            raw = r;
            column = c;
        }

        public FieldInfo NextColumn()
        {
            column++;

            return this;
        }

        public FieldInfo SetColumn(int c)
        {
            column = c;

            return this;
        }

        public string GetColumnAlphabet()
        {
            StringBuilder sb = new StringBuilder();

            int cc = (int)(column / 26);

            if (cc > 0)
            {
                sb.Append((char)((cc - 1) + 65));
            }

            sb.Append((char)(column % 26 + 65));

            return sb.ToString();
        }
    }

    public static void ThrowException(FieldInfo fi, ERROR_TYPE e, params string[] msg)
    {
        try
        {
            if (isFiling)
                SetErrorLog(fi, e, msg);
            else
            {
                throw new Exception(GetErrorMsg(fi, e, msg));
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    static bool isFiling = false;

    static StringBuilder errorLog = new StringBuilder("FILE NAME,RAW,COLUMN,VALUE,ERROR\n");
    static void SetErrorLog(FieldInfo fi, ERROR_TYPE e, params string[] ps)
    {
        StringBuilder sb = new StringBuilder();

        if (fi != null)
        {
            sb.Append(fi.classname);
            sb.Append(",");
            sb.Append(fi.raw + 1);
            sb.Append(",");
            sb.Append(fi.GetColumnAlphabet());
            sb.Append(",");
        }

        for (int i = 0; i < ps.Length; i++)
        {
            sb.Append(ps[i]);
            sb.Append(",");
        }

        errorLog.AppendLine(sb.ToString());
    }
}

