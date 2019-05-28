local Actor = Class("Actor", CONST.ClassType.CORE)
function Actor:ctor(actorHandle,baseInfo,inGamePanel)
    self.actorHandle = actorHandle
    self.baseInfo = baseInfo
    self.inGamePanel = inGamePanel
    self.buffQuene = {} --人物实际挂载的buff
    self._coroutineMap = {}
    self.curInfo = {
        hp = baseInfo.hp,
        speed = baseInfo.speed,
        attack = baseInfo.attack
    }
end

function Actor:Attack()
   
end

function Actor:BeAttack()
   
end

function Actor:Hpchange()
   
end

function Actor:TurnAction(table,callback)
    self:StartCoroutine(function()
        WaitForSeconds(2)
        print("我行动了 handle:" .. self.actorHandle)
        callback()
    end)
 
end

function Actor:StartCoroutine(func, ...)
    local cid = StartCoroutine(func, self, ...)
    table.insert(self._coroutineMap, cid)
    return cid
end

return Actor