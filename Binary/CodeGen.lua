package.path = './?.lua;Templates/?.lua;'

local CSharpGenerator = require('CSharpGenerator')
--[[
    savePath 代码保存路径
    tbName 表名，一般用来作为生成代码的类名
    colsName 列名
    dataType 列数据类型
    fieldConstraint 列字段约束
]]
local function CodeGen(savePath,tbName,colsName,dataType,fieldConstraint)
    -- CCharp 代码生成器
    CSharpGenerator.CodeGen(savePath,tbName,colsName,dataType,fieldConstraint)
    -- 编写其他语言的 代码生成器
end

_G['CodeGen'] = CodeGen