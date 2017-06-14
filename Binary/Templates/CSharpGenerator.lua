local CodeWriter = require('CodeWriter')
local Common = require("Common")

local M = {}

-- for CSharp SimpleJson
M.jsonConvert = {
    int = 'AsInt',
    uint = 'AsUInt',
    long = 'AsLong',
    ulong = 'AsULong',
    float = 'AsFloat',
    double = 'AsDouble',
    bool = 'AsBool',
    string = 'Value',
}

local function DictGetValue(dict, key)
    local bexit, v = dict:TryGetValue(key)
    if bexit then
        return v
    end
    return nil
end

--[[
是否是多个key
]]
local function IsMulKey(colsName,fieldConstraints, dataType)
    local keys = {}
    local enum = fieldConstraints:GetEnumerator()
    while enum:MoveNext() do
        if enum.Current.Value == 'key' then
            table.insert(keys, enum.Current.Key)
        end
    end
    if #keys > 1 then
        return true, keys, 'string'
    end
    --没有设置主键，默认第一列为主键
    if #keys == 0 then
        keys[1] = colsName[0]
    end
    return false, keys, DictGetValue(dataType, keys[1])
end

local function MakeKeyParams(bMul,keys, dataType)
    local params = '' --组合 Find函数的形参 e. int key1,int key2
    local args = '' --组合 多个主键为字符串
    local addargs = '' --组合 Add 函数的实参
    local first = true
    for k, v in pairs(keys) do
        if not first then
            params = params .. ', '
            args = args .. ' + '
            addargs = addargs .. ','
        end
        params = params .. DictGetValue(dataType, v) .. ' ' .. v
        if bMul then
            args = args .. ' ' .. v .. '.ToString()'
        else
            args = args .. ' ' .. v
        end
        addargs = addargs .. 'd.' .. v
        first = false
    end
    return params, args, addargs
end

function M.CodeGen(savePath, tbName, colsName, dataType, fieldConstraints)
    local bMul, keys, keyType = IsMulKey(colsName,fieldConstraints, dataType)

    CodeWriter.Open(savePath .. '/' .. tbName .. '.cs')
    CodeWriter.WriteLine("using SimpleJSON;")
    CodeWriter.WriteLine("using System.Collections.Generic;")
    CodeWriter.WriteLine()
    CodeWriter.WriteLine("namespace Config")
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine("public class " .. tbName);
    CodeWriter.WriteLeftBrace()
    --定义属性
    for i = 0, colsName.Count - 1 do
        local colName = colsName[i]
        local vType = DictGetValue(dataType, colName)
        if vType then
            if Common.IsBaseDataType(vType) then
                CodeWriter.WriteLine('public ' .. vType .. ' ' .. colName .. ' {get;private set;}')
            else
                CodeWriter.WriteLine('public ' .. M.CustomDataType[vType].define .. ' ' .. colName .. ' {get;private set;}')
            end
        end
    end

    local ContainerType = 'SortedDictionary<' .. keyType .. ',' .. tbName .. '>'

    CodeWriter.WriteLine()
    CodeWriter.WriteLine('static ' .. ContainerType .. ' _datas = new ' .. ContainerType .. '();')

    local params, args, addargs = MakeKeyParams(bMul, keys, dataType)

    --静态查找函数
    CodeWriter.WriteLine()
    CodeWriter.WriteLine('static public ' .. tbName .. ' Find(' .. params .. ')')
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine('var _key = ' .. args .. ';')
    CodeWriter.WriteLine('if(_datas.ContainsKey(_key))')
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine('return _datas[_key];')
    CodeWriter.WriteRightBrace()
    CodeWriter.WriteLine('return null;')
    CodeWriter.WriteRightBrace()

    --静态添加函数
    CodeWriter.WriteLine()
    CodeWriter.WriteLine('static bool Add( ' .. params .. ',' .. tbName .. ' v)')
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine('var _key = ' .. args .. ';')
    CodeWriter.WriteLine('if(!_datas.ContainsKey(_key))')
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine('_datas.Add(_key,v);')
    CodeWriter.WriteLine('return true;')
    CodeWriter.WriteRightBrace()
    CodeWriter.WriteLine('return false;')
    CodeWriter.WriteRightBrace()

    -- 加载函数
    CodeWriter.WriteLine()
    CodeWriter.WriteLine('static public bool Load(string content)')
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine('_datas.Clear();')
    CodeWriter.WriteLine("var parser = JSONNode.Parse(content);");
    CodeWriter.WriteLine("var dataArray = parser.AsArray;");
    CodeWriter.WriteLine("if(null == dataArray) return false;");
    CodeWriter.WriteLine("foreach (var item in dataArray.Childs)");
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine("var data = item.AsArray;");
    CodeWriter.WriteLine("if(null == data) return false;");
    CodeWriter.WriteLine(tbName .. " d = new " .. tbName .. "();");

    for i = 0, colsName.Count - 1 do
        local colName = colsName[i]
        local vType = DictGetValue(dataType, colName)
        if vType then
            if Common.IsBaseDataType(vType) then
                CodeWriter.WriteLine('d.' .. colName .. " = " .. "data[" .. tostring(i) .. "]." .. M.jsonConvert[vType] .. ";")
            else
                CodeWriter.WriteLine('d.' .. colName .. " = " ..M.CustomDataType[vType].parser("data[" .. tostring(i) .. "].Value").. ";")
            end
        end
    end

    CodeWriter.WriteLine('if(!Add(' .. addargs .. ',d))')
    CodeWriter.WriteLeftBrace()
    CodeWriter.WriteLine("return false;")
    CodeWriter.WriteRightBrace()

    CodeWriter.WriteRightBrace()
    CodeWriter.WriteLine("return true;")
    CodeWriter.WriteRightBrace()
    CodeWriter.WriteRightBrace()
    CodeWriter.WriteRightBrace()
    CodeWriter.Close()
end

M.CustomDataType = {}

--如果自定义类型没有解析方法，按string处理
M.CustomDataType.__index = function()
    return {
        define = 'string',
        parser = function(data)
            return data
        end
    }
end

setmetatable(M.CustomDataType,M.CustomDataType)

--自定义数据类型
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



return M
