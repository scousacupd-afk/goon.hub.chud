using Blitz_Tag.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Blitz_Tag
{
    public class MongoDB
    {
        private static readonly MongoClient MongoClient = new("mongodb+srv://bxt:lFONqPXFTH8FH5OW@cluster0.c0dtzon.mongodb.net/?retryWrites=true&w=majority");
        private static readonly IMongoDatabase BlitzTag = MongoClient.GetDatabase("BlitzTag");
        public static readonly IMongoCollection<Player> Players = BlitzTag.GetCollection<Player>("Players");
        public static readonly IMongoCollection<Room> Rooms = BlitzTag.GetCollection<Room>("Rooms");
        public static readonly IMongoCollection<Poll> Polls = BlitzTag.GetCollection<Poll>("Polls");
        public static readonly IMongoCollection<Code> Codes = BlitzTag.GetCollection<Code>("Codes");
        public static readonly IMongoCollection<MonkeBlocksMap> SharedBlocks = BlitzTag.GetCollection<MonkeBlocksMap>("SharedBlocks");

        public class Room
        {
            [BsonId]
            public ObjectId _id { get; set; }

            [BsonElement("gameId")]
            public string GameId { get; set; } = "";

            [BsonElement("region")]
            public string Region { get; set; } = "";

            [BsonElement("players")]
            public Dictionary<string, int> Players { get; set; } = [];

            [BsonElement("map")]
            public string Map { get; set; } = "";

            [BsonElement("queue")]
            public string Queue { get; set; } = "";
        }

        public class Player
        {
            [BsonId]
            public ObjectId _id { get; set; }
            
            [BsonElement("mothershipId")]
            public string MothershipId { get; set; } = "";

            [BsonElement("oculusId")]
            public string OculusId { get; set; } = "";

            [BsonElement("orgScopedId")]
            public string OrgScopedId { get; set; } = "";

            [BsonElement("oculusAlias")]
            public string OculusAlias { get; set; } = "";
            
            [BsonElement("mothershipUserData")]
            public List<MothershipUserDataGetResponseData> MothershipUserData { get; set; } = [];
            
            [BsonElement("roomInfo")]
            public TempFriendPresenceResponseData RoomInfo { get; set; } = new();
            
            [BsonElement("privacyState")]
            public PrivacyState PrivacyState { get; set; } = PrivacyState.VISIBLE;
            
            [BsonElement("friends")]
            public List<string> Friends { get; set; } = [];
            
            [BsonElement("questStatus")]
            public TempUserQuestStatusResponseData QuestStatus { get; set; } = new TempUserQuestStatusResponseData
            {
                DailyPoints = [],
                WeeklyPoints = [],
                UserPointsTotal = 0
            };

            [BsonElement("votes")]
            public List<Vote> Votes { get; set; } = [];

            [BsonElement("isFanClub")]
            public bool? IsFanClub { get; set; } = null;

            [BsonElement("progression")]
            public int? Progression { get; set; } = null;
            
            [JsonIgnore]
            public string PlayFabId => RoomInfo.FriendLinkId;
        }

        public class Vote
        {
            [BsonElement("pollId")]
            public int PollId { get; set; }

            [BsonElement("optionIndex")]
            public int OptionIndex { get; set; }

            [BsonElement("isPrediction")]
            public bool IsPrediction { get; set; }
        }

        public class Poll
        {
            [BsonId]
            public ObjectId _id { get; set; }

            [BsonElement("pollId")]
            public int PollId { get; set; } = 0;

            [BsonElement("question")]
            public string Question { get; set; } = "";

            [BsonElement("voteOptions")]
            public List<string> VoteOptions { get; set; } = [];

            [BsonElement("voteCount")]
            public List<int> VoteCount { get; set; } = [];

            [BsonElement("predictionCount")]
            public List<int> PredictionCount { get; set; } = [];

            [BsonElement("startTime")]
            public DateTime StartTime { get; set; } = DateTime.MinValue;

            [BsonElement("endTime")]
            public DateTime EndTime { get; set; } = DateTime.MinValue;
        }

        public class Code
        {
            [BsonId]
            public ObjectId _id { get; set; }

            [BsonElement("itemGuid")]
            public string ItemGuid { get; set; } = "";

            [BsonElement("itemID")]
            public string ItemID { get; set; } = "";

            [BsonElement("alreadyRedeemed")]
            public bool AlreadyRedeemed { get; set; } = false;

            [BsonElement("redeemedByUser")]
            public string RedeemedByUser { get; set; } = "";
        }

        public class MonkeBlocksMap
        {
            [BsonId]
            public ObjectId _id { get; set; }

            [BsonElement("mapCode")]
            public string MapCode { get; set; } = "";

            [BsonElement("mothershipId")]
            public string Mothershipid { get; set; } = "";

            [BsonElement("metadataKey")]
            public string MetadataKey { get; set; } = "";

            [BsonElement("playerNickname")]
            public string PlayerNickname { get; set; } = "";
        }
    }
}