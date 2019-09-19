--[[
通用函数
]]
local math = require('math')

local M = {}

--获取CSharp 字典 Value
function M.DictGetValue(dict, key)
    local bexist, v = dict:TryGetValue(key)
    if bexist then
        return v
    end
    return nil
end

-- 是否是基础数据类型，区分自定义数据类型
function M.IsBaseDataType(datatype)
    datatype = string.lower( datatype )
    if datatype == 'int'
        or datatype == 'uint'
        or datatype == 'long'
        or datatype == 'ulong'
        or datatype == 'float'
        or datatype == 'double'
        or datatype == 'bool'
        or datatype == 'string' then
        return true
    end
    return false
end

function M.DataConvert(datatype, v)
    datatype = string.lower( datatype )
    if datatype == 'int'
        or datatype == 'uint'
        or datatype == 'long'
        or datatype == 'ulong' then
        return M.checkint(v)
    end

    if datatype == 'float'
        or datatype == 'double' then
        return M.checknumber(v)
    end

    if datatype == 'bool' then
        return string.lower(tostring(v)) == 'true'
    end

    if datatype == 'string' then
        return tostring(v)
    end

    return tostring(v)
end

function M.round(value)
    value = M.checknumber(value)
    return math.floor(value + 0.5)
end

function M.checknumber(value, base)
    return tonumber(value, base) or 0
end

function M.checkint(value)
    return M.round(M.checknumber(value))
end

function M.split(input, delimiter, converter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter == '') then return false end
    local pos, arr = 0, {}
    -- for each divider found
    for st, sp in function() return string.find(input, delimiter, pos, true) end do
        local sss = string.sub(input, pos, st - 1)
        if sss and #sss > 0 then
            if converter then
                table.insert(arr, converter(sss))
            else
                table.insert(arr, sss)
            end
        end
        pos = sp + 1
    end

    local sss = string.sub(input, pos)
    if sss and #sss > 0 then
        if converter then
            table.insert(arr, converter(sss))
        else
            table.insert(arr, sss)
        end
    end
    return arr
end

function M.serialize(obj)
    local lua = ""
    local t = type(obj)
    if t == "number" then
        lua = lua .. obj
    elseif t == "boolean" then
        lua = lua .. tostring(obj)
    elseif t == "string" then
        lua = lua .. string.format("%q", obj)
    elseif t == "table" then
        lua = lua .. "{\n"
        for k, v in pairs(obj) do
            lua = lua .. "[" .. M.serialize(k) .. "]=" .. M.serialize(v) .. ",\n"
        end
        local metatable = getmetatable(obj)
        if metatable ~= nil and type(metatable.__index) == "table" then
            for k, v in pairs(metatable.__index) do
                lua = lua .. "[" .. M.serialize(k) .. "]=" .. M.serialize(v) .. ",\n"
            end
        end
        lua = lua .. "}"
    elseif t == "nil" then
        return nil
    else
        error("can not serialize a " .. t .. " type.")
    end
    return lua
end

return M
