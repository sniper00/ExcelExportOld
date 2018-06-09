package.path = './?.lua;Templates/?.lua;'
local Common = require("Common")

local Generators = {}

--BEGIN_Generator
Generators["Json"] = require('Generator.Json')
Generators["Lua"] = require('Generator.Lua')
Generators["CSharp"] = require('Generator.CSharp')
--END_Generator

local function OnData(dt,checkScript)

    local tableName = dt.TableName
    local columnsName = {}
    local dataType = {}
    local fieldConstraint = {}--字段约束

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
        table.insert(columnsName,colname)
 
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

    -- -- minisize = true, json 将保存数据为数组， false 保存为对象

    for k,v in pairs(Generators) do
        local Enable  = DataExport.GetBoolConfig("Generator."..k..".Checked",false)
        v.Enable = Enable
    end

    for k,v in pairs(Generators)  do
        if not v.IsCode and v.Enable then
            if k == 'Json' then
                v.minisize = DataExport.MinisizeJson
            end

            local OutPath  = DataExport.GetStringConfig("Generator."..k..".Path","")
            if string.len(OutPath)>0 then
                v.Begin(OutPath..'/'.. tableName .. v.Fileext)
            else
                v.Enable = false
            end
        end
    end

    for row = skipRowNum , rows.Count-1 do

        for _,v in pairs(Generators)  do
            if not v.IsCode and v.Enable then
                v.OnRowBegin(row == skipRowNum)
            end
        end


        local rowData = {}

        for col,colname in pairs(columnsName) do
            local datatype = dataType[colname]
            local value =  rows[row][col-1]

            for _,v in pairs(Generators) do
                if not v.IsCode and v.Enable then
                    v.OnColumn(col ==1, value, datatype,colname)
                end
            end

            rowData[colname] = Common.DataConvert(datatype, value)
        end

        if #checkScript > 0 then
            local checker = load(checkScript)()
            local ret = checker(rowData)
            if ret ~= 'ok' then
                DataExport:PushInfo(1,string.format("Row %d Col Named %s",row+2,ret))
            end
        end

        for _,v in pairs(Generators) do
            if not v.IsCode and v.Enable then
                v.OnRowEnd()
            end
        end
    end

    for _,v in pairs(Generators)  do
        if not v.IsCode and v.Enable then
            v.End(columnsName,dataType,fieldConstraint)
        end
    end

    for k,v in pairs(Generators)  do
        if v.IsCode and v.Enable then
            local OutPath  = DataExport.GetStringConfig("Generator."..k..".Path","")
            if string.len(OutPath)>0 then
                v.minisize = DataExport.MinisizeJson
                v.CodeGen(OutPath,tableName,columnsName,dataType,fieldConstraint)
            end
        end
    end

    -- CCharp 代码生成器
    -- CSharpGenerator.minisize = JsonGenerator.minisize
    -- CSharpGenerator.CodeGen(DataExport.CodePath,tableName,columnsName,dataType,fieldConstraint)
end

_G['OnData'] = OnData