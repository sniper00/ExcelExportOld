local io = require("io")

local M = {}

M.tableCount = 0

function M.Open(f)
    M.file = io.open(f,'w')
    M.tableCount = 0
end

function M.WriteLine(s)
    if not s then
        M.file:write('\n')
        return
    end

    local tabs = ""
    for i = 0,M.tableCount-1 do
        tabs = tabs..'\t'
    end
    M.file:write(tabs..s..'\n')
end

function M.WriteLeftBrace()
    M.WriteLine('{')
    M.tableCount = M.tableCount + 1
end

function M.WriteRightBrace()
    M.tableCount = M.tableCount - 1
    M.WriteLine('}')
end

function M.Close()
    M.file:flush()
    M.file:close()
end

return M

