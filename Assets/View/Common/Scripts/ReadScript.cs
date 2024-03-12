using UnityEngine;

using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Linq;
using System;


public class ReadScript
{

    public static Regex regexFloat = new Regex(@"^[+-]?\d*(\.?\d*)$");

    public static int parseInt(string s)
    {
        s = s.Replace("\r", "");
        return int.Parse(Regex.Replace(s, @"[^\d]", ""));
    }

    public static float parseFloat(string s)
    {
        s = s.Replace("\r", "");
        return float.Parse(ReadScript.regexFloat.Match(s).Value);
    }

    public static string[] readFile(string data, bool IsCheckCRC = false, string dataName = "")
    {

#if !UNITY_EDITOR
        //if (filename != null && IsCheckCRC == true)
        //{
        //    var arrayOfBytes = Encoding.ASCII.GetBytes(filename);

        //    uint crc = Get(arrayOfBytes);

        //    string crcStr = crc.ToString("X");
        //    ulong crcLong = CRCINT + crc;

        //    CRCINT = (uint)crcLong & 0xFFFFFFFF;

        //    DATA_CRC += crcStr;
        //    Debug.Log("CRC:" + dataName + ":" + crcStr);
        //}
#endif

        string[] lines;

        if (data != null)
            lines = data.Split('\n');
        else
            lines = null;

        return lines;
    }

    #region 암호화 관련 작업
    public static uint CRCINT = 0xFFFFFFFF;

    // 주로 데이터 암호화 시에 xor 비트연산으로 사용함.
    // 자원 및 중요 데이터에 걸어둘 것.
    public static int XOR = 0x18273645;

    public static float FXOR = 2.123f;
    public static string DATA_CRC = "";


    static public void SetXOR()
    {
        System.Random random = new System.Random();
        XOR = random.Next();
        FXOR = UnityEngine.Random.Range(2.0f, 100.0f);
        DATA_CRC = "";
        CRCINT = 0xFFFFFFFF;
        InitCrc32();
    }

    static readonly UInt32 s_generator = 0xEDB88320;
    static public void InitCrc32()
    {
       // Constructs the checksum lookup table.Used to optimize the checksum.
       m_checksumTable = Enumerable.Range(0, 256).Select(i =>
       {
           var tableEntry = (uint)i;
           for (var j = 0; j < 8; ++j)
           {
               tableEntry = ((tableEntry & 1) != 0)
                   ? (s_generator ^ (tableEntry >> 1))
                   : (tableEntry >> 1);
           }
           return tableEntry;
       }).ToArray();
    }

    static UInt32[] m_checksumTable;
    static public UInt32 Get<T>(IEnumerable<T> byteStream)
    {
#if UNITY_EDITOR
        if (m_checksumTable == null)
            SetXOR();
#endif
        try
        {
         //   Initialize checksumRegister to 0xFFFFFFFF and calculate the checksum.
            return ~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
                      (m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
        }
        catch (FormatException e)
        {
            Debug.LogError("Could not read the stream out as bytes." + e.Message);
        }
        catch (InvalidCastException e)
        {
            Debug.LogError("Could not read the stream out as bytes." + e.Message);
        }
        catch (OverflowException e)
        {
            Debug.LogError("Could not read the stream out as bytes." + e.Message);
        }

        return 0;
    }

    #endregion

}
