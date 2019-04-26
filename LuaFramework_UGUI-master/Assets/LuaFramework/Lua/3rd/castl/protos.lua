--[[
    Copyright (c) 2014, Paul Bernier
    
    CASTL is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    CASTL is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.
    You should have received a copy of the GNU Lesser General Public License
    along with CASTL. If not, see <http://www.gnu.org/licenses/>.
--]]

local protos = {}

-- forward declaration
protos.arrayProto = {}
protos.booleanProto = {}
protos.dateProto = {}
protos.functionProto = {}
protos.numberProto = {}
protos.objectProto = {}
protos.regexpProto = {}
protos.stringProto = {}

protos.errorProto = {}
protos.rangeErrorProto = {}
protos.referenceErrorProto = {}
protos.syntaxErrorProto = {}
protos.typeErrorProto = {}
protos.evalErrorProto = {}
protos.uriErrorProto = {}
protos.callSiteProto = {}


-- load definition
protos.loadPrototypesDefinition = function()
    require("3rd.castl.prototype.array")(protos.arrayProto)
    require("3rd.castl.prototype.boolean")(protos.booleanProto)
    require("3rd.castl.prototype.date")(protos.dateProto)
    require("3rd.castl.prototype.function")(protos.functionProto)
    require("3rd.castl.prototype.number")(protos.numberProto)
    require("3rd.castl.prototype.object")(protos.objectProto)
    require("3rd.castl.prototype.regexp")(protos.regexpProto)
    require("3rd.castl.prototype.string")(protos.stringProto)

    require("3rd.castl.prototype.error.error")(protos.errorProto)
    require("3rd.castl.prototype.error.range_error")(protos.rangeErrorProto)
    require("3rd.castl.prototype.error.reference_error")(protos.referenceErrorProto)
    require("3rd.castl.prototype.error.syntax_error")(protos.syntaxErrorProto)
    require("3rd.castl.prototype.error.type_error")(protos.typeErrorProto)
    require("3rd.castl.prototype.error.eval_error")(protos.evalErrorProto)
    require("3rd.castl.prototype.error.uri_error")(protos.uriErrorProto)
    require("3rd.castl.prototype.error.call_site")(protos.callSiteProto)

    for k,proto in pairs(protos) do
        proto.__fromCastl = true
    end
end

return protos
