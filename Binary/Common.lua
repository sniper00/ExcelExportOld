
--[[
    通用函数
]]

local M = {}

--获取CSharp 字典 Value
function M.DictGetValue(dict, key)
    local bexit, v = dict:TryGetValue(key)
    if bexit then
        return v
    end
    return nil
end

-- 是否是基础数据类型，区分自定义数据类型
function M.IsBaseDataType(datatype)
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

return M