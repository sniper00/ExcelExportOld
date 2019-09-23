package.path = './?.lua;Generator/?.lua;'

local Path = CS.System.IO.Path
local Directory = CS.System.IO.Directory
local DataExport = DataExport

local Generators = {}

local function LoadGenerator(  )
    local GeneratorFiles = Directory.GetFiles("./Generator",'*.lua*')
    for i=0,GeneratorFiles.Length-1 do
        local filename = Path.GetFileNameWithoutExtension(GeneratorFiles[i])
        Generators[filename] = require(filename)
    end
end

LoadGenerator()

local function OnData(dt,checkScript)
    local tableName = dt.TableName
    local columnName = {}
    local dataType = {}
    local fieldConstraint = {}--字段约束

    local checker = nil
    if #checkScript then
        checker = load(checkScript)()
    end

    local rows = dt.Rows
    local columns = dt.Columns

     --除去特殊字段 至少要有一行数据
    if rows.Count < 3 then
        return
    end

    --第一行 列名
    --第二行 数据类型
    --第三行 主键约束
    for i = 0,columns.Count - 1 do
        local colname = columns[i].ColumnName
        assert(#colname>0, string.format("colname %d must not null",i+1))
        table.insert(columnName,colname)

        local datatype = tostring(rows[0][i])
        assert(#datatype>0, string.format("datatype %d must not null",i+1))
        dataType[colname] = string.lower(datatype)
        local fieldConst = tostring(rows[1][i])
        fieldConstraint[colname] = fieldConst
    end

    local skipRowNum = 2
    for i = skipRowNum,rows.Count-1 do
        local d = tostring(rows[i][0])
        if string.sub(d,1,1) == '#' then
            skipRowNum = skipRowNum + 1
        else
            break
        end
    end

    for k,v in pairs(Generators) do
        local Enable  = DataExport.GetBoolConfig("Generator."..k..".Checked",false)
        local OutPath  = DataExport.GetStringConfig("Generator."..k..".Path","")
        if Enable and not Directory.Exists(OutPath) then
            DataExport:PushInfo("Warn",string.format("Generator [%s] skipped: not set output path.",k))
        end
        v.Enable = Enable and Directory.Exists(OutPath)
    end

    --begin make save path
    for k,v in pairs(Generators) do
        if v.Enable and v.Begin then
            v.minijson = DataExport.MinisizeJson
            local OutPath  = DataExport.GetStringConfig("Generator."..k..".Path","")
            v.Begin(OutPath..'/'.. tableName .. v.Fileext)
        end
    end

    --write row data
    for nrow = skipRowNum , rows.Count-1 do
        local row = rows[nrow]
        for _,v in pairs(Generators)  do
            if v.Enable and v.OnRow then
                v.OnRow(nrow, nrow == skipRowNum, columnName, fieldConstraint, dataType, row, checker)
            end
        end
    end

    --end
    for _,v in pairs(Generators)  do
        if v.Enable and v.End then
            v.End(rows.Count- skipRowNum)
        end
    end

    for k,v in pairs(Generators)  do
        if v.Enable and v.CodeGen then
            local OutPath  = DataExport.GetStringConfig("Generator."..k..".Path","")
            v.minijson = DataExport.MinisizeJson
            v.CodeGen(OutPath,tableName,columnName,dataType,fieldConstraint)
        end
    end
end

_G['OnData'] = OnData