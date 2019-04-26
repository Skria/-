local _ENV = require("3rd.castl.runtime")

function printTable(tab)
	print(tostring(tab).."\n".._dumpTable(tab))
end

function _dumpTable(tab)
	local reference = {}
    local function dumpTab(tab,ind)
        if tab == nil then 
            return "nil "
		end
        if tab == _ENV.null then
            return "null "
		end
        if not tab.__prt then
            if (tab.__obj or tab.__cls) and ind ~= nil then
                return tostring(tab).." "
            end
		end
        if reference[tab] then
            return "[reference] "
        end
        reference[tab] = tab
        local str="{ "
        if ind == nil then 
            ind = "  "
		end
        for k,v in pairs(tab) do
            if type(k) == "string" then
                k = tostring(k).." = "
            else
                k = "["..tostring(k).."] = "
            end
            local s = ""
            if type(v) == "nil" then
                s = "nil"
            elseif type(v) == "boolean" then
                if v then
                    s = "true"
                else 
                    s = "false"
                end
            elseif type(v) == "number" then
                s = v
            elseif type(v) == "string" then
                s = "\""..v.."\""
            elseif type(v) == "table" then
                s = dumpTab(v,ind.."  ")
                s = string.sub(s,1,#s-1)
            elseif type(v) == "function" then
                s = "function : "..tostring(v)
            elseif type(v) == "thread" then
                s = "thread : "..tostring(v)
            elseif type(v) == "userdata" then
                s = "userdata : "..tostring(v)
            else
                s = "nuknow : "..tostring(v)
            end
            str = str.."\n"..ind..k..s.." ,"
        end
        local sss = string.sub(str, 1, #str-1)
        if #ind > 0 then
            ind = string.sub(ind, 1, #ind-2)
        end
        sss = sss.."\n"..ind.."}\n"
        return sss
    end
    return dumpTab(tab)
end