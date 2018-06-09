using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class StringUtil
{
    //推荐分隔符
    static public List<T> ParseArray<T>(string s)
    {
        List<T> ret = new List<T>();
        Regex reg = new Regex(@"(\w+)");
        MatchCollection result = reg.Matches(s);
        foreach (Match m in result)
        {
            if (m.Value.Length == 0)
                continue;

            ret.Add((T)Convert.ChangeType(m.Value, typeof(T)));
        }
        return ret;
    }

    static public List<KeyValuePair<TKey, TValue>> ParsePairArray<TKey, TValue>(string s)
    {
        List<KeyValuePair<TKey, TValue>> ret = new List<KeyValuePair<TKey, TValue>>();
        Regex reg = new Regex(@"(\w+),(\w+)");
        MatchCollection result = reg.Matches(s);
        foreach (Match m in result)
        {
            Regex reg2 = new Regex(@"(\w+)");
            MatchCollection result2 = reg2.Matches(m.Value);
            if (result2.Count != 2)
                throw new Exception("error PairArray format");
            var key = (TKey)Convert.ChangeType(result2[0].Value, typeof(TKey));
            var value = (TValue)Convert.ChangeType(result2[1].Value, typeof(TValue));
            ret.Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        return ret;
    }

    //推荐分隔符 ,
    static public KeyValuePair<TKey, TValue> ParsePair<TKey, TValue>(string s, char split)
    {
        var array = s.Split(split);
        if (array.Length != 2)
        {
            throw new Exception("ParsePair must have 2 elms.");
        }
        var key = (TKey)Convert.ChangeType(array[0], typeof(TKey));
        var value = (TValue)Convert.ChangeType(array[1], typeof(TValue));
        return new KeyValuePair<TKey, TValue>(key, value);
    }

    //// e.  a,b|c,d|e,h
    //static public List<KeyValuePair<TKey, TValue>> ParsePairArray<TKey, TValue>(string s, char arraysplit,char pairsplit)
    //{
    //    List<KeyValuePair<TKey, TValue>> ret = new List<KeyValuePair<TKey, TValue>>();
    //    var array = ParseArray<string>(s, arraysplit);
    //    foreach(var a in array)
    //    {
    //        ret.Add(ParsePair<TKey, TValue>(a, pairsplit));
    //    }
    //    return ret;
    //}
}