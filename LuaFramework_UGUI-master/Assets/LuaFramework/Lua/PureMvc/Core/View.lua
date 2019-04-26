IView = require("PureMVC/Interfaces/IView")
Observer = require("PureMVC/Patterns/Observer/Observer")

View = Class("View", CONST.ClassType.CORE, {}, IView)

View._instance = nil
View._mediatorMap = nil
View._observerMap = nil
function View:ctor()
    self.record = {}
    self.frameRecord = {}
    self.frameRecordCount = 0
    self.lastFrameTime = 0
    self._mediatorMap = {}
    self._observerMap = {}
    
    self:InitializeView()
end

function View:InitializeView()
end


function View:RegisterObserver(notificationName, observer)
    local lst = self._observerMap[notificationName]
    if lst == nil then
        lst = {}
        self._observerMap[notificationName] = lst
    end
    table.insert(lst, observer)
end


function View:NotifyObservers(notification)
    local name = notification:GetName()
    local lst = self._observerMap[name]
    --[[if AppConst.IsEditor then
        if Time.time == self.lastFrameTime then
            if not self.frameRecord[name] then
                self.frameRecord[name] = 0
            end
            self.frameRecord[name] = self.frameRecord[name] + 1
            self.frameRecordCount = self.frameRecordCount + 1
        else
            print("HandleNotifyInFrame:"..self.frameRecordCount)
            printTable(self.frameRecord)
            self.frameRecord = {}
            self.lastFrameTime = 0
            self.frameRecordCount = 0
        end

        if not self.record[name] then
            self.record[name] = {
                count = 0,
                obs = 0,
            }
        end
        self.record[name].count = self.record[name].count + 1
    end]]

    if lst then
        --[[if AppConst.IsEditor then
            if #lst > self.record[name].obs then
                self.record[name].obs = #lst
            end
        end]]
        for _,observer in ipairs(lst) do
            observer:NotifyObserver(notification)
        end
    end
    if notification then
        notification:Release()
    end
    self.lastFrameTime = Time.time
end

function View:NotifyObserversAsync(notification)
    StartCoroutine(function()
        local lst = self._observerMap[notification:GetName()]
        if lst then
            WaitForEndOfFrame()
            for _,observer in ipairs(lst) do
                observer:NotifyObserver(notification)
            end
        end
        if notification then
            notification:Release()
        end
    end,self)
end

function View:RemoveObserver(notificationName, notifyContext)
    local lst = self._observerMap[notificationName]
    if lst then
        for i,observer in ipairs(lst) do
            if observer:CompareNotifyContext(notifyContext) then
                table.remove(lst, i)
                break
            end
        end
        if #lst == 0 then
            self._observerMap[notificationName] = nil
        end
    end
end

function View:RegisterMediator(mediator)
    local mediatorName = mediator:GetMediatorName()
    self._mediatorMap[mediatorName] = mediator
    local interests = mediator:ListNotificationInterests()
    if interests and mediator._HandleNotification then
        local observer = Observer.New(mediator._HandleNotification, mediator)
        local max = 0
        for k,ntName in pairs(interests) do
            self:RegisterObserver(ntName, observer)
            if k > max then
                max = k
            end
        end
        for i=1,max do
            if not interests[i] then
                LogError("Please Check Index:"..i.." In "..mediatorName.."'s ListNotificationInterests!")
            end
        end
    end
    mediator:OnRegister()
end

function View:RetrieveMediator(mediatorName)
    return self._mediatorMap[mediatorName]
end

function View:RemoveMediator(mediatorName)
    local mediator = self._mediatorMap[mediatorName]
    if mediator == nil then
        return nil
    end
    self._mediatorMap[mediatorName] = nil
    local interests = mediator:ListNotificationInterests()
    if interests then
        for k,ntName in pairs(interests) do
            self:RemoveObserver(ntName, mediator)
        end
    end
    for ntName,v in pairs(mediator._observers) do
        if v then
            self:RemoveObserver(ntName, mediator)
        end
    end
    mediator._observers = nil
    mediator:OnRemove()
    return mediator
end

function View:HasMediator(mediatorName)
    return (self._mediatorMap[mediatorName] ~= nil)
end

function View:Clean()
    local needRemove = {}
    for k,v in pairs(self._mediatorMap) do
        table.insert(needRemove,k)
    end
    for i,k in ipairs(needRemove) do
        local v = self._mediatorMap[k]
        local view = v:GetViewComponent()
        if view then
            view:StopAllCoroutine()
        end
        self:RemoveMediator(k)
    end
end

function View.GetInstance()
    return _G["View"]._instance
end

View._instance = View.New()

return View