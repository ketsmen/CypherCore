DELETE FROM `spell_proc` WHERE `SpellId` IN (234113,235003,235007,235011,235015,235017,235019,236308,236503,238101,238133,238142,238524,242207,242209,242215,242248,242262,242285,242628,243994,244034,244066,244134,244330,245729,247625,248168,248171,248173,248175,248198,248199,248200,248218,248256,248257,248260,248262,248264,248265,248266,248267,248268,248269,248274,248276,248277,248278,250075,250768,250846);
INSERT INTO `spell_proc` (`SpellId`,`SchoolMask`,`SpellFamilyName`,`SpellFamilyMask0`,`SpellFamilyMask1`,`SpellFamilyMask2`,`SpellFamilyMask3`,`ProcFlags`,`ProcFlags2`,`SpellTypeMask`,`SpellPhaseMask`,`HitMask`,`AttributesMask`,`DisableEffectsMask`,`ProcsPerMinute`,`Chance`,`Cooldown`,`Charges`) VALUES
(234113,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x3,0x0,0x0,0,0,0,0), -- Arrogance
(235003,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Iron Protection
(235007,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x402,0x0,0x0,0,0,0,0), -- Searing Words
(235011,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x2,0x2,0x403,0x0,0x0,0,0,0,0), -- Price of Progress
(235015,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x2,0x2,0x403,0x0,0x0,0,0,0,0), -- Melon-choly
(235017,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Fermenting Furuncle
(235019,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Windswept
(236308,0x00,4,0x00000000,0x00004000,0x00000000,0x00000800,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Storm of Destruction
(236503,0x00,107,0x00200000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x4,0x2,0x0,0x0,0x0,0,0,0,0), -- Demon Essence Absorber
(238101,0x00,6,0x00000000,0x00080000,0x00000000,0x00000000,0x0,0x0,0x1,0x1,0x403,0x0,0x0,0,0,0,0), -- Mind Quickening
(238133,0x00,10,0x00800000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Blessed Stalwart
(238142,0x00,11,0x00200000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Lashing Flames
(238524,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Fel Barbs
(242207,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x3,0x0,0x0,0,0,0,0), -- Infernal Skin
(242209,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Infernal Skin
(242215,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Infernal Cinders
(242248,0x00,3,0x00000000,0x00000020,0x00000000,0x00000000,0x0,0x0,0x4,0x1,0x0,0x0,0x0,0,0,0,0), -- Item - Mage T20 Arcane 2P Bonus
(242262,0x00,10,0x00000000,0x00000000,0x00040000,0x00000000,0x0,0x0,0x4,0x1,0x0,0x0,0x0,0,0,0,0), -- Item - Paladin T20 Holy 4P Bonus
(242285,0x00,11,0x00000000,0x00000010,0x00000000,0x00000000,0x0,0x0,0x4,0x1,0x0,0x0,0x0,0,0,0,0), -- Item - Shaman T20 Enhancement 4P Bonus
(242628,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Cunning of the Deceiver
(243994,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Empyrean Demolisher
(244034,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Emerald Shadowfang
(244066,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Madness of the Betrayer
(244134,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x2,0x2,0x403,0x0,0x0,0,0,0,0), -- Memento of Tyrande
(244330,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Siphon Essence
(245729,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Meteor Shard
(247625,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Lightblood Elixir
(248168,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Force of Magma
(248171,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Smoldering Claw
(248173,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Drakefang Butcher
(248175,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Flame Wrath
(248198,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Stoneslayer
(248199,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Demonshear
(248200,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Naglering
(248218,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Houndmaster's Weapons
(248256,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Firebreather
(248257,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Bloodfist
(248260,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Barman Shanker
(248262,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Skullforge Reaver
(248264,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Keris of Zul'Serak
(248265,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- The Cruel Hand of Timmy
(248266,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Fist of the Damned
(248267,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Wraith Scythe
(248268,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Soul Breaker
(248269,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Blade of the Wretched
(248274,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Galgann's Firehammer
(248276,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Venomspitter
(248277,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Hookfang Shanker
(248278,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0), -- Lord General's Sword
(250075,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x402,0x0,0x0,0,0,0,0), -- Item - Attack Crits Proc Agi
(250768,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x2,0x2,0x403,0x0,0x0,0,0,0,0), -- Whispers of L'ura
(250846,0x00,0,0x00000000,0x00000000,0x00000000,0x00000000,0x0,0x0,0x1,0x2,0x403,0x0,0x0,0,0,0,0); -- Void Tendril

DELETE FROM `spell_script_names` WHERE `ScriptName` IN ('spell_item_seal_of_darkshire_nobility','spell_item_lightblood_elixir');
INSERT INTO `spell_script_names` (`spell_id`, `ScriptName`) VALUES
(234113 ,'spell_item_seal_of_darkshire_nobility'),
(247625 ,'spell_item_lightblood_elixir');
