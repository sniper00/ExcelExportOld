local FileWriter = require('FileWriter')
local Common = require("Common")


--[[
是否是多个key
]]
local function IsMulKey(colsName,fieldConstraints)
    local keys = {}

    for _, v in pairs(colsName) do
        if "key" == fieldConstraints[v] then
            table.insert(keys, v)
        end
    end

    if #keys > 1 then
        return true, keys
    end
    --没有设置主键，默认第一列为主键
    if #keys == 0 then
        keys[1] = colsName[1]
    end
    return false, keys
end

local function MakeKeyParams(keys)
    local params = '' --组合 Find函数的形参 e. int key1,int key2
    local args = '' --组合 多个主键为字符串
    local args2 = '' --组合 多个主键为字符串
    local first = true
    for _, v in pairs(keys) do
        if not first then
            params = params .. ', '
            args = args .. ' .. '
            args2= args2 .. ' .. '
        end

        params = params .. v
        args = args .. 'v.' .. v
        args2 = args2 .. v
        first = false

    end
    return params, args, args2
end


local M = {}

M.Fileext = '.lua'

function M.Begin(f)
    M.file = FileWriter.new(f)
    M.file:WriteLine("local M = {}")
    M.file:WriteLine()
    M.file:WriteLine("M.rawdata =")
    M.file:WriteLeftBrace()
end

function M.OnRowBegin(firstrow)
    if not firstrow then
        M.file:Write(",\n")
    end
    M.file:WriteLeftBrace()
end

function M.OnColumn(firstcol, value, datatype, colname)
    if not firstcol then
        M.file:RawWrite(",\n")
    end

    if Common.IsBaseDataType(datatype) and datatype ~= "string" then
        if type(value) ~= 'userdata' then
            value = Common.DataConvert(datatype, value)
            M.file:Write(colname ..' = '..tostring(value))
        else
            M.file:Write(colname ..' = nil')
        end
    else
        M.file:Write(colname..' = ' .. M.CustomDataType[datatype].parser(value))
    end
end

function M.OnRowEnd()
    M.file:WriteLine()
    M.file:WriteRightBrace()
end

function M.End(colsName,dataType,fieldConstraints)
    M.file:WriteRightBrace()

    local _, keys= IsMulKey(colsName,fieldConstraints, dataType)
    local params, args, args2 = MakeKeyParams(keys)

    M.file:WriteLine()
    M.file:WriteLine('M.keyIndex = {}')
    M.file:WriteLine('for _,v in pairs(M.rawdata) do')
    M.file:WriteLine('\tM.keyIndex['..args..'] = v')
    M.file:WriteLine('end')

    M.file:WriteLine()

    M.file:WriteLine('function M.find('..params..')')
    M.file:WriteLine('\treturn M.keyIndex['..args2..']')
    M.file:WriteLine('end')
    M.file:WriteLine()
    M.file:WriteLine('return M')
    M.file:Close()
end

function M.find(a1,a2)
    return M.keyIndex[a1..a2]
end

M.CustomDataType = {}

--如果自定义类型没有解析方法，按string处理
M.CustomDataType.__index = function()
    return {
        define = 'string',
        parser = function(data)
            return '"'..tostring(data)..'"'
        end
    }
end

setmetatable(M.CustomDataType,M.CustomDataType)

--自定义数据类型
M.CustomDataType['array<int>'] = {
    parser = function(data)
        if type(data) ~= 'string' then
            return "{}"
        end
        local tmp = {}
        for it in string.gmatch(data, "(%w+)") do
            table.insert(tmp,Common.checkint(it))
        end
        return '{'.. table.concat(tmp,',')..'}'
    end
}

--自定义数据类型
M.CustomDataType['pairarray<int,int>'] = {
    parser = function(data)
        if type(data) ~= 'string' then
            return ""
        end
        local tmp = {}
        for k,v in string.gmatch(data, "(%w+),(%w+)") do
            table.insert(tmp,string.format("{%d,%d}",Common.checkint(k),Common.checkint(v)))
        end
        return '{'.. table.concat(tmp,',')..'}'
    end
}

return M
