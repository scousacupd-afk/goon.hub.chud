using Blitz_Tag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    public class MothershipService : ControllerBase
    {
        #region Constants

        private static readonly RotatingQuestList AllActiveQuests = new()
        {
            DailyQuests = [
                new QuestGroup
                {
                    Name = "Gameplay",
                    SelectCount = 1,
                    Quests = [
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 11,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY INFECTION",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "INFECTION",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.forest,
                                GTZone.canyon,
                                GTZone.beach,
                                GTZone.mountain,
                                GTZone.skyJungle,
                                GTZone.cave,
                                GTZone.Metropolis,
                                GTZone.rotating,
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 19,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY PAINTBRAWL",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "PAINTBRAWL",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.forest,
                                GTZone.canyon,
                                GTZone.beach,
                                GTZone.mountain,
                                GTZone.skyJungle,
                                GTZone.cave,
                                GTZone.Metropolis,
                                GTZone.rotating,
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 13,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY FREEZE TAG",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "FREEZE TAG",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.forest,
                                GTZone.canyon,
                                GTZone.beach,
                                GTZone.mountain,
                                GTZone.skyJungle,
                                GTZone.cave,
                                GTZone.Metropolis,
                                GTZone.rotating,
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 1,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY GUARDIAN",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "GUARDIAN",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.forest,
                                GTZone.canyon,
                                GTZone.beach,
                                GTZone.mountain,
                                GTZone.cave,
                                GTZone.Metropolis,
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 4,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "TAG PLAYERS",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GameModeTag",
                            RequiredOccurenceCount = 2,
                            RequiredZones = [
                                GTZone.none
                            ]
                        }
                    ]
                },
                new QuestGroup
                {
                    Name = "Ghost Reactor",
                    SelectCount = 1,
                    Quests = [
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 35,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "COLLECT GHOST CORES AS A CREW",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRCollectCore",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 36,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "SMASH BREAKABLES IN GHOST REACTOR",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRSmashBreakable",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 37,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PURGE GHOSTS AS A CREW",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRKillEnemy",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 39,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "BREAK A GHOST'S ARMOR",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRArmorBreak",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 40,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "END A SHIFT WITH MORE PURGES THAN INCIDENTS",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRShiftGoodKD",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        }
                    ]
                },
                new QuestGroup
                {
                    Name = "Exploration",
                    SelectCount = 2,
                    Quests = [
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 5,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "RIDE THE SHARK",
                            QuestType = QuestType.grabObject,
                            QuestOccurenceFilter = "ReefSharkRing",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 9,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY THE PIANO",
                            QuestType = QuestType.tapObject,
                            QuestOccurenceFilter = "Piano_Collapsed_Key",
                            RequiredOccurenceCount = 10,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 14,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "THROW SNOWBALLS",
                            QuestType = QuestType.launchedProjectile,
                            QuestOccurenceFilter = "SnowballProjectile",
                            RequiredOccurenceCount = 10,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 15,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "GO FOR A SWIM",
                            QuestType = QuestType.swimDistance,
                            QuestOccurenceFilter = "",
                            RequiredOccurenceCount = 200,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 21,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "CLIMB THE TALLEST TREE",
                            QuestType = QuestType.enterLocation,
                            QuestOccurenceFilter = "TallestTree",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.forest
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 22,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "COMPLETE THE OBSTACLE COURSE",
                            QuestType = QuestType.enterLocation,
                            QuestOccurenceFilter = "ObstacleCourse",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 23,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "SWIM UNDER A WATERFALL",
                            QuestType = QuestType.enterLocation,
                            QuestOccurenceFilter = "UnderWaterfall",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 24,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "SNEAK UPSTAIRS IN THE STORE",
                            QuestType = QuestType.enterLocation,
                            QuestOccurenceFilter = "SecretStore",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 25,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "CLIMB INTO THE CROW'S NEST",
                            QuestType = QuestType.enterLocation,
                            QuestOccurenceFilter = "CrowsNest",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 26,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "GO FOR A WALK",
                            QuestType = QuestType.moveDistance,
                            QuestOccurenceFilter = "",
                            RequiredOccurenceCount = 500,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 28,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "GET SMALL",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "SizeSmall",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 29,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "GET BIG",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "SizeLarge",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 31,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "ADD A CRITTER TO YOUR COLLECTION",
                            QuestType = QuestType.critter,
                            QuestOccurenceFilter = "Collect",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 32,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "DONATE A CRITTER",
                            QuestType = QuestType.critter,
                            QuestOccurenceFilter = "Donate",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        }
                    ]
                },
                new QuestGroup
                {
                    Name = "Social",
                    SelectCount = 1,
                    Quests = [
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 2,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "HIGH FIVE PLAYERS",
                            QuestType = QuestType.triggerHandEffect,
                            QuestOccurenceFilter = "HIGH_FIVE",
                            RequiredOccurenceCount = 10,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 3,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "FIST BUMP PLAYERS",
                            QuestType = QuestType.triggerHandEffect,
                            QuestOccurenceFilter = "FIST_BUMP",
                            RequiredOccurenceCount = 10,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 16,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "FIND SOMETHING TO EAT",
                            QuestType = QuestType.eatObject,
                            QuestOccurenceFilter = "",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 30,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "MAKE A FRIENDSHIP BRACELET",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "FriendshipGroupJoined",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        }
                    ]
                }
            ],
            WeeklyQuests = [
                new QuestGroup
                {
                    Name = "Gameplay",
                    SelectCount = 1,
                    Quests = [
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 17,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY INFECTION",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "INFECTION",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 20,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY PAINTBRAWL",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "PAINTBRAWL",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 8,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY FREEZE TAG",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "FREEZE TAG",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 10,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PLAY GUARDIAN",
                            QuestType = QuestType.gameModeRound,
                            QuestOccurenceFilter = "GUARDIAN",
                            RequiredOccurenceCount = 25,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 12,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "TAG PLAYERS",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GameModeTag",
                            RequiredOccurenceCount = 10,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 41,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "PURGE GHOSTS AS A CREW",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRKillEnemy",
                            RequiredOccurenceCount = 25,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 42,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "SMASH BREAKABLES IN GHOST REACTOR",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRSmashBreakable",
                            RequiredOccurenceCount = 25,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 38,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "GET A GORILLACORP PROMOTION",
                            QuestType = QuestType.misc,
                            QuestOccurenceFilter = "GRPromoted",
                            RequiredOccurenceCount = 1,
                            RequiredZones = [
                                GTZone.none
                            ]
                        }
                    ]
                },
                new QuestGroup
                {
                    Name = "Exploration and Social",
                    SelectCount = 1,
                    Quests = [
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 33,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "COLLECT CRITTERS",
                            QuestType = QuestType.critter,
                            QuestOccurenceFilter = "Collect",
                            RequiredOccurenceCount = 5,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = true,
                            QuestId = 34,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "DONATE CRITTERS",
                            QuestType = QuestType.critter,
                            QuestOccurenceFilter = "Donate",
                            RequiredOccurenceCount = 10,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 6,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "THROW SNOWBALLS",
                            QuestType = QuestType.launchedProjectile,
                            QuestOccurenceFilter = "SnowballProjectile",
                            RequiredOccurenceCount = 50,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 7,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "GO FOR A LONG SWIM",
                            QuestType = QuestType.swimDistance,
                            QuestOccurenceFilter = "",
                            RequiredOccurenceCount = 1000,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 18,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "EAT FOOD",
                            QuestType = QuestType.eatObject,
                            QuestOccurenceFilter = "",
                            RequiredOccurenceCount = 25,
                            RequiredZones = [
                                GTZone.none
                            ]
                        },
                        new RotatingQuest
                        {
                            Disable = false,
                            QuestId = 27,
                            Weight = 1,
                            Category = QuestCategory.NONE,
                            QuestName = "GO FOR A LONG WALK",
                            QuestType = QuestType.moveDistance,
                            QuestOccurenceFilter = "",
                            RequiredOccurenceCount = 2500,
                            RequiredZones = [
                                GTZone.none
                            ]
                        }
                    ]
                }
            ]
        };

        private static readonly List<MothershipTitleDataResponseKey> MothershipTitleData = [
            new MothershipTitleDataResponseKey
            {
                Key = "PlayersInfoData",
                Data = JsonConvert.SerializeObject(FunctionService.Function.insta)
            },
            new MothershipTitleDataResponseKey
            {
                Key = "PromoHutSignText",
                Data = "NEW GT MERCH DROP!\nGRAB WHILE YOU CAN\nIN-GAME REWARD WITH SELECT PURCHASES!\nSHOPGTAG.COM"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "SeasonalStoreBoardSign",
                Data = "WELCOME TO BLITZ TAG!"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "TOBDefCompTxt",
                Data = "PLEASE SELECT A PACK TO TRY ON AND BUY"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "TOBAlreadyOwnCompTxt",
                Data = "YOU OWN THE BUNDLE ALREADY! THANK YOU!"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "TOBAlreadyOwnCompTxt",
                Data = "YOU OWN THE BUNDLE ALREADY! THANK YOU!"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "CityBoxes",
                Data = "{\"Data\":[{\"TitleDataObjectID\":\"BoxesGroup1\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"1/30/2026 19:00:00\",\"EndDateTime\":\"1/30/3333 19:00:00\"}],\"RelativeDateTimeWindow\":[{\"StartDateTime\":{\"DaysPast\":0,\"Hour\":0,\"Minute\":0},\"EndDateTime\":{\"DaysPast\":0,\"Hour\":0,\"Minute\":0}}]},{\"TitleDataObjectID\":\"BoxesGroup2\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/3/2026 19:00:00\",\"EndDateTime\":\"1/30/3333 19:00:00\"}],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"BoxesGroup3\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/6/2026 19:00:00\",\"EndDateTime\":\"1/30/3333 19:00:00\"}],\"RelativeDateTimeWindow\":[]}]}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "CityObjectSchedule",
                Data = "{\"Data\":[{\"TitleDataObjectID\":\"Clock\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"1/1/2036 12:00:00 AM\",\"EndDateTime\":\"1/1/3001 12:00:00 AM\"}],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"HowManyMonke\",\"AbsoluteDateTimeWindow\":[],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"GiantTV\",\"AbsoluteDateTimeWindow\":[],\"RelativeDateTimeWindow\":[]}]}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "HBD2L",
                Data = "{\"Data\":[{\"TitleDataObjectID\":\"pre-event\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"1/1/2000 12:00:00 AM\",\"EndDateTime\":\"2/7/2026 6:55:00 PM\"}],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"intro-event\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/7/2026 6:55:00 PM\",\"EndDateTime\":\"2/7/2026 7:00:00 PM\"}],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"event\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/7/2026 7:00:00 PM\",\"EndDateTime\":\"2/7/2026 7:04:49 PM\"}],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"post-event\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/7/2026 7:04:49 PM\",\"EndDateTime\":\"1/1/3000 12:00:00 AM\"}],\"RelativeDateTimeWindow\":[]}]}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "BirthdayProps",
                Data = "{\"Data\":[{\"TitleDataObjectID\":\"Candles\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/7/2026 7:00:00 PM\",\"EndDateTime\":\"2/9/2026 8:00:00 AM\"}],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"TimedStore\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/7/2026 7:00:00 PM\",\"EndDateTime\":\"2/9/2026 7:59:00 AM\"}],\"RelativeDateTimeWindow\":[]},{\"TitleDataObjectID\":\"TimedSubs\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"2/5/2026 6:00:00 PM\",\"EndDateTime\":\"2/9/2026 7:59:00 AM\"}],\"RelativeDateTimeWindow\":[]}]}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "PromoStand20260130",
                Data = "{\"Data\":[{\"TitleDataObjectID\":\"PromoStand20260130\",\"AbsoluteDateTimeWindow\":[{\"StartDateTime\":\"1/30/2026 6:00:00 PM\",\"EndDateTime\":\"1/1/2999 12:00:00 AM\"}],\"RelativeDateTimeWindow\":[]}]}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "ClockHeadings",
                Data = "GORILLA CORP TIME;YOUR LOCAL TIME"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "SpeakerVoiceToLoudnessConfig",
                Data = "{\"EnableLoudnessLimit\":true,\"LoudnessLimitThreshold\":0.5}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "CountdownTimer",
                Data = "2/7/2026 19:00:00"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "JMANTYS",
                Data = ""
            },
            new MothershipTitleDataResponseKey
            {
                Key = "TOBDefPurchaseBtnDefTxt",
                Data = "PURCHASE ITEMS IN YOUR CART AT THE CHECKOUT COUNTER"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "TOBAlreadyOwnPurchaseBtnTxt",
                Data = "-"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "PublicCrittersGrabSettings",
                Data = $"{(int)AllowGrabbingFlags.None}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "PrivateCrittersGrabSettings",
                Data = $"{AllowGrabbingFlags.EntireBag | AllowGrabbingFlags.FromBags | AllowGrabbingFlags.OutOfHands}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "MOTD",
                Data = "[ <color=red>W</color><color=orange>E</color><color=yellow>L</color><color=green>C</color><color=green>O</color><color=blue>M</color><color=purple>E</color> <color=red>T</color><color=orange>O</color> <color=yellow>B</color><color=green>L</color><color=green>I</color><color=blue>T</color><color=purple>Z</color> <color=red>T</color><color=orange>A</color><color=yellow>G</color> ]\n<color=green>DISCORD.GG/ANOTHERAXIOM</color>\n<color=red>THE CURRENT UPDATE: NEWEST!!</color>\n<color=orange>YOU CAN CHANGE YOUR ARM LENGTH ON THE SETTINGS TAB ON THE COMPUTER</color>\n\n\n\n\n\n\n\n\n<color=yellow>MAIN OWNERS: NEVA, BXT.</color>"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "PUNErrorLogging",
                Data = $"{PUNLogFlags.SerializeView | PUNLogFlags.OwnershipTransfer | PUNLogFlags.OwnershipRequest | PUNLogFlags.OwnershipUpdate | PUNLogFlags.RPC | PUNLogFlags.Instantiate | PUNLogFlags.Destroy | PUNLogFlags.DestroyPlayer}"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "AllPurchasesPromo",
                Data = "purchases get\nlemming plush"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "UltraRarePromo",
                Data = "may win rare\ngold lemming"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "EnableTempCosmeticUnlocks",
                Data = "true"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "COC",
                Data = "-NO RACISM, SEXISM, HOMOPHOBIA, TRANSPHOBIA, OR OTHER BIGOTRY\n-NO CHEATS OR MODS\n-DO NOT HARASS OTHER PLAYERS OR INTENTIONALLY MAKE THEM UNCOMFORTABLE\n-DO NOT TROLL OR GRIEF LOBBIES BY BEING UNCATCHABLE OR BY ESCAPING THE MAP. TRY TO MAKE SURE EVERYONE IS HAVING FUN\n-IF SOMEONE IS BREAKING THIS CODE, PLEASE REPORT THEM\n-PLEASE BE NICE GORILLAS AND HAVE A GOOD TIME"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "PropHuntProps",
                Data = "LMAAZ.\nLHABC.\nLMAAI.\nLHAIA.\nLHABR.\nLMABJ.\nLFACR.\nLMAOY.\nLMAGR.\nLMAAJ.\nLMAHX.\nLFADU.\nLBABC.\nLHADV.\nLFABC.\nLMAAN.\nLHAGJ.\nLHACY.\nLHAEU.\nLHAHM.\nLFACB.\nLFACA.\nLHAAE.\nLMAHY.\nLHABK.\nLHAEK.\nLMANV.\nLMADL.\nLHAIB.\nLMABS.\nLMALF.\nLHAAG.\nLHADL.\nLHACU.\nLHAIG.\nLHAHX.\nLHAGA.\nLFACG.\nLHAAK.\nLFABX.\nLMACS.\nLHAAH.\nLHAJC.\nLMALO.\nLMAAK.\nLMAJU.\nLMALH.\nLHACE.\nLMANB.\nLMAEK.\nLFAFH.\nLMAKK.\nLFAAJ.\nLBABB.\nLHAGY.\nLMAAA.\nLMAFG.\nLHAFG.\nLFAAW.\nLHADW.\nLMAEU.\nLHACS.\nLMADX.\nLMALS.\nLHACJ.\nLMAFI.\nLFAFP.\nLFACK.\nLHAEZ.\nLHABY.\nLMABR.\nLHAFB.\nLMALY.\nLHABD.\nLHAGC.\nLMABH.\nLHADM.\nLHABA.\nLFAFB.\nLHAED.\nLFAFY.\nLHADQ.\nLMAFE.\nLFAEL.\nLFAEA.\nLMADW.\nLMAFA.\nLHACA.\nLHAHV.\nLFAEC.\nLMAKD.\nLFAAO.\nLFAAV.\nLHAAA.\nLMAGZ.\nLHABG.\nLMAOL.\nLHAHQ.\nLMAKF.\nLFAFZ.\nLFAFY.\nLFAHA.\nLHAJG.\nLFAHK.\nLHACX.\nLBAAP.\nLFABE.\nLMABC.\nLMAAW.\nLHAAC."
            },
            new MothershipTitleDataResponseKey
            {
                Key = "ArenaForestSign",
                Data = "CLOSED - \nNO MONKE BUSINESS\n\n-THE BOARD"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "VODSchedule",
                Data = JsonConvert.SerializeObject(new VODSchedule {
                    Hourly = [
                        /*new VODHourlyStream
                        {
                           Stream = new VODStream
                           {
                               Name = "Binly's Review",
                               Url = "https://cdn.nest.rip/uploads/ee460d91-08f1-4ab2-81d9-184504701cec.mp4",
                               Duration = 0,
                               Type = VODStream.VODStreamType.VIDEO
                           },
                           Minute = 0,
                           StartDateTime = DateTime.MinValue.ToString(),
                           EndDateTime = DateTime.MaxValue.ToString()
                        },
                        new VODHourlyStream
                        {
                           Stream = new VODStream
                           {
                               Name = "Binly's Review",
                               Url = "https://cdn.nest.rip/uploads/ee460d91-08f1-4ab2-81d9-184504701cec.mp4",
                               Duration = 0,
                               Type = VODStream.VODStreamType.VIDEO
                           },
                           Minute = 30,
                           StartDateTime = DateTime.MinValue.ToString(),
                           EndDateTime = DateTime.MaxValue.ToString()
                        }*/
                    ]
                })
            },
            new MothershipTitleDataResponseKey
            {
                Key = "SharedBlocksStartingMapConfig",
                Data = "{\n    \"pageNumber\": 0,\n    \"pageSize\": 6,\n    \"sortMethod\": \"Top\",\n    \"useMapID\": false,\n    \"mapID\": \"\"\n  }"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "BdayStoreEnds",
                Data = "2/9/2026 7:59:00 AM"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "EnableLckCosmetics",
                Data = "true"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "ActivationReferenceDate",
                Data = "2/6/2026"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "DeployFeatureFlags",
                Data = JsonConvert.SerializeObject(new FeatureFlagListData
                {
                    Flags = [
                        new FeatureFlagData { Name = "2024-06-CosmeticsAuthenticationV2", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-04-CosmeticsAuthenticationV2-SetData", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-04-CosmeticsAuthenticationV2-ReadData", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-04-CosmeticsAuthenticationV2-Compat", Value = 0, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-04-KIDOptIn", Value = 0, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-04-KIDRequired", Value = 0, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-07-UseMothershipIdAsGgwpId", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-07-ReportReportEventsToGgwp", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-07-ReportMuteEventsToGgwp", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-07-ValidateNamesWithGgwp", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-09-PlayFabAnalyticsSampleRate", Value = 100, ValueType = "percent" }, // disable when dashboard complete
                        new FeatureFlagData { Name = "2025-09-MothershipAnalyticsSampleRate", Value = 0, ValueType = "percent" }, // enable when dashboard complete
                        new FeatureFlagData { Name = "2025-10-FriendCodes", Value = 0, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-10-TelemetryZoneEventSampling", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-10-PlayfabBansEnabled", Value = 100, ValueType = "percent" },
                        new FeatureFlagData { Name = "2025-10-MothershipBansEnabled", Value = 0, ValueType = "percent" },
                    ]
                })
            },
            new MothershipTitleDataResponseKey
            {
                Key = "CreditsData",
                Data = JsonConvert.SerializeObject(new List<CreditsSection>
                {
                    new() 
                    {
                        Title = "DEV TEAM",
                        Entries = [
                            "bxt - Lead Developer",
                            "Neva - Developer"
                        ]
                    },
                    new()
                    {
                        Title = "SPECIAL THANKS",
                        Entries = [
                            "Czangilos - Telling me i'm stupid when I need it",
                            "Yeezy - Staff Menu",
                            "Pubert - GUI/HUD Manager",
                            "The \"Sticks\"",
                            "And you, for supporting Blitz Tag"
                        ]
                    }
                })
            },
            new MothershipTitleDataResponseKey
            {
                Key = "COC",
                Data = "-NO RACISM, SEXISM, HOMOPHOBIA, TRANSPHOBIA, OR OTHER BIGOTRY\n-NO CHEATS OR MODS\n-DO NOT HARASS OTHER PLAYERS OR INTENTIONALLY MAKE THEM UNCOMFORTABLE\n-DO NOT TROLL OR GRIEF LOBBIES BY BEING UNCATCHABLE OR BY ESCAPING THE MAP. TRY TO MAKE SURE EVERYONE IS HAVING FUN\n-IF SOMEONE IS BREAKING THIS CODE, PLEASE REPORT THEM\n-PLEASE BE NICE GORILLAS AND HAVE A GOOD TIME"
            },
            new MothershipTitleDataResponseKey
            {
                Key = "KIDData",
                Data = JsonConvert.SerializeObject(new KIDTitleData{
                    KIDEnabled = "false",
                    KIDPhase = 0,
                    KIDNewPlayerIsoTimestamp = DateTime.MaxValue.ToString("o")
                    // gtag has more values but there not even read by the game?
                })
            },
            new MothershipTitleDataResponseKey
            {
                Key = "BundleData",
                Data = "{}" // never used probably a server-server api for IAP
            },
            new MothershipTitleDataResponseKey
            {
                Key = "AnnouncementData",
                Data = JsonConvert.SerializeObject(new SAnnouncementData
                {
                    ShowAnnouncement = "false",
                    AnnouncementID = "kID_Prelaunch",
                    AnnouncementTitle = "IMPORTANT NEWS",
                    Message = "We're working to make Gorilla Tag a better, more age-appropriate experience in our next update. To learn more, please check out our Discord."
                })
            },
            new MothershipTitleDataResponseKey
            {
                Key = "AllActiveQuests",
                Data = JsonConvert.SerializeObject(AllActiveQuests)
            },
            new MothershipTitleDataResponseKey
            {
                Key = "HowManyMonkeCheck",
                Data = "-1"
            }
        ];

        private static readonly Dictionary<string, DateTime> AttestationNonces = [];

        #endregion

        #region Client APIS

        private static string? NameToMetadataId(string? keyName)
        {
            if (keyName == null) // saves cpu or memory idk
            {
                return null;
            }

            return keyName switch
            {
                "monkeblocks_slot_1" => "27c13292-b290-46f7-88cb-8855f25e1b94",
                "monkeblocks_slot_2" => "9be38264-3fe9-4588-acf5-72405cdb6aa8",
                "monkeblocks_slot_3" => "6a319a64-e0e7-490a-959d-82fe519c8be4",
                "monkeblocks_slot_dev" => "c84d6bd1-d1e7-4f93-ba08-cbb4dfc63009",
                "saved_outfits" => "d2c9c001-037b-4041-92c4-06fb35d3ac51",
                "ScavengerHunt" => "2532c985-4615-4419-8f47-c3d696402a2d",
                _ => null
            };
        }

        [HttpPost("/v2/player/client/auth/begin/QUEST")]
        public async Task<IActionResult> BeginQuest([FromBody] MothershipBeginQuestRequestData? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
                return StatusCode(400);

            if (await OculusServer.GetUserAsync(request.UserId) == null)
                return StatusCode(401);

            var bytes = new byte[100];
            RandomNumberGenerator.Fill(bytes);

            var raw = Convert.ToBase64String(bytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");

            var nonce = raw.Length > 50 ? raw[..50] : raw;

            AttestationNonces.Add(nonce, DateTime.UtcNow.AddMinutes(3));

            return StatusCode(201, new MothershipBeginQuestResponseData
            {
                AttestationNonce = nonce
            });
        }

        [HttpPost("/v2/player/client/auth/complete/QUEST")]
        public async Task<IActionResult> CompleteQuest([FromBody] MothershipCompleteQuestRequestData? request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }

            var metaUser = await OculusServer.GetUserAsync(request.UserId);
            if (metaUser?.Alias == null || metaUser.OrgScopedId == null)
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }

            var nonceVerification = await OculusServer.VerifyNonceAsync(request.UserId, request.MetaNonce);
            if (!nonceVerification)
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }

            /*
            
            OculusServer.SuccessResponse? entitlementCheck = await OculusServer.GetEntitlementAsync(request.UserId);
            if (entitlementCheck == null || entitlementCheck.Success == false)
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }
            else
            {
                
                Console.WriteLine(entitlementCheck.GrantTime);
            }
            
            */

            OculusServer.MetaAttestationClaims? claim;

            try
            {
                claim = await OculusServer.VerifyAttestationTokenAsync(request.AttestationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return MothershipErrorMessages.AttestationFailed;
            }

            if (claim.DeviceState.DeviceIntegrityState == OculusServer.DeviceIntegrityState.NotTrusted)
            {
                Console.WriteLine(claim.DeviceState.DeviceIntegrityState);
                return MothershipErrorMessages.AttestationFailed;
            }

            if (claim.AppState.AppIntegrityState != OculusServer.AppIntegrityState.StoreRecognized)
            {
                Console.WriteLine(claim.AppState.AppIntegrityState);
                return MothershipErrorMessages.AttestationFailed;
            }

            if (claim.AppState.PackageCertSha256Digest.Count != 1 && claim.AppState.PackageCertSha256Digest[0] != "76f64fec638b484dd1820405d931c0a722916b622a9c6912dd78ee1f8c47c755")
            {
                Console.WriteLine(claim.AppState.PackageCertSha256Digest[0]);
                return MothershipErrorMessages.AttestationFailed;
            }

            if (claim.AppState.PackageId != "synex.synex.synex.synex.synex")
            {
                Console.WriteLine(claim.AppState.PackageId);
                return MothershipErrorMessages.AttestationFailed;
            }

            if (AttestationNonces.TryGetValue(claim.RequestDetails.Nonce, out var exp))
            {
                if (exp < DateTime.UtcNow)
                {
                    return MothershipErrorMessages.AttestationFailed;
                }
            }
            else
            {
                Console.WriteLine($"cant find {claim.RequestDetails.Nonce}");
                return MothershipErrorMessages.AttestationFailed;
            }

            var players = await MongoDB.Players.FindAsync<MongoDB.Player>(Builders<MongoDB.Player>.Filter.Eq(p => p.OculusId, request.UserId));
            var player = await players.FirstOrDefaultAsync();

            if (player == null)
            {
                player = new MongoDB.Player()
                {
                    MothershipId = Guid.NewGuid().ToString(),
                    OrgScopedId = metaUser.OrgScopedId,
                    OculusId = request.UserId,
                    OculusAlias = metaUser.Alias,
                    MothershipUserData = [],
                    RoomInfo = new TempFriendPresenceResponseData(),
                    Friends = [],
                    PrivacyState = PrivacyState.VISIBLE,
                    QuestStatus = new TempUserQuestStatusResponseData()
                    {
                        DailyPoints = [],
                        WeeklyPoints = [],
                        UserPointsTotal = 0
                    }
                };
                await MongoDB.Players.InsertOneAsync(player);
            }

            Dictionary<string, object?> mothershipToken = new()
            {
                ["sub"] = player.MothershipId,
                ["did"] = Request.Headers["x-mothership-deployment-id"].ToString(),
                ["env"] = Request.Headers["x-mothership-env-id"].ToString(),
                ["externalService"] = "QUEST",
                ["externalServiceId"] = request.UserId,
                ["tid"] = Request.Headers["x-mothership-title-id"].ToString(),
                ["tags"] = null,
                ["orgScopedExternalServiceId"] = metaUser.OrgScopedId,
                ["nbf"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ["exp"] = DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds(),
                ["iat"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            string token = JsonWebToken.Generate(mothershipToken, TimeSpan.FromHours(6));

            return Ok(new MothershipCompleteResponseData
            {
                ExternalProviderId = request.UserId,
                ExternalProviderUsername = metaUser.Alias,
                IsPrimaryId = true,
                PlayerId = player.MothershipId,
                Tags = null,
                Token = token,
                ExpirationTime = DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeMilliseconds()
            });
        }

        [HttpGet("/v1/title-data/client")]
        public IActionResult TitleData(/*[FromQuery] string? keys = null*/)
        {
            if (!JsonWebToken.Verify(Request.Headers["x-mothership-token"]))
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }

            return Ok(new MothershipTitleDataResponseData
            {
                Results = MothershipTitleData
            });

            /*if (keys == null)
            {
            }

            string[] keysToGet = keys.Split(',');
            if (keysToGet.Length == 0)
            {
                return Ok(new MothershipTitleDataResponseData
                {
                    Results = []
                });
            }

            List<MothershipTitleDataResponseKey> tdKeys = [];
            foreach (MothershipTitleDataResponseKey key in MothershipTitleData)
            {
                if (keysToGet.Contains(key.Key))
                {
                    tdKeys.Add(key);
                }
            }

            return Ok(new MothershipTitleDataResponseData
            {
                Results = tdKeys
            });*/
        }

        [HttpGet("/v1/purchase/client/refresh-iap")]
        public IActionResult PurchaseRefreshIap()
        {
            if (!JsonWebToken.Verify(Request.Headers["x-mothership-token"]))
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }

            return Ok(new MothershipPurchaseRefreshIAPResponseData
            {
                NewInventoryChangesAvailable = false
            });
        }

        [HttpGet("/v1/userdata/client")]
        public async Task<IActionResult> GetUserdata([FromQuery] string? key_name)
        {
            if (!JsonWebToken.Verify(Request.Headers["x-mothership-token"], out var mothershipId))
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }

            var userDoc = await MongoDB.Players.Find(x => x.MothershipId == mothershipId).FirstOrDefaultAsync();

            if (userDoc == null)
                return NotFound();

            var slot = userDoc.MothershipUserData.FirstOrDefault(s => s.KeyName == key_name);
            
            if (slot == null)
            {
                return MothershipErrorMessages.SomethingWentWrong;
            }

            return Ok(slot);
        }

        [HttpDelete("/v1/userdata/client")]
        public async Task<IActionResult> DeleteUserdata([FromQuery] string? key_name)
        {
            if (!JsonWebToken.Verify(Request.Headers["x-mothership-token"], out var mothershipId))
            {
                return MothershipErrorMessages.ClientAuthenticationFailed;
            }

            var user = await MongoDB.Players
                .Find(x => x.MothershipId == mothershipId)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            var slot = user.MothershipUserData
                .FirstOrDefault(s => s.KeyName == key_name);

            if (slot == null)
                return NotFound();

            await MongoDB.Players.UpdateOneAsync(
                x => x.MothershipId == mothershipId,
                Builders<MongoDB.Player>.Update.PullFilter(x => x.MothershipUserData, s => s.KeyName == key_name)
            );

            return Ok(slot);
        }

        [HttpPost("/v1/userdata/client")]
        public async Task<IActionResult> SetUserdata([FromBody] MothershipUserDataSetRequestData rjson)
        {
            if (!JsonWebToken.Verify(Request.Headers["x-mothership-token"], out var mothershipId))
                return MothershipErrorMessages.ClientAuthenticationFailed;

            if (string.IsNullOrWhiteSpace(rjson.KeyName))
                return BadRequest();

            var player = await MongoDB.Players.Find(p => p.MothershipId == mothershipId) .FirstOrDefaultAsync();

            if (player == null || mothershipId == null)
                return MothershipErrorMessages.ClientAuthenticationFailed;

            var metadataId = NameToMetadataId(rjson.KeyName);
            if (metadataId == null)
                return MothershipErrorMessages.ClientAuthenticationFailed;

            var encodedValue = Base64UrlEncoder.Encode(rjson.Value);
            var existingSlot = player.MothershipUserData.FirstOrDefault(x => x.KeyName == rjson.KeyName);

            if (existingSlot == null)
            {
                var newSlot = new MothershipUserDataGetResponseData
                {
                    MetadataId = metadataId,
                    KeyName = rjson.KeyName,
                    Value = encodedValue,
                    Generation = rjson.Generation,
                    CreatedBy = mothershipId,
                    LastWrittenBy = mothershipId,
                    UserId = mothershipId,
                    Id = Guid.NewGuid().ToString(),
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow
                };

                await MongoDB.Players.UpdateOneAsync(
                    p => p.MothershipId == mothershipId,
                    Builders<MongoDB.Player>.Update.Push(p => p.MothershipUserData, newSlot)
                );

                existingSlot = newSlot;
            }
            else
            {
                await MongoDB.Players.UpdateOneAsync(
                    p => p.MothershipId == mothershipId &&
                         p.MothershipUserData.Any(s => s.KeyName == rjson.KeyName),
                    Builders<MongoDB.Player>.Update
                        .Set("MothershipUserData.$.Value", encodedValue)
                        .Set("MothershipUserData.$.Generation", rjson.Generation)
                        .Set("MothershipUserData.$.LastWrittenBy", mothershipId)
                        .Set("MothershipUserData.$.LastUpdatedTime", DateTime.UtcNow)
                );
            }

            return Ok(new MothershipUserDataSetResponseData
            {
                UserId = mothershipId,
                KeyName = rjson.KeyName,
                Generation = rjson.Generation,
                Id = existingSlot.Id
            });
        }

        #endregion

        #region Server APIS

        [HttpGet("/v1/data/client")]
        public IActionResult GetData([FromQuery] string v)
        {
            if (v != "")
            {
                return BadRequest(new MothershipClientDataFailureResponseData
                {
                    Message = "Please update to the latest version of Blitz Tag!",
                    Error = MothershipClientDataError.NewUpdate
                });
            }

            return Ok(new MothershipClientDataResponseData
            {
                Admins = [
                    new MothershipClientDataUser
                    {
                        UserId = "6494FFA906D59FB1",
                        Username = "BXT",
                        Role = MothershipClientDataRole.Owner
                    },
                    new MothershipClientDataUser
                    {
                        UserId = "1FC3C8925ED44C4C",
                        Username = "NOTFISHVR",
                        Role = MothershipClientDataRole.Admin
                    }
                ]
            });
        }

        #endregion
        private static class MothershipErrorMessages
        {
            private static ObjectResult CreateErrorResult(MothershipErrorCode code, string message, int statusCode, bool useTraceId)
            {
                var errorMessage = new MothershipErrorMessageResponseData
                {
                    MothershipErrorCode = (int?)code,
                    ClientMessage = message,
                    TraceId = useTraceId ? Guid.NewGuid().ToString("N") : null
                };

                var errorResponse = new MothershipErrorResponseData
                {
                    Message = JsonConvert.SerializeObject(errorMessage),
                    StatusCode = statusCode
                };

                return new ObjectResult(errorResponse) { StatusCode = statusCode };
            }

            public static readonly ObjectResult ClientAuthenticationFailed = CreateErrorResult(
                MothershipErrorCode.None, 
                "Client Authentication Failed", 
                401, 
                true
            );

            public static readonly ObjectResult SomethingWentWrong = CreateErrorResult(
                MothershipErrorCode.None,
                "Something went wrong", 
                400, 
                true
            );

            public static readonly ObjectResult AttestationFailed = CreateErrorResult(
                MothershipErrorCode.None, 
                "Attestation failed. Make sure you've installed this title through a legitimate store and are using a device with the latest updates.", 
                401, 
                false
            );
        }
    }
}
