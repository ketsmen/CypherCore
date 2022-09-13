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
using Framework.Database;
using Game.BattleFields;
using Game.BattleGrounds;
using Game.DataStorage;
using Game.Entities;
using Game.Loots;
using Game.Maps;
using Game.Networking;
using Game.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Groups
{
    public class Group
    {
        public Group()
        {
            m_leaderName = "";
            m_groupFlags = GroupFlags.None;
            m_dungeonDifficulty = Difficulty.Normal;
            m_raidDifficulty = Difficulty.NormalRaid;
            m_legacyRaidDifficulty = Difficulty.Raid10N;
            m_lootMethod = LootMethod.FreeForAll;
            m_lootThreshold = ItemQuality.Uncommon;
        }

        public void Update(uint diff)
        {
            if (_isLeaderOffline)
            {
                _leaderOfflineTimer.Update(diff);
                if (_leaderOfflineTimer.Passed())
                {
                    SelectNewPartyOrRaidLeader();
                    _isLeaderOffline = false;
                }
            }

            UpdateReadyCheck(diff);
        }

        void SelectNewPartyOrRaidLeader()
        {
            Player newLeader = null;

            // Attempt to give leadership to main assistant first
            if (IsRaidGroup())
            {
                foreach (var memberSlot in m_memberSlots)
                {
                    if (memberSlot.flags.HasFlag(GroupMemberFlags.Assistant))
                    {
                        Player player = Global.ObjAccessor.FindPlayer(memberSlot.guid);
                        if (player != null)
                        {
                            newLeader = player;
                            break;
                        }
                    }
                }
            }

            // If there aren't assistants in raid, or if the group is not a raid, pick the first available member
            if (!newLeader)
            {
                foreach (var memberSlot in m_memberSlots)
                {
                    Player player = Global.ObjAccessor.FindPlayer(memberSlot.guid);
                    if (player != null)
                    {
                        newLeader = player;
                        break;
                    }
                }
            }

            if (newLeader)
            {
                ChangeLeader(newLeader.GetGUID());
                SendUpdate();
            }
        }

        public bool Create(Player leader)
        {
            ObjectGuid leaderGuid = leader.GetGUID();

            m_guid = ObjectGuid.Create(HighGuid.Party, Global.GroupMgr.GenerateGroupId());
            m_leaderGuid = leaderGuid;
            m_leaderFactionGroup = Player.GetFactionGroupForRace(leader.GetRace());
            m_leaderName = leader.GetName();
            leader.SetPlayerFlag(PlayerFlags.GroupLeader);

            if (IsBGGroup() || IsBFGroup())
            {
                m_groupFlags = GroupFlags.MaskBgRaid;
                m_groupCategory = GroupCategory.Instance;
            }

            if (m_groupFlags.HasAnyFlag(GroupFlags.Raid))
                _initRaidSubGroupsCounter();

            if (!IsLFGGroup())
                m_lootMethod = LootMethod.GroupLoot;

            m_lootThreshold = ItemQuality.Uncommon;
            m_looterGuid = leaderGuid;

            m_dungeonDifficulty = Difficulty.Normal;
            m_raidDifficulty = Difficulty.NormalRaid;
            m_legacyRaidDifficulty = Difficulty.Raid10N;

            if (!IsBGGroup() && !IsBFGroup())
            {
                m_dungeonDifficulty = leader.GetDungeonDifficultyID();
                m_raidDifficulty = leader.GetRaidDifficultyID();
                m_legacyRaidDifficulty = leader.GetLegacyRaidDifficultyID();

                m_dbStoreId = Global.GroupMgr.GenerateNewGroupDbStoreId();

                Global.GroupMgr.RegisterGroupDbStoreId(m_dbStoreId, this);

                // Store group in database
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.INS_GROUP);

                byte index = 0;

                stmt.AddValue(index++, m_dbStoreId);
                stmt.AddValue(index++, m_leaderGuid.GetCounter());
                stmt.AddValue(index++, (byte)m_lootMethod);
                stmt.AddValue(index++, m_looterGuid.GetCounter());
                stmt.AddValue(index++, (byte)m_lootThreshold);
                stmt.AddValue(index++, m_targetIcons[0].GetRawValue());
                stmt.AddValue(index++, m_targetIcons[1].GetRawValue());
                stmt.AddValue(index++, m_targetIcons[2].GetRawValue());
                stmt.AddValue(index++, m_targetIcons[3].GetRawValue());
                stmt.AddValue(index++, m_targetIcons[4].GetRawValue());
                stmt.AddValue(index++, m_targetIcons[5].GetRawValue());
                stmt.AddValue(index++, m_targetIcons[6].GetRawValue());
                stmt.AddValue(index++, m_targetIcons[7].GetRawValue());
                stmt.AddValue(index++, (byte)m_groupFlags);
                stmt.AddValue(index++, (byte)m_dungeonDifficulty);
                stmt.AddValue(index++, (byte)m_raidDifficulty);
                stmt.AddValue(index++, (byte)m_legacyRaidDifficulty);
                stmt.AddValue(index++, m_masterLooterGuid.GetCounter());

                DB.Characters.Execute(stmt);

                ConvertLeaderInstancesToGroup(leader, this, false);

                Cypher.Assert(AddMember(leader)); // If the leader can't be added to a new group because it appears full, something is clearly wrong.
            }
            else if (!AddMember(leader))
                return false;

            return true;
        }

        public void LoadGroupFromDB(SQLFields field)
        {
            m_dbStoreId = field.Read<uint>(17);
            m_guid = ObjectGuid.Create(HighGuid.Party, Global.GroupMgr.GenerateGroupId());
            m_leaderGuid = ObjectGuid.Create(HighGuid.Player, field.Read<ulong>(0));

            // group leader not exist
            var leader = Global.CharacterCacheStorage.GetCharacterCacheByGuid(m_leaderGuid);
            if (leader == null)
                return;

            m_leaderFactionGroup = Player.GetFactionGroupForRace(leader.RaceId);
            m_leaderName = leader.Name;
            m_lootMethod = (LootMethod)field.Read<byte>(1);
            m_looterGuid = ObjectGuid.Create(HighGuid.Player, field.Read<ulong>(2));
            m_lootThreshold = (ItemQuality)field.Read<byte>(3);

            for (byte i = 0; i < MapConst.TargetIconsCount; ++i)
                m_targetIcons[i].SetRawValue(field.Read<byte[]>(4 + i));

            m_groupFlags = (GroupFlags)field.Read<byte>(12);
            if (m_groupFlags.HasAnyFlag(GroupFlags.Raid))
                _initRaidSubGroupsCounter();

            m_dungeonDifficulty = Player.CheckLoadedDungeonDifficultyID((Difficulty)field.Read<byte>(13));
            m_raidDifficulty = Player.CheckLoadedRaidDifficultyID((Difficulty)field.Read<byte>(14));
            m_legacyRaidDifficulty = Player.CheckLoadedLegacyRaidDifficultyID((Difficulty)field.Read<byte>(15));

            m_masterLooterGuid = ObjectGuid.Create(HighGuid.Player, field.Read<ulong>(16));

            if (m_groupFlags.HasAnyFlag(GroupFlags.Lfg))
                Global.LFGMgr._LoadFromDB(field, GetGUID());
        }

        public void LoadMemberFromDB(ulong guidLow, byte memberFlags, byte subgroup, LfgRoles roles)
        {
            MemberSlot member = new();
            member.guid = ObjectGuid.Create(HighGuid.Player, guidLow);

            // skip non-existed member
            var character = Global.CharacterCacheStorage.GetCharacterCacheByGuid(member.guid);
            if (character == null)
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_GROUP_MEMBER);
                stmt.AddValue(0, guidLow);
                DB.Characters.Execute(stmt);
                return;
            }

            member.name = character.Name;
            member.race = character.RaceId;
            member._class = (byte)character.ClassId;
            member.group = subgroup;
            member.flags = (GroupMemberFlags)memberFlags;
            member.roles = roles;
            member.readyChecked = false;

            m_memberSlots.Add(member);

            SubGroupCounterIncrease(subgroup);

            Global.LFGMgr.SetupGroupMember(member.guid, GetGUID());
        }

        public void ConvertToLFG()
        {
            m_groupFlags = (m_groupFlags | GroupFlags.Lfg | GroupFlags.LfgRestricted);
            m_groupCategory = GroupCategory.Instance;
            m_lootMethod = LootMethod.GroupLoot;
            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_TYPE);

                stmt.AddValue(0, (byte)m_groupFlags);
                stmt.AddValue(1, m_dbStoreId);

                DB.Characters.Execute(stmt);
            }

            SendUpdate();
        }

        public void ConvertToRaid()
        {
            m_groupFlags |= GroupFlags.Raid;

            _initRaidSubGroupsCounter();

            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_TYPE);

                stmt.AddValue(0, (byte)m_groupFlags);
                stmt.AddValue(1, m_dbStoreId);

                DB.Characters.Execute(stmt);
            }

            SendUpdate();

            // update quest related GO states (quest activity dependent from raid membership)
            foreach (var member in m_memberSlots)
            {
                Player player = Global.ObjAccessor.FindPlayer(member.guid);
                if (player != null)
                    player.UpdateVisibleGameobjectsOrSpellClicks();
            }
        }

        public void ConvertToGroup()
        {
            if (m_memberSlots.Count > 5)
                return; // What message error should we send?

            m_groupFlags = GroupFlags.None;

            m_subGroupsCounts = null;

            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_TYPE);

                stmt.AddValue(0, (byte)m_groupFlags);
                stmt.AddValue(1, m_dbStoreId);

                DB.Characters.Execute(stmt);
            }

            SendUpdate();

            // update quest related GO states (quest activity dependent from raid membership)
            foreach (var member in m_memberSlots)
            {
                Player player = Global.ObjAccessor.FindPlayer(member.guid);
                if (player != null)
                    player.UpdateVisibleGameobjectsOrSpellClicks();
            }
        }

        public bool AddInvite(Player player)
        {
            if (player == null || player.GetGroupInvite())
                return false;
            Group group = player.GetGroup();
            if (group && (group.IsBGGroup() || group.IsBFGroup()))
                group = player.GetOriginalGroup();
            if (group)
                return false;

            RemoveInvite(player);

            m_invitees.Add(player);

            player.SetGroupInvite(this);

            Global.ScriptMgr.OnGroupInviteMember(this, player.GetGUID());

            return true;
        }

        public bool AddLeaderInvite(Player player)
        {
            if (!AddInvite(player))
                return false;

            m_leaderGuid = player.GetGUID();
            m_leaderFactionGroup = Player.GetFactionGroupForRace(player.GetRace());
            m_leaderName = player.GetName();
            return true;
        }

        public void RemoveInvite(Player player)
        {
            if (player != null)
            {
                m_invitees.Remove(player);
                player.SetGroupInvite(null);
            }
        }

        public void RemoveAllInvites()
        {
            foreach (var pl in m_invitees)
                if (pl != null)
                    pl.SetGroupInvite(null);

            m_invitees.Clear();
        }

        public Player GetInvited(ObjectGuid guid)
        {
            foreach (var pl in m_invitees)
            {
                if (pl != null && pl.GetGUID() == guid)
                    return pl;
            }
            return null;
        }

        public Player GetInvited(string name)
        {
            foreach (var pl in m_invitees)
            {
                if (pl != null && pl.GetName() == name)
                    return pl;
            }
            return null;
        }

        public bool AddMember(Player player)
        {
            // Get first not-full group
            byte subGroup = 0;
            if (m_subGroupsCounts != null)
            {
                bool groupFound = false;
                for (; subGroup < MapConst.MaxRaidSubGroups; ++subGroup)
                {
                    if (m_subGroupsCounts[subGroup] < MapConst.MaxGroupSize)
                    {
                        groupFound = true;
                        break;
                    }
                }
                // We are raid group and no one slot is free
                if (!groupFound)
                    return false;
            }

            MemberSlot member = new();
            member.guid = player.GetGUID();
            member.name = player.GetName();
            member.race = player.GetRace();
            member._class = (byte)player.GetClass();
            member.group = subGroup;
            member.flags = 0;
            member.roles = 0;
            member.readyChecked = false;
            m_memberSlots.Add(member);

            SubGroupCounterIncrease(subGroup);

            player.SetGroupInvite(null);
            if (player.GetGroup() != null)
            {
                if (IsBGGroup() || IsBFGroup()) // if player is in group and he is being added to BG raid group, then call SetBattlegroundRaid()
                    player.SetBattlegroundOrBattlefieldRaid(this, subGroup);
                else //if player is in bg raid and we are adding him to normal group, then call SetOriginalGroup()
                    player.SetOriginalGroup(this, subGroup);
            }
            else //if player is not in group, then call set group
                player.SetGroup(this, subGroup);

            player.SetPartyType(m_groupCategory, GroupType.Normal);
            player.ResetGroupUpdateSequenceIfNeeded(this);

            // if the same group invites the player back, cancel the homebind timer
            player.m_InstanceValid = player.CheckInstanceValidity(false);

            if (!IsRaidGroup())                                      // reset targetIcons for non-raid-groups
            {
                for (byte i = 0; i < MapConst.TargetIconsCount; ++i)
                    m_targetIcons[i].Clear();
            }

            // insert into the table if we're not a Battlegroundgroup
            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.INS_GROUP_MEMBER);

                stmt.AddValue(0, m_dbStoreId);
                stmt.AddValue(1, member.guid.GetCounter());
                stmt.AddValue(2, (byte)member.flags);
                stmt.AddValue(3, member.group);
                stmt.AddValue(4, (byte)member.roles);

                DB.Characters.Execute(stmt);

            }

            SendUpdate();
            Global.ScriptMgr.OnGroupAddMember(this, player.GetGUID());

            if (!IsLeader(player.GetGUID()) && !IsBGGroup() && !IsBFGroup())
            {
                // reset the new member's instances, unless he is currently in one of them
                // including raid/heroic instances that they are not permanently bound to!
                player.ResetInstances(InstanceResetMethod.GroupJoin, false, false);
                player.ResetInstances(InstanceResetMethod.GroupJoin, true, false);
                player.ResetInstances(InstanceResetMethod.GroupJoin, true, true);

                if (player.GetDungeonDifficultyID() != GetDungeonDifficultyID())
                {
                    player.SetDungeonDifficultyID(GetDungeonDifficultyID());
                    player.SendDungeonDifficulty();
                }
                if (player.GetRaidDifficultyID() != GetRaidDifficultyID())
                {
                    player.SetRaidDifficultyID(GetRaidDifficultyID());
                    player.SendRaidDifficulty(false);
                }
                if (player.GetLegacyRaidDifficultyID() != GetLegacyRaidDifficultyID())
                {
                    player.SetLegacyRaidDifficultyID(GetLegacyRaidDifficultyID());
                    player.SendRaidDifficulty(true);
                }
            }
            player.SetGroupUpdateFlag(GroupUpdateFlags.Full);
            Pet pet = player.GetPet();
            if (pet)
                pet.SetGroupUpdateFlag(GroupUpdatePetFlags.Full);

            UpdatePlayerOutOfRange(player);

            // quest related GO state dependent from raid membership
            if (IsRaidGroup())
                player.UpdateVisibleGameobjectsOrSpellClicks();

            {
                // Broadcast new player group member fields to rest of the group
                UpdateData groupData = new(player.GetMapId());
                UpdateObject groupDataPacket;

                // Broadcast group members' fields to player
                for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
                {
                    if (refe.GetSource() == player)
                        continue;

                    Player existingMember = refe.GetSource();
                    if (existingMember != null)
                    {
                        if (player.HaveAtClient(existingMember))
                            existingMember.BuildValuesUpdateBlockForPlayerWithFlag(groupData, UpdateFieldFlag.PartyMember, player);

                        if (existingMember.HaveAtClient(player))
                        {
                            UpdateData newData = new(player.GetMapId());
                            UpdateObject newDataPacket;
                            player.BuildValuesUpdateBlockForPlayerWithFlag(newData, UpdateFieldFlag.PartyMember, existingMember);
                            if (newData.HasData())
                            {
                                newData.BuildPacket(out newDataPacket);
                                existingMember.SendPacket(newDataPacket);
                            }
                        }
                    }
                }

                if (groupData.HasData())
                {
                    groupData.BuildPacket(out groupDataPacket);
                    player.SendPacket(groupDataPacket);
                }
            }

            if (m_maxEnchantingLevel < player.GetSkillValue(SkillType.Enchanting))
                m_maxEnchantingLevel = player.GetSkillValue(SkillType.Enchanting);

            return true;
        }

        public bool RemoveMember(ObjectGuid guid, RemoveMethod method = RemoveMethod.Default, ObjectGuid kicker = default, string reason = null)
        {
            BroadcastGroupUpdate();

            Global.ScriptMgr.OnGroupRemoveMember(this, guid, method, kicker, reason);

            Player player = Global.ObjAccessor.FindConnectedPlayer(guid);
            if (player)
            {
                for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
                {
                    Player groupMember = refe.GetSource();
                    if (groupMember)
                    {
                        if (groupMember.GetGUID() == guid)
                            continue;

                        groupMember.RemoveAllGroupBuffsFromCaster(guid);
                        player.RemoveAllGroupBuffsFromCaster(groupMember.GetGUID());
                    }
                }
            }

            // LFG group vote kick handled in scripts
            if (IsLFGGroup() && method == RemoveMethod.Kick)
                return m_memberSlots.Count != 0;

            // remove member and change leader (if need) only if strong more 2 members _before_ member remove (BG/BF allow 1 member group)
            if (GetMembersCount() > ((IsBGGroup() || IsLFGGroup() || IsBFGroup()) ? 1 : 2))
            {
                if (player)
                {
                    // Battlegroundgroup handling
                    if (IsBGGroup() || IsBFGroup())
                        player.RemoveFromBattlegroundOrBattlefieldRaid();
                    else
                    // Regular group
                    {
                        if (player.GetOriginalGroup() == this)
                            player.SetOriginalGroup(null);
                        else
                            player.SetGroup(null);

                        // quest related GO state dependent from raid membership
                        player.UpdateVisibleGameobjectsOrSpellClicks();


                    }
                    player.SetPartyType(m_groupCategory, GroupType.None);

                    if (method == RemoveMethod.Kick || method == RemoveMethod.KickLFG)
                        player.SendPacket(new GroupUninvite());

                    _homebindIfInstance(player);
                }

                // Remove player from group in DB
                if (!IsBGGroup() && !IsBFGroup())
                {
                    PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_GROUP_MEMBER);
                    stmt.AddValue(0, guid.GetCounter());
                    DB.Characters.Execute(stmt);
                    DelinkMember(guid);
                }

                // Reevaluate group enchanter if the leaving player had enchanting skill or the player is offline
                if (!player || player.GetSkillValue(SkillType.Enchanting) != 0)
                    ResetMaxEnchantingLevel();

                // Remove player from loot rolls
                foreach (var roll in RollId)
                {
                    if (!roll.playerVote.ContainsKey(guid))
                        continue;

                    var vote = roll.playerVote[guid];
                    if (vote == RollType.Greed || vote == RollType.Disenchant)
                        --roll.totalGreed;
                    else if (vote == RollType.Need)
                        --roll.totalNeed;
                    else if (vote == RollType.Pass)
                        --roll.totalPass;

                    if (vote != RollType.NotValid)
                        --roll.totalPlayersRolling;

                    roll.playerVote.Remove(guid);

                    if (roll.totalPass + roll.totalNeed + roll.totalGreed >= roll.totalPlayersRolling)
                        CountTheRoll(roll, null);
                }

                // Update subgroups
                var slot = _getMemberSlot(guid);
                if (slot != null)
                {
                    SubGroupCounterDecrease(slot.group);
                    m_memberSlots.Remove(slot);
                }

                // Pick new leader if necessary
                if (m_leaderGuid == guid)
                {
                    foreach (var member in m_memberSlots)
                    {
                        if (Global.ObjAccessor.FindPlayer(member.guid) != null)
                        {
                            ChangeLeader(member.guid);
                            break;
                        }
                    }
                }

                SendUpdate();

                if (IsLFGGroup() && GetMembersCount() == 1)
                {
                    Player leader = Global.ObjAccessor.FindPlayer(GetLeaderGUID());
                    uint mapId = Global.LFGMgr.GetDungeonMapId(GetGUID());
                    if (mapId == 0 || leader == null || (leader.IsAlive() && leader.GetMapId() != mapId))
                    {
                        Disband();
                        return false;
                    }
                }

                if (m_memberMgr.GetSize() < ((IsLFGGroup() || IsBGGroup()) ? 1 : 2))
                    Disband();
                else if (player)
                {
                    // send update to removed player too so party frames are destroyed clientside
                    SendUpdateDestroyGroupToPlayer(player);
                }

                return true;
            }
            // If group size before player removal <= 2 then disband it
            else
            {
                Disband();
                return false;
            }
        }

        public void ChangeLeader(ObjectGuid newLeaderGuid, sbyte partyIndex = 0)
        {
            var slot = _getMemberSlot(newLeaderGuid);
            if (slot == null)
                return;

            Player newLeader = Global.ObjAccessor.FindPlayer(slot.guid);

            // Don't allow switching leader to offline players
            if (newLeader == null)
                return;

            Global.ScriptMgr.OnGroupChangeLeader(this, newLeaderGuid, m_leaderGuid);

            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt;
                SQLTransaction trans = new();

                // Remove the groups permanent instance bindings
                foreach (var difficultyDic in m_boundInstances.Values)
                {
                    foreach (var pair in difficultyDic.ToList())
                    {
                        // Do not unbind saves of instances that already had map created (a newLeader entered)
                        // forcing a new instance with another leader requires group disbanding (confirmed on retail)
                        if (pair.Value.perm && Global.MapMgr.FindMap(pair.Key, pair.Value.save.GetInstanceId()) == null)
                        {
                            stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_GROUP_INSTANCE_PERM_BINDING);
                            stmt.AddValue(0, m_dbStoreId);
                            stmt.AddValue(1, pair.Value.save.GetInstanceId());
                            trans.Append(stmt);

                            pair.Value.save.RemoveGroup(this);
                            difficultyDic.Remove(pair.Key);
                        }
                    }
                }

                // Copy the permanent binds from the new leader to the group
                ConvertLeaderInstancesToGroup(newLeader, this, true);

                // Update the group leader
                stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_LEADER);

                stmt.AddValue(0, newLeader.GetGUID().GetCounter());
                stmt.AddValue(1, m_dbStoreId);

                trans.Append(stmt);

                DB.Characters.CommitTransaction(trans);
            }

            Player oldLeader = Global.ObjAccessor.FindConnectedPlayer(m_leaderGuid);
            if (oldLeader)
                oldLeader.RemovePlayerFlag(PlayerFlags.GroupLeader);

            newLeader.SetPlayerFlag(PlayerFlags.GroupLeader);
            m_leaderGuid = newLeader.GetGUID();
            m_leaderFactionGroup = Player.GetFactionGroupForRace(newLeader.GetRace());
            m_leaderName = newLeader.GetName();
            ToggleGroupMemberFlag(slot, GroupMemberFlags.Assistant, false);

            GroupNewLeader groupNewLeader = new();
            groupNewLeader.Name = m_leaderName;
            groupNewLeader.PartyIndex = partyIndex;
            BroadcastPacket(groupNewLeader, true);
        }

        public static void ConvertLeaderInstancesToGroup(Player player, Group group, bool switchLeader)
        {
            // copy all binds to the group, when changing leader it's assumed the character
            // will not have any solo binds
            foreach (var difficultyDic in player.m_boundInstances.Values)
            {
                foreach (var pair in difficultyDic)
                {
                    if (!switchLeader || group.GetBoundInstance(pair.Value.save.GetDifficultyID(), pair.Key) == null)
                        if (pair.Value.extendState != 0) // not expired
                            group.BindToInstance(pair.Value.save, pair.Value.perm, false);

                    // permanent binds are not removed
                    if (switchLeader && !pair.Value.perm)
                    {
                        player.UnbindInstance(pair, difficultyDic, false);
                    }
                }
            }

            // if group leader is in a non-raid dungeon map and nobody is actually bound to this map then the group can "take over" the instance
            // (example: two-player group disbanded by disconnect where the player reconnects within 60 seconds and the group is reformed)
            Map playerMap = player.GetMap();
            if (playerMap)
            {
                if (!switchLeader && playerMap.IsNonRaidDungeon())
                {
                    InstanceSave save = Global.InstanceSaveMgr.GetInstanceSave(playerMap.GetInstanceId());
                    if (save != null)
                    {
                        if (save.GetGroupCount() == 0 && save.GetPlayerCount() == 0)
                        {
                            Log.outDebug(LogFilter.Maps, "Group.ConvertLeaderInstancesToGroup: Group for player {0} is taking over unbound instance map {1} with Id {2}", player.GetName(), playerMap.GetId(), playerMap.GetInstanceId());
                            // if nobody is saved to this, then the save wasn't permanent
                            group.BindToInstance(save, false, false);
                        }
                    }
                }
            }
        }

        public void Disband(bool hideDestroy = false)
        {
            Global.ScriptMgr.OnGroupDisband(this);

            Player player;
            foreach (var member in m_memberSlots)
            {
                player = Global.ObjAccessor.FindPlayer(member.guid);
                if (player == null)
                    continue;

                //we cannot call _removeMember because it would invalidate member iterator
                //if we are removing player from Battlegroundraid
                if (IsBGGroup() || IsBFGroup())
                    player.RemoveFromBattlegroundOrBattlefieldRaid();
                else
                {
                    //we can remove player who is in Battlegroundfrom his original group
                    if (player.GetOriginalGroup() == this)
                        player.SetOriginalGroup(null);
                    else
                        player.SetGroup(null);
                }

                player.SetPartyType(m_groupCategory, GroupType.None);

                // quest related GO state dependent from raid membership
                if (IsRaidGroup())
                    player.UpdateVisibleGameobjectsOrSpellClicks();

                if (!hideDestroy)
                    player.SendPacket(new GroupDestroyed());

                SendUpdateDestroyGroupToPlayer(player);

                _homebindIfInstance(player);
            }
            RollId.Clear();
            m_memberSlots.Clear();

            RemoveAllInvites();

            if (!IsBGGroup() && !IsBFGroup())
            {
                SQLTransaction trans = new();

                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_GROUP);
                stmt.AddValue(0, m_dbStoreId);
                trans.Append(stmt);

                stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_GROUP_MEMBER_ALL);
                stmt.AddValue(0, m_dbStoreId);
                trans.Append(stmt);

                DB.Characters.CommitTransaction(trans);

                ResetInstances(InstanceResetMethod.GroupDisband, false, false, null);
                ResetInstances(InstanceResetMethod.GroupDisband, true, false, null);
                ResetInstances(InstanceResetMethod.GroupDisband, true, true, null);

                stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_LFG_DATA);
                stmt.AddValue(0, m_dbStoreId);
                DB.Characters.Execute(stmt);

                Global.GroupMgr.FreeGroupDbStoreId(this);
            }

            Global.GroupMgr.RemoveGroup(this);
        }

        void SendLootStartRollToPlayer(uint countDown, uint mapId, Player p, bool canNeed, Roll r)
        {
            StartLootRoll startLootRoll = new();
            startLootRoll.LootObj = r.GetTarget().GetGUID();
            startLootRoll.MapID = (int)mapId;
            startLootRoll.RollTime = countDown;
            startLootRoll.ValidRolls = r.rollTypeMask;
            if (!canNeed)
                startLootRoll.ValidRolls &= ~RollMask.Need;
            startLootRoll.Method = GetLootMethod();
            r.FillPacket(startLootRoll.Item);

            ItemDisenchantLootRecord disenchant = r.GetItemDisenchantLoot(p);
            if (disenchant != null)
                if (m_maxEnchantingLevel >= disenchant.SkillRequired)
                    startLootRoll.ValidRolls |= RollMask.Disenchant;

            p.SendPacket(startLootRoll);
        }

        void SendLootRoll(ObjectGuid playerGuid, int rollNumber, RollType rollType, Roll roll, bool autoPass = false)
        {
            LootRollBroadcast lootRoll = new();
            lootRoll.LootObj = roll.GetTarget().GetGUID();
            lootRoll.Player = playerGuid;
            lootRoll.Roll = rollNumber;
            lootRoll.RollType = rollType;
            lootRoll.Autopassed = autoPass;
            roll.FillPacket(lootRoll.Item);

            foreach (var pair in roll.playerVote)
            {
                Player p = Global.ObjAccessor.FindConnectedPlayer(pair.Key);
                if (!p || !p.GetSession())
                    continue;

                if (pair.Value != RollType.NotValid)
                    p.SendPacket(lootRoll);
            }
        }

        void SendLootRollWon(ObjectGuid winnerGuid, int rollNumber, RollType rollType, Roll roll)
        {
            LootRollWon lootRollWon = new();
            lootRollWon.LootObj = roll.GetTarget().GetGUID();
            lootRollWon.Winner = winnerGuid;
            lootRollWon.Roll = rollNumber;
            lootRollWon.RollType = rollType;
            roll.FillPacket(lootRollWon.Item);
            lootRollWon.MainSpec = true;    // offspec rolls not implemented

            foreach (var pair in roll.playerVote)
            {
                Player p = Global.ObjAccessor.FindConnectedPlayer(pair.Key);
                if (!p || !p.GetSession())
                    continue;

                if (pair.Value != RollType.NotValid)
                    p.SendPacket(lootRollWon);
            }
        }

        void SendLootAllPassed(Roll roll)
        {
            LootAllPassed lootAllPassed = new();
            lootAllPassed.LootObj = roll.GetTarget().GetGUID();
            roll.FillPacket(lootAllPassed.Item);

            foreach (var pair in roll.playerVote)
            {
                Player player = Global.ObjAccessor.FindConnectedPlayer(pair.Key);
                if (!player || !player.GetSession())
                    continue;

                if (pair.Value != RollType.NotValid)
                    player.SendPacket(lootAllPassed);
            }
        }

        void SendLootRollsComplete(Roll roll)
        {
            LootRollsComplete lootRollsComplete = new();
            lootRollsComplete.LootObj = roll.GetTarget().GetGUID();
            lootRollsComplete.LootListID = (byte)(roll.itemSlot + 1);

            foreach (var pair in roll.playerVote)
            {
                Player player = Global.ObjAccessor.FindConnectedPlayer(pair.Key);
                if (!player || !player.GetSession())
                    continue;

                if (pair.Value != RollType.NotValid)
                    player.SendPacket(lootRollsComplete);
            }
        }

        // notify group members which player is the allowed looter for the given creature
        public void SendLooter(Creature creature, Player groupLooter)
        {
            Cypher.Assert(creature);

            LootList lootList = new();
            lootList.Owner = creature.GetGUID();
            lootList.LootObj = creature.loot.GetGUID();

            if (GetLootMethod() == LootMethod.MasterLoot && creature.loot.HasOverThresholdItem())
                lootList.Master = GetMasterLooterGuid();

            if (groupLooter)
                lootList.RoundRobinWinner = groupLooter.GetGUID();

            BroadcastPacket(lootList, false);
        }

        public bool CanRollOnItem(LootItem item, Player player)
        {
            // Players can't roll on unique items if they already reached the maximum quantity of that item
            ItemTemplate proto = Global.ObjectMgr.GetItemTemplate(item.itemid);
            if (proto == null)
                return false;

            uint itemCount = player.GetItemCount(item.itemid);
            if (proto.GetMaxCount() > 0 && itemCount >= proto.GetMaxCount())
                return false;

            if (!item.AllowedForPlayer(player))
                return false;

            return true;
        }

        public void GroupLoot(Loot loot, WorldObject lootedObject)
        {
            byte itemSlot = 0;
            for (var i = 0; i < loot.items.Count; ++i, ++itemSlot)
            {
                LootItem lootItem = loot.items[i];
                if (lootItem.freeforall)
                    continue;

                ItemTemplate item = Global.ObjectMgr.GetItemTemplate(lootItem.itemid);
                if (item == null)
                    continue;

                //roll for over-threshold item if it's one-player loot
                if (item.GetQuality() >= m_lootThreshold)
                {
                    Roll r = new(lootItem);

                    for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
                    {
                        Player playerToRoll = refe.GetSource();
                        if (!playerToRoll || playerToRoll.GetSession() == null)
                            continue;

                        if (playerToRoll.IsAtGroupRewardDistance(lootedObject))
                        {
                            r.totalPlayersRolling++;
                            RollType vote = playerToRoll.GetPassOnGroupLoot() ? RollType.Pass : RollType.NotEmitedYet;
                            if (!CanRollOnItem(lootItem, playerToRoll))
                            {
                                vote = RollType.Pass;
                                r.totalPass++; // Can't broadcast the pass now. need to wait until all rolling players are known
                            }

                            r.playerVote[playerToRoll.GetGUID()] = vote;
                        }
                    }

                    if (r.totalPlayersRolling > 0)
                    {
                        r.SetLoot(loot);
                        r.itemSlot = itemSlot;

                        if (item.HasFlag(ItemFlags2.CanOnlyRollGreed))
                            r.rollTypeMask &= ~RollMask.Need;

                        loot.items[itemSlot].is_blocked = true;

                        //Broadcast Pass and Send Rollstart
                        foreach (var pair in r.playerVote)
                        {
                            Player p = Global.ObjAccessor.FindPlayer(pair.Key);
                            if (!p || p.GetSession() == null)
                                continue;

                            if (pair.Value == RollType.Pass)
                                SendLootRoll(p.GetGUID(), -1, RollType.Pass, r, true);
                            else
                                SendLootStartRollToPlayer(60000, lootedObject.GetMapId(), p, p.CanRollForItemInLFG(item, lootedObject) == InventoryResult.Ok, r);
                        }

                        RollId.Add(r);
                        Creature creature = lootedObject.ToCreature();
                        GameObject go = lootedObject.ToGameObject();
                        if (creature)
                        {
                            creature.m_groupLootTimer = 60000;
                            creature.lootingGroupLowGUID = GetGUID();
                        }
                        else if (go)
                        {
                            go.m_groupLootTimer = 60000;
                            go.lootingGroupLowGUID = GetGUID();
                        }
                    }
                }
                else
                    lootItem.is_underthreshold = true;
            }

            foreach (var i in loot.quest_items)
            {
                if (!i.follow_loot_rules)
                    continue;

                ItemTemplate item = Global.ObjectMgr.GetItemTemplate(i.itemid);
                Roll r = new(i);

                for (var refe = GetFirstMember(); refe != null; refe = refe.Next())
                {
                    Player playerToRoll = refe.GetSource();
                    if (!playerToRoll || playerToRoll.GetSession() == null)
                        continue;

                    if (playerToRoll.IsAtGroupRewardDistance(lootedObject))
                    {
                        r.totalPlayersRolling++;
                        RollType vote = RollType.NotEmitedYet;
                        if (!CanRollOnItem(i, playerToRoll))
                        {
                            vote = RollType.Pass;
                            ++r.totalPass;
                        }
                        r.playerVote[playerToRoll.GetGUID()] = vote;
                    }
                }

                if (r.totalPlayersRolling > 0)
                {
                    r.SetLoot(loot);
                    r.itemSlot = itemSlot;

                    loot.quest_items[itemSlot - loot.items.Count].is_blocked = true;

                    //Broadcast Pass and Send Rollstart
                    foreach (var pair in r.playerVote)
                    {
                        Player p = Global.ObjAccessor.FindPlayer(pair.Key);
                        if (!p || p.GetSession() == null)
                            continue;

                        if (pair.Value == RollType.Pass)
                            SendLootRoll(p.GetGUID(), -1, RollType.Pass, r, true);
                        else
                            SendLootStartRollToPlayer(60000, lootedObject.GetMapId(), p, p.CanRollForItemInLFG(item, lootedObject) == InventoryResult.Ok, r);
                    }

                    RollId.Add(r);

                    Creature creature = lootedObject.ToCreature();
                    GameObject go = lootedObject.ToGameObject();
                    if (creature)
                    {
                        creature.m_groupLootTimer = 60000;
                        creature.lootingGroupLowGUID = GetGUID();
                    }
                    else if (go)
                    {
                        go.m_groupLootTimer = 60000;
                        go.lootingGroupLowGUID = GetGUID();
                    }
                }
            }
        }

        public void MasterLoot(Loot loot, WorldObject pLootedObject)
        {
            MasterLootCandidateList masterLootCandidateList = new();
            masterLootCandidateList.LootObj = loot.GetGUID();

            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player looter = refe.GetSource();
                if (!looter.IsInWorld)
                    continue;

                if (looter.IsAtGroupRewardDistance(pLootedObject))
                    masterLootCandidateList.Players.Add(looter.GetGUID());
            }

            masterLootCandidateList.Write();

            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player looter = refe.GetSource();
                if (looter.IsAtGroupRewardDistance(pLootedObject))
                    looter.SendPacket(masterLootCandidateList);
            }
        }

        public void CountRollVote(ObjectGuid playerGuid, ObjectGuid lootObjectGuid, byte lootListId, RollType choice)
        {
            var roll = GetRoll(lootObjectGuid, lootListId);
            if (roll == null)
                return;

            // this condition means that player joins to the party after roll begins
            if (!roll.playerVote.ContainsKey(playerGuid))
                return;

            if (roll.GetLoot() != null)
                if (roll.GetLoot().items.Empty())
                    return;

            RollType rollType = RollType.MaxTypes;
            switch (choice)
            {
                case RollType.Pass:                                     // Player choose pass
                    SendLootRoll(playerGuid, -1, RollType.Pass, roll);
                    ++roll.totalPass;
                    rollType = RollType.Pass;
                    break;
                case RollType.Need:                                     // player choose Need
                    SendLootRoll(playerGuid, 0, RollType.Need, roll);
                    ++roll.totalNeed;
                    rollType = RollType.Need;
                    break;
                case RollType.Greed:                                    // player choose Greed
                    SendLootRoll(playerGuid, -7, RollType.Greed, roll);
                    ++roll.totalGreed;
                    rollType = RollType.Greed;
                    break;
                case RollType.Disenchant:                               // player choose Disenchant
                    SendLootRoll(playerGuid, -8, RollType.Disenchant, roll);
                    ++roll.totalGreed;
                    rollType = RollType.Disenchant;
                    break;
            }

            roll.playerVote[playerGuid] = rollType;

            if (roll.totalPass + roll.totalNeed + roll.totalGreed >= roll.totalPlayersRolling)
                CountTheRoll(roll, null);
        }

        public void EndRoll(Loot pLoot, Map allowedMap)
        {
            foreach (var roll in RollId.ToList())
            {
                if (roll.GetLoot() == pLoot)
                {
                    CountTheRoll(roll, allowedMap);           //i don't have to edit player votes, who didn't vote ... he will pass
                }
            }
        }

        void CountTheRoll(Roll roll, Map allowedMap)
        {
            if (!roll.IsValid())                                   // is loot already deleted ?
            {
                RollId.Remove(roll);
                return;
            }

            //end of the roll
            if (roll.totalNeed > 0)
            {
                if (!roll.playerVote.Empty())
                {
                    byte maxresul = 0;
                    ObjectGuid maxguid = ObjectGuid.Empty;
                    Player player;

                    foreach (var pair in roll.playerVote)
                    {
                        if (pair.Value != RollType.Need)
                            continue;

                        player = Global.ObjAccessor.FindPlayer(pair.Key);
                        if (!player || (allowedMap != null && player.GetMap() != allowedMap))
                        {
                            --roll.totalNeed;
                            continue;
                        }

                        byte randomN = (byte)RandomHelper.IRand(1, 100);
                        SendLootRoll(pair.Key, randomN, RollType.Need, roll);
                        if (maxresul < randomN)
                        {
                            maxguid = pair.Key;
                            maxresul = randomN;
                        }
                    }

                    if (!maxguid.IsEmpty())
                    {
                        SendLootRollWon(maxguid, maxresul, RollType.Need, roll);
                        player = Global.ObjAccessor.FindConnectedPlayer(maxguid);

                        if (player && player.GetSession())
                        {
                            player.UpdateCriteria(CriteriaType.RollNeed, roll.itemid, maxresul);

                            List<ItemPosCount> dest = new();
                            LootItem item = (roll.itemSlot >= roll.GetLoot().items.Count ? roll.GetLoot().quest_items[roll.itemSlot - roll.GetLoot().items.Count] : roll.GetLoot().items[roll.itemSlot]);
                            InventoryResult msg = player.CanStoreNewItem(ItemConst.NullBag, ItemConst.NullSlot, dest, roll.itemid, item.count);
                            if (msg == InventoryResult.Ok)
                            {
                                item.is_looted = true;
                                roll.GetLoot().NotifyItemRemoved(roll.itemSlot, allowedMap);
                                roll.GetLoot().unlootedCount--;
                                player.StoreNewItem(dest, roll.itemid, true, item.randomBonusListId, item.GetAllowedLooters(), item.context, item.BonusListIDs);
                            }
                            else
                            {
                                item.is_blocked = false;
                                item.rollWinnerGUID = player.GetGUID();
                                player.SendEquipError(msg, null, null, roll.itemid);
                            }
                        }
                    }
                    else
                        roll.totalNeed = 0;
                }
            }

            if (roll.totalNeed == 0 && roll.totalGreed > 0) // if (roll->totalNeed == 0 && ...), not else if, because numbers can be modified above if player is on a different map
            {
                if (!roll.playerVote.Empty())
                {
                    byte maxresul = 0;
                    ObjectGuid maxguid = ObjectGuid.Empty;
                    Player player;
                    RollType rollVote = RollType.NotValid;

                    foreach (var pair in roll.playerVote)
                    {
                        if (pair.Value != RollType.Greed && pair.Value != RollType.Disenchant)
                            continue;

                        player = Global.ObjAccessor.FindPlayer(pair.Key);
                        if (!player || (allowedMap != null && player.GetMap() != allowedMap))
                        {
                            --roll.totalGreed;
                            continue;
                        }

                        byte randomN = (byte)RandomHelper.IRand(1, 100);
                        SendLootRoll(pair.Key, randomN, pair.Value, roll);
                        if (maxresul < randomN)
                        {
                            maxguid = pair.Key;
                            maxresul = randomN;
                            rollVote = pair.Value;
                        }
                    }

                    if (!maxguid.IsEmpty())
                    {
                        SendLootRollWon(maxguid, maxresul, rollVote, roll);
                        player = Global.ObjAccessor.FindConnectedPlayer(maxguid);

                        if (player && player.GetSession() != null)
                        {
                            player.UpdateCriteria(CriteriaType.RollGreed, roll.itemid, maxresul);

                            LootItem item = roll.itemSlot >= roll.GetLoot().items.Count ? roll.GetLoot().quest_items[roll.itemSlot - roll.GetLoot().items.Count] : roll.GetLoot().items[roll.itemSlot];

                            if (rollVote == RollType.Greed)
                            {
                                List<ItemPosCount> dest = new();
                                InventoryResult msg = player.CanStoreNewItem(ItemConst.NullBag, ItemConst.NullSlot, dest, roll.itemid, item.count);
                                if (msg == InventoryResult.Ok)
                                {
                                    item.is_looted = true;
                                    roll.GetLoot().NotifyItemRemoved(roll.itemSlot, allowedMap);
                                    roll.GetLoot().unlootedCount--;
                                    player.StoreNewItem(dest, roll.itemid, true, item.randomBonusListId, item.GetAllowedLooters(), item.context, item.BonusListIDs);
                                }
                                else
                                {
                                    item.is_blocked = false;
                                    item.rollWinnerGUID = player.GetGUID();
                                    player.SendEquipError(msg, null, null, roll.itemid);
                                }
                            }
                            else if (rollVote == RollType.Disenchant)
                            {
                                item.is_looted = true;
                                roll.GetLoot().NotifyItemRemoved(roll.itemSlot, allowedMap);
                                roll.GetLoot().unlootedCount--;
                                player.UpdateCriteria(CriteriaType.CastSpell, 13262); // Disenchant

                                ItemDisenchantLootRecord disenchant = roll.GetItemDisenchantLoot(player);

                                List<ItemPosCount> dest = new();
                                InventoryResult msg = player.CanStoreNewItem(ItemConst.NullBag, ItemConst.NullSlot, dest, roll.itemid, item.count);
                                if (msg == InventoryResult.Ok)
                                    player.AutoStoreLoot(disenchant.Id, LootStorage.Disenchant, ItemContext.None, true);
                                else // If the player's inventory is full, send the disenchant result in a mail.
                                {
                                    Loot loot = new(allowedMap, roll.GetLoot().GetOwnerGUID(), LootType.Disenchanting);
                                    loot.FillLoot(disenchant.Id, LootStorage.Disenchant, player, true);

                                    uint max_slot = loot.GetMaxSlotInLootFor(player);
                                    for (uint i = 0; i < max_slot; ++i)
                                    {
                                        LootItem lootItem = loot.LootItemInSlot(i, player);
                                        player.SendEquipError(msg, null, null, lootItem.itemid);
                                        player.SendItemRetrievalMail(lootItem.itemid, lootItem.count, lootItem.context);
                                    }
                                }
                            }
                        }
                    }
                    else
                        roll.totalGreed = 0;
                }
            }

            if (roll.totalNeed == 0 && roll.totalGreed == 0) // if, not else, because numbers can be modified above if player is on a different map
            {
                SendLootAllPassed(roll);

                // remove is_blocked so that the item is lootable by all players
                LootItem item = roll.itemSlot >= roll.GetLoot().items.Count ? roll.GetLoot().quest_items[roll.itemSlot - roll.GetLoot().items.Count] : roll.GetLoot().items[roll.itemSlot];
                item.is_blocked = false;
            }

            SendLootRollsComplete(roll);

            RollId.Remove(roll);
        }

        public void SetTargetIcon(byte symbol, ObjectGuid target, ObjectGuid changedBy, sbyte partyIndex)
        {
            if (symbol >= MapConst.TargetIconsCount)
                return;

            // clean other icons
            if (!target.IsEmpty())
                for (byte i = 0; i < MapConst.TargetIconsCount; ++i)
                    if (m_targetIcons[i] == target)
                        SetTargetIcon(i, ObjectGuid.Empty, changedBy, partyIndex);

            m_targetIcons[symbol] = target;

            SendRaidTargetUpdateSingle updateSingle = new();
            updateSingle.PartyIndex = partyIndex;
            updateSingle.Target = target;
            updateSingle.ChangedBy = changedBy;
            updateSingle.Symbol = (sbyte)symbol;
            BroadcastPacket(updateSingle, true);
        }

        public void SendTargetIconList(WorldSession session, sbyte partyIndex)
        {
            if (session == null)
                return;

            SendRaidTargetUpdateAll updateAll = new();
            updateAll.PartyIndex = partyIndex;
            for (byte i = 0; i < MapConst.TargetIconsCount; i++)
                updateAll.TargetIcons.Add(i, m_targetIcons[i]);

            session.SendPacket(updateAll);
        }

        public void SendUpdate()
        {
            foreach (var member in m_memberSlots)
                SendUpdateToPlayer(member.guid, member);
        }

        public void SendUpdateToPlayer(ObjectGuid playerGUID, MemberSlot memberSlot = null)
        {
            Player player = Global.ObjAccessor.FindPlayer(playerGUID);

            if (player == null || player.GetSession() == null || player.GetGroup() != this)
                return;

            // if MemberSlot wasn't provided
            if (memberSlot == null)
            {
                var slot = _getMemberSlot(playerGUID);
                if (slot == null) // if there is no MemberSlot for such a player
                    return;

                memberSlot = slot;
            }
            PartyUpdate partyUpdate = new();

            partyUpdate.PartyFlags = m_groupFlags;
            partyUpdate.PartyIndex = (byte)m_groupCategory;
            partyUpdate.PartyType = IsCreated() ? GroupType.Normal : GroupType.None;

            partyUpdate.PartyGUID = m_guid;
            partyUpdate.LeaderGUID = m_leaderGuid;
            partyUpdate.LeaderFactionGroup = m_leaderFactionGroup;

            partyUpdate.SequenceNum = player.NextGroupUpdateSequenceNumber(m_groupCategory);

            partyUpdate.MyIndex = -1;
            byte index = 0;
            for (var i = 0; i < m_memberSlots.Count; ++i, ++index)
            {
                var member = m_memberSlots[i];
                if (memberSlot.guid == member.guid)
                    partyUpdate.MyIndex = index;

                Player memberPlayer = Global.ObjAccessor.FindConnectedPlayer(member.guid);

                PartyPlayerInfo playerInfos = new();

                playerInfos.GUID = member.guid;
                playerInfos.Name = member.name;
                playerInfos.Class = member._class;

                playerInfos.FactionGroup = Player.GetFactionGroupForRace(member.race);

                playerInfos.Connected = memberPlayer?.GetSession() != null && !memberPlayer.GetSession().PlayerLogout();

                playerInfos.Subgroup = member.group;         // groupid
                playerInfos.Flags = (byte)member.flags;            // See enum GroupMemberFlags
                playerInfos.RolesAssigned = (byte)member.roles;    // Lfg Roles

                partyUpdate.PlayerList.Add(playerInfos);
            }

            if (GetMembersCount() > 1)
            {
                // LootSettings
                PartyLootSettings lootSettings = new();

                lootSettings.Method = (byte)m_lootMethod;
                lootSettings.Threshold = (byte)m_lootThreshold;
                lootSettings.LootMaster = m_lootMethod == LootMethod.MasterLoot ? m_masterLooterGuid : ObjectGuid.Empty;

                partyUpdate.LootSettings = lootSettings;

                // Difficulty Settings
                PartyDifficultySettings difficultySettings = new();

                difficultySettings.DungeonDifficultyID = (uint)m_dungeonDifficulty;
                difficultySettings.RaidDifficultyID = (uint)m_raidDifficulty;
                difficultySettings.LegacyRaidDifficultyID = (uint)m_legacyRaidDifficulty;

                partyUpdate.DifficultySettings = difficultySettings;
            }

            // LfgInfos
            if (IsLFGGroup())
            {
                PartyLFGInfo lfgInfos = new();

                lfgInfos.Slot = Global.LFGMgr.GetLFGDungeonEntry(Global.LFGMgr.GetDungeon(m_guid));
                lfgInfos.BootCount = 0;
                lfgInfos.Aborted = false;

                lfgInfos.MyFlags = (byte)(Global.LFGMgr.GetState(m_guid) == LfgState.FinishedDungeon ? 2 : 0);
                lfgInfos.MyRandomSlot = Global.LFGMgr.GetSelectedRandomDungeon(player.GetGUID());

                lfgInfos.MyPartialClear = 0;
                lfgInfos.MyGearDiff = 0.0f;
                lfgInfos.MyFirstReward = false;

                DungeonFinding.LfgReward reward = Global.LFGMgr.GetRandomDungeonReward(partyUpdate.LfgInfos.Value.MyRandomSlot, player.GetLevel());
                if (reward != null)
                {
                    Quest quest = Global.ObjectMgr.GetQuestTemplate(reward.firstQuest);
                    if (quest != null)
                        lfgInfos.MyFirstReward = player.CanRewardQuest(quest, false);
                }

                lfgInfos.MyStrangerCount = 0;
                lfgInfos.MyKickVoteCount = 0;

                partyUpdate.LfgInfos = lfgInfos;
            }

            player.SendPacket(partyUpdate);
        }

        void SendUpdateDestroyGroupToPlayer(Player player)
        {
            PartyUpdate partyUpdate = new();
            partyUpdate.PartyFlags = GroupFlags.Destroyed;
            partyUpdate.PartyIndex = (byte)m_groupCategory;
            partyUpdate.PartyType = GroupType.None;
            partyUpdate.PartyGUID = m_guid;
            partyUpdate.MyIndex = -1;
            partyUpdate.SequenceNum = player.NextGroupUpdateSequenceNumber(m_groupCategory);
            player.SendPacket(partyUpdate);
        }

        public void UpdatePlayerOutOfRange(Player player)
        {
            if (!player || !player.IsInWorld)
                return;

            PartyMemberFullState packet = new();
            packet.Initialize(player);

            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player member = refe.GetSource();
                if (member && member != player && (!member.IsInMap(player) || !member.IsWithinDist(player, member.GetSightRange(), false)))
                    member.SendPacket(packet);
            }
        }

        public void BroadcastAddonMessagePacket(ServerPacket packet, string prefix, bool ignorePlayersInBGRaid, int group = -1, ObjectGuid ignore = default)
        {
            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player player = refe.GetSource();
                if (player == null || (!ignore.IsEmpty() && player.GetGUID() == ignore) || (ignorePlayersInBGRaid && player.GetGroup() != this))
                    continue;

                if ((group == -1 || refe.GetSubGroup() == group))
                    if (player.GetSession().IsAddonRegistered(prefix))
                        player.SendPacket(packet);
            }
        }

        public void BroadcastPacket(ServerPacket packet, bool ignorePlayersInBGRaid, int group = -1, ObjectGuid ignore = default)
        {
            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player player = refe.GetSource();
                if (!player || (!ignore.IsEmpty() && player.GetGUID() == ignore) || (ignorePlayersInBGRaid && player.GetGroup() != this))
                    continue;

                if (player.GetSession() != null && (group == -1 || refe.GetSubGroup() == group))
                    player.SendPacket(packet);
            }
        }

        bool _setMembersGroup(ObjectGuid guid, byte group)
        {
            var slot = _getMemberSlot(guid);
            if (slot == null)
                return false;

            slot.group = group;

            SubGroupCounterIncrease(group);

            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_MEMBER_SUBGROUP);

                stmt.AddValue(0, group);
                stmt.AddValue(1, guid.GetCounter());

                DB.Characters.Execute(stmt);
            }

            return true;
        }

        public bool SameSubGroup(Player member1, Player member2)
        {
            if (!member1 || !member2)
                return false;

            if (member1.GetGroup() != this || member2.GetGroup() != this)
                return false;
            else
                return member1.GetSubGroup() == member2.GetSubGroup();
        }

        public void ChangeMembersGroup(ObjectGuid guid, byte group)
        {
            // Only raid groups have sub groups
            if (!IsRaidGroup())
                return;

            // Check if player is really in the raid
            var slot = _getMemberSlot(guid);
            if (slot == null)
                return;

            byte prevSubGroup = slot.group;
            // Abort if the player is already in the target sub group
            if (prevSubGroup == group)
                return;

            // Update the player slot with the new sub group setting
            slot.group = group;

            // Increase the counter of the new sub group..
            SubGroupCounterIncrease(group);

            // ..and decrease the counter of the previous one
            SubGroupCounterDecrease(prevSubGroup);

            // Preserve new sub group in database for non-raid groups
            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_MEMBER_SUBGROUP);

                stmt.AddValue(0, group);
                stmt.AddValue(1, guid.GetCounter());

                DB.Characters.Execute(stmt);
            }

            // In case the moved player is online, update the player object with the new sub group references
            Player player = Global.ObjAccessor.FindPlayer(guid);
            if (player)
            {
                if (player.GetGroup() == this)
                    player.GetGroupRef().SetSubGroup(group);
                else
                {
                    // If player is in BG raid, it is possible that he is also in normal raid - and that normal raid is stored in m_originalGroup reference
                    player.GetOriginalGroupRef().SetSubGroup(group);
                }
            }

            // Broadcast the changes to the group
            SendUpdate();
        }

        public void SwapMembersGroups(ObjectGuid firstGuid, ObjectGuid secondGuid)
        {
            if (!IsRaidGroup())
                return;

            MemberSlot[] slots = new MemberSlot[2];
            slots[0] = _getMemberSlot(firstGuid);
            slots[1] = _getMemberSlot(secondGuid);
            if (slots[0] == null || slots[1] == null)
                return;

            if (slots[0].group == slots[1].group)
                return;

            byte tmp = slots[0].group;
            slots[0].group = slots[1].group;
            slots[1].group = tmp;

            SQLTransaction trans = new();
            for (byte i = 0; i < 2; i++)
            {
                // Preserve new sub group in database for non-raid groups
                if (!IsBGGroup() && !IsBFGroup())
                {
                    PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_MEMBER_SUBGROUP);
                    stmt.AddValue(0, slots[i].group);
                    stmt.AddValue(1, slots[i].guid.GetCounter());

                    trans.Append(stmt);
                }

                Player player = Global.ObjAccessor.FindConnectedPlayer(slots[i].guid);
                if (player)
                {
                    if (player.GetGroup() == this)
                        player.GetGroupRef().SetSubGroup(slots[i].group);
                    else
                        player.GetOriginalGroupRef().SetSubGroup(slots[i].group);
                }
            }
            DB.Characters.CommitTransaction(trans);

            SendUpdate();
        }

        public void UpdateLooterGuid(WorldObject pLootedObject, bool ifneed = false)
        {
            switch (GetLootMethod())
            {
                case LootMethod.MasterLoot:
                case LootMethod.FreeForAll:
                    return;
                default:
                    // round robin style looting applies for all low
                    // quality items in each loot method except free for all and master loot
                    break;
            }

            ObjectGuid oldLooterGUID = GetLooterGuid();
            var memberSlot = _getMemberSlot(oldLooterGUID);
            if (memberSlot != null)
            {
                if (ifneed)
                {
                    // not update if only update if need and ok
                    Player looter = Global.ObjAccessor.FindPlayer(memberSlot.guid);
                    if (looter && looter.IsAtGroupRewardDistance(pLootedObject))
                        return;
                }
            }

            // search next after current
            Player pNewLooter = null;
            foreach (var member in m_memberSlots)
            {
                if (member == memberSlot)
                    continue;

                Player player = Global.ObjAccessor.FindPlayer(member.guid);
                if (player)
                    if (player.IsAtGroupRewardDistance(pLootedObject))
                    {
                        pNewLooter = player;
                        break;
                    }
            }

            if (!pNewLooter)
            {
                // search from start
                foreach (var member in m_memberSlots)
                {
                    Player player = Global.ObjAccessor.FindPlayer(member.guid);
                    if (player)
                        if (player.IsAtGroupRewardDistance(pLootedObject))
                        {
                            pNewLooter = player;
                            break;
                        }
                }
            }

            if (pNewLooter)
            {
                if (oldLooterGUID != pNewLooter.GetGUID())
                {
                    SetLooterGuid(pNewLooter.GetGUID());
                    SendUpdate();
                }
            }
            else
            {
                SetLooterGuid(ObjectGuid.Empty);
                SendUpdate();
            }
        }

        public GroupJoinBattlegroundResult CanJoinBattlegroundQueue(Battleground bgOrTemplate, BattlegroundQueueTypeId bgQueueTypeId, uint MinPlayerCount, uint MaxPlayerCount, bool isRated, uint arenaSlot, out ObjectGuid errorGuid)
        {
            errorGuid = new ObjectGuid();
            // check if this group is LFG group
            if (IsLFGGroup())
                return GroupJoinBattlegroundResult.LfgCantUseBattleground;

            BattlemasterListRecord bgEntry = CliDB.BattlemasterListStorage.LookupByKey(bgOrTemplate.GetTypeID());
            if (bgEntry == null)
                return GroupJoinBattlegroundResult.BattlegroundJoinFailed;            // shouldn't happen

            // check for min / max count
            uint memberscount = GetMembersCount();

            if (memberscount > bgEntry.MaxGroupSize)                // no MinPlayerCount for Battlegrounds
                return GroupJoinBattlegroundResult.None;                        // ERR_GROUP_JOIN_Battleground_TOO_MANY handled on client side

            // get a player as reference, to compare other players' stats to (arena team id, queue id based on level, etc.)
            Player reference = GetFirstMember().GetSource();
            // no reference found, can't join this way
            if (!reference)
                return GroupJoinBattlegroundResult.BattlegroundJoinFailed;

            PvpDifficultyRecord bracketEntry = Global.DB2Mgr.GetBattlegroundBracketByLevel(bgOrTemplate.GetMapId(), reference.GetLevel());
            if (bracketEntry == null)
                return GroupJoinBattlegroundResult.BattlegroundJoinFailed;

            uint arenaTeamId = reference.GetArenaTeamId((byte)arenaSlot);
            Team team = reference.GetTeam();
            bool isMercenary = reference.HasAura(BattlegroundConst.SpellMercenaryContractHorde) || reference.HasAura(BattlegroundConst.SpellMercenaryContractAlliance);

            // check every member of the group to be able to join
            memberscount = 0;
            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next(), ++memberscount)
            {
                Player member = refe.GetSource();
                // offline member? don't let join
                if (!member)
                    return GroupJoinBattlegroundResult.BattlegroundJoinFailed;
                // rbac permissions
                if (!member.CanJoinToBattleground(bgOrTemplate))
                    return GroupJoinBattlegroundResult.JoinTimedOut;
                // don't allow cross-faction join as group
                if (member.GetTeam() != team)
                {
                    errorGuid = member.GetGUID();
                    return GroupJoinBattlegroundResult.JoinTimedOut;
                }
                // not in the same Battleground level braket, don't let join
                PvpDifficultyRecord memberBracketEntry = Global.DB2Mgr.GetBattlegroundBracketByLevel(bracketEntry.MapID, member.GetLevel());
                if (memberBracketEntry != bracketEntry)
                    return GroupJoinBattlegroundResult.JoinRangeIndex;
                // don't let join rated matches if the arena team id doesn't match
                if (isRated && member.GetArenaTeamId((byte)arenaSlot) != arenaTeamId)
                    return GroupJoinBattlegroundResult.BattlegroundJoinFailed;
                // don't let join if someone from the group is already in that bg queue
                if (member.InBattlegroundQueueForBattlegroundQueueType(bgQueueTypeId))
                    return GroupJoinBattlegroundResult.BattlegroundJoinFailed;            // not blizz-like
                // don't let join if someone from the group is in bg queue random
                bool isInRandomBgQueue = member.InBattlegroundQueueForBattlegroundQueueType(Global.BattlegroundMgr.BGQueueTypeId((ushort)BattlegroundTypeId.RB, BattlegroundQueueIdType.Battleground, false, 0))
                    || member.InBattlegroundQueueForBattlegroundQueueType(Global.BattlegroundMgr.BGQueueTypeId((ushort)BattlegroundTypeId.RandomEpic, BattlegroundQueueIdType.Battleground, false, 0));
                if (bgOrTemplate.GetTypeID() != BattlegroundTypeId.AA && isInRandomBgQueue)
                    return GroupJoinBattlegroundResult.InRandomBg;
                // don't let join to bg queue random if someone from the group is already in bg queue
                if ((bgOrTemplate.GetTypeID() == BattlegroundTypeId.RB || bgOrTemplate.GetTypeID() == BattlegroundTypeId.RandomEpic) && member.InBattlegroundQueue(true) && !isInRandomBgQueue)
                    return GroupJoinBattlegroundResult.InNonRandomBg;
                // check for deserter debuff in case not arena queue
                if (bgOrTemplate.GetTypeID() != BattlegroundTypeId.AA && member.IsDeserter())
                    return GroupJoinBattlegroundResult.Deserters;
                // check if member can join any more Battleground queues
                if (!member.HasFreeBattlegroundQueueId())
                    return GroupJoinBattlegroundResult.TooManyQueues;        // not blizz-like
                // check if someone in party is using dungeon system
                if (member.IsUsingLfg())
                    return GroupJoinBattlegroundResult.LfgCantUseBattleground;
                // check Freeze debuff
                if (member.HasAura(9454))
                    return GroupJoinBattlegroundResult.BattlegroundJoinFailed;
                if (isMercenary != (member.HasAura(BattlegroundConst.SpellMercenaryContractHorde) || member.HasAura(BattlegroundConst.SpellMercenaryContractAlliance)))
                    return GroupJoinBattlegroundResult.BattlegroundJoinMercenary;
            }

            // only check for MinPlayerCount since MinPlayerCount == MaxPlayerCount for arenas...
            if (bgOrTemplate.IsArena() && memberscount != MinPlayerCount)
                return GroupJoinBattlegroundResult.ArenaTeamPartySize;

            return GroupJoinBattlegroundResult.None;
        }

        public void SetDungeonDifficultyID(Difficulty difficulty)
        {
            m_dungeonDifficulty = difficulty;
            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_DIFFICULTY);

                stmt.AddValue(0, (byte)m_dungeonDifficulty);
                stmt.AddValue(1, m_dbStoreId);

                DB.Characters.Execute(stmt);
            }

            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player player = refe.GetSource();
                if (player.GetSession() == null)
                    continue;

                player.SetDungeonDifficultyID(difficulty);
                player.SendDungeonDifficulty();
            }
        }

        public void SetRaidDifficultyID(Difficulty difficulty)
        {
            m_raidDifficulty = difficulty;
            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_RAID_DIFFICULTY);

                stmt.AddValue(0, (byte)m_raidDifficulty);
                stmt.AddValue(1, m_dbStoreId);

                DB.Characters.Execute(stmt);
            }

            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player player = refe.GetSource();
                if (player.GetSession() == null)
                    continue;

                player.SetRaidDifficultyID(difficulty);
                player.SendRaidDifficulty(false);
            }
        }

        public void SetLegacyRaidDifficultyID(Difficulty difficulty)
        {
            m_legacyRaidDifficulty = difficulty;
            if (!IsBGGroup() && !IsBFGroup())
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_LEGACY_RAID_DIFFICULTY);

                stmt.AddValue(0, (byte)m_legacyRaidDifficulty);
                stmt.AddValue(1, m_dbStoreId);

                DB.Characters.Execute(stmt);
            }

            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
            {
                Player player = refe.GetSource();
                if (player.GetSession() == null)
                    continue;

                player.SetLegacyRaidDifficultyID(difficulty);
                player.SendRaidDifficulty(true);
            }
        }

        public Difficulty GetDifficultyID(MapRecord mapEntry)
        {
            if (!mapEntry.IsRaid())
                return m_dungeonDifficulty;

            MapDifficultyRecord defaultDifficulty = Global.DB2Mgr.GetDefaultMapDifficulty(mapEntry.Id);
            if (defaultDifficulty == null)
                return m_legacyRaidDifficulty;

            DifficultyRecord difficulty = CliDB.DifficultyStorage.LookupByKey(defaultDifficulty.DifficultyID);
            if (difficulty == null || difficulty.Flags.HasAnyFlag(DifficultyFlags.Legacy))
                return m_legacyRaidDifficulty;

            return m_raidDifficulty;
        }

        public Difficulty GetDungeonDifficultyID() { return m_dungeonDifficulty; }
        public Difficulty GetRaidDifficultyID() { return m_raidDifficulty; }
        public Difficulty GetLegacyRaidDifficultyID() { return m_legacyRaidDifficulty; }

        public void ResetInstances(InstanceResetMethod method, bool isRaid, bool isLegacy, Player SendMsgTo)
        {
            if (IsBGGroup() || IsBFGroup())
                return;

            // method can be INSTANCE_RESET_ALL, INSTANCE_RESET_CHANGE_DIFFICULTY, INSTANCE_RESET_GROUP_DISBAND

            // we assume that when the difficulty changes, all instances that can be reset will be
            Difficulty diff = GetDungeonDifficultyID();
            if (isRaid)
            {
                if (!isLegacy)
                    diff = GetRaidDifficultyID();
                else
                    diff = GetLegacyRaidDifficultyID();
            }

            var difficultyDic = m_boundInstances.LookupByKey(diff);
            if (difficultyDic == null)
                return;

            foreach (var pair in difficultyDic.ToList())
            {
                InstanceSave instanceSave = pair.Value.save;
                MapRecord entry = CliDB.MapStorage.LookupByKey(pair.Key);
                if (entry == null || entry.IsRaid() != isRaid || (!instanceSave.CanReset() && method != InstanceResetMethod.GroupDisband))
                    continue;

                if (method == InstanceResetMethod.All)
                {
                    // the "reset all instances" method can only reset normal maps
                    if (entry.InstanceType == MapTypes.Raid || diff == Difficulty.Heroic)
                        continue;
                }

                bool isEmpty = true;
                // if the map is loaded, reset it
                Map map = Global.MapMgr.FindMap(instanceSave.GetMapId(), instanceSave.GetInstanceId());
                if (map && map.IsDungeon() && !(method == InstanceResetMethod.GroupDisband && !instanceSave.CanReset()))
                {
                    if (instanceSave.CanReset())
                        isEmpty = ((InstanceMap)map).Reset(method);
                    else
                        isEmpty = !map.HavePlayers();
                }

                if (SendMsgTo)
                {
                    if (!isEmpty)
                        SendMsgTo.SendResetInstanceFailed(ResetFailedReason.Failed, instanceSave.GetMapId());
                    else if (WorldConfig.GetBoolValue(WorldCfg.InstancesResetAnnounce))
                    {
                        Group group = SendMsgTo.GetGroup();
                        if (group)
                        {
                            for (GroupReference refe = group.GetFirstMember(); refe != null; refe = refe.Next())
                            {
                                Player player = refe.GetSource();
                                if (player)
                                    player.SendResetInstanceSuccess(instanceSave.GetMapId());
                            }
                        }
                        else
                            SendMsgTo.SendResetInstanceSuccess(instanceSave.GetMapId());
                    }
                    else
                        SendMsgTo.SendResetInstanceSuccess(instanceSave.GetMapId());
                }

                if (isEmpty || method == InstanceResetMethod.GroupDisband || method == InstanceResetMethod.ChangeDifficulty)
                {
                    // do not reset the instance, just unbind if others are permanently bound to it
                    if (instanceSave.CanReset())
                    {
                        if (map != null && map.IsDungeon() && SendMsgTo)
                        {
                            AreaTriggerStruct instanceEntrance = Global.ObjectMgr.GetGoBackTrigger(map.GetId());

                            if (instanceEntrance == null)
                                Log.outDebug(LogFilter.Misc, $"Instance entrance not found for map {map.GetId()}");
                            else
                            {
                                WorldSafeLocsEntry graveyardLocation = Global.ObjectMgr.GetClosestGraveYard(
                                    new WorldLocation(instanceEntrance.target_mapId, instanceEntrance.target_X, instanceEntrance.target_Y, instanceEntrance.target_Z),
                                    SendMsgTo.GetTeam(), null);
                                uint zoneId = Global.TerrainMgr.GetZoneId(PhasingHandler.EmptyPhaseShift, graveyardLocation.Loc.GetMapId(),
                                    graveyardLocation.Loc.GetPositionX(), graveyardLocation.Loc.GetPositionY(), graveyardLocation.Loc.GetPositionZ());

                                foreach (MemberSlot member in GetMemberSlots())
                                {
                                    if (!Global.ObjAccessor.FindConnectedPlayer(member.guid))
                                    {
                                        var stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_CHARACTER_POSITION_BY_MAPID);

                                        stmt.AddValue(0, graveyardLocation.Loc.GetPositionX());
                                        stmt.AddValue(1, graveyardLocation.Loc.GetPositionY());
                                        stmt.AddValue(2, graveyardLocation.Loc.GetPositionZ());
                                        stmt.AddValue(3, instanceEntrance.target_Orientation);
                                        stmt.AddValue(4, graveyardLocation.Loc.GetMapId());
                                        stmt.AddValue(5, zoneId);
                                        stmt.AddValue(6, member.guid.GetCounter());
                                        stmt.AddValue(7, map.GetId());

                                        DB.Characters.Execute(stmt);
                                    }
                                }
                            }
                        }
                        instanceSave.DeleteFromDB();
                    }
                    else
                    {
                        PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_GROUP_INSTANCE_BY_INSTANCE);
                        stmt.AddValue(0, instanceSave.GetInstanceId());

                        DB.Characters.Execute(stmt);
                    }


                    difficultyDic.Remove(pair.Key);
                    // this unloads the instance save unless online players are bound to it
                    // (eg. permanent binds or GM solo binds)
                    instanceSave.RemoveGroup(this);
                }
            }
        }

        public InstanceBind GetBoundInstance(Player player)
        {
            uint mapid = player.GetMapId();
            MapRecord mapEntry = CliDB.MapStorage.LookupByKey(mapid);
            return GetBoundInstance(mapEntry);
        }

        public InstanceBind GetBoundInstance(Map aMap)
        {
            return GetBoundInstance(aMap.GetEntry());
        }

        public InstanceBind GetBoundInstance(MapRecord mapEntry)
        {
            if (mapEntry == null || !mapEntry.IsDungeon())
                return null;

            Difficulty difficulty = GetDifficultyID(mapEntry);
            return GetBoundInstance(difficulty, mapEntry.Id);
        }

        public InstanceBind GetBoundInstance(Difficulty difficulty, uint mapId)
        {
            // some instances only have one difficulty
            Global.DB2Mgr.GetDownscaledMapDifficultyData(mapId, ref difficulty);

            var difficultyDic = m_boundInstances.LookupByKey(difficulty);
            if (difficultyDic == null)
                return null;

            var instanceBind = difficultyDic.LookupByKey(mapId);
            if (instanceBind != null)
                return instanceBind;

            return null;
        }

        public InstanceBind BindToInstance(InstanceSave save, bool permanent, bool load = false)
        {
            if (save == null || IsBGGroup() || IsBFGroup())
                return null;

            if (!m_boundInstances.ContainsKey(save.GetDifficultyID()))
                m_boundInstances[save.GetDifficultyID()] = new Dictionary<uint, InstanceBind>();

            if (!m_boundInstances[save.GetDifficultyID()].ContainsKey(save.GetMapId()))
                m_boundInstances[save.GetDifficultyID()][save.GetMapId()] = new InstanceBind();

            InstanceBind bind = m_boundInstances[save.GetDifficultyID()][save.GetMapId()];
            if (!load && (bind.save == null || permanent != bind.perm || save != bind.save))
            {
                PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.REP_GROUP_INSTANCE);

                stmt.AddValue(0, m_dbStoreId);
                stmt.AddValue(1, save.GetInstanceId());
                stmt.AddValue(2, permanent);

                DB.Characters.Execute(stmt);
            }

            if (bind.save != save)
            {
                if (bind.save != null)
                    bind.save.RemoveGroup(this);
                save.AddGroup(this);
            }

            bind.save = save;
            bind.perm = permanent;
            if (!load)
                Log.outDebug(LogFilter.Maps, "Group.BindToInstance: Group ({0}, storage id: {1}) is now bound to map {2}, instance {3}, difficulty {4}",
                GetGUID().ToString(), m_dbStoreId, save.GetMapId(), save.GetInstanceId(), save.GetDifficultyID());

            m_boundInstances[save.GetDifficultyID()][save.GetMapId()] = bind;

            return bind;
        }

        public void UnbindInstance(uint mapid, Difficulty difficulty, bool unload = false)
        {
            var difficultyDic = m_boundInstances.LookupByKey(difficulty);
            if (difficultyDic == null)
                return;

            var instanceBind = difficultyDic.LookupByKey(mapid);
            if (instanceBind != null)
            {
                if (!unload)
                {
                    PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.DEL_GROUP_INSTANCE_BY_GUID);

                    stmt.AddValue(0, m_dbStoreId);
                    stmt.AddValue(1, instanceBind.save.GetInstanceId());

                    DB.Characters.Execute(stmt);
                }

                instanceBind.save.RemoveGroup(this);                // save can become invalid
                difficultyDic.Remove(mapid);
            }
        }

        void _homebindIfInstance(Player player)
        {
            if (player && !player.IsGameMaster() && CliDB.MapStorage.LookupByKey(player.GetMapId()).IsDungeon())
                player.m_InstanceValid = false;
        }

        public void BroadcastGroupUpdate()
        {
            // FG: HACK: force flags update on group leave - for values update hack
            // -- not very efficient but safe
            foreach (var member in m_memberSlots)
            {
                Player pp = Global.ObjAccessor.FindPlayer(member.guid);
                if (pp && pp.IsInWorld)
                {
                    pp.m_values.ModifyValue(pp.m_unitData).ModifyValue(pp.m_unitData.PvpFlags);
                    pp.m_values.ModifyValue(pp.m_unitData).ModifyValue(pp.m_unitData.FactionTemplate);
                    pp.ForceUpdateFieldChange();
                    Log.outDebug(LogFilter.Server, "-- Forced group value update for '{0}'", pp.GetName());
                }
            }
        }

        public void ResetMaxEnchantingLevel()
        {
            m_maxEnchantingLevel = 0;
            Player member;
            foreach (var memberSlot in m_memberSlots)
            {
                member = Global.ObjAccessor.FindPlayer(memberSlot.guid);
                if (member && m_maxEnchantingLevel < member.GetSkillValue(SkillType.Enchanting))
                    m_maxEnchantingLevel = member.GetSkillValue(SkillType.Enchanting);
            }
        }

        public void SetLootMethod(LootMethod method)
        {
            m_lootMethod = method;
        }

        public void SetLooterGuid(ObjectGuid guid)
        {
            m_looterGuid = guid;
        }

        public void SetMasterLooterGuid(ObjectGuid guid)
        {
            m_masterLooterGuid = guid;
        }

        public void SetLootThreshold(ItemQuality threshold)
        {
            m_lootThreshold = threshold;
        }

        public void SetLfgRoles(ObjectGuid guid, LfgRoles roles)
        {
            var slot = _getMemberSlot(guid);
            if (slot == null)
                return;

            slot.roles = roles;
            SendUpdate();
        }

        public LfgRoles GetLfgRoles(ObjectGuid guid)
        {
            MemberSlot slot = _getMemberSlot(guid);
            if (slot == null)
                return 0;

            return slot.roles;
        }

        void UpdateReadyCheck(uint diff)
        {
            if (!m_readyCheckStarted)
                return;

            m_readyCheckTimer -= TimeSpan.FromMilliseconds(diff);
            if (m_readyCheckTimer <= TimeSpan.Zero)
                EndReadyCheck();
        }

        public void StartReadyCheck(ObjectGuid starterGuid, sbyte partyIndex, TimeSpan duration)
        {
            if (m_readyCheckStarted)
                return;

            MemberSlot slot = _getMemberSlot(starterGuid);
            if (slot == null)
                return;

            m_readyCheckStarted = true;
            m_readyCheckTimer = duration;

            SetOfflineMembersReadyChecked();

            SetMemberReadyChecked(slot);

            ReadyCheckStarted readyCheckStarted = new();
            readyCheckStarted.PartyGUID = m_guid;
            readyCheckStarted.PartyIndex = partyIndex;
            readyCheckStarted.InitiatorGUID = starterGuid;
            readyCheckStarted.Duration = (uint)duration.TotalMilliseconds;
            BroadcastPacket(readyCheckStarted, false);
        }

        void EndReadyCheck()
        {
            if (!m_readyCheckStarted)
                return;

            m_readyCheckStarted = false;
            m_readyCheckTimer = TimeSpan.Zero;

            ResetMemberReadyChecked();

            ReadyCheckCompleted readyCheckCompleted = new();
            readyCheckCompleted.PartyIndex = 0;
            readyCheckCompleted.PartyGUID = m_guid;
            BroadcastPacket(readyCheckCompleted, false);
        }

        bool IsReadyCheckCompleted()
        {
            foreach (var member in m_memberSlots)
                if (!member.readyChecked)
                    return false;
            return true;
        }

        public void SetMemberReadyCheck(ObjectGuid guid, bool ready)
        {
            if (!m_readyCheckStarted)
                return;

            MemberSlot slot = _getMemberSlot(guid);
            if (slot != null)
                SetMemberReadyCheck(slot, ready);
        }

        void SetMemberReadyCheck(MemberSlot slot, bool ready)
        {
            ReadyCheckResponse response = new();
            response.PartyGUID = m_guid;
            response.Player = slot.guid;
            response.IsReady = ready;
            BroadcastPacket(response, false);

            SetMemberReadyChecked(slot);
        }

        void SetOfflineMembersReadyChecked()
        {
            foreach (MemberSlot member in m_memberSlots)
            {
                Player player = Global.ObjAccessor.FindConnectedPlayer(member.guid);
                if (!player || !player.GetSession())
                    SetMemberReadyCheck(member, false);
            }
        }

        void SetMemberReadyChecked(MemberSlot slot)
        {
            slot.readyChecked = true;
            if (IsReadyCheckCompleted())
                EndReadyCheck();
        }

        void ResetMemberReadyChecked()
        {
            foreach (MemberSlot member in m_memberSlots)
                member.readyChecked = false;
        }

        public void AddRaidMarker(byte markerId, uint mapId, float positionX, float positionY, float positionZ, ObjectGuid transportGuid = default)
        {
            if (markerId >= MapConst.RaidMarkersCount || m_markers[markerId] != null)
                return;

            m_activeMarkers |= (1u << markerId);
            m_markers[markerId] = new RaidMarker(mapId, positionX, positionY, positionZ, transportGuid);
            SendRaidMarkersChanged();
        }

        public void DeleteRaidMarker(byte markerId)
        {
            if (markerId > MapConst.RaidMarkersCount)
                return;

            for (byte i = 0; i < MapConst.RaidMarkersCount; i++)
            {
                if (m_markers[i] != null && (markerId == i || markerId == MapConst.RaidMarkersCount))
                {
                    m_markers[i] = null;
                    m_activeMarkers &= ~(1u << i);
                }
            }

            SendRaidMarkersChanged();
        }

        public void SendRaidMarkersChanged(WorldSession session = null, sbyte partyIndex = 0)
        {
            RaidMarkersChanged packet = new();

            packet.PartyIndex = partyIndex;
            packet.ActiveMarkers = m_activeMarkers;

            for (byte i = 0; i < MapConst.RaidMarkersCount; i++)
            {
                if (m_markers[i] != null)
                    packet.RaidMarkers.Add(m_markers[i]);
            }

            if (session)
                session.SendPacket(packet);
            else
                BroadcastPacket(packet, false);
        }

        public bool IsFull()
        {
            return IsRaidGroup() ? (m_memberSlots.Count >= MapConst.MaxRaidSize) : (m_memberSlots.Count >= MapConst.MaxGroupSize);
        }

        public bool IsLFGGroup()
        {
            return m_groupFlags.HasAnyFlag(GroupFlags.Lfg);
        }
        public bool IsRaidGroup()
        {
            return m_groupFlags.HasAnyFlag(GroupFlags.Raid);
        }

        public bool IsBGGroup()
        {
            return m_bgGroup != null;
        }

        public bool IsBFGroup()
        {
            return m_bfGroup != null;
        }

        public bool IsCreated()
        {
            return GetMembersCount() > 0;
        }

        public bool IsRollLootActive() { return !RollId.Empty(); }

        public ObjectGuid GetLeaderGUID()
        {
            return m_leaderGuid;
        }

        public ObjectGuid GetGUID()
        {
            return m_guid;
        }

        public ulong GetLowGUID()
        {
            return m_guid.GetCounter();
        }

        string GetLeaderName()
        {
            return m_leaderName;
        }

        public LootMethod GetLootMethod()
        {
            return m_lootMethod;
        }

        public ObjectGuid GetLooterGuid()
        {
            if (GetLootMethod() == LootMethod.FreeForAll)
                return ObjectGuid.Empty;

            return m_looterGuid;
        }

        public ObjectGuid GetMasterLooterGuid()
        {
            return m_masterLooterGuid;
        }

        public ItemQuality GetLootThreshold()
        {
            return m_lootThreshold;
        }

        public bool IsMember(ObjectGuid guid)
        {
            return _getMemberSlot(guid) != null;
        }

        public bool IsLeader(ObjectGuid guid)
        {
            return GetLeaderGUID() == guid;
        }

        public bool IsAssistant(ObjectGuid guid)
        {
            return GetMemberFlags(guid).HasAnyFlag(GroupMemberFlags.Assistant);
        }

        public ObjectGuid GetMemberGUID(string name)
        {
            foreach (var member in m_memberSlots)
                if (member.name == name)
                    return member.guid;
            return ObjectGuid.Empty;
        }

        public GroupMemberFlags GetMemberFlags(ObjectGuid guid)
        {
            var mslot = _getMemberSlot(guid);
            if (mslot == null)
                return 0;

            return mslot.flags;
        }

        public bool SameSubGroup(ObjectGuid guid1, ObjectGuid guid2)
        {
            var mslot2 = _getMemberSlot(guid2);
            if (mslot2 == null)
                return false;
            return SameSubGroup(guid1, mslot2);
        }

        public bool SameSubGroup(ObjectGuid guid1, MemberSlot slot2)
        {
            var mslot1 = _getMemberSlot(guid1);
            if (mslot1 == null || slot2 == null)
                return false;
            return (mslot1.group == slot2.group);
        }

        public bool HasFreeSlotSubGroup(byte subgroup)
        {
            return (m_subGroupsCounts != null && m_subGroupsCounts[subgroup] < MapConst.MaxGroupSize);
        }

        public byte GetMemberGroup(ObjectGuid guid)
        {
            var mslot = _getMemberSlot(guid);
            if (mslot == null)
                return (byte)(MapConst.MaxRaidSubGroups + 1);
            return mslot.group;
        }

        public void SetBattlegroundGroup(Battleground bg)
        {
            m_bgGroup = bg;
        }

        public void SetBattlefieldGroup(BattleField bg)
        {
            m_bfGroup = bg;
        }

        public void SetGroupMemberFlag(ObjectGuid guid, bool apply, GroupMemberFlags flag)
        {
            // Assistants, main assistants and main tanks are only available in raid groups
            if (!IsRaidGroup())
                return;

            // Check if player is really in the raid
            var slot = _getMemberSlot(guid);
            if (slot == null)
                return;

            // Do flag specific actions, e.g ensure uniqueness
            switch (flag)
            {
                case GroupMemberFlags.MainAssist:
                    RemoveUniqueGroupMemberFlag(GroupMemberFlags.MainAssist);         // Remove main assist flag from current if any.
                    break;
                case GroupMemberFlags.MainTank:
                    RemoveUniqueGroupMemberFlag(GroupMemberFlags.MainTank);           // Remove main tank flag from current if any.
                    break;
                case GroupMemberFlags.Assistant:
                    break;
                default:
                    return;                                                      // This should never happen
            }

            // Switch the actual flag
            ToggleGroupMemberFlag(slot, flag, apply);

            // Preserve the new setting in the db
            PreparedStatement stmt = DB.Characters.GetPreparedStatement(CharStatements.UPD_GROUP_MEMBER_FLAG);

            stmt.AddValue(0, (byte)slot.flags);
            stmt.AddValue(1, guid.GetCounter());

            DB.Characters.Execute(stmt);

            // Broadcast the changes to the group
            SendUpdate();
        }

        Roll GetRoll(ObjectGuid lootObjectGuid, byte lootListId)
        {
            foreach (var roll in RollId)
                if (roll.IsValid() && roll.GetTarget().GetGUID() == lootObjectGuid && roll.itemSlot == lootListId)
                    return roll;

            return null;
        }

        public void LinkMember(GroupReference pRef)
        {
            m_memberMgr.InsertFirst(pRef);
        }

        void DelinkMember(ObjectGuid guid)
        {
            GroupReference refe = m_memberMgr.GetFirst();
            while (refe != null)
            {
                GroupReference nextRef = refe.Next();
                if (refe.GetSource().GetGUID() == guid)
                {
                    refe.Unlink();
                    break;
                }
                refe = nextRef;
            }
        }

        public Dictionary<uint, InstanceBind> GetBoundInstances(Difficulty difficulty)
        {
            return m_boundInstances.LookupByKey(difficulty);
        }

        void _initRaidSubGroupsCounter()
        {
            // Sub group counters initialization
            if (m_subGroupsCounts == null)
                m_subGroupsCounts = new byte[MapConst.MaxRaidSubGroups];

            foreach (var memberSlot in m_memberSlots)
                ++m_subGroupsCounts[memberSlot.group];
        }

        MemberSlot _getMemberSlot(ObjectGuid guid)
        {
            foreach (var member in m_memberSlots)
                if (member.guid == guid)
                    return member;
            return null;
        }

        void SubGroupCounterIncrease(byte subgroup)
        {
            if (m_subGroupsCounts != null)
                ++m_subGroupsCounts[subgroup];
        }

        void SubGroupCounterDecrease(byte subgroup)
        {
            if (m_subGroupsCounts != null)
                --m_subGroupsCounts[subgroup];
        }

        public void RemoveUniqueGroupMemberFlag(GroupMemberFlags flag)
        {
            foreach (var member in m_memberSlots)
                if (member.flags.HasAnyFlag(flag))
                    member.flags &= ~flag;
        }

        void ToggleGroupMemberFlag(MemberSlot slot, GroupMemberFlags flag, bool apply)
        {
            if (apply)
                slot.flags |= flag;
            else
                slot.flags &= ~flag;
        }

        public void StartLeaderOfflineTimer()
        {
            _isLeaderOffline = true;
            _leaderOfflineTimer.Reset(2 * Time.Minute * Time.InMilliseconds);
        }

        public void StopLeaderOfflineTimer()
        {
            _isLeaderOffline = false;
        }

        public void SetEveryoneIsAssistant(bool apply)
        {
            if (apply)
                m_groupFlags |= GroupFlags.EveryoneAssistant;
            else
                m_groupFlags &= ~GroupFlags.EveryoneAssistant;

            foreach (MemberSlot member in m_memberSlots)
                ToggleGroupMemberFlag(member, GroupMemberFlags.Assistant, apply);

            SendUpdate();
        }

        public GroupCategory GetGroupCategory() { return m_groupCategory; }

        public uint GetDbStoreId() { return m_dbStoreId; }
        public List<MemberSlot> GetMemberSlots() { return m_memberSlots; }
        public GroupReference GetFirstMember() { return (GroupReference)m_memberMgr.GetFirst(); }
        public uint GetMembersCount() { return (uint)m_memberSlots.Count; }
        public uint GetInviteeCount() { return (uint)m_invitees.Count; }
        public GroupFlags GetGroupFlags() { return m_groupFlags; }

        bool IsReadyCheckStarted() { return m_readyCheckStarted; }

        public void BroadcastWorker(Action<Player> worker)
        {
            for (GroupReference refe = GetFirstMember(); refe != null; refe = refe.Next())
                worker(refe.GetSource());
        }

        List<MemberSlot> m_memberSlots = new();
        GroupRefManager m_memberMgr = new();
        List<Player> m_invitees = new();
        ObjectGuid m_leaderGuid;
        byte m_leaderFactionGroup;
        string m_leaderName;
        GroupFlags m_groupFlags;
        GroupCategory m_groupCategory;
        Difficulty m_dungeonDifficulty;
        Difficulty m_raidDifficulty;
        Difficulty m_legacyRaidDifficulty;
        Battleground m_bgGroup;
        BattleField m_bfGroup;
        ObjectGuid[] m_targetIcons = new ObjectGuid[MapConst.TargetIconsCount];
        LootMethod m_lootMethod;
        ItemQuality m_lootThreshold;
        ObjectGuid m_looterGuid;
        ObjectGuid m_masterLooterGuid;
        List<Roll> RollId = new();
        Dictionary<Difficulty, Dictionary<uint, InstanceBind>> m_boundInstances = new();
        byte[] m_subGroupsCounts;
        ObjectGuid m_guid;
        uint m_maxEnchantingLevel;
        uint m_dbStoreId;
        bool _isLeaderOffline;
        TimeTracker _leaderOfflineTimer = new();

        // Ready Check
        bool m_readyCheckStarted;
        TimeSpan m_readyCheckTimer;

        // Raid markers
        RaidMarker[] m_markers = new RaidMarker[MapConst.RaidMarkersCount];
        uint m_activeMarkers;

        public static implicit operator bool(Group group)
        {
            return group != null;
        }
    }

    public class Roll : LootValidatorRef
    {
        public Roll(LootItem li)
        {
            itemid = li.itemid;
            itemRandomBonusListId = li.randomBonusListId;
            itemCount = li.count;
            rollTypeMask = RollMask.AllNoDisenchant;
        }

        public void SetLoot(Loot pLoot)
        {
            Link(pLoot, this);
        }

        public Loot GetLoot()
        {
            return GetTarget();
        }

        public override void TargetObjectBuildLink()
        {
            // called from link()
            GetTarget().AddLootValidatorRef(this);
        }

        public void FillPacket(LootItemData lootItem)
        {
            lootItem.UIType = (totalPlayersRolling > totalNeed + totalGreed + totalPass) ? LootSlotType.RollOngoing : LootSlotType.AllowLoot;
            lootItem.Quantity = itemCount;
            lootItem.LootListID = (byte)(itemSlot + 1);

            LootItem lootItemInSlot = GetTarget().GetItemInSlot(itemSlot);
            if (lootItemInSlot != null)
            {
                lootItem.CanTradeToTapList = lootItemInSlot.allowedGUIDs.Count > 1;
                lootItem.Loot = new ItemInstance(lootItemInSlot);
            }
        }

        public ItemDisenchantLootRecord GetItemDisenchantLoot(Player player)
        {
            LootItem lootItemInSlot = GetTarget().GetItemInSlot(itemSlot);
            if (lootItemInSlot != null)
            {
                ItemInstance itemInstance = new(lootItemInSlot);
                BonusData bonusData = new(itemInstance);
                if (!bonusData.CanDisenchant)
                    return null;

                ItemTemplate itemTemplate = Global.ObjectMgr.GetItemTemplate(itemid);
                uint itemLevel = Item.GetItemLevel(itemTemplate, bonusData, player.GetLevel(), 0, 0, 0, 0, false, 0);
                return Item.GetDisenchantLoot(itemTemplate, (uint)bonusData.Quality, itemLevel);
            }

            return null;
        }

        public uint itemid;
        public uint itemRandomBonusListId;
        public byte itemCount;
        public Dictionary<ObjectGuid, RollType> playerVote = new();
        public byte totalPlayersRolling;
        public byte totalNeed;
        public byte totalGreed;
        public byte totalPass;
        public byte itemSlot;
        public RollMask rollTypeMask;
    }

    public class MemberSlot
    {
        public ObjectGuid guid;
        public string name;
        public Race race;
        public byte _class;
        public byte group;
        public GroupMemberFlags flags;
        public LfgRoles roles;
        public bool readyChecked;
    }

    public class RaidMarker
    {
        public RaidMarker(uint mapId, float positionX, float positionY, float positionZ, ObjectGuid transportGuid = default)
        {
            Location = new WorldLocation(mapId, positionX, positionY, positionZ);
            TransportGUID = transportGuid;
        }

        public WorldLocation Location;
        public ObjectGuid TransportGUID;
    }
}
