local FileWriter = require("FileWriter")
local Common = require("Common")

local M = {}

M.Fileext = '.cs'

M.IsCode = true

-- for CSharp SimpleJson
M.jsonConvert = {
    int = "AsInt",
    uint = "AsUInt",
    long = "AsLong",
    ulong = "AsULong",
    float = "AsFloat",
    double = "AsDouble",
    bool = "AsBool",
    string = "Value"
}

--[[
是否是多个key
]]
local function IsMulKey(colsName, fieldConstraints, dataType)
    local keys = {}

    for _, v in pairs(colsName) do
        if "key" == fieldConstraints[v] then
            table.insert(keys, v)
        end
    end

    if #keys > 1 then
        return true, keys, "string"
    end
    --没有设置主键，默认第一列为主键
    if #keys == 0 then
        keys[1] = colsName[1]
    end
    return false, keys, dataType[keys[1]]
end

local function MakeKeyParams(bMul, keys, dataType)
    local params = "" --组合 Find函数的形参 e. int key1,int key2
    local args = "" --组合 多个主键为字符串
    local addargs = "" --组合 Add 函数的实参
    local first = true
    for _, v in pairs(keys) do
        if not first then
            params = params .. ", "
            args = args .. " + "
            addargs = addargs .. ","
        end
        params = params .. dataType[v] .. " " .. v
        if bMul then
            args = args .. " " .. v .. ".ToString()"
        else
            args = args .. " " .. v
        end
        addargs = addargs .. "d." .. v
        first = false
    end
    return params, args, addargs
end

function M.CodeGen(savePath, tbName, colsName, dataType, fieldConstraints)
    local bMul, keys, keyType = IsMulKey(colsName, fieldConstraints, dataType)

    local fileWriter = FileWriter.new(savePath .. "/" .. tbName .. ".cs")
    fileWriter:WriteLine("using SimpleJSON;")
    fileWriter:WriteLine("using System.Collections.Generic;")
    fileWriter:WriteLine()
    fileWriter:WriteLine("namespace Config")
    fileWriter:WriteLeftBrace()
    fileWriter:WriteLine("public class " .. tbName)
    fileWriter:WriteLeftBrace()
    --定义属性
    for _, colName in pairs(colsName) do
        local vType = dataType[colName]
        if vType then
            if Common.IsBaseDataType(vType) then
                fileWriter:WriteLine("public " .. vType .. " " .. colName .. " {get;private set;}")
            else
                fileWriter:WriteLine(
                    "public " .. M.CustomDataType[vType].define .. " " .. colName .. " {get;private set;}"
                )
            end
        end
    end

    local ContainerType = "SortedDictionary<" .. keyType .. "," .. tbName .. ">"

    fileWriter:WriteLine()
    fileWriter:WriteLine("static " .. ContainerType .. " _datas = new " .. ContainerType .. "();")

    local params, args, addargs = MakeKeyParams(bMul, keys, dataType)

    --静态查找函数
    fileWriter:WriteLine()
    fileWriter:WriteLine("static public " .. tbName .. " Find(" .. params .. ")")
    fileWriter:WriteLeftBrace()
    fileWriter:WriteLine("var _key = " .. args .. ";")
    fileWriter:WriteLine("if(_datas.ContainsKey(_key))")
    fileWriter:WriteLeftBrace()
    fileWriter:WriteLine("return _datas[_key];")
    fileWriter:WriteRightBrace()
    fileWriter:WriteLine("return null;")
    fileWriter:WriteRightBrace()

    --静态添加函数
    fileWriter:WriteLine()
    fileWriter:WriteLine("static bool Add( " .. params .. "," .. tbName .. " v)")
    fileWriter:WriteLeftBrace()
    fileWriter:WriteLine("var _key = " .. args .. ";")
    fileWriter:WriteLine("if(!_datas.ContainsKey(_key))")
    fileWriter:WriteLeftBrace()
    fileWriter:WriteLine("_datas.Add(_key,v);")
    fileWriter:WriteLine("return true;")
    fileWriter:WriteRightBrace()
    fileWriter:WriteLine("return false;")
    fileWriter:WriteRightBrace()

    -- 加载函数
    fileWriter:WriteLine()
    fileWriter:WriteLine("static public bool Load(string content)")
    fileWriter:WriteLeftBrace()
    fileWriter:WriteLine("_datas.Clear();")
    fileWriter:WriteLine("var parser = JSONNode.Parse(content);")
    fileWriter:WriteLine("var dataArray = parser.AsArray;")
    fileWriter:WriteLine("if(dataArray == null) return false;")
    fileWriter:WriteLine("foreach (var item in dataArray.Children)")
    fileWriter:WriteLeftBrace()
    if M.minisize then
        fileWriter:WriteLine("var data = item.AsArray;")
    else
        fileWriter:WriteLine("var data = item.AsObject;")
    end
    fileWriter:WriteLine("if(data == null) return false;")
    fileWriter:WriteLine(tbName .. " d = new " .. tbName .. "();")

    for col, colName in pairs(colsName) do
        local vType = dataType[colName]
        if vType then
            if M.minisize then
                if Common.IsBaseDataType(vType) then
                    fileWriter:WriteLine(
                        "d." .. colName .. " = " .. "data[" .. tostring(col - 1) .. "]." .. M.jsonConvert[vType] .. ";"
                    )
                else
                    fileWriter:WriteLine(
                        "d." ..
                            colName ..
                                " = " ..
                                    M.CustomDataType[vType].parser("data[" .. tostring(col - 1) .. "].Value") .. ";"
                    )
                end
            else
                if Common.IsBaseDataType(vType) then
                    fileWriter:WriteLine(
                        "d." .. colName .. " = " .. 'data["' .. colName .. '"].' .. M.jsonConvert[vType] .. ";"
                    )
                else
                    fileWriter:WriteLine(
                        "d." ..
                            colName .. " = " .. M.CustomDataType[vType].parser('data["' .. colName .. '"].Value') .. ";"
                    )
                end
            end
        end
    end

    fileWriter:WriteLine("if(!Add(" .. addargs .. ",d))")
    fileWriter:WriteLeftBrace()
    fileWriter:WriteLine("return false;")
    fileWriter:WriteRightBrace()

    fileWriter:WriteRightBrace()
    fileWriter:WriteLine("return true;")
    fileWriter:WriteRightBrace()
    fileWriter:WriteRightBrace()
    fileWriter:WriteRightBrace()
    fileWriter:Close()
end

M.CustomDataType = {}

--如果自定义类型没有解析方法，按string处理
M.CustomDataType.__index =
    function()
    return {
        define = "string",
        parser = function(data)
            return data
        end
    }
end

setmetatable(M.CustomDataType, M.CustomDataType)

--自定义数据类型
M.CustomDataType["array<int>"] = {
    define = "List<int>", --定义数据类型
    parser = function(data)
        return "StringUtil.ParseArray<int>(" .. data .. ")" --数据解析方法
    end
}

M.CustomDataType["pairarray<int,int>"] = {
    define = "List<KeyValuePair<int, int>>", --定义数据类型
    parser = function(data)
        return "StringUtil.ParsePairArray<int,int>(" .. data .. ")" --数据解析方法
    end
}

return M
