﻿/*
 * Copyright (C) 2012-2020 CypherCore <http://github.com/CypherCore>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Framework.Constants;
using Game.DataStorage;
using Game.Entities;
using Game.Groups;
using Game.Loots;
using Game.Maps;
using Game.Networking;
using Game.Networking.Packets;
using Game.Spells;
using System.Collections.Generic;

namespace Game
{
    public partial class WorldSession
    {
        [WorldPacketHandler(ClientOpcodes.LootItem)]
        void HandleAutostoreLootItem(LootItemPkt packet)
        {
            Player player = GetPlayer();
            AELootResult aeResult = player.GetAELootView().Count > 1 ? new AELootResult() : null;

            // @todo Implement looting by LootObject guid
            foreach (LootRequest req in packet.Loot)
            {
                Loot loot = null;
                ObjectGuid lguid = player.GetLootWorldObjectGUID(req.Object);

                if (lguid.IsGameObject())
                {
                    GameObject go = player.GetMap().GetGameObject(lguid);

                    // not check distance for GO in case owned GO (fishing bobber case, for example) or Fishing hole GO
                    if (!go || ((go.GetOwnerGUID() != player.GetGUID() && go.GetGoType() != GameObjectTypes.FishingHole) && !go.IsWithinDistInMap(player)))
                    {
                        player.SendLootRelease(lguid);
                        continue;
                    }

                    loot = go.GetLootForPlayer(player);
                }
                else if (lguid.IsItem())
                {
                    Item pItem = player.GetItemByGuid(lguid);

                    if (!pItem)
                    {
                        player.SendLootRelease(lguid);
                        continue;
                    }

                    loot = pItem.GetLootForPlayer(player);
                }
                else if (lguid.IsCorpse())
                {
                    Corpse bones = ObjectAccessor.GetCorpse(player, lguid);
                    if (!bones)
                    {
                        player.SendLootRelease(lguid);
                        continue;
                    }

                    loot = bones.GetLootForPlayer(player);
                }
                else
                {
                    Creature creature = player.GetMap().GetCreature(lguid);
                    if (creature == null)
                    {
                        player.SendLootError(req.Object, lguid, LootError.NoLoot);
                        continue;
                    }

                    if (!creature.IsWithinDistInMap(player, AELootCreatureCheck.LootDistance))
                    {
                        player.SendLootError(req.Object, lguid, LootError.TooFar);
                        continue;
                    }

                    loot = creature.GetLootForPlayer(player);
                    if (creature.IsAlive() != (loot != null && loot.loot_type == LootType.Pickpocketing))
                    {
                        player.SendLootError(req.Object, lguid, LootError.DidntKill);
                        continue;
                    }
                }

                if (loot == null)
                {
                    player.SendLootRelease(lguid);
                    continue;
                }

                player.StoreLootItem(lguid, (byte)(req.LootListID - 1), loot, aeResult);

                // If player is removing the last LootItem, delete the empty container.
                if (loot.IsLooted() && lguid.IsItem())
                    player.GetSession().DoLootRelease(lguid);
            }

            if (aeResult != null)
            {
                foreach (var resultValue in aeResult.GetByOrder())
                {
                    player.SendNewItem(resultValue.item, resultValue.count, false, false, true);
                    player.UpdateCriteria(CriteriaType.LootItem, resultValue.item.GetEntry(), resultValue.count);
                    player.UpdateCriteria(CriteriaType.GetLootByType, resultValue.item.GetEntry(), resultValue.count, (ulong)resultValue.lootType);
                    player.UpdateCriteria(CriteriaType.LootAnyItem, resultValue.item.GetEntry(), resultValue.count);
                }
            }

            Unit.ProcSkillsAndAuras(player, null, new ProcFlagsInit(ProcFlags.Looted), new ProcFlagsInit(), ProcFlagsSpellType.MaskAll, ProcFlagsSpellPhase.None, ProcFlagsHit.None, null, null, null);
        }

        [WorldPacketHandler(ClientOpcodes.LootMoney)]
        void HandleLootMoney(LootMoney lootMoney)
        {
            Player player = GetPlayer();

            foreach (var lootView in player.GetAELootView())
            {
                ObjectGuid guid = lootView.Value;
                Loot loot = null;
                bool shareMoney = true;

                switch (guid.GetHigh())
                {
                    case HighGuid.GameObject:
                        {
                            GameObject go = player.GetMap().GetGameObject(guid);

                            // do not check distance for GO if player is the owner of it (ex. fishing bobber)
                            if (go && (go.GetOwnerGUID() == player.GetGUID() || go.IsWithinDistInMap(player)))
                                loot = go.GetLootForPlayer(player);

                        break;
                        }
                    case HighGuid.Corpse:                               // remove insignia ONLY in BG
                        {
                            Corpse bones = ObjectAccessor.GetCorpse(player, guid);

                            if (bones && bones.IsWithinDistInMap(player, SharedConst.InteractionDistance))
                            {
                                loot = bones.GetLootForPlayer(player);
                            shareMoney = false;
                            }

                            break;
                        }
                    case HighGuid.Item:
                        {
                            Item item = player.GetItemByGuid(guid);
                            if (item)
                            {
                                loot = item.GetLootForPlayer(player);
                            shareMoney = false;
                            }
                            break;
                        }
                    case HighGuid.Creature:
                    case HighGuid.Vehicle:
                    {
                        Creature creature = player.GetMap().GetCreature(guid);
                        if (creature == null)
                        {
                            player.SendLootError(lootView.Key, guid, LootError.NoLoot);
                            continue;
                        }

                        if (!creature.IsWithinDistInMap(player, AELootCreatureCheck.LootDistance))
                        {
                            player.SendLootError(lootView.Key, guid, LootError.TooFar);
                            continue;
                        }

                        loot = creature.GetLootForPlayer(player);
                        if (creature.IsAlive() != (loot != null && loot.loot_type == LootType.Pickpocketing))
                        {
                            player.SendLootError(lootView.Key, guid, LootError.DidntKill);
                            continue;
                        }

                        if (loot != null && loot.loot_type != LootType.Corpse)
                            shareMoney = false;
                        break;
                    }
                    default:
                        continue;                                         // unlootable type
                }

                if (loot == null)
                    continue;

                loot.NotifyMoneyRemoved(player.GetMap());
                if (shareMoney && player.GetGroup() != null)      //item, pickpocket and players can be looted only single player
                {

                    Group group = player.GetGroup();

                    List<Player> playersNear = new();
                    for (GroupReference refe = group.GetFirstMember(); refe != null; refe = refe.Next())
                    {
                        Player member = refe.GetSource();
                        if (!member)
                            continue;

                        if (player.IsAtGroupRewardDistance(member))
                            playersNear.Add(member);
                    }

                    ulong goldPerPlayer = (ulong)(loot.gold / playersNear.Count);

                    foreach (var pl in playersNear)
                    {
                        ulong goldMod = MathFunctions.CalculatePct(goldPerPlayer, pl.GetTotalAuraModifierByMiscValue(AuraType.ModMoneyGain, 1));

                        pl.ModifyMoney((long)(goldPerPlayer + goldMod));
                        pl.UpdateCriteria(CriteriaType.MoneyLootedFromCreatures, goldPerPlayer);

                        LootMoneyNotify packet = new();
                        packet.Money = goldPerPlayer;
                        packet.MoneyMod = goldMod;
                        packet.SoleLooter = playersNear.Count <= 1 ? true : false;
                        pl.SendPacket(packet);
                    }
                }
                else
                {
                    ulong goldMod = MathFunctions.CalculatePct(loot.gold, player.GetTotalAuraModifierByMiscValue(AuraType.ModMoneyGain, 1));

                    player.ModifyMoney((long)(loot.gold + goldMod));
                    player.UpdateCriteria(CriteriaType.MoneyLootedFromCreatures, loot.gold);

                    LootMoneyNotify packet = new();
                    packet.Money = loot.gold;
                    packet.MoneyMod = goldMod;
                    packet.SoleLooter = true; // "You loot..."
                    SendPacket(packet);
                }

                loot.gold = 0;

                // Delete the money loot record from the DB
                if (loot.loot_type == LootType.Item)
                    Global.LootItemStorage.RemoveStoredMoneyForContainer(guid.GetCounter());

                // Delete container if empty
                if (loot.IsLooted() && guid.IsItem())
                    player.GetSession().DoLootRelease(guid);
            }
        }

        class AELootCreatureCheck : ICheck<Creature>
        {
            public static float LootDistance = 30.0f;

            public AELootCreatureCheck(Player looter, ObjectGuid mainLootTarget)
            {
                _looter = looter;
                _mainLootTarget = mainLootTarget;
            }

            public bool Invoke(Creature creature)
            {
                if (creature.IsAlive())
                    return false;

                if (creature.GetGUID() == _mainLootTarget)
                    return false;

                if (!_looter.IsWithinDist(creature, LootDistance))
                    return false;

                return _looter.IsAllowedToLoot(creature);
            }

            Player _looter;
            ObjectGuid _mainLootTarget;
        }

        [WorldPacketHandler(ClientOpcodes.LootUnit)]
        void HandleLoot(LootUnit packet)
        {
            // Check possible cheat
            if (!GetPlayer().IsAlive() || !packet.Unit.IsCreatureOrVehicle())
                return;

            // interrupt cast
            if (GetPlayer().IsNonMeleeSpellCast(false))
                GetPlayer().InterruptNonMeleeSpells(false);

            GetPlayer().RemoveAurasWithInterruptFlags(SpellAuraInterruptFlags.Looting);

            List<Creature> corpses = new();
            AELootCreatureCheck check = new(_player, packet.Unit);
            CreatureListSearcher searcher = new(_player, corpses, check);
            Cell.VisitGridObjects(_player, searcher, AELootCreatureCheck.LootDistance);

            if (!corpses.Empty())
                SendPacket(new AELootTargets((uint)corpses.Count + 1));

            GetPlayer().SendLoot(packet.Unit, LootType.Corpse);

            if (!corpses.Empty())
            {
                // main target
                SendPacket(new AELootTargetsAck());

                foreach (Creature creature in corpses)
                {
                    GetPlayer().SendLoot(creature.GetGUID(), LootType.Corpse, true);
                    SendPacket(new AELootTargetsAck());
                }
            }
        }

        [WorldPacketHandler(ClientOpcodes.LootRelease)]
        void HandleLootRelease(LootRelease packet)
        {
            // cheaters can modify lguid to prevent correct apply loot release code and re-loot
            // use internal stored guid
            if (GetPlayer().HasLootWorldObjectGUID(packet.Unit))
                DoLootRelease(packet.Unit);
        }

        public void DoLootRelease(ObjectGuid lguid)
        {
            Player player = GetPlayer();
            Loot loot;

            if (player.GetLootGUID() == lguid)
                player.SetLootGUID(ObjectGuid.Empty);

            player.SendLootRelease(lguid);
            player.RemoveAELootedWorldObject(lguid);

            player.RemoveUnitFlag(UnitFlags.Looting);

            if (!player.IsInWorld)
                return;

            if (lguid.IsGameObject())
            {
                GameObject go = player.GetMap().GetGameObject(lguid);

                // not check distance for GO in case owned GO (fishing bobber case, for example) or Fishing hole GO
                if (!go || ((go.GetOwnerGUID() != player.GetGUID() && go.GetGoType() != GameObjectTypes.FishingHole) && !go.IsWithinDistInMap(player)))
                    return;

                loot = go.GetLootForPlayer(player);

                if (go.GetGoType() == GameObjectTypes.Door)
                {
                    // locked doors are opened with spelleffect openlock, prevent remove its as looted
                    go.UseDoorOrButton();
                }
                else if (loot.IsLooted() || go.GetGoType() == GameObjectTypes.FishingNode)
                {
                    if (go.GetGoType() == GameObjectTypes.FishingHole)
                    {                                               // The fishing hole used once more
                        go.AddUse();                               // if the max usage is reached, will be despawned in next tick
                        if (go.GetUseCount() >= go.GetGoValue().FishingHole.MaxOpens)
                            go.SetLootState(LootState.JustDeactivated);
                        else
                            go.SetLootState(LootState.Ready);
                    }
                    else
                        go.SetLootState(LootState.JustDeactivated);

                    loot.Clear();
                }
                else
                {
                    // not fully looted object
                    go.SetLootState(LootState.Activated, player);

                    // if the round robin player release, reset it.
                    if (player.GetGUID() == loot.roundRobinPlayer)
                        loot.roundRobinPlayer.Clear();
                }
            }
            else if (lguid.IsCorpse())        // ONLY remove insignia at BG
            {
                Corpse corpse = ObjectAccessor.GetCorpse(player, lguid);
                if (!corpse || !corpse.IsWithinDistInMap(player, SharedConst.InteractionDistance))
                    return;

                loot = corpse.GetLootForPlayer(player);

                if (loot != null && loot.IsLooted())
                {
                    loot.Clear();
                    corpse.RemoveCorpseDynamicFlag(CorpseDynFlags.Lootable);
                }
            }
            else if (lguid.IsItem())
            {
                Item pItem = player.GetItemByGuid(lguid);
                if (!pItem)
                    return;

                ItemTemplate proto = pItem.GetTemplate();

                loot = pItem.GetLootForPlayer(player);

                // destroy only 5 items from stack in case prospecting and milling
                if (loot != null && (loot.loot_type == LootType.Prospecting || loot.loot_type == LootType.Milling))
                {
                    pItem.m_lootGenerated = false;
                    pItem.loot = null;

                    uint count = pItem.GetCount();

                    // >=5 checked in spell code, but will work for cheating cases also with removing from another stacks.
                    if (count > 5)
                        count = 5;

                    player.DestroyItemCount(pItem, ref count, true);
                }
                else
                {
                    // Only delete item if no loot or money (unlooted loot is saved to db) or if it isn't an openable item
                    if ((loot != null && loot.IsLooted()) || !proto.HasFlag(ItemFlags.HasLoot))
                        player.DestroyItem(pItem.GetBagSlot(), pItem.GetSlot(), true);
                }
                return;                                             // item can be looted only single player
            }
            else
            {
                Creature creature = player.GetMap().GetCreature(lguid);
                if (creature == null)
                    return;

                if (!creature.IsWithinDistInMap(player, AELootCreatureCheck.LootDistance))
                    return;

                loot = creature.GetLootForPlayer(player);
                if (creature.IsAlive() != (loot != null && loot.loot_type == LootType.Pickpocketing))
                    return;

                if (loot == null || loot.IsLooted())
                {
                    creature.RemoveDynamicFlag(UnitDynFlags.Lootable);

                    // skip pickpocketing loot for speed, skinning timer reduction is no-op in fact
                    if (!creature.IsAlive())
                        creature.AllLootRemovedFromCorpse();
                }
                else
                {
                    // if the round robin player release, reset it.
                    if (player.GetGUID() == loot.roundRobinPlayer)
                    {
                        loot.roundRobinPlayer.Clear();

                        Group group = player.GetGroup();
                        if (group)
                        {
                            if (group.GetLootMethod() != LootMethod.MasterLoot)
                                group.SendLooter(creature, null);
                        }
                        // force dynflag update to update looter and lootable info
                        creature.m_values.ModifyValue(creature.m_objectData).ModifyValue(creature.m_objectData.DynamicFlags);
                        creature.ForceUpdateFieldChange();
                    }
                }
            }

            //Player is not looking at loot list, he doesn't need to see updates on the loot list
            if (loot != null)
                loot.RemoveLooter(player.GetGUID());
        }

        public void DoLootReleaseAll()
        {
            Dictionary<ObjectGuid, ObjectGuid> lootView = _player.GetAELootView();
            foreach (var lootPair in lootView)
                DoLootRelease(lootPair.Value);
        }

        [WorldPacketHandler(ClientOpcodes.MasterLootItem)]
        void HandleLootMasterGive(MasterLootItem masterLootItem)
        {
            AELootResult aeResult = new();

            if (GetPlayer().GetGroup() == null || GetPlayer().GetGroup().GetLooterGuid() != GetPlayer().GetGUID() || GetPlayer().GetGroup().GetLootMethod() != LootMethod.MasterLoot)
            {
                GetPlayer().SendLootError(ObjectGuid.Empty, ObjectGuid.Empty, LootError.DidntKill);
                return;
            }

            // player on other map
            Player target = Global.ObjAccessor.GetPlayer(_player, masterLootItem.Target);
            if (!target)
            {
                GetPlayer().SendLootError(ObjectGuid.Empty, ObjectGuid.Empty, LootError.PlayerNotFound);
                return;
            }

            foreach (LootRequest req in masterLootItem.Loot)
            {
                Loot loot = null;
                ObjectGuid lootguid = _player.GetLootWorldObjectGUID(req.Object);

                if (!_player.IsInRaidWith(target) || !_player.IsInMap(target))
                {
                    _player.SendLootError(req.Object, ObjectGuid.Empty, LootError.MasterOther);
                    Log.outInfo(LogFilter.Cheat, $"MasterLootItem: Player {GetPlayer().GetName()} tried to give an item to ineligible player {target.GetName()} !");
                    return;
                }

                if (GetPlayer().GetLootGUID().IsCreatureOrVehicle())
                {
                    Creature creature = GetPlayer().GetMap().GetCreature(lootguid);
                    if (!creature)
                        return;

                    loot = creature.GetLootForPlayer(_player);
                }
                else if (GetPlayer().GetLootGUID().IsGameObject())
                {
                    GameObject go = GetPlayer().GetMap().GetGameObject(lootguid);
                    if (!go)
                        return;

                    loot = go.GetLootForPlayer(_player);
                }

                if (loot == null)
                    return;

                byte slotid = (byte)(req.LootListID - 1);
                if (slotid >= loot.items.Count + loot.quest_items.Count)
                {
                    Log.outDebug(LogFilter.Loot, $"MasterLootItem: Player {GetPlayer().GetName()} might be using a hack! (slot {slotid}, size {loot.items.Count})");
                    return;
                }

                LootItem item = slotid >= loot.items.Count ? loot.quest_items[slotid - loot.items.Count] : loot.items[slotid];

                List<ItemPosCount> dest = new();
                InventoryResult msg = target.CanStoreNewItem(ItemConst.NullBag, ItemConst.NullSlot, dest, item.itemid, item.count);
                if (!item.AllowedForPlayer(target, true))
                    msg = InventoryResult.CantEquipEver;
                if (msg != InventoryResult.Ok)
                {
                    if (msg == InventoryResult.ItemMaxCount)
                        _player.SendLootError(req.Object, ObjectGuid.Empty, LootError.MasterUniqueItem);
                    else if (msg == InventoryResult.InvFull)
                        _player.SendLootError(req.Object, ObjectGuid.Empty, LootError.MasterInvFull);
                    else
                        _player.SendLootError(req.Object, ObjectGuid.Empty, LootError.MasterOther);

                    return;
                }

                // now move item from loot to target inventory
                Item newitem = target.StoreNewItem(dest, item.itemid, true, item.randomBonusListId, item.GetAllowedLooters(), item.context, item.BonusListIDs);
                aeResult.Add(newitem, item.count, loot.loot_type);

                // mark as looted
                item.count = 0;
                item.is_looted = true;

                loot.NotifyItemRemoved(slotid, GetPlayer().GetMap());
                --loot.unlootedCount;
            }

            foreach (var resultValue in aeResult.GetByOrder())
            {
                target.SendNewItem(resultValue.item, resultValue.count, false, false, true);
                target.UpdateCriteria(CriteriaType.LootItem, resultValue.item.GetEntry(), resultValue.count);
                target.UpdateCriteria(CriteriaType.GetLootByType, resultValue.item.GetEntry(), resultValue.count, (ulong)resultValue.lootType);
                target.UpdateCriteria(CriteriaType.LootAnyItem, resultValue.item.GetEntry(), resultValue.count);
            }
        }

        [WorldPacketHandler(ClientOpcodes.SetLootSpecialization)]
        void HandleSetLootSpecialization(SetLootSpecialization packet)
        {
            if (packet.SpecID != 0)
            {
                ChrSpecializationRecord chrSpec = CliDB.ChrSpecializationStorage.LookupByKey(packet.SpecID);
                if (chrSpec != null)
                {
                    if (chrSpec.ClassID == (uint)GetPlayer().GetClass())
                        GetPlayer().SetLootSpecId(packet.SpecID);
                }
            }
            else
                GetPlayer().SetLootSpecId(0);
        }
    }
}
