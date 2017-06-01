# Excel 导表工具

游戏开发中常用Excel配置游戏数据，配置表数据是固定的，解析代码也是固定的，完全可以用工具代替人工编写，减少重复劳动。这个工具的目的就是尽量让人少参与配置的解析，并且把配置表的数据检测放在导表期间，提前发现错误。

导表工具现在只支持单个Excel sheet导出，sheet名会作为加载代码的类名，尽量保持Excel文件名、 sheet名一致。
表格的前三行是固定的，以#开头的行会作为注释，可以有N行注释

- 列名
- 数据类型
- 字段约束（现在只支持主键约束）

使用lua脚本进行扩展，现在支持的特性：
1. 导出json格式数据(以后准备支持lua)
2. 生成数据加载代码(暂时有CSharp 版 ，使用SimpleJson库)
3. 支持多列索引，多个索引会拼接成字符串作为key
4. 支持自定义数据类型解析函数
5. 支持行数据自定义条件检测

## 依赖：
- [xlua](https://github.com/Tencent/xLua)
- [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader)

## 自定义数据加载代码

详见CodeGen.lua
```lua
local function CodeGen(savePath,tbName,colsName,dataType,fieldConstraint)
    -- CCharp 代码生成器
    CSharpGenerator.CodeGen(savePath,tbName,colsName,dataType,fieldConstraint)
    -- 编写其他语言的 代码生成器
end
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


