﻿// Copyright (c) CypherCore <http://github.com/CypherCore> All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE. See LICENSE file in the project root for full license information.

using Framework.Constants;
using Game.Entities;
using Game.Networking.Packets;
using Game.Scripting;
using Game.Spells;
using System.Collections.Generic;

namespace Scripts.Spells.Azerite
{
    struct SpellIds
    {
        // Strengthinnumbers        
        public const uint StrengthInNumbersTrait = 271546;
        public const uint StrengthInNumbersBuff = 271550;

        // Blessedportents        
            public const uint BlessedPortentsTrait = 267889;
        public const uint BlessedPortentsHeal = 280052;

        // Concentratedmending        
        public const uint ConcentratedMendingTrait = 267882;

        // Bracingchill        
        public const uint BracingChillTrait = 267884;
        public const uint BracingChill = 272276;
        public const uint BracingChillHeal = 272428;
        public const uint BracingChillSearchJumpTarget = 272436;

        // Orbitalprecision        
        public const uint MageFrozenOrb = 84714;

        // Bluroftalons
        public const uint HunterCoordinatedAssault = 266779;
    }

    [Script] // 270658 - Azerite Fortification
    class spell_item_azerite_fortification : AuraScript
    {
        bool CheckProc(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            Spell procSpell = eventInfo.GetProcSpell();
            if (procSpell == null)
                return false;

            return procSpell.GetSpellInfo().HasAura(AuraType.ModStun)
                || procSpell.GetSpellInfo().HasAura(AuraType.ModRoot)
                || procSpell.GetSpellInfo().HasAura(AuraType.ModRoot2)
                || procSpell.GetSpellInfo().HasEffect(SpellEffectName.KnockBack);
        }

        public override void Register()
        {
            DoCheckEffectProc.Add(new CheckEffectProcHandler(CheckProc, 0, AuraType.ProcTriggerSpell));
        }
    }

    [Script] // 271548 - Strength in Numbers
    class spell_item_strength_in_numbers : SpellScript
    {
        public override bool Validate(SpellInfo spellInfo)
        {
            return ValidateSpellInfo(SpellIds.StrengthInNumbersTrait, SpellIds.StrengthInNumbersBuff);
        }

        void TriggerHealthBuff()
        {
            AuraEffect trait = GetCaster().GetAuraEffect(SpellIds.StrengthInNumbersTrait, 0, GetCaster().GetGUID());
            if (trait != null)
            {
                long enemies = GetUnitTargetCountForEffect(0);
                if (enemies != 0)
                    GetCaster().CastSpell(GetCaster(), SpellIds.StrengthInNumbersBuff, new CastSpellExtraArgs(TriggerCastFlags.FullMask)
                        .AddSpellMod(SpellValueMod.BasePoint0, trait.GetAmount())
                        .AddSpellMod(SpellValueMod.AuraStack, (int)enemies));
            }
        }

        public override void Register()
        {
            AfterHit.Add(new HitHandler(TriggerHealthBuff));
        }
    }

    [Script] // 271843 - Blessed Portents
    class spell_item_blessed_portents : AuraScript
    {
        public override bool Validate(SpellInfo spellInfo)
        {
            return ValidateSpellInfo(SpellIds.BlessedPortentsTrait, SpellIds.BlessedPortentsHeal);
        }

        void CheckProc(AuraEffect aurEff, DamageInfo dmgInfo, ref uint absorbAmount)
        {
            if (GetTarget().HealthBelowPctDamaged(50, dmgInfo.GetDamage()))
            {
                Unit caster = GetCaster();
                if (caster != null)
                {
                    AuraEffect trait = caster.GetAuraEffect(SpellIds.BlessedPortentsTrait, 0, caster.GetGUID());
                    if (trait != null)
                        caster.CastSpell(GetTarget(), SpellIds.BlessedPortentsHeal, new CastSpellExtraArgs(TriggerCastFlags.FullMask)
                            .AddSpellMod(SpellValueMod.BasePoint0, trait.GetAmount()));
                }
            }
            else
                PreventDefaultAction();
        }

        public override void Register()
        {
            OnEffectAbsorb.Add(new EffectAbsorbHandler(CheckProc, 0));
        }
    }

    [Script] // 272260 - Concentrated Mending
    class spell_item_concentrated_mending : AuraScript
    {
        public override bool Validate(SpellInfo spellInfo)
        {
            return ValidateSpellInfo(SpellIds.ConcentratedMendingTrait);
        }

        void RecalculateHealAmount(AuraEffect aurEff)
        {
            Unit caster = GetCaster();
            if (caster != null)
            {
                AuraEffect trait = caster.GetAuraEffect(SpellIds.ConcentratedMendingTrait, 0, caster.GetGUID());
                if (trait != null)
                    aurEff.ChangeAmount((int)(trait.GetAmount() * aurEff.GetTickNumber()));
            }
        }

        public override void Register()
        {
            OnEffectUpdatePeriodic.Add(new EffectUpdatePeriodicHandler(RecalculateHealAmount, 0, AuraType.PeriodicHeal));
        }
    }

    [Script] // 272276 - Bracing Chill
    class spell_item_bracing_chill_proc : AuraScript
    {
        public override bool Validate(SpellInfo spellInfo)
        {
            return ValidateSpellInfo(SpellIds.BracingChillTrait, SpellIds.BracingChillHeal, SpellIds.BracingChillSearchJumpTarget);
        }

        bool CheckHealCaster(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            return GetCasterGUID() == eventInfo.GetActor().GetGUID();
        }

        void HandleProc(AuraEffect aurEff, ProcEventInfo procInfo)
        {
            Unit caster = procInfo.GetActor();
            if (!caster)
                return;

            AuraEffect trait = caster.GetAuraEffect(SpellIds.BracingChillTrait, 0, caster.GetGUID());
            if (trait != null)
                caster.CastSpell(procInfo.GetProcTarget(), SpellIds.BracingChillHeal,
                    new CastSpellExtraArgs(TriggerCastFlags.FullMask).AddSpellMod(SpellValueMod.BasePoint0, trait.GetAmount()));

            if (GetStackAmount() > 1)
                caster.CastSpell((WorldObject)null, SpellIds.BracingChillSearchJumpTarget,
                    new CastSpellExtraArgs(TriggerCastFlags.FullMask).AddSpellMod(SpellValueMod.AuraStack, GetStackAmount() - 1));

            Remove();
        }

        public override void Register()
        {
            DoCheckEffectProc.Add(new CheckEffectProcHandler(CheckHealCaster, 0, AuraType.Dummy));
            AfterEffectProc.Add(new EffectProcHandler(HandleProc, 0, AuraType.Dummy));
        }
    }

    [Script] // 272436 - Bracing Chill
    class spell_item_bracing_chill_search_jump_target : SpellScript
    {
        void FilterTarget(List<WorldObject> targets)
        {
            if (targets.Empty())
                return;

            List<WorldObject> copy = new(targets);
            copy.RandomResize(target =>
            {
                return target.IsUnit() && !target.ToUnit().HasAura(SpellIds.BracingChill, GetCaster().GetGUID());
            }, 1);

            if (!copy.Empty())
            {
                // found a preferred target, use that
                targets = copy;
                return;
            }

            WorldObject target = targets.SelectRandom();
            targets.Clear();
            targets.Add(target);
        }

        void MoveAura(uint effIndex)
        {
            GetCaster().CastSpell(GetHitUnit(), SpellIds.BracingChill,
                new CastSpellExtraArgs(TriggerCastFlags.FullMask).AddSpellMod(SpellValueMod.AuraStack, GetSpellValue().AuraStackAmount));
        }

        public override void Register()
        {
            OnObjectAreaTargetSelect.Add(new ObjectAreaTargetSelectHandler(FilterTarget, 0, Targets.UnitDestAreaAlly));
            OnEffectHitTarget.Add(new EffectHandler(MoveAura, 0, SpellEffectName.Dummy));
        }
    }

    [Script] // 272837 - Trample the Weak
    class spell_item_trample_the_weak : AuraScript
    {
        bool CheckHealthPct(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            return eventInfo.GetActor().GetHealthPct() > eventInfo.GetActionTarget().GetHealthPct();
        }

        public override void Register()
        {
            DoCheckEffectProc.Add(new CheckEffectProcHandler(CheckHealthPct, 0, AuraType.ProcTriggerSpell));
        }
    }

    [Script] // 275514 - Orbital Precision
    class spell_item_orbital_precision : AuraScript
    {
        public override bool Validate(SpellInfo spellInfo)
        {
            return ValidateSpellInfo(SpellIds.MageFrozenOrb);
        }

        bool CheckFrozenOrbActive(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            return eventInfo.GetActor().GetAreaTrigger(SpellIds.MageFrozenOrb) != null;
        }

        public override void Register()
        {
            DoCheckEffectProc.Add(new CheckEffectProcHandler(CheckFrozenOrbActive, 0, AuraType.ProcTriggerSpell));
        }
    }

    [Script] // 277966 - Blur of Talons
    class spell_item_blur_of_talons : AuraScript
    {
        public override bool Validate(SpellInfo spellInfo)
        {
            return ValidateSpellInfo(SpellIds.HunterCoordinatedAssault);
        }

        bool CheckCoordinatedAssaultActive(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            return eventInfo.GetActor().HasAura(SpellIds.HunterCoordinatedAssault, eventInfo.GetActor().GetGUID());
        }

        public override void Register()
        {
            DoCheckEffectProc.Add(new CheckEffectProcHandler(CheckCoordinatedAssaultActive, 0, AuraType.ProcTriggerSpell));
        }
    }

    [Script] // 278519 - Divine Right
    class spell_item_divine_right : AuraScript
    {
        bool CheckHealthPct(AuraEffect aurEff, ProcEventInfo eventInfo)
        {
            return eventInfo.GetProcTarget().HasAuraState(AuraStateType.Wounded20Percent, eventInfo.GetSpellInfo(), eventInfo.GetActor());
        }

        public override void Register()
        {
            DoCheckEffectProc.Add(new CheckEffectProcHandler(CheckHealthPct, 0, AuraType.ProcTriggerSpell));
        }
    }

    [Script] // 277253 - Heart of Azeroth
    class spell_item_heart_of_azeroth : AuraScript
    {
        void SetEquippedFlag(AuraEffect effect, AuraEffectHandleModes mode)
        {
            SetState(true);
        }

        void ClearEquippedFlag(AuraEffect effect, AuraEffectHandleModes mode)
        {
            SetState(false);
        }

        void SetState(bool equipped)
        {
            Player target = GetTarget().ToPlayer();
            if (target != null)
            {
                target.ApplyAllAzeriteEmpoweredItemMods(equipped);

                PlayerAzeriteItemEquippedStatusChanged statusChanged = new();
                statusChanged.IsHeartEquipped = equipped;
                target.SendPacket(statusChanged);
            }
        }

        public override void Register()
        {
            OnEffectApply.Add(new EffectApplyHandler(SetEquippedFlag, 0, AuraType.Dummy, AuraEffectHandleModes.Real));
            OnEffectRemove.Add(new EffectApplyHandler(ClearEquippedFlag, 0, AuraType.Dummy, AuraEffectHandleModes.Real));
        }
    }
}
