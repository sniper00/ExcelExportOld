local io = require("io")

local table_insert = table.insert

local FileWriter = {}

FileWriter.__index = FileWriter

function FileWriter.new(f)

    local tb = {}
    tb.path = f
    tb.tableCount = 0
    tb.cache = {}

    setmetatable(tb,FileWriter)

    return tb
end

function FileWriter:WriteLine(s)
    if not s then
        table_insert( self.cache ,'\n')
        return
    end

    for i = 0,self.tableCount-1 do
        table_insert( self.cache ,'\t')
    end
    table_insert( self.cache ,s)
    table_insert( self.cache ,'\n')
end

function FileWriter:WriteLeftBrace()
    self:WriteLine('{')
    self.tableCount = self.tableCount + 1
end

function FileWriter:WriteRightBrace()
    self.tableCount = self.tableCount - 1
    self:WriteLine('}')
end

function FileWriter:Write(s)
    if not s then
        return
    end

    for i = 0,self.tableCount-1 do
        table_insert( self.cache ,'\t')
    end
    table_insert( self.cache ,s)
end

function FileWriter:RawWrite(s)
    if not s then
        return
    end
    table_insert( self.cache ,s)
end

function FileWriter:Close()
    DataExport:WriteFile(self.path,table.concat(self.cache))
end

return FileWriter

