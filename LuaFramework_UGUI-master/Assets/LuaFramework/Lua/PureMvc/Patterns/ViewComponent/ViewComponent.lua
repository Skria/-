local ViewComponent = Class("ViewComponent", CONST.ClassType.CORE)
ViewComponent._coroutineMap = nil

function ViewComponent:ctor()
    self._coroutineMap = {}
    self._openedFlag = 0
end

function ViewComponent:InitView(gameObject)
    self.gameObject = gameObject
end

function ViewComponent:_OpenView()
    self._openedFlag = 1
end

function ViewComponent:_CloseView()
    self._openedFlag = -1
end

function ViewComponent:_ReopenView()
    self._openedFlag = 1
end

function ViewComponent:OpenView()

end

function ViewComponent:ReopenView()
    
end

function ViewComponent:CloseView()
    self:StopAllCoroutine()
end

function ViewComponent:DestroyView()
    if self.mediator then
        Facade:RemoveMediator(self.mediator:GetMediatorName())
    end
    self:StopAllCoroutine()
    ClassGC:ReleaseInstance(self)
end

function ViewComponent:StartCoroutine(func, ...)
    local cid = StartCoroutine(func, self, ...)
    table.insert(self._coroutineMap, cid)
    return cid
end

function ViewComponent:StopCoroutine(cid)
    StopCoroutine(cid)
    for i,v in ipairs(self._coroutineMap) do
        if v == cid then
            table.remove(self._coroutineMap, i)
            break
        end
    end
end

function ViewComponent:StopAllCoroutine()
    for i,cid in ipairs(self._coroutineMap) do
        StopCoroutine(cid)
    end
    self._coroutineMap = {}
end

function ViewComponent:Find(path)
    if self.gameObject then
        if path == "" or path == nil then
            return self.gameObject
        end
        return self.gameObject.transform:Find(path).gameObject
    end
    return nil
end

function ViewComponent:GetComponentByObj(obj, name)
    if obj then
        return obj:GetComponent(name)
    end
    return nil
end

function ViewComponent:GetComponentByPath(path, name)
    local obj = self:Find(path)
    return self:GetComponentByObj(obj, name)
end

function ViewComponent:IsActive()
    if self.gameObject == nil then
        return false
    end
    return self.gameObject.activeSelf and self._openedFlag >= 0
end

return ViewComponent