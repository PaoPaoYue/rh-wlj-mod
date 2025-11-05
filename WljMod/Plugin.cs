using System.Collections.Generic;
using BaseMod;
using BepInEx;
using BepInEx.Logging;
using cfg;

namespace WljMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("BaseMod", BepInDependency.DependencyFlags.HardDependency)]
public class Plugin : BaseUnityPlugin
{
    public static readonly string ModName = "王老菊MOD";
    internal static new ManualLogSource Logger;
    internal static ModRegister Register;

    public enum Attribute
    {
        BattleCry = 100_001,
        Tired = 100_002,
        Invited = 100_003,
        Invincible = 100_004,
        Durability = 100_005,
        SubIcon = 100_006,
    }

    public enum Event
    {
        OnBattleCry = 100_001,
        OnTired = 100_002,
        AfterNormalRefresh = 100_003,
    }

    public enum RelicGlobalValue
    {
        Alarm = 100_001,
        Encourage = 100_002,
        Alchemy = 100_003,
        Fire = 100_004,
    }

    private void Awake()
    {
        Logger = base.Logger;
        Register = ModRegister.Create(ModName);

        // reigster localization
        var battleCryTitle = Register.RegisterLocalization(100_001, CN: "战吼");
        var battleCryDesc = Register.RegisterLocalization(100_002, CN: "首次放入轮盘时触发战吼效果。");
        var tiredTitle = Register.RegisterLocalization(100_003, CN: "摸鱼");
        var tiredDesc = Register.RegisterLocalization(100_004, CN: "一定回合内无法被选中，但是可以触发被动效果。");
        var invitedTitle = Register.RegisterLocalization(100_005, CN: "邀为同道");
        var invitedDesc = Register.RegisterLocalization(100_006, CN: "层数等于单位稀有度（最低为1）时，在准备区获得一个原版复制。");
        var invincibleTitle = Register.RegisterLocalization(100_007, CN: "免伤");
        var invincibleDesc = Register.RegisterLocalization(100_008, CN: "抵挡一次受到的伤害。");
        var durabilityTitle = Register.RegisterLocalization(100_009, CN: "耐久");

        var roleName = Register.RegisterLocalization(200_001, CN: "王老菊");
        var roleDesc = Register.RegisterLocalization(200_002, CN: "每出售<color=#008E11>{0}</color>个未来科技单位，获得一个<sprite=14>。");

        // Custom Roles
        Register.RegisterRole(200_001, ReflectionUtil.CreateReadonly<Role>(
            200_001, // ID
            "wlj",  // Icon
            "wlj_portrait", // Portrait
            "wlj_half", // HalfPortrait
            roleName, // Name
            roleDesc, // Description
            40,  // hp
            26,  // gold
            new UnlockCondition(),
            1,  // enable
            new List<int> { }, // passive
            200_001,  // trigger
            new List<int> { }, // trigger param
            200_001,  // action
            new List<int> { 3 },  // action param
            0,  // sound (unused?)
            new List<int> { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 },  // gold growth by level
            new List<int> { }  // trigger value decrease by level
        ));

        // custom attributes
        Register.RegisterEntityAttribute((int)Attribute.BattleCry);
        Register.RegisterVisableAttribute((int)Attribute.Tired, tiredTitle, "wlj_attr_tired");
        Register.RegisterVisableAttribute((int)Attribute.Invited, invitedTitle, "wlj_attr_invited");
        Register.RegisterVisableAttribute((int)Attribute.Invincible, invincibleTitle, "wlj_attr_invincible");
        Register.RegisterVisableAttribute((int)Attribute.Durability, durabilityTitle, "wlj_attr_durability");
        Register.RegisterEntityAttribute((int)Attribute.SubIcon);

        // custom keywords
        Register.RegisterDescTip(100_001, new DescTip(battleCryTitle, battleCryDesc));
        Register.RegisterDescTip(100_002, new DescTip(tiredTitle, tiredDesc));
        Register.RegisterDescTip(100_003, new DescTip(invitedTitle, invitedDesc));
        Register.RegisterDescTip(100_004, new DescTip(invincibleTitle, invincibleDesc));

        // custom events
        Register.RegisterEvent((int)Event.OnBattleCry);
        Register.RegisterEvent((int)Event.OnTired);
        Register.RegisterEvent((int)Event.AfterNormalRefresh);

        // custom triggers
        Register.RegisterElementTrigger(100_001, new BattleCryTrigger());
        Register.RegisterElementTrigger(100_002, new OnBattleCryTrigger());
        Register.RegisterElementTrigger(100_003, new OnPlayerTurnEndTrigger());
        Register.RegisterElementTrigger(100_004, new OnTiredTrigger());

        Register.RegisterPlayerTrigger(200_001, new SellFutureTechElementTrigger());

        Register.RegisterRelicTrigger(300_001, new AfterNormalRefreshTrigger());

        // custom actions
        Register.RegisterEventAction(100_001, new ActionTire());
        Register.RegisterEventAction(100_002, new ActionSumTire());
        Register.RegisterEventAction(100_003, new ActionBlockOrAttackAdd());
        Register.RegisterEventAction(100_004, new ActionTiredCountHeal());
        Register.RegisterEventAction(100_005, new ActionAddCacheAttackAndBlock());
        Register.RegisterEventAction(100_006, new ActionSelfStealAttackFromEnemy());
        Register.RegisterEventAction(100_007, new ActionStealHalfAttrsFromRandomTired());
        Register.RegisterEventAction(100_008, new ActionSummonPrepare());
        Register.RegisterEventAction(100_009, new ActionSumSelfUpdgrade());
        Register.RegisterEventAction(100_010, new ActionInvite());
        Register.RegisterEventAction(100_011, new ActionSummonPrepareNoBattleCry());
        Register.RegisterEventAction(100_012, new ActionSumSpecialRefresh());
        Register.RegisterEventAction(100_013, new ActionSmallRotateAndChange());
        Register.RegisterEventAction(100_014, new ActionSummonAndSplit());
        Register.RegisterEventAction(100_015, new ActionSumSpecialChange());
        Register.RegisterEventAction(100_016, new ActionSmallRotateSumTire());
        Register.RegisterEventAction(100_017, new ActionSumAllAddAndChange());

        Register.RegisterEventAction(200_001, new ActionSumAddEvolution());

        Register.RegisterEventAction(300_001, new ActionAddBirdAttrToEmployee());
        Register.RegisterEventAction(300_002, new ActionAddBattleCryElementToCache());
        Register.RegisterEventAction(300_003, new ActionInviteSelected());
        Register.RegisterEventAction(300_004, new ActionResetHelmet());

        // global relic values
        Register.RegisterRelicGlobalValue((int)RelicGlobalValue.Alarm);
        Register.RegisterRelicGlobalValue((int)RelicGlobalValue.Encourage);
        Register.RegisterRelicGlobalValue((int)RelicGlobalValue.Alchemy);
        Register.RegisterRelicGlobalValue((int)RelicGlobalValue.Fire);

        // apply patches
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(BattleLoopManagerPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(BattleManagerPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(ElementEntityPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(PrepareCellPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(QiDiaoChanPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(HelmetPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(AlchemyPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(LotteCellPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(UIGameInfoPatch));
    }
}
