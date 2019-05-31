local Buff = Class("Buff", CONST.ClassType.CORE)

function Buff:ctor(inGamePanel,buffId,fromActor,toActor)
    self.buffId = buffId
    self.fromActor = fromActor
    self.toActor = toActor
    self.inGamePanel = inGamePanel
    self.buffUsed = false   --buff在本回合是否使用，在自己回合进行刷新
    self.buffLastRound = 0 --持续回合
    self.isUseNow = false  --buff挂载后是否立即生效

    self:BuffInit()
end


function Buff:DoBuff()
    --根据不同的buff种类对角色进行处理
    if not self.buffUsed then
        self.toActor.curInfo.hp = self.toActor.curInfo.hp - self.fromActor.curInfo.attack
        print("角色" .. self.fromActor.actorHandle .. "对角色" .. self.toActor.actorHandle .. "造成了" .. self.fromActor.curInfo.attack)
        print("角色"..self.toActor.actorHandle .. "剩余生命" ..self.toActor.curInfo.hp )

        self.buffUsed = true
    end
end

function Buff:BuffInit()
    self.buffType = 1
    self.buffLastRound = 0
    self.isUseNow = true
end


return Buff