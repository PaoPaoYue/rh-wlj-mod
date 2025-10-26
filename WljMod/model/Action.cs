using Game;
using System;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Feif.UIFramework;
using BaseMod;

namespace WljMod;

// -------------------- Player Actions --------------------

public class ActionSumAddEvolution : EventActionBase
{
    public override void ExcutePlayer(Role rRoleConf)
    {
        int threshold = rRoleConf.Params[0]
            + Singleton<Model>.Instance.Relic.GetRelicGlobalValue(225, base.Owner)
            + Singleton<Model>.Instance.Relic.GetRelicGlobalValue(104, base.Owner);

        if (threshold <= 0) threshold = 1;

        var player = Singleton<Model>.Instance.Buff.GetPlayerEntity(base.Owner);
        int currentValue = player.GetAttribute(23);
        int increment = (base.Param != null && rRoleConf.Params.Count == 2) ? Mathf.Abs(base.Param[0]) : 1;

        currentValue += increment;

        if (currentValue >= threshold)
        {
            player.SetAttribute(23, currentValue % threshold, true);
            Singleton<Model>.Instance.Buff.AddRandomItem(1);
            Singleton<GameEventManager>.Instance.Dispatch(20015, [base.Owner]);
        }
        else
        {
            player.ChangeAttribute(23, increment);
        }

        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
    }
}


// -------------------- Element Actions --------------------

public class ActionTire : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("TiredAction: no trigger value found.");
            return;
        }

        int triggerValue = value[0];
        bool isPrevious = value.Count > 1 && value[1] == 1;
        if (isPrevious)
        {
            int nIndex = (elementData.Index + Singleton<Model>.Instance.Element.LoopItemCount - 1) % Singleton<Model>.Instance.Element.LoopItemCount;
            elementData = Singleton<Model>.Instance.Element.GetElementData(nIndex, base.Owner);
        }

        var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        var eventId = Plugin.Register.GetEventId((int)Plugin.Event.OnTired);
        for (int i = 0; i < (DongShiZhangPatch.CanBattleCryRepeat ? 2 : 1); i++)
        {
            elementData.ChangeAttribute(attrId, triggerValue);
            Singleton<GameEventManager>.Instance.Dispatch(eventId, [elementData.Index, triggerValue, base.Owner]);
        }

        int orderID = base.OrderID;
        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
    }
}

public class ActionSumTire : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSumTire: no trigger value found.");
            return;
        }

        int triggerThreshold = value[0];
        triggerThreshold += Singleton<Model>.Instance.Relic.GetRelicGlobalValue(225, base.Owner);
        triggerThreshold = Math.Max(triggerThreshold, 1);

        int triggerValue = value[1];
        int currentValue = elementData.GetAttribute(23);

        currentValue += 1;

        if (currentValue >= triggerThreshold)
        {
            int nValue = currentValue / triggerThreshold * triggerValue;

            currentValue %= triggerThreshold;
            elementData.SetAttribute(23, currentValue, true);

            var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
            ElementEntity targetElement = elementData;
            bool isPrevious = value.Count > 2 && value[2] == 1;
            if (isPrevious)
            {
                int nIndex = (elementData.Index + Singleton<Model>.Instance.Element.LoopItemCount - 1) % Singleton<Model>.Instance.Element.LoopItemCount;
                targetElement = Singleton<Model>.Instance.Element.GetElementData(nIndex, base.Owner);
            }
            targetElement.ChangeAttribute(attrId, nValue);

            var eventId = Plugin.Register.GetEventId((int)Plugin.Event.OnTired);
            Singleton<GameEventManager>.Instance.Dispatch(eventId, [targetElement.Index, nValue, base.Owner]);
            Singleton<GameEventManager>.Instance.Dispatch(EventName.OnSumEnd, [base.Owner]);
        }
        else
        {
            elementData.ChangeAttribute(23, 1);
        }

        int orderID = base.OrderID;
        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
    }
}

public class ActionBlockOrAttackAdd : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionBlockOrAttackAdd: no trigger value found.");
            return;
        }

        int triggerValue = value[0];

        int previousId = (elementData.Index + Singleton<Model>.Instance.Element.LoopItemCount - 1) % Singleton<Model>.Instance.Element.LoopItemCount;
        int nextId = (elementData.Index + 1) % Singleton<Model>.Instance.Element.LoopItemCount;
        var relicAttrId = Plugin.Register.GetRelicGlobalValueId((int)Plugin.RelicGlobalValue.Alarm);
        bool isAll = Singleton<Model>.Instance.Relic.GetRelicGlobalValue(relicAttrId, base.Owner) > 0;
        bool repeat = DongShiZhangPatch.CanBattleCryRepeat;
        for (int i = 0; i < (repeat ? 2 : 1); i++)
        {
            ChangeBlockOrAttack(Singleton<Model>.Instance.Element.GetElementData(previousId, base.Owner), triggerValue, isAll);
            ChangeBlockOrAttack(Singleton<Model>.Instance.Element.GetElementData(nextId, base.Owner), triggerValue, isAll);
        }

        int orderID = base.OrderID;
        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
    }

    private static void ChangeBlockOrAttack(ElementEntity elementData, int triggerValue, bool all)
    {
        if (elementData == null || !elementData.Fill)
            return;
        if (all)
        {
            elementData.ChangeAttribute(3, triggerValue);
            elementData.ChangeAttribute(14, triggerValue);
            return;
        }
        bool isBlock = UnityEngine.Random.RandomRangeInt(0, 2) == 0;
        int attrId = isBlock ? 14 : 3;
        elementData.ChangeAttribute(attrId, triggerValue);
    }
}

public class ActionTiredCountHeal : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionTiredCountHeal: no trigger value found.");
            return;
        }
        var triggerValue = value[0];

        int attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        int nValue = 0;
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, base.Owner);
            if (tempElement == null || !tempElement.Fill)
                continue;
            nValue += tempElement.GetAttribute(attrId) > 0 ? triggerValue : 0;
        }
        bool repeat = DongShiZhangPatch.CanBattleCryRepeat;
        for (int i = 0; i < (repeat ? 2 : 1); i++)
        {
            var player = Singleton<Model>.Instance.Buff.GetPlayerEntity(base.Owner);
            Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(base.Index, base.Owner);
            Vector3 playerPosition = Singleton<Model>.Instance.Buff.GetPlayerPosition(base.Owner);
            UIAnimText.ShowFlyText(lotteCellPosition, playerPosition, Singleton<Model>.Instance.Buff.GetAttributeIcon(2), nValue.ToString(), delegate
            {
                player.ChangeAttribute(2, nValue);
            });
        }

        if (nValue > 0)
        {
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(base.OrderID, 0.6f);
            return;
        }
        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
    }
}

public class ActionAddCacheAttackAndBlock : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionAddCacheAttackAndBlock: no trigger value found.");
            return;
        }

        int triggerValue = value[0];

        elementData.ChangeCacheAttribute(3, triggerValue);
        elementData.ChangeCacheAttribute(14, triggerValue);

        int orderID = base.OrderID;
        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
    }
}

public class ActionSelfStealAttackFromEnemy : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;
        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSelfStealAttackFromEnemy: no trigger value found.");
            return;
        }

        int nAddType = 3;
        int nAddCount = value[0];

        string attributeIcon = Singleton<Model>.Instance.Buff.GetAttributeIcon(nAddType);
        Entity rEnemy = Singleton<Model>.Instance.Enemy.GetEnemyEntity(base.Owner);
        Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(base.Index, base.Owner);
        Vector3 enemyPosition = Singleton<Model>.Instance.Enemy.GetEnemyPosition(base.Owner);
        int orderID = base.OrderID;
        if (rEnemy.GetAttribute(nAddType) < nAddCount)
        {
            nAddCount = rEnemy.GetAttribute(nAddType);
        }
        if (nAddCount <= 0)
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
            return;
        }
        UIAnimText.ShowFlyText(enemyPosition, lotteCellPosition, attributeIcon, StringUtil.AddNumberToString(nAddCount), delegate
        {
            elementData.ChangeAttribute(nAddType, nAddCount);
            rEnemy.ChangeCacheAttribute(nAddType, -nAddCount);
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
        });
    }
}

public class ActionStealHalfAttrsFromRandomTired : EventActionBase
{
    private static List<int> allowedAttrTypes = [3, 8, 9, 11, 14, 18];

    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;
        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionStealAttrsFromRandomTired: no trigger value found.");
            return;
        }

        int nStealCount = value[0];

        List<ElementEntity> tiredElements = new List<ElementEntity>();
        var tiredAttrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Tired);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            var tempElement = Singleton<Model>.Instance.Element.GetElementData(i, base.Owner);
            if (tempElement != null && tempElement.Fill && tempElement.GetAttribute(tiredAttrId) > 0 && tempElement.Index != elementData.Index)
            {
                tiredElements.Add(tempElement);
            }
        }
        bool hasTired = tiredElements.Count > 0;

        int actualStealCount = Math.Min(nStealCount, tiredElements.Count);
        Plugin.Logger.LogInfo($"ActionStealHalfAttrsFromRandomTired: found {tiredElements.Count} tired elements, stealing from {actualStealCount} of them.");
        for (int i = 0; i < actualStealCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tiredElements.Count);
            var targetElement = tiredElements[randomIndex];
            Vector3 fromlotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(targetElement.Index, base.Owner);
            Vector3 tolotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(elementData.Index, base.Owner);
            SingletonMono<AssetManager>.Instance.InstantiateLink(fromlotteCellPosition, tolotteCellPosition);
            StealHalfAttr(targetElement, elementData);
            tiredElements.RemoveAt(randomIndex);
        }

        int orderID = base.OrderID;
        if (hasTired)
        {
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(orderID, 0.6f);
        }
        else
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
        }
    }

    private void StealHalfAttr(ElementEntity fromElement, ElementEntity toElement)
    {
        foreach (var attrType in allowedAttrTypes)
        {
            fromElement.AttributeDict.TryGetValue(attrType, out int attrValue);
            if (attrValue > 0)
            {
                // upper half
                int stealValue = (attrValue + 1) / 2;
                fromElement.ChangeAttribute(attrType, -stealValue);
                toElement.ChangeAttribute(attrType, stealValue);
            }
        }
    }
}

public class ActionSummonPrepare : EventActionBase
{
    public override void ExcuteElement()
    {
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;
        List<int> otherValue = base.ElementConf.OtherValue;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSummonPrepare: no trigger value found.");
            return;
        }
        if (otherValue.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSummonPrepare: no other value found for element ID.");
            return;
        }

        int triggerValue = value[0];
        int elementID = otherValue[0];
        int nCount = triggerValue + Singleton<Model>.Instance.Relic.GetRelicGlobalValue(271, base.Owner);
        int nIndex = -1;
        if (nCount > 0)
        {
            Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(base.Index, base.Owner);
            for (int i = 0; i < 4; i++)
            {
                if (!Singleton<Model>.Instance.Element.GetPrepareElement(i, base.Owner).Fill)
                {
                    Singleton<Model>.Instance.Element.SetPrepareElement(i, elementID, 1, base.Owner);
                    Vector3 prepareLotteCellPosition = Singleton<Model>.Instance.Element.GetPrepareLotteCellPosition(i, base.Owner);
                    SingletonMono<AssetManager>.Instance.InstantiateLink(lotteCellPosition, prepareLotteCellPosition);
                    nIndex = i;
                    nCount--;
                    if (nCount <= 0)
                    {
                        break;
                    }
                }
            }
        }
        if (nIndex != -1)
        {
            Singleton<SoundManager>.Instance.PlaySound(133);
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(base.OrderID, 0.6f);
        }
        else
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
        }
    }
}

public class ActionSumSelfUpdgrade : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSumSelfUpdgrade: no trigger value found.");
            return;
        }

        int triggerThreshold = value[0];
        triggerThreshold += Singleton<Model>.Instance.Relic.GetRelicGlobalValue(225, base.Owner);
        triggerThreshold = Math.Max(triggerThreshold, 1);

        int currentValue = elementData.GetAttribute(23);

        currentValue += 1;

        if (currentValue >= triggerThreshold)
        {

            currentValue %= triggerThreshold;
            elementData.SetAttribute(23, currentValue, true);


            elementData.Upgrade(false);
            ((LotteCell)Singleton<Model>.Instance.Element.GetLotteCell(base.Index, base.Owner)).OnUpgrade();
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(base.OrderID, 0.6f);
        }
        else
        {
            elementData.ChangeAttribute(23, 1);
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
        }
    }
}

public class ActionInvite : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSumTire: no trigger value found.");
            return;
        }

        int previousId = (elementData.Index + Singleton<Model>.Instance.Element.LoopItemCount - 1) % Singleton<Model>.Instance.Element.LoopItemCount;
        ElementEntity previousElement = Singleton<Model>.Instance.Element.GetElementData(previousId, base.Owner);
        Element previousElementConf = Singleton<Model>.Instance.Element.GetElementConf(previousElement.ID);
        if (previousElement == null || !previousElement.Fill)
        {
            return;
        }

        int triggerThreshold = previousElementConf.Rare;
        triggerThreshold = Math.Max(triggerThreshold, 1);

        int triggerValue = value[0];
        var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Invited);
        int currentValue = previousElement.GetAttribute(attrId);

        int nIndex = -1;
        bool repeat = DongShiZhangPatch.CanBattleCryRepeat;
        for (int i = 0; i < (repeat ? 2 : 1); i++)
        {
            currentValue += 1;
            if (currentValue >= triggerThreshold)
            {
                int nCount = currentValue / triggerThreshold * triggerValue;

                currentValue %= triggerThreshold;
                previousElement.SetAttribute(attrId, currentValue, true);

                Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(previousElement.Index, base.Owner);
                for (int j = 0; j < 4; j++)
                {
                    if (!Singleton<Model>.Instance.Element.GetPrepareElement(j, base.Owner).Fill)
                    {
                        Singleton<Model>.Instance.Element.SetPrepareElement(j, previousElement.ID, 1, base.Owner);
                        Vector3 prepareLotteCellPosition = Singleton<Model>.Instance.Element.GetPrepareLotteCellPosition(j, base.Owner);
                        SingletonMono<AssetManager>.Instance.InstantiateLink(lotteCellPosition, prepareLotteCellPosition);
                        nIndex = j;
                        nCount--;
                        if (nCount <= 0)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                previousElement.ChangeAttribute(attrId, 1);
            }
        }


        if (nIndex != -1)
        {
            Singleton<SoundManager>.Instance.PlaySound(133);
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(base.OrderID, 0.6f);
        }
        else
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
        }
    }
}

public class ActionSummonPrepareNoBattleCry : EventActionBase
{
    public override void ExcuteElement()
    {
        List<int> otherValue = base.ElementConf.OtherValue;

        if (otherValue.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSummonPrepareNoBattleCry: no other value found.");
            return;
        }

        int summonElementId = otherValue[0];
        int nCount = 1 + Singleton<Model>.Instance.Relic.GetRelicGlobalValue(271, base.Owner);
        int nIndex = -1;
        bool repeat = DongShiZhangPatch.CanBattleCryRepeat;
        for (int i = 0; i < (repeat ? 2 : 1); i++)
        {
            Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(base.Index, base.Owner);
            for (int j = 0; j < 4; j++)
            {
                if (!Singleton<Model>.Instance.Element.GetPrepareElement(j, base.Owner).Fill)
                {
                    Singleton<Model>.Instance.Element.SetPrepareElement(j, summonElementId, 1, base.Owner);
                    var summonedElement = Singleton<Model>.Instance.Element.GetPrepareElement(j, base.Owner);
                    summonedElement.SetAttribute(Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.BattleCry), 1, false);
                    Vector3 prepareLotteCellPosition = Singleton<Model>.Instance.Element.GetPrepareLotteCellPosition(j, base.Owner);
                    SingletonMono<AssetManager>.Instance.InstantiateLink(lotteCellPosition, prepareLotteCellPosition);
                    nIndex = j;
                    nCount--;
                    if (nCount <= 0)
                    {
                        break;
                    }
                }
            }
        }

        if (nIndex == -1)
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
        }
        else
        {
            UIGameInfo uIGameInfo = UIFrame.Get<UIGameInfo>() as UIGameInfo;
            ReflectionUtil.TryGetPrivateField(uIGameInfo, "prepareList", out List<PrepareCell> prepareList);
            prepareList[nIndex].UpdateData();
            Singleton<SoundManager>.Instance.PlaySound(133);
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(base.OrderID, 0.6f);
        }
    }
}

public class ActionSumSpecialRefresh : EventActionBase
{
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;

        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSumTire: no trigger value found.");
            return;
        }

        int triggerThreshold = value[0];
        triggerThreshold += Singleton<Model>.Instance.Relic.GetRelicGlobalValue(225, base.Owner);
        triggerThreshold = Math.Max(triggerThreshold, 1);

        int increment = 1;
        if (base.Param != null && base.ElementConf.TriggerParam.Count == 2)
        {
            increment = Mathf.Abs(base.Param[0]);
        }

        int currentValue = elementData.GetAttribute(23);
        currentValue += increment;

        if (currentValue >= triggerThreshold)
        {
            currentValue %= triggerThreshold;
            elementData.SetAttribute(23, currentValue, true);

            UIGameInfo uigameInfo = UIFrame.Get<UIGameInfo>() as UIGameInfo;
            ReflectionUtil.TryGetPrivateField<LuckyInfo>(uigameInfo, "lucky", out LuckyInfo lucky);
            if (lucky == null)
            {
                Plugin.Logger.LogError("Failed to patch ActionSumSpecialRefresh, lucky is null!");
                return;
            }
            int luckyThreshold = Singleton<Model>.Instance.Global.LuckyRefreshCount + Singleton<Model>.Instance.Relic.GetRelicGlobalValue(225, EEntityType.Player);
            if (Singleton<Model>.Instance.Element.LuckyEnable)
            {
                if (Singleton<Model>.Instance.Element.LuckyCount < luckyThreshold)
                {
                    Singleton<Model>.Instance.Element.LuckyCount = luckyThreshold;
                    lucky.UpdateCount(1);
                    return;
                }
            }
            else
            {
                Singleton<Model>.Instance.Element.LuckyCount = luckyThreshold;
                if (Singleton<Model>.Instance.Element.LuckyCount >= luckyThreshold)
                {
                    Singleton<Model>.Instance.Element.LuckyEnable = true;
                    Singleton<Model>.Instance.Element.LuckyCount -= luckyThreshold;
                    lucky.UpdateInfo(1, true);
                    return;
                }
                lucky.UpdateCount(1);
            }

            Singleton<GameEventManager>.Instance.Dispatch(EventName.OnSumEnd, [base.Owner]);
        }
        else
        {
            elementData.ChangeAttribute(23, increment);
        }

        int orderID = base.OrderID;
        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
    }
}


public class ActionSmallRotateAndChange : EventActionBase
{
    public override void ExcuteElement()
    {
        if (!Singleton<Model>.Instance.Element.Running || Singleton<BattleManager>.Instance.TurnType != ETurnType.PlayerTurn)
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
            return;
        }
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> value = base.ElementConf.TriggerValue[base.Level - 1].Value;
        List<int> otherValue = base.ElementConf.OtherValue;
        if (value.Count < 1)
        {
            Plugin.Logger.LogWarning("ActionSmallRotateAndChange: insufficient trigger values.");
            return;
        }
        if (otherValue.Count < 1)
        {
            Plugin.Logger.LogWarning("ActionSmallRotateAndChange: insufficient other values.");
            return;
        }

        int nMaxCount = value[0];
        int summonElementId = otherValue[0];

        ILotteCell lotteCell = Singleton<Model>.Instance.Element.GetLotteCell(base.Index, base.Owner);
        string rIcon = string.Format("element_dice{0}", nMaxCount);
        int nOrderID = base.OrderID;
        int index = base.Index;
        UIAnimText.ShowFlyText(lotteCell.Trans.position, Singleton<Model>.Instance.Buff.GetSmallPointPosition(base.Owner), rIcon, StringUtil.AddNumberToString(nMaxCount), delegate
        {
            Singleton<BattleManager>.Instance.LoopManager.SmallLoopingCount += nMaxCount;
            Singleton<Model>.Instance.Element.DeleteElement(index, true, base.Owner, false);
            Singleton<Model>.Instance.Element.SetElement(index, summonElementId, 1, base.Owner, true);
            Singleton<SoundManager>.Instance.PlaySound(113);
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(nOrderID);
        });
    }
}

public class ActionSummonAndSplitAttr : EventActionBase
{
    private static List<int> allowedAttrTypes = [3, 8, 9, 11, 14, 18];
    public override void ExcuteElement()
    {
        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(base.Index, base.Owner);
        List<int> otherValue = base.ElementConf.OtherValue;
        if (otherValue.Count < 1)
        {
            Plugin.Logger.LogWarning("ActionSummonAndSplitAttr: insufficient other values.");
            return;
        }

        int summonElementId = otherValue[0];
        int nIndex = -1;
        Vector3 sourceLotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(base.Index, base.Owner);
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            ElementEntity tmpElement = Singleton<Model>.Instance.Element.GetElementData(i, base.Owner);
            if (!tmpElement.Fill && !tmpElement.Wait && tmpElement.Enable)
            {
                Singleton<Model>.Instance.Element.SetElement(i, summonElementId, 1, base.Owner, false);
                Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(i, base.Owner);
                SingletonMono<AssetManager>.Instance.InstantiateLink(sourceLotteCellPosition, lotteCellPosition);
                nIndex = i;
                break;
            }
        }
        if (nIndex != -1)
        {
            ElementEntity summoned = Singleton<Model>.Instance.Element.GetElementData(nIndex, base.Owner);
            SplitAttrs(elementData, summoned);
            Singleton<Model>.Instance.Element.CheckRaceCount(summoned, EEntityType.Player, true);
        }
        else
        {
            for (int j = 0; j < 4; j++)
            {
                if (!Singleton<Model>.Instance.Element.GetPrepareElement(j, base.Owner).Fill)
                {
                    Singleton<Model>.Instance.Element.SetPrepareElement(j, summonElementId, 1, base.Owner);
                    Vector3 prepareLotteCellPosition = Singleton<Model>.Instance.Element.GetPrepareLotteCellPosition(j, base.Owner);
                    SingletonMono<AssetManager>.Instance.InstantiateLink(sourceLotteCellPosition, prepareLotteCellPosition);
                    nIndex = j;
                    break;
                }
            }
            if (nIndex != -1)
            {
                ElementEntity summoned = Singleton<Model>.Instance.Element.GetPrepareElement(nIndex, base.Owner);
                SplitAttrs(elementData, summoned);
            }
        }
        if (nIndex != -1)
        {
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(base.OrderID, 0.3f);
        }
        else
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
        }
    }

    private void SplitAttrs(ElementEntity fromElement, ElementEntity toElement)
    {
        foreach (var attrType in allowedAttrTypes)
        {
            int attrValue = fromElement.GetAttribute(attrType);
            if (attrValue > 0)
            {
                int splitValue = (attrValue + 1) / 2;
                fromElement.SetAttribute(attrType, attrValue - splitValue, true);
                toElement.SetAttribute(attrType, splitValue, true);
            }
        }
    }
}

public class ActionSumSpecialChange : EventActionBase
{
    public override void ExcuteElement()
    {

        ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(Index, Owner);
        List<int> value = ElementConf.TriggerValue[Level - 1].Value;
        List<int> otherValue = ElementConf.OtherValue;
        if (value.Count == 0)
        {
            Plugin.Logger.LogError("ActionSumSpecialChange: no trigger value found.");
            return;
        }
        if (otherValue.Count == 0)
        {
            Plugin.Logger.LogError("ActionSumSpecialChange: no other value found for increment.");
            return;
        }

        int summonElementId = otherValue[0];
        int level = elementData.Level;

        int threshold = value[0] + Singleton<Model>.Instance.Relic.GetRelicGlobalValue(225, Owner);
        threshold = Math.Max(threshold, 1);

        int currentAttr = elementData.GetAttribute(23);

        if (currentAttr + 1 >= threshold)
        {

            Singleton<Model>.Instance.Element.DeleteElement(Index, false, Owner, false);
            Singleton<Model>.Instance.Element.SetElement(Index, summonElementId, level, Owner, true);

            Singleton<GameEventManager>.Instance.Dispatch(20015, [Owner]);
        }
        else
        {
            elementData.ChangeAttribute(23, 1);
        }

        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(OrderID);
    }
}

// -------------------- Relic Actions --------------------
public class ActionAddBirdAttrToEmployee : EventActionBase
{
    public override void ExcuteRelic(RelicEntity rRelicEntity, Relics rRelicConf)
    {
        if (rRelicConf.TriggerValue.Count == 0)
        {
            Plugin.Logger.LogError("ActionAddBirdAttrToEmployee: no trigger value found.");
            return;
        }
        int num = rRelicConf.TriggerValue[0];
        Vector3 relicPosition = Singleton<Model>.Instance.Relic.GetRelicPosition(base.Index, base.Owner);
        bool flag = false;
        for (int i = 0; i < Singleton<Model>.Instance.Element.LoopItemCount; i++)
        {
            ElementEntity elementData = Singleton<Model>.Instance.Element.GetElementData(i, base.Owner);
            if (elementData.Fill && elementData.Enable && !elementData.Wait)
            {
                if (ModRegister.IsValidModId(elementData.ID) && elementData.RaceAttrributeDict.Count <= 0)
                {
                    elementData.ChangeRaceAttribute(cfg.element.ERaceType.Bird, 1);
                    num--;
                    Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(i, base.Owner);
                    SingletonMono<AssetManager>.Instance.InstantiateLink(relicPosition, lotteCellPosition);
                    flag = true;
                    if (num == 0)
                    {
                        break;
                    }
                }
            }
        }
        int orderID = base.OrderID;
        if (flag)
        {
            Singleton<Model>.Instance.Element.UpdateAllRaceAttribute(base.Owner);
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(orderID, 0.6f);
            return;
        }
        Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(orderID);
    }
}

public class ActionAddBattleCryElementToCache : EventActionBase
{

    static List<int> battleCryElementIds = [100001, 100003, 100005, 100011, 100012];

    public override void ExcuteRelic(RelicEntity rRelicEntity, Relics rRelicConf)
    {
        if (rRelicConf.TriggerValue.Count == 0)
        {
            Plugin.Logger.LogError("ActionAddBattleCryElementToCache: no trigger value found.");
            return;
        }
        int prob = rRelicConf.TriggerValue[0];
        int rand = UnityEngine.Random.Range(0, 100);
        if (rand < prob)
        {
            var elementModel = Singleton<Model>.Instance.Element;
            var modModel = Singleton<Model>.Instance.Mod;
            if (elementModel.Speical) // skip if in special mode
            {
                return;
            }
            if (ReflectionUtil.TryGetPrivateField(elementModel, "mLockChooseList", out List<int> lockChooseList))
            {
                if (lockChooseList.Count < elementModel.CacheElement.Count)
                {
                    for (int i = 0; i < elementModel.CacheElement.Count; i++)
                    {
                        if (!lockChooseList.Contains(i))
                        {
                            // randomly select a battle cry element
                            int randomIndex = UnityEngine.Random.Range(0, battleCryElementIds.Count);
                            int elementId = battleCryElementIds[randomIndex];
                            elementModel.CacheElement[i] = modModel.ModElementConf.DataMap[elementId];
                        }
                    }

                }
            }
        }
    }
}

public class ActionInviteSelected : EventActionBase
{
    public override void ExcuteRelic(RelicEntity rRelicEntity, Relics rRelicConf)
    {
        List<int> value = rRelicConf.TriggerValue;
        if (value.Count == 0)
        {
            Plugin.Logger.LogWarning("ActionSumTire: no trigger value found.");
            return;
        }
        int prob = value[0];
        int nIndex = -1;

        foreach (var index in Singleton<BattleManager>.Instance.LoopManager.TargetIndex)
        {
            if (UnityEngine.Random.Range(0, 100) >= prob)
            {
                continue;
            }
            ElementEntity previousElement = Singleton<Model>.Instance.Element.GetElementData(index, base.Owner);
            Element previousElementConf = Singleton<Model>.Instance.Element.GetElementConf(previousElement.ID);
            if (previousElement == null || !previousElement.Fill)
            {
                return;
            }

            int triggerThreshold = previousElementConf.Rare;
            triggerThreshold = Math.Max(triggerThreshold, 1);

            var attrId = Plugin.Register.GetEntityAttributeId((int)Plugin.Attribute.Invited);
            int currentValue = previousElement.GetAttribute(attrId);

            currentValue += 1;
            if (currentValue >= triggerThreshold)
            {
                int nCount = currentValue / triggerThreshold;

                currentValue %= triggerThreshold;
                previousElement.SetAttribute(attrId, currentValue, true);

                Vector3 lotteCellPosition = Singleton<Model>.Instance.Element.GetLotteCellPosition(previousElement.Index, base.Owner);
                for (int j = 0; j < 4; j++)
                {
                    if (!Singleton<Model>.Instance.Element.GetPrepareElement(j, base.Owner).Fill)
                    {
                        Singleton<Model>.Instance.Element.SetPrepareElement(j, previousElement.ID, 1, base.Owner);
                        Vector3 prepareLotteCellPosition = Singleton<Model>.Instance.Element.GetPrepareLotteCellPosition(j, base.Owner);
                        SingletonMono<AssetManager>.Instance.InstantiateLink(lotteCellPosition, prepareLotteCellPosition);
                        nIndex = j;
                        nCount--;
                        if (nCount <= 0)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                previousElement.ChangeAttribute(attrId, 1);
            }

        }

        if (nIndex != -1)
        {
            Singleton<SoundManager>.Instance.PlaySound(133);
            Singleton<BattleManager>.Instance.OrderManager.DelayBattleOrderExcuteEnd(base.OrderID, 0.6f);
        }
        else
        {
            Singleton<BattleManager>.Instance.OrderManager.OnBattleOrderExcuteEnd(base.OrderID);
        }
    }
}

public class ActionResetHelmet : EventActionBase
{
    public override void ExcuteRelic(RelicEntity rRelicEntity, Relics rRelicConf)
    {
        HelmetPatch.Active = true;
    }
}