# Excel 导表工具

游戏开发中常用Excel配置游戏数据，配置表数据是固定的，解析代码也是固定的，完全可以用工具代替人工编写，减少重复劳动。这个工具的目的就是尽量让人少参与配置的解析，并且把配置表的数据检测放在导表期间，提前发现错误。

导表工具现在只支持单个Excel sheet导出，sheet名会作为加载代码的类名，尽量保持Excel文件名、 sheet名一致。
表格的前三行是固定的，以#开头的行会作为注释，可以有N行注释

- 列名
- 数据类型
- 字段约束（现在只支持主键约束）

使用lua脚本进行扩展，现在支持的特性：
1. 导出json,lua格式数据
2. 支持自定义编写导出逻辑,参考```Binary/Generator```目录
3. 生成数据加载代码(暂时有CSharp 版 ，使用SimpleJson库)
4. 支持多列索引，多个索引会拼接成字符串作为key
5. 支持自定义数据类型解析函数
6. 支持行数据自定义条件检测
7. 支持编写自定义生成器

## 使用
Binary 目录有编译好的可执行文件：
- 设置好工作目录（Excel 文件所在的路径）
- 左边列表，根据需要勾选导出导出项，并在 **输出路径** 列下鼠标右键设置输出目录
- 点击start即可开始

![image](https://github.com/sniper00/ExcelExport/raw/master/Image/ui.png)

## 示例

![image](https://github.com/sniper00/ExcelExport/raw/master/Image/example.png)

导出json数据
```json
[
    [
        99,
        1010,
        0.7,
        "adadada",
        "1|2|3|4|",
        "1,2|3,4|5,6",
        "a12345"
    ],
    [
        102,
        1020,
        0.5,
        "dadad",
        "1|2|3|4",
        "1,2|3,4|5,6",
        "b2345"
    ],
    [
        103,
        1040,
        0.6,
        "fghj",
        "1|2|3|4",
        "1,2|3,4|5,6",
        "c4567"
    ]
]
```

生成CSharp加载代码
```CSharp
using SimpleJSON;
using System.Collections.Generic;

namespace Config
{
	public class example
	{
		public int key1 {get;private set;}
		public int someid {get;private set;}
		public float rate {get;private set;}
		public string name {get;private set;}
		public List<int> testarray {get;private set;}
		public List<KeyValuePair<int, int>> testpairarray {get;private set;}
		public string testcustomtype {get;private set;}

		static SortedDictionary<string,example> _datas = new SortedDictionary<string,example>();

		static public example Find(int key1, float rate)
		{
			var _key =  key1.ToString() +  rate.ToString();
			if(_datas.ContainsKey(_key))
			{
				return _datas[_key];
			}
			return null;
		}

		static bool Add( int key1, float rate,example v)
		{
			var _key =  key1.ToString() +  rate.ToString();
			if(!_datas.ContainsKey(_key))
			{
				_datas.Add(_key,v);
				return true;
			}
			return false;
		}

		static public bool Load(string content)
		{
			_datas.Clear();
			var parser = JSONNode.Parse(content);
			var dataArray = parser.AsArray;
			if(null == dataArray) return false;
			foreach (var item in dataArray.Childs)
			{
				var data = item.AsArray;
				if(null == data) return false;
				example d = new example();
				d.key1 = data[0].AsInt;
				d.someid = data[1].AsInt;
				d.rate = data[2].AsFloat;
				d.name = data[3].Value;
				d.testarray = StringUtil.ParseArray<int>(data[4].Value,'|');
				d.testpairarray = StringUtil.ParsePairArray<int,int>(data[5].Value,'|',',');
				d.testcustomtype = data[6].Value;
				if(!Add(d.key1,d.rate,d))
				{
					return false;
				}
			}
			return true;
		}
	}
}

```

## 依赖：
- [xlua](https://github.com/Tencent/xLua)
- [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader)

## 自定义生成器
参考下面格式编写生成器，编写完成后放到Generator目录，然后重启ExcelExport.exe。
```lua
local M = {}

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- OutPutFilePath 输出文件路径
function M.Begin(OutPutFilePath)
    -- 这里编写生成开始处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- nrow 第几行数据，主要用于打印错误信息
-- firstRow 是否是第一行
-- columnName 所有的列名，数据类型 table 数组
-- fieldConstraint 所有列对应的条件约束，数据类型 table[columnName] Constraint
-- dataType 所有列对应的数据类型，数据类型 table[columnName] type
-- row 行数据，C#对象，数组，下标 0开始
-- checker 条件检测函数
function M.OnRow(nrow, firstRow, columnName, fieldConstraint, dataType, row, checker)
    -- 这里编写行开始的处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
function M.End()
    -- 这里编写生成结束处理逻辑，一般是写文件
    -- 参考 Json.lua
end

-- 此函数用于配置加载代码生成器 如 C++, C# 加载Json配置文件的代码
-- OutPutFilePath 输出文件路径
-- tableName  ExcelSheet Name
-- columnsName  所有的列名，数据类型 table 数组
-- dataType 所有列对应的数据类型，数据类型 table[columnName] type
-- fieldConstraint 所有列对应的条件约束，数据类型 table[columnName] Constraint
function M.CodeGen(OutPutFilePath,tableName,columnsName,dataType,fieldConstraint  )
    -- body
    -- 参考 CSharp.lua
end

return M
```

## 自定义数据类型解析函数
推荐先用CSharp编写通用的字符串解析函数（参考Example/StringUtil.cs），然后在这里调用
详见Templates/CSharpGenerator.lua
```lua
M.CustomDataType['array<int>'] = {
    define = 'List<int>', --定义数据类型
    parser = function(data)
        return "StringUtil.ParseArray<int>("..data..",'|')" --数据解析方法
    end
}

M.CustomDataType['pairarray<int,int>'] = {
    define = 'List<KeyValuePair<int, int>>', --定义数据类型
    parser = function(data)
        return "StringUtil.ParsePairArray<int,int>("..data..",'|',',')" --数据解析方法
    end
}
```
## 数据自定义条件检测
如果Excel文件存在同名的 .checker.lua 脚本，导出数据时会调用这个脚本，脚本规范参考Example\Excel\example.checker.lua
```lua
local Check = {}
Check['key1'] ={
    --条件检测函数，参数是行数据
    check = function(rowData)
        local data = Common.DictGetValue(rowData,'key1')
        return tonumber(data)>=100
    end,
    errmsg = "key1 must >= 100"
}

Check['someid'] ={
    check = function(rowData)
        --检测不同列间的条件关系
        local key1 = Common.DictGetValue(rowData,'key1')
        local someid = Common.DictGetValue(rowData,'someid')
        return tonumber(key1)*10 == tonumber(someid)
    end,
    errmsg = "someid must = key1*10"
}
```


