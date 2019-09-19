local FileWriter = require('FileWriter')
local Common = require("Common")

local M = {}

M.Fileext = '.json'

M.minijson = false

function M.Begin(f)
    M.file = FileWriter.new(f)
    M.file:Write("[")
end

function M.OnRow(nrow, firstRow,columnName, fieldConstraint, dataType, row, checker)
    if not firstRow then
        M.file:Write(",")
    end
    if M.minijson then
        M.file:Write("[")
    else
        M.file:Write("{")
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
                rowData[name] = "null"
            end
        else
            rowData[name] = '"'..tostring(value)..'"'
        end
    end

    if checker then
        local res = checker(rowData)
        if res ~= 'ok' then
            DataExport:PushInfo("Error",string.format("Row %d Col Named [%s]",nrow+2,res))
        end
    end

    for col,name in pairs(columnName) do
        if col~=1 then
            M.file:Write(",")
        end
        local value =  rowData[name]
        if M.minijson then
            M.file:Write(tostring(value))
        else
            M.file:Write(string.format('"%s":%s',name,tostring(value)))
        end
    end

    if M.minijson then
        M.file:Write("]")
    else
        M.file:Write("}")
    end
end

function M.End()
    M.file:Write("]")
    M.file:Close()
end

return M
