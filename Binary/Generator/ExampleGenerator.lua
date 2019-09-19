local M = {}

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- OutPutFilePath 输出文件路径
function M.Begin(OutPutFilePath)
    -- 这里编写生成开始处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
-- nrow 第几行数据，主要用于打印错误信息
-- firstRow 是否是第一行
-- columnName 所有的列名，数据类型 table 数组
-- fieldConstraint 所有列对应的条件约束，数据类型 table[columnName] Constraint
-- dataType 所有列对应的数据类型，数据类型 table[columnName] type
-- row 行数据，C#对象，数组，下标 0开始
-- checker 条件检测函数
function M.OnRow(nrow, firstRow, columnName, fieldConstraint, dataType, row, checker)
    -- 这里编写行开始的处理逻辑
    -- 参考 Json.lua
end

-- 此函数用于 json,lua,xml 等文本文件，配置加载代码生成器不用定义此函数。
function M.End()
    -- 这里编写生成结束处理逻辑，一般是写文件
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