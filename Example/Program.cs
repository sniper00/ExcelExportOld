using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            //加载配置
            Config.example.Load(File.ReadAllText("example.json"));

            //根据key索引
            var cfg =Config.example.Find(103, 0.6f);

            System.Diagnostics.Debug.Assert(cfg.key1 == 103);
            System.Diagnostics.Debug.Assert(cfg.someid == 1040);
            System.Diagnostics.Debug.Assert(cfg.rate == 0.6f);
            System.Diagnostics.Debug.Assert(cfg.name == "fghj");
            System.Diagnostics.Debug.Assert(cfg.testarray.Count == 4);
            System.Diagnostics.Debug.Assert(cfg.testarray[3] == 4);
            System.Diagnostics.Debug.Assert(cfg.testpairarray.Count == 3);
            System.Diagnostics.Debug.Assert(cfg.testpairarray[0].Key == 1);
            System.Diagnostics.Debug.Assert(cfg.testpairarray[0].Value == 2);
        }
    }
}
