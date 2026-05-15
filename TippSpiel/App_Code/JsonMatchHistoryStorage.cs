using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Newtonsoft.Json;
using Tippspiel.Contracts;
using Tippspiel.Contracts.Models;

namespace BhFS.Tippspiel.Utils
{
    public class JsonMatchHistoryStorage : IMatchHistory
    {
        private readonly string _filePath;
        private readonly Lazy<IReadOnlyList<MatchDataModel>> _matches;

        public JsonMatchHistoryStorage()
            : this(ResolveDefaultFilePath())
        {
        }

        public JsonMatchHistoryStorage(string filePath)
        {
            _filePath = filePath;
            _matches = new Lazy<IReadOnlyList<MatchDataModel>>(LoadMatches, true);
        }

        public List<GroupInfoModel> GetAllGroups()
        {
            return _matches.Value
                .Where(m => m.Group != null)
                .GroupBy(m => m.Group.Id)
                .Select(g => new GroupInfoModel
                {
                    Id = g.Key,
                    Text = g.First().Group.Text
                })
                .OrderBy(g => g.Id)
                .ToList();
        }

        public MatchDataModel GetMatchData(int matchId)
        {
            return _matches.Value.SingleOrDefault(m => m.MatchId == matchId);
        }

        public List<MatchDataModel> GetMatchesByGroup(int groupId)
        {
            return _matches.Value
                .Where(m => m.Group != null && m.Group.Id == groupId)
                .ToList();
        }

        public List<MatchDataModel> GetAllMatches()
        {
            return _matches.Value.ToList();
        }

        private static string ResolveDefaultFilePath()
        {
            var hostingPath = HostingEnvironment.MapPath("~/Content/MatchData/MatchHistory.json");
            if (!string.IsNullOrWhiteSpace(hostingPath) && File.Exists(hostingPath))
            {
                return hostingPath;
            }

            var currentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (currentDirectory != null)
            {
                var directPath = Path.Combine(currentDirectory.FullName, "Content", "MatchData", "MatchHistory.json");
                if (File.Exists(directPath))
                {
                    return directPath;
                }

                var solutionPath = Path.Combine(currentDirectory.FullName, "TippSpiel", "Content", "MatchData", "MatchHistory.json");
                if (File.Exists(solutionPath))
                {
                    return solutionPath;
                }

                currentDirectory = currentDirectory.Parent;
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "MatchData", "MatchHistory.json");
        }

        private IReadOnlyList<MatchDataModel> LoadMatches()
        {
            if (string.IsNullOrWhiteSpace(_filePath) || !File.Exists(_filePath))
            {
                return new List<MatchDataModel>();
            }

            var content = File.ReadAllText(_filePath);
            var matches = JsonConvert.DeserializeObject<List<MatchDataModel>>(content);

            return matches ?? new List<MatchDataModel>();
        }
    }
}