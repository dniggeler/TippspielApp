using FluentAssertions;
using BhFS.Tippspiel.Utils;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Tippspiel.Tests
{
    [Trait("Tippspiel","History Storage")]
    public class JsonMatchHistoryStorageTests : IDisposable
    {
        private readonly string _filePath;
        private readonly JsonMatchHistoryStorage _matchHistoryStorage;

        public JsonMatchHistoryStorageTests()
        {
            _filePath = Path.GetTempFileName();
            File.WriteAllText(_filePath, @"[
  {
    ""matchID"": 1001,
    ""leagueShortcut"": ""bl1"",
    ""matchDateTime"": ""2025-08-22T20:30:00"",
    ""matchDateTimeUTC"": ""2025-08-22T18:30:00Z"",
    ""lastUpdateDateTime"": ""2025-08-23T01:00:54.147"",
    ""matchIsFinished"": true,
    ""group"": {
      ""groupOrderID"": 1,
      ""groupName"": ""1. Spieltag""
    },
    ""team1"": {
      ""teamId"": 40,
      ""teamName"": ""FC Bayern München"",
      ""shortName"": ""Bayern"",
      ""teamIconUrl"": ""https://example.com/bayern.png""
    },
    ""team2"": {
      ""teamId"": 1635,
      ""teamName"": ""RB Leipzig"",
      ""shortName"": ""Leipzig"",
      ""teamIconUrl"": ""https://example.com/leipzig.png""
    },
    ""matchResults"": []
  },
  {
    ""matchID"": 1002,
    ""leagueShortcut"": ""bl1"",
    ""matchDateTime"": ""2025-08-23T15:30:00"",
    ""matchDateTimeUTC"": ""2025-08-23T13:30:00Z"",
    ""lastUpdateDateTime"": ""2025-08-23T17:26:05.92"",
    ""matchIsFinished"": true,
    ""group"": {
      ""groupOrderID"": 1,
      ""groupName"": ""1. Spieltag""
    },
    ""team1"": {
      ""teamId"": 6,
      ""teamName"": ""Bayer 04 Leverkusen"",
      ""shortName"": ""Leverkusen"",
      ""teamIconUrl"": ""https://example.com/leverkusen.png""
    },
    ""team2"": {
      ""teamId"": 175,
      ""teamName"": ""TSG Hoffenheim"",
      ""shortName"": ""Hoffenheim"",
      ""teamIconUrl"": ""https://example.com/hoffenheim.png""
    },
    ""matchResults"": []
  },
  {
    ""matchID"": 1003,
    ""leagueShortcut"": ""bl1"",
    ""matchDateTime"": ""2025-08-30T15:30:00"",
    ""matchDateTimeUTC"": ""2025-08-30T13:30:00Z"",
    ""lastUpdateDateTime"": ""2025-08-30T17:30:00"",
    ""matchIsFinished"": false,
    ""group"": {
      ""groupOrderID"": 2,
      ""groupName"": ""2. Spieltag""
    },
    ""team1"": {
      ""teamId"": 16,
      ""teamName"": ""VfB Stuttgart"",
      ""shortName"": ""Stuttgart"",
      ""teamIconUrl"": ""https://example.com/stuttgart.png""
    },
    ""team2"": {
      ""teamId"": 95,
      ""teamName"": ""FC Augsburg"",
      ""shortName"": ""Augsburg"",
      ""teamIconUrl"": ""https://example.com/augsburg.png""
    },
    ""matchResults"": []
  }
]");
            _matchHistoryStorage = new JsonMatchHistoryStorage(_filePath);
        }

        [Fact(DisplayName = "Get Match By Match Id")]
        public void GetMatchDataTest()
        {
            var match = _matchHistoryStorage.GetMatchData(1002);

            match.Should().NotBeNull();
            match.MatchId.Should().Be(1002);
            match.Group.Id.Should().Be(1);
            match.HomeTeam.ShortName.Should().Be("Leverkusen");
        }

        [Fact(DisplayName = "Get Matches By Group Id")]
        public void GetMatchesByGroupTest()
        {
            var matches = _matchHistoryStorage.GetMatchesByGroup(1);

            matches.Should().HaveCount(2);
            matches.Select(m => m.MatchId).Should().BeEquivalentTo(new[] { 1001, 1002 });
        }

        [Fact(DisplayName = "Get All Groups")]
        public void GetAllGroupsTest()
        {
            var groups = _matchHistoryStorage.GetAllGroups();

            groups.Should().HaveCount(2);
            groups.Select(g => g.Id).Should().Equal(1, 2);
        }

        public void Dispose()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
