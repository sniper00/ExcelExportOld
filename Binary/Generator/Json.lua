local FileWriter = require('FileWriter')
local Common = require("Common")

local M = {}

M.Fileext = '.json'

M.minisize = false

function M.Begin(f)
    M.file = FileWriter.new(f)
    M.file:Write("[")
end

function M.OnRowBegin(firstrow)
    if not firstrow then
        M.file:Write(",")
    end
    if M.minisize then
        M.file:Write("[")
    else
        M.file:Write("{")
    end
end

function M.OnColumn(firstcol, value, datatype, colname)
    if not firstcol then
        M.file:Write(",")
    end

    if Common.IsBaseDataType(datatype) and datatype ~= "string" then
		if type(value) ~= 'userdata' then
            value = Common.DataConvert(datatype, value)
            if M.minisize then
                M.file:Write(tostring(value))
            else
                M.file:Write(string.format('"%s":%s',colname,tostring(value)))
            end
        else
            if M.minisize then
                M.file:Write("null")
            else
                M.file:Write(string.format('"%s":null',colname))
            end
        end
    else
        if M.minisize then
            M.file:Write('"' .. tostring(value) .. '"')
        else
            M.file:Write(string.format('"%s":"%s"',colname,tostring(value)))
        end
    end
end

function M.OnRowEnd()
    if M.minisize then
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
