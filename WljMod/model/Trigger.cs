using BaseMod;
using Game;
using cfg;
using System.Collections.Generic;

namespace WljMod;

public class BattleCryTrigger : ElementTrigger
{
    public BattleCryTrigger() : base(EventName.OnLoopElementChange) { }
    public override bool OnTrigger(Entity element, Element elementConf, EventArg rEventArg, out List<int> actionParams)
    {
        actionParams = null;
        var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.BattleCry);
        if (element.EntityType == EEntityType.Element)
        {
            var hasDoneBattleCry = element.GetAttribute(attrId);
            if (hasDoneBattleCry == 0)
            {
                element.SetAttribute(attrId, 1, false); // Mark as done
                var eventId = Plugin.Register.GetEventId((int)Plugin.Event.OnBattleCry);
                for (int i = 0; i < (DongShiZhangPatch.CanBattleCryRepeat ? 2 : 1); i++)
                {
                    Singleton<GameEventManager>.Instance.Dispatch(eventId, [((ElementEntity)element).Index, element.Owner]);
                }
                return true;
            }
        }
        return false;
    }
}

public class OnBattleCryTrigger : ElementTrigger
{
    public OnBattleCryTrigger() : base(Plugin.Register.GetEventId((int)Plugin.Event.OnBattleCry)) { }

    public override bool OnTrigger(Entity element, Element elementConf, EventArg rEventArg, out List<int> actionParams)
    {
        actionParams = [rEventArg.Get<int>(0), rEventArg.Get<int>(1)];
        return element.EntityType == EEntityType.Element;
    }
}

public class OnPlayerTurnEndTrigger : ElementTrigger
{
    public OnPlayerTurnEndTrigger() : base(EventName.OnPlayerTurnEnd) { }

    public override bool OnTrigger(Entity element, Element elementConf, EventArg rEventArg, out List<int> actionParams)
    {
        actionParams = null;
        return element.EntityType == EEntityType.Element;
    }
}

public class OnTiredTrigger : ElementTrigger
{
    public OnTiredTrigger() : base(Plugin.Register.GetEventId((int)Plugin.Event.OnTired)) { }

    public override bool OnTrigger(Entity element, Element elementConf, EventArg rEventArg, out List<int> actionParams)
    {
        actionParams = [rEventArg.Get<int>(0)];
        return element.EntityType == EEntityType.Element;
    }
}