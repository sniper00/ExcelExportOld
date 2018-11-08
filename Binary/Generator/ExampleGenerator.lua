local M = {}

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- OutPutFilePath 输出文件路径
function M.Begin(OutPutFilePath)
    -- 这里编写生成开始处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- IsFirstRow 是否是第一行
function M.OnRowBegin(IsFirstRow)
    -- 这里编写行开始的处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- IsFirstCol 是否是第一列
-- value 当前值
-- dataType 值类型
-- colname 列名
function M.OnColumn(IsFirstCol,value,dataType,colname)
    -- 这里编写当前行特定列的处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
function M.OnRowEnd()
    -- 这里编写行结束的处理逻辑
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- columnsName  所有的列名，数据类型 table 数组
-- dataType 所有列对应的数据类型，数据类型 table[columnName] type
-- fieldConstraint 所有列对应的条件约束，数据类型 table[columnName] Constraint
function M.End(columnsName,dataType,fieldConstraint)
    -- 这里编写生成结束处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于配置加载代码生成器 如 C++, C# 加载Json配置文件的代码
-- OutPutFilePath 输出文件路径
-- tableName  ExcelSheet Name
-- columnsName  所有的列名，数据类型 table 数组
-- dataType 所有列对应的数据类型，数据类型 table[columnName] type
-- fieldConstraint 所有列对应的条件约束，数据类型 table[columnName] Constraint
function M.CodeGen(OutPutFilePath,tableName,columnsName,dataType,fieldConstraint  )
    -- body
    -- 参考 CSharp.lua
end

return M