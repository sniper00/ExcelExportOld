using System;
using System.Collections.Generic;

public class StringUtil
{
    //推荐分隔符 |
    static public List<T> ParseArray<T>(string s, char split)
    {
        List<T> ret = new List<T>();
        var array = s.Split(split);
        foreach (var e in array)
        {
            if (e.Length == 0)
                continue;

            ret.Add((T)Convert.ChangeType(e, typeof(T)));
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

    // e.  a,b|c,d|e,h
    static public List<KeyValuePair<TKey, TValue>> ParsePairArray<TKey, TValue>(string s, char arraysplit,char pairsplit)
    {
        List<KeyValuePair<TKey, TValue>> ret = new List<KeyValuePair<TKey, TValue>>();
        var array = ParseArray<string>(s, arraysplit);
        foreach(var a in array)
        {
            ret.Add(ParsePair<TKey, TValue>(a, pairsplit));
        }
        return ret;
    }
}