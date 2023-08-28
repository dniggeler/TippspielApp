using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using log4net;
using Tippspiel.Contracts;
using Tippspiel.Contracts.Models;
using Tippspiel.Implementation;

namespace TippSpiel.Repository
{
    public class BuLiDataRepository : IFussballDataRepository, IAccessStats
    {
        private readonly HttpClient httpClient;
        private readonly string CACHE_MATCH_PREFIX = "cacheGame";

        private static int _remoteHits;
        private static int _cacheHits;
        private const int CacheDuration = 60;
        private readonly ICacheProvider _cache;
        private readonly ILog _log;

        private readonly int _leagueId;
        private readonly string _leagueTag;
        private readonly string _saisonTag;

        public BuLiDataRepository(SportsdataConfigInfo info, ICacheProvider cacheProvider, ILog log)
            :  this(info.LeagueId, info.LeagueShortcut, info.LeagueSaison)
        {
            _cache = cacheProvider;
            _log = log;
        }

        public BuLiDataRepository(int leagueId, string leagueShortcut, string leagueSeason)
        {
            _leagueId = leagueId;
            _leagueTag = leagueShortcut;
            _saisonTag = leagueSeason;

            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www.openligadb.de/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public int GetRemoteHits()
        {
            return _remoteHits;
        }

        public int GetCacheHits()
        {
            return _cacheHits;
        }

        public async Task<List<TeamModel>> GetTeamsAsync(bool disableCache)
        {
            const string cacheTag = "cacheTeams";

            List<TeamModel> g;
            if (_cache.IsSet(cacheTag))
            {
                g = (List<TeamModel>)_cache.Get(cacheTag);
                _cacheHits++;
            }
            else
            {
                g = await GetFromApiAsync<List<TeamModel>>($"getavailableteams/{_leagueTag}/{_saisonTag}");
                _cache.Set(cacheTag, g, CacheDuration);

                _remoteHits++;
            }

            return g;
        }

        public async Task<GroupInfoModel> GetCurrentGroupAsync(bool disableCache)
        {
            const string cacheTag = "cacheCurrGrp";

            GroupInfoModel g;
            if (_cache.IsSet(cacheTag))
            {
                g = (GroupInfoModel)_cache.Get(cacheTag);
                _cacheHits++;
            }
            else
            {
                g = await GetFromApiAsync<GroupInfoModel>($"getcurrentgroup/{_leagueTag}");
                _cache.Set(cacheTag, g, CacheDuration);

                _remoteHits++;
            }

            return g;
        }

        public async Task<List<GroupInfoModel>> GetAllGroupsAsync(bool disableCache)
        {
            const string cacheTag = "cacheAllGrps";

            List<GroupInfoModel> groups;
            if (_cache.IsSet(cacheTag))
            {
                groups = (List<GroupInfoModel>)_cache.Get(cacheTag);
                _cacheHits++;
            }
            else
            {
                groups = await GetFromApiAsync<List<GroupInfoModel>>($"getavailablegroups/{_leagueTag}/{_saisonTag}");
                _cache.Set(cacheTag, groups, CacheDuration);
                _remoteHits++;
            }

            return groups;
        }

        public async Task<List<MatchDataModel>> GetAllMatchesAsync(bool disableCache)
        {
            string cacheAllMatchTag = "cacheAllMatches" + _leagueTag + _saisonTag;
            string cacheMatchTag = CACHE_MATCH_PREFIX + _leagueTag;

            List<MatchDataModel> matches;
            if (_cache.IsSet(cacheAllMatchTag))
            {
                matches = (List<MatchDataModel>)_cache.Get(cacheAllMatchTag);

                _cacheHits++;
            }
            else
            {
                matches = await GetFromApiAsync<List<MatchDataModel>>($"getmatchdata/{_leagueTag}/{_saisonTag}");
                _log.Debug($"Remote hit GetAllMatches(), Count={matches?.Count}");

                _cache.Set(cacheAllMatchTag, matches, CacheDuration);

                // cache single matches
                foreach (var m in matches ?? new List<MatchDataModel>())
                {
                    _cache.Set(cacheMatchTag + m.MatchId, m, CacheDuration);
                }

                _remoteHits++;
            }

            return matches;            
        }

        public async Task<MatchDataModel> GetNextMatchAsync(bool disableCache)
        {
            string cacheNxtMatchTag = "cacheNxtGame"+_leagueTag;
            string cacheMatchTag = CACHE_MATCH_PREFIX + _leagueTag;

            if (_cache.IsSet(cacheNxtMatchTag))
            {
                var m = (MatchDataModel)_cache.Get(cacheNxtMatchTag);
                _cacheHits++;

                return m;
            }
            var teams = await GetTeamsAsync(disableCache);
            MatchDataModel nextMatch = null;
            foreach (TeamModel team in teams)
            {
                var m = await GetFromApiAsync<MatchDataModel>($"getnextmatchbyleagueteam/{_leagueId}/{team.Id}");

                if (nextMatch == null || m.KickoffTimeUTC < nextMatch.KickoffTimeUTC)
                {
                    nextMatch = m;
                }

                _log.Debug($"Remote hit GetNextMatch(), {m?.Group.Id}");

                _cache.Set(cacheNxtMatchTag, m, CacheDuration);
                _cache.Set(cacheMatchTag + m?.MatchId, m, CacheDuration);
                _remoteHits++;
            }

            return nextMatch;
        }

        public async Task<MatchDataModel> GetLastMatchAsync(bool disableCache)
        {
            string cacheLastMatchTag = "cacheLastGame" + _leagueTag;
            string cacheMatchTag = CACHE_MATCH_PREFIX + _leagueTag;

            if (_cache.IsSet(cacheLastMatchTag))
            {
                MatchDataModel m = (MatchDataModel)_cache.Get(cacheLastMatchTag);
                _cacheHits++;

                return m;
            }

            var teams = await GetTeamsAsync(disableCache);
            MatchDataModel mostRecentLastMatch = null;
            foreach (TeamModel team in teams)
            {
                var m = await GetFromApiAsync<MatchDataModel>($"getlastmatchbyleagueteam/{_leagueId}/{team.Id}");

                if (mostRecentLastMatch == null || m.KickoffTimeUTC > mostRecentLastMatch.KickoffTimeUTC)
                {
                    mostRecentLastMatch = m;
                }

                _log.Debug($"Remote hit GetLastMatch(), {m?.Group.Id}");

                _cache.Set(cacheLastMatchTag, m, CacheDuration);
                _cache.Set(cacheMatchTag + m?.MatchId, m, CacheDuration);
                _remoteHits++;
            }

            return mostRecentLastMatch;
        }

        public async Task<MatchDataModel> GetMatchDataAsync(int matchId, bool disableCache)
        {
            string cacheMatchTag = CACHE_MATCH_PREFIX + _leagueTag+matchId;

            MatchDataModel m;
            if (_cache.IsSet(cacheMatchTag))
            {
                m = (MatchDataModel)_cache.Get(cacheMatchTag);
                _cacheHits++;
            }
            else
            {
                m = await GetFromApiAsync<MatchDataModel>($"getmatchdata/{matchId}");
                _cache.Set(cacheMatchTag, m, CacheDuration);
                _remoteHits++;
            }

            return m;
        }

        public async Task<List<MatchDataModel>> GetMatchesByGroupAsync(int groupId, bool disableCache)
        {
            string cacheMatchGroupTag = "cacheGameByGrp" + groupId + _leagueTag + _saisonTag;
            string cacheMatchTag = CACHE_MATCH_PREFIX + _leagueTag;

            List<MatchDataModel> matches;
            if (_cache.IsSet(cacheMatchGroupTag))
            {
                matches = (List<MatchDataModel>)_cache.Get(cacheMatchGroupTag);

                _cacheHits++;
            }
            else
            {
                matches = await GetFromApiAsync<List<MatchDataModel>>($"getmatchdata/{_leagueTag}/{_saisonTag}/{groupId}");

                // check if data has been found at all
                if(matches.Count == 1)
                {
                    if (matches.First().MatchId == -1)
                    {
                        return new List<MatchDataModel>();
                    }
                }

                _cache.Set(cacheMatchGroupTag, matches, CacheDuration);

                // cache single matches
                foreach(var m in matches)
                {
                    _cache.Set(cacheMatchTag + m.MatchId, m, CacheDuration);
                }

                _remoteHits++;
            }

            return matches;
        }

        public async Task<List<MatchDataModel>> GetMatchesByCurrentGroupAsync(bool disableCache)
        {
            GroupInfoModel group = await GetCurrentGroupAsync(disableCache);

            return await GetMatchesByGroupAsync(group.Id, disableCache);
        }

        public async Task<bool> IsSpieltagCompleteAsync(bool disableCache)
        {
            var mNext = await GetNextMatchAsync(disableCache);
            var dataLast = await GetLastMatchAsync(disableCache);

            if (mNext == null)
            {
                return true;
            }

            if (dataLast == null)
            {
                return false;
            }

            if (dataLast.Group.Id < mNext.Group.Id)
            {
                return true;
            }

            return false;
        }

        private async Task<T> GetFromApiAsync<T>(string path) where T : class
        {
            T item = null;
            HttpResponseMessage response = await httpClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                item = await response.Content.ReadAsAsync<T>();
            }
            return item;
        }
    }
}