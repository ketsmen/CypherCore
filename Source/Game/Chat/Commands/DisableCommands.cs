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
using Game.DataStorage;
using System;

namespace Game.Chat.Commands
{
    [CommandGroup("disable")]
    class DisableCommands
    {
        [CommandGroup("add")]
        class DisableAddCommands
        {
            static bool HandleAddDisables(uint entry, uint flags, string disableComment, CommandHandler handler, DisableType disableType)
            {
                if (entry == 0)
                    return false;

                if (disableComment.IsEmpty())
                    return false;

                switch (disableType)
                {
                    case DisableType.Spell:
                        {
                            if (!Global.SpellMgr.HasSpellInfo(entry, Difficulty.None))
                            {
                                handler.SendSysMessage(CypherStrings.CommandNospellfound);
                                return false;
                            }
                            break;
                        }
                    case DisableType.Quest:
                        {
                            if (Global.ObjectMgr.GetQuestTemplate(entry) == null)
                            {
                                handler.SendSysMessage(CypherStrings.CommandNoquestfound, entry);
                                return false;
                            }
                            break;
                        }
                    case DisableType.Map:
                        {
                            if (!CliDB.MapStorage.ContainsKey(entry))
                            {
                                handler.SendSysMessage(CypherStrings.CommandNomapfound);
                                return false;
                            }
                            break;
                        }
                    case DisableType.Battleground:
                        {
                            if (!CliDB.BattlemasterListStorage.ContainsKey(entry))
                            {
                                handler.SendSysMessage(CypherStrings.CommandNoBattlegroundFound);
                                return false;
                            }
                            break;
                        }
                    case DisableType.Criteria:
                        {
                            if (Global.CriteriaMgr.GetCriteria(entry) == null)
                            {
                                handler.SendSysMessage(CypherStrings.CommandNoAchievementCriteriaFound);
                                return false;
                            }
                            break;
                        }
                    case DisableType.OutdoorPVP:
                        {
                            if (entry > (int)OutdoorPvPTypes.Max)
                            {
                                handler.SendSysMessage(CypherStrings.CommandNoOutdoorPvpForund);
                                return false;
                            }
                            break;
                        }
                    case DisableType.VMAP:
                        {
                            if (!CliDB.MapStorage.ContainsKey(entry))
                            {
                                handler.SendSysMessage(CypherStrings.CommandNomapfound);
                                return false;
                            }
                            break;
                        }
                    case DisableType.MMAP:
                        {
                            if (!CliDB.MapStorage.ContainsKey(entry))
                            {
                                handler.SendSysMessage(CypherStrings.CommandNomapfound);
                                return false;
                            }
                            break;
                        }
                    default:
                        break;
                }

                PreparedStatement stmt = DB.World.GetPreparedStatement(WorldStatements.SEL_DISABLES);
                stmt.AddValue(0, entry);
                stmt.AddValue(1, (byte)disableType);
                SQLResult result = DB.World.Query(stmt);
                if (!result.IsEmpty())
                {
                    handler.SendSysMessage($"This {disableType} (Id: {entry}) is already disabled.");
                    return false;
                }

                stmt = DB.World.GetPreparedStatement(WorldStatements.INS_DISABLES);
                stmt.AddValue(0, entry);
                stmt.AddValue(1, (byte)disableType);
                stmt.AddValue(2, flags);
                stmt.AddValue(3, disableComment);
                DB.World.Execute(stmt);

                handler.SendSysMessage($"Add Disabled {disableType} (Id: {entry}) for reason {disableComment}");
                return true;
            }

            [Command("spell", RBACPermissions.CommandDisableAddSpell, true)]
            static bool HandleAddDisableSpellCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.Spell);
            }

            [Command("quest", RBACPermissions.CommandDisableAddQuest, true)]
            static bool HandleAddDisableQuestCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.Quest);
            }

            [Command("map", RBACPermissions.CommandDisableAddMap, true)]
            static bool HandleAddDisableMapCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.Map);
            }

            [Command("Battleground", RBACPermissions.CommandDisableAddBattleground, true)]
            static bool HandleAddDisableBattlegroundCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.Battleground);
            }

            [Command("criteria", RBACPermissions.CommandDisableAddCriteria, true)]
            static bool HandleAddDisableCriteriaCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.Criteria);
            }

            [Command("outdoorpvp", RBACPermissions.CommandDisableAddOutdoorpvp, true)]
            static bool HandleAddDisableOutdoorPvPCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.OutdoorPVP);
            }

            [Command("vmap", RBACPermissions.CommandDisableAddVmap, true)]
            static bool HandleAddDisableVmapCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.VMAP);
            }

            [Command("mmap", RBACPermissions.CommandDisableAddMmap, true)]
            static bool HandleAddDisableMMapCommand(CommandHandler handler, uint entry, uint flags, string disableComment)
            {
                return HandleAddDisables(entry, flags, disableComment, handler, DisableType.MMAP);
            }
        }

        [CommandGroup("remove")]
        class DisableRemoveCommands
        {
            static bool HandleRemoveDisables(uint entry, CommandHandler handler, DisableType disableType)
            {
                if (entry == 0)
                    return false;

                PreparedStatement stmt = DB.World.GetPreparedStatement(WorldStatements.SEL_DISABLES);
                stmt.AddValue(0, entry);
                stmt.AddValue(1, (byte)disableType);
                SQLResult result = DB.World.Query(stmt);
                if (result.IsEmpty())
                {
                    handler.SendSysMessage($"This {disableType} (Id: {entry}) is not disabled.");
                    return false;
                }

                stmt = DB.World.GetPreparedStatement(WorldStatements.DEL_DISABLES);
                stmt.AddValue(0, entry);
                stmt.AddValue(1, (byte)disableType);
                DB.World.Execute(stmt);

                handler.SendSysMessage($"Remove Disabled {disableType} (Id: {entry})");
                return true;
            }

            [Command("spell", RBACPermissions.CommandDisableRemoveSpell, true)]
            static bool HandleRemoveDisableSpellCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.Spell);
            }

            [Command("quest", RBACPermissions.CommandDisableRemoveQuest, true)]
            static bool HandleRemoveDisableQuestCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.Quest);
            }

            [Command("map", RBACPermissions.CommandDisableRemoveMap, true)]
            static bool HandleRemoveDisableMapCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.Map);
            }

            [Command("Battleground", RBACPermissions.CommandDisableRemoveBattleground, true)]
            static bool HandleRemoveDisableBattlegroundCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.Battleground);
            }

            [Command("criteria", RBACPermissions.CommandDisableRemoveCriteria, true)]
            static bool HandleRemoveDisableCriteriaCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.Criteria);
            }

            [Command("outdoorpvp", RBACPermissions.CommandDisableRemoveOutdoorpvp, true)]
            static bool HandleRemoveDisableOutdoorPvPCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.OutdoorPVP);
            }

            [Command("vmap", RBACPermissions.CommandDisableRemoveVmap, true)]
            static bool HandleRemoveDisableVmapCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.VMAP);
            }

            [Command("mmap", RBACPermissions.CommandDisableRemoveMmap, true)]
            static bool HandleRemoveDisableMMapCommand(CommandHandler handler, uint entry)
            {
                return HandleRemoveDisables(entry, handler, DisableType.MMAP);
            }
        }
    }
}
