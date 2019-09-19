local FileWriter = require('FileWriter')
local Common = require("Common")

local function MakeKey(colsName, fieldConstraints, row)
    local keyname = {}
    for _, v in pairs(colsName) do
        if "key" == fieldConstraints[v] then
            table.insert(keyname, v)
        end
    end

    --没有设置主键，默认第一列为主键
    if #keyname == 0 then
        keyname[1] = colsName[1]
    end

    local res = "["
    if #keyname > 1 then
        res = res.."'"
    end
    for _, v in pairs(keyname) do
        if type(row[v])=="number" then
            res = res..tostring(math.floor(row[v]))
        else
            res = res..row[v]
        end
    end
    if #keyname > 1 then
        res = res.."'"
    end
    res = res.."]"
    return res
end


local M = {}

M.Fileext = '.lua'

function M.Begin(f)
    M.file = FileWriter.new(f)
    M.file:WriteLine("local M =")
    M.file:WriteLeftBrace()
end

function M.OnRow(nrow, firstRow, columnName, fieldConstraint, dataType, row, checker)
    if not firstRow then
        M.file:Write(",\n")
    end

    local rowData = {}
    for ncol, name in pairs(columnName) do
        local value = row[ncol-1]
        local datatype = dataType[name]
        if Common.IsBaseDataType(datatype) and datatype ~= "string" then
            if type(value) ~= 'userdata' then
                value = Common.DataConvert(datatype, value)
                rowData[name] = tostring(value)
            else
                rowData[name] = "nil"
            end
        else
            rowData[name] = M.CustomDataType[datatype].parser(value)
        end
    end

    if checker then
        local res = checker(rowData)
        if res ~= 'ok' then
            DataExport:PushInfo("Error",string.format("Row %d Col Named [%s]",nrow+2,res))
        end
    end

    M.file:WriteLine(MakeKey(columnName, fieldConstraint, rowData)..' = {')
    M.file.tableCount = M.file.tableCount + 1

    for col,name in pairs(columnName) do
        if col~=1 then
            M.file:RawWrite(",\n")
        end
        local value =  rowData[name]
        M.file:Write(name ..' = '..value)
    end

    M.file:WriteLine()
    M.file:WriteRightBrace()
end

function M.End()
    M.file:WriteRightBrace()
    M.file:WriteLine()
    M.file:WriteLine('return M')
    M.file:Close()
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
        return '{'.. table.concat(tmp,', ')..'}'
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
            table.insert(tmp,string.format("{%d, %d}",Common.checkint(k),Common.checkint(v)))
        end
        return '{'.. table.concat(tmp,', ')..'}'
    end
}

return M
