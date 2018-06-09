using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;
using System.IO;

namespace ExcelExport
{
    public class SimpleConfig
    {
        string _configpath;
        JSONNode _root;
        public SimpleConfig(string configpath)
        {
            _configpath = configpath;
            if (File.Exists(configpath))
            {
                var content = File.ReadAllText(configpath);
                if(content.Length !=0)
                {
                    _root = JSON.Parse(content);
                    if (_root == null)
                    {
                        throw new InvalidOperationException("parse config json file failed.");
                    }
                }
            }
        }

        public void Save()
        {
            if(_root == null)
            {
                return;
            }

            using (StreamWriter sw = new StreamWriter(_configpath, false))
            {
                sw.Write(_root.ToString());
                sw.Flush();
                sw.Close();
            }
        }

        JSONNode Find(JSONNode jnode, string key)
        {
            if (jnode == null )
            {
                return null;
            }

            var jobject = jnode.AsObject;
            var enumerator = jobject.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var c = (KeyValuePair<string, JSONNode>)enumerator.Current;
                if (c.Key == key)
                {
                    return c.Value;
                }
            }

            return null;
        }

        public string Get(string keypath, string v)
        {
            JSONNode tmp = _root;
            string[] keys = keypath.Split('.');
            foreach (var key in keys)
            {
                tmp = Find(tmp, key);
                if (tmp == null)
                {
                    return v;
                }
            }
            return tmp.Value;
        }

        public T Get<T>(string keypath, T v)
        {
            JSONNode tmp = _root;
            string[] keys = keypath.Split('.');
            foreach (var key in keys)
            {
                tmp = Find(tmp, key);
                if (tmp == null)
                {
                    return v;
                }
            }
            return (T)Convert.ChangeType(tmp.Value, typeof(T));
        }

        void SetImp(string keypath,Func<JSONNode> creater)
        {
            string[] keys = keypath.Split('.');
            int n = 0;
            JSONNode tmp = _root;
            foreach (var key in keys)
            {
                if (_root == null)
                {
                    _root = new JSONObject();
                    tmp = _root;
                }

                var jo = Find(tmp, key);
                if (jo == null)
                {
                    if (n < (keys.Length - 1))
                    {
                        tmp[key] = new JSONObject();
                    }
                }

                if (n == (keys.Length - 1))
                {
                    tmp[key] = creater();
                }

                tmp = tmp[key];
                n++;
            }

            Save();
        }

        public void Set(string keypath, string v)
        {
            SetImp(keypath, () => new JSONString(v));
        }

        public void Set(string keypath, bool v)
        {
            SetImp(keypath, () => new JSONBool(v));
        }

        public void Set(string keypath, object v)
        {
            SetImp(keypath, () => new JSONNull());
        }

        public void Set<T>(string keypath, T v)
        {
            double dv;
            if(double.TryParse(v.ToString(), out  dv))
            {
                SetImp(keypath, () => new JSONNumber(dv));
            } 
        }
    }
}
