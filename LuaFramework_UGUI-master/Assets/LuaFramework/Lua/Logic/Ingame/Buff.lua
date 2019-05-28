local Buff = Class("Buff", CONST.ClassType.CORE)

function Actor:ctor(fromActorHandle,toActorHandle,inGamePanel,buffId)

    self.buffId = buffId
    self.fromActorHandle = fromActorHandle
    self.toActorHandle = toActorHandle
    self.inGamePanel = inGamePanel

end