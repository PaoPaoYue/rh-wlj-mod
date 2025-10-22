using BaseMod;
using BepInEx;
using BepInEx.Logging;

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
    }

    public enum Event
    {
        OnBattleCry = 100_001,
        OnTired = 100_002,
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
        var invincibleTitle = Register.RegisterLocalization(100_007, CN: "无敌");
        var invincibleDesc = Register.RegisterLocalization(100_008, CN: "抵挡一次受到的伤害。");

        // custom attributes
        Register.RegisterEntityAttribute((int)Attribute.BattleCry);
        Register.RegisterVisableAttribute((int)Attribute.Tired, "wlj_attr_tired");
        Register.RegisterVisableAttribute((int)Attribute.Invited, "wlj_attr_invited");
        Register.RegisterVisableAttribute((int)Attribute.Invincible, "wlj_attr_invincible");

        // custom keywords
        Register.RegisterDescTip(100_001, new DescTip(battleCryTitle, battleCryDesc));
        Register.RegisterDescTip(100_002, new DescTip(tiredTitle, tiredDesc));
        Register.RegisterDescTip(100_003, new DescTip(invitedTitle, invitedDesc));
        Register.RegisterDescTip(100_004, new DescTip(invincibleTitle, invincibleDesc));

        // custom events
        Register.RegisterEvent((int)Event.OnBattleCry);
        Register.RegisterEvent((int)Event.OnTired);

        // custom triggers
        Register.RegisterElementTrigger(100_001, new BattleCryTrigger());
        Register.RegisterElementTrigger(100_002, new OnBattleCryTrigger());
        Register.RegisterElementTrigger(100_003, new OnPlayerTurnEndTrigger());
        Register.RegisterElementTrigger(100_004, new OnTiredTrigger());

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
        Register.RegisterEventAction(100_014, new ActionSummonAndSplitAttr());
        Register.RegisterEventAction(100_015, new ActionSumSpecialChange());

        // apply patches
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(BattleLoopManagerPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(BattleManagerPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(DongShiZhangPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(ElementEntityPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(PrepareCellPatch));
        HarmonyLib.Harmony.CreateAndPatchAll(typeof(QiDiaoChanPatch));
    }
}
