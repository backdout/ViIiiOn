using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class CSVUtil
{
    static Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

    public static readonly string[] SPLIT_CHAR = new string[] { ",", ";" };

    public static string[] Split(string target)
    {
        string[] split = CSVParser.Split(target);
        for (int i = 0; i < split.Length; i++)
        {
            string s = split[i];
            if (s.IndexOf('\"') == 0 && s.LastIndexOf('\"') == s.Length - 1)
            {
                s = s.Substring(1, s.Length - 2);
            }

            split[i] = s.Replace("\"\"", "\"");
        }
        return split;
    }
}
