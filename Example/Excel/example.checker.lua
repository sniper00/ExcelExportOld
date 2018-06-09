local Check = {}
Check['key1'] ={
    --条件检测函数，参数是行数据
    check = function(rowData)
        local data = rowData['key1']
        return tonumber(data)>=100
    end,
    errmsg = "key1 must >= 100"
}

Check['someid'] ={
    check = function(rowData)
        --检测不同列间的条件关系
        local key1 = rowData['key1']
        local someid = rowData['someid']
        return tonumber(key1)*10 == tonumber(someid)
    end,
    errmsg = "someid must = key1*10"
}

local  function RowCheck(rowData)
    for _,v in pairs(Check) do
        if not v.check(rowData) then
            return v.errmsg
        end
    end
    return "ok"
end

return RowCheck