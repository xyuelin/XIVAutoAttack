using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using System.Numerics;

namespace XIVComboPlus.Combos;

internal class RDMCombo : CustomComboJob<RDMGauge>
{
    internal override uint JobID => 35;
    protected override bool CanHealSingleSpell => false;
    //看看现在有没有促进

    private protected override BaseAction Raise => new BaseAction(7523, true);
    internal struct Actions
    {

        public static readonly BaseAction
            //震荡
            Jolt = new BaseAction(7503)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //回刺
            Riposte = new BaseAction(7504)
            {
                OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
            },

            //赤闪雷
            Verthunder = new BaseAction(7505)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //短兵相接
            CorpsAcorps = new BaseAction(7506)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Bind1,
                    ObjectStatus.Bind2,
                }
            },

            //赤疾风
            Veraero = new BaseAction(7507)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //散碎
            Scatter = new BaseAction(7509)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤震雷
            Verthunder2 = new BaseAction(16524u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤烈风
            Veraero2 = new BaseAction(16525u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤火炎
            Verfire = new BaseAction(7510)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerfireReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤飞石
            Verstone = new BaseAction(7511)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerstoneReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //交击斩
            Zwerchhau = new BaseAction(7512)
            {
                OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
            },

            //交剑
            Engagement = new BaseAction(16527),

            //飞剑
            Fleche = new BaseAction(7517),

            //连攻
            Redoublement = new BaseAction(7516)
            {
                OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
            },


            //促进
            Acceleration = new BaseAction(7518)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Acceleration },
            },

            //划圆斩
            Moulinet = new BaseAction(7513),

            //赤治疗
            Vercure = new BaseAction(7514, true)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
            },

            //六分反击
            ContreSixte = new BaseAction(7519u),

            //鼓励
            Embolden = new BaseAction(7520, true),

            //倍增
            Manafication = new BaseAction(7521)
            {
                OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50,
            },

            //续斩
            Reprise = new BaseAction(16529),

            //抗死
            MagickBarrier = new BaseAction(25857),

            //赤核爆
            Verflare = new BaseAction(7525),

            //赤神圣
            Verholy = new BaseAction(7526),

            //焦热
            Scorch = new BaseAction(16530),

            //决断
            Resolution = new BaseAction(25858);
    }

    private protected override bool EmergercyAbility( byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        //鼓励要放到魔回刺或者魔Z斩或魔划圆斩之后
        if (nextGCD.ActionID == Actions.Zwerchhau.ActionID || nextGCD.ActionID == Actions.Redoublement.ActionID || nextGCD.ActionID == Actions.Moulinet.ActionID)
        {
            if (Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //开场爆发的时候释放。
        if (GetRightValue(JobGauge.WhiteMana) & GetRightValue(JobGauge.BlackMana))
        {
            if (Actions.Manafication.ShouldUseAction(out act)) return true;
            if (Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
        }
        //倍增要放到魔连攻击之后
        if (JobGauge.ManaStacks == 3 || (Service.ClientState.LocalPlayer.Level < 68 && nextGCD.ActionID != Actions.Zwerchhau.ActionID && nextGCD.ActionID != Actions.Riposte.ActionID))
        {
            if (Actions.Manafication.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if (JobGauge.ManaStacks == 0)
        {
            //即刻咏唱
            if (GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;

            //促进满了就用。 
            if (Actions.Acceleration.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //加个醒梦
        if (GeneralActions.LucidDreaming.ShouldUseAction(out act)) return true;


        //攻击四个能力技。
        if (Actions.ContreSixte.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.Fleche.ShouldUseAction(out act)) return true;
        if (Actions.Engagement.ShouldUseAction(out act, Empty: BaseAction.HaveStatusSelfFromSelf(1239))) return true;

        var target = Service.TargetManager.Target;
        if (Vector3.Distance(Service.ClientState.LocalPlayer.Position, target.Position) - target.HitboxRadius < 1)
        {
            if (Actions.CorpsAcorps.ShouldUseAction(out act)) return true;
        }
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        //如果已经在爆发了，那继续！
        if (CanBreak(lastComboActionID, out act)) return true;

        if (JobGauge.ManaStacks == 3) return false;

        #region 常规输出
        if (Actions.Verfire.ShouldUseAction(out act)) return true;
        if (Actions.Verstone.ShouldUseAction(out act)) return true;

        //试试看散碎
        if (Actions.Scatter.ShouldUseAction(out act)) return true;
        //平衡魔元
        if (JobGauge.WhiteMana < JobGauge.BlackMana)
        {
            if (Actions.Veraero2.ShouldUseAction(out act)) return true;
            if (Actions.Veraero.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.Verthunder2.ShouldUseAction(out act)) return true;
            if (Actions.Verthunder.ShouldUseAction(out act)) return true;
        }
        if (Actions.Jolt.ShouldUseAction(out act)) return true;
        #endregion
        //赤治疗，加即刻。
        if (Actions.Vercure.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out BaseAction act)
    {
        if (Actions.Vercure.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.CorpsAcorps.ShouldUseAction(out act, mustUse: true)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //混乱
        if (GeneralActions.Addle.ShouldUseAction(out act)) return true;
        if (Actions.MagickBarrier.ShouldUseAction(out act)) return true;
        return false;
    }

    //private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    //{
    //    if (Actions.Manafication.ShouldUseAction(out act)) return true;
    //    if (Actions.Embolden.ShouldUseAction(out act, mustUse: true)) return true;
    //    return false;
    //}

    private static bool CanBreak(uint lastComboActionID, out BaseAction act)
    {
        byte level = Service.ClientState.LocalPlayer.Level;
        #region 远程三连
        //如果魔元结晶满了。
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Actions.Verholy.ShouldUseAction(out act, mustUse: true)) return true;
            }
            if (Actions.Verflare.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //如果上一次打了赤神圣或者赤核爆了
        if (lastComboActionID == Actions.Verholy.ActionID || lastComboActionID == Actions.Verflare.ActionID)
        {
            if (Actions.Scorch.ShouldUseAction(out act, mustUse: true)) return true;
        }

        //如果上一次打了焦热
        if (lastComboActionID == Actions.Scorch.ActionID)
        {
            if (Actions.Resolution.ShouldUseAction(out act, mustUse: true)) return true;
        }
        #endregion

        #region 近战三连

        if (lastComboActionID == Actions.Moulinet.ActionID && JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20)
        {
            if (Actions.Moulinet.ShouldUseAction(out act)) return true;
            if (Actions.Riposte.ShouldUseAction(out act)) return true;
        }
        if (Actions.Zwerchhau.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Redoublement.ShouldUseAction(out act, lastComboActionID)) return true;

        //如果倍增好了，或者魔元满了，或者正在爆发，或者处于开场爆发状态，就马上用！
        bool mustStart = BaseAction.HaveStatusSelfFromSelf(1971) || JobGauge.BlackMana == 100 || JobGauge.WhiteMana == 100 || !Actions.Embolden.IsCoolDown;

        //在魔法元没有溢出的情况下，要求较小的魔元不带触发，也可以强制要求跳过判断。
        if (!mustStart)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;

            //要求较小的魔元不带触发，也可以强制要求跳过判断。
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerstoneReady))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerfireReady))
                {
                    return false;
                }
            }

            //看看有没有即刻相关的技能。
            foreach (var buff in Actions.Vercure.BuffsProvide)
            {
                if (BaseAction.HaveStatusSelfFromSelf(buff))
                {
                    return false;
                }
            }

            //如果倍增的时间快到了，但还是没好。
            float emboldenRemain = Actions.Embolden.RecastTimeRemain;
            if (emboldenRemain < 30 && emboldenRemain > 1)
            {
                return false;
            }
        }

        #endregion

        #region 开启爆发

        //要来可以使用近战三连了。
        if (Actions.Moulinet.ShouldUseAction(out act))
        {
            if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60) return true;
        }
        else
        {
            if (JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50 && Actions.Riposte.ShouldUseAction(out act)) return true;
        }

        #endregion
        return false;
    }

}
