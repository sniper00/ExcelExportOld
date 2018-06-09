local M = {}

M.rawdata =
{
	{
		key1 = 99,
		someid = nil,
		rate = 0.7,
		name = "adadada",
		testarray = {1,2,3,4},
		testpairarray = "1,2|3,4|5,6",
		testcustomtype = "a12345",
		isdog = true
	}
	,
	{
		key1 = 102,
		someid = 1020,
		rate = 0.5,
		name = "dadad",
		testarray = {1,2,3,4},
		testpairarray = "1,2|3,4|5,6",
		testcustomtype = "b2345",
		isdog = false
	}
}

M.keyIndex = {}
for _,v in pairs(M.rawdata) do
	M.keyIndex[v.key1 .. v.rate] = v
end

function M.find(key1, rate)
	return M.keyIndex[key1 .. rate]
end

return M
