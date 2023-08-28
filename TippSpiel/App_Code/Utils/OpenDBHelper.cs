using System;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using FussballTippApp.Models;
using Tippspiel.Contracts;
using Tippspiel.Contracts.Models;
using Tippspiel.Implementation;

namespace BhFS.Tippspiel.Utils
{
    public class OpenDBHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public struct SpieltagInfo
        {
            public int CurrentSpieltag { get; set; }
            public int TippSpieltag { get; set; }
            public bool IsCompleted { get; set; }
        }

        public struct CompleteInfo
        {
            public bool IsCompleted { get; set; }
            // recently means round not completed yesterday
            public bool IsCompletedRecently { get; set; }
            public DateTime? CompletedSince { get; set; }
        }

        public static async Task<CompleteInfo> IsSpieltagComplete(
            IFussballDataRepository repository, bool disableCache = false)
        {
            var dataNext = await repository.GetNextMatchAsync();
            var dataLast = await repository.GetLastMatchAsync();

            Log.Debug($"Last:{dataLast?.Group.Id}, Next: {dataNext?.Group.Id}");

            if (dataNext == null)
            {

                return new CompleteInfo
                {
                    IsCompleted = true,
                    CompletedSince = dataLast.KickoffTime.AddHours(3)
                };
            }
            if (dataLast == null)
            {
                return new CompleteInfo
                {
                    IsCompleted = false,
                    CompletedSince = null,
                };
            }
            var result = new CompleteInfo();

            if (dataLast.Group.Id < dataNext.Group.Id)
            {
                result.IsCompleted = true;
                // check if emails already sent the day before
                {
                    var lastMatchDate = dataLast.KickoffTime;
                    var yesterday = DateTime.Now.AddDays(-1);
                    if (lastMatchDate > yesterday)
                    {
                        result.IsCompletedRecently = true;
                    }
                }
                result.CompletedSince = dataLast.KickoffTime.AddHours(3);
            }

            return result;
        }

        public static async Task<SpieltagInfo> GetSpieltagInfo(
            IFussballDataRepository repository, bool disableCache = false)
        {
            var spieltagInfo = new SpieltagInfo();
            spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = 1;

            var dataNext = await repository.GetNextMatchAsync();
            var dataLast = await repository.GetLastMatchAsync();

            Log.Debug($"Last is null: {dataLast==null}, Next is null: {dataNext==null}, Last:{dataLast?.Group.Id}, Next: {dataNext?.Group.Id}");

            if (dataNext == null && dataLast == null)
            {
                return spieltagInfo;
            }
            if (dataNext == null || (dataNext.MatchId==-1)) //e.g. last season
            {
                spieltagInfo.CurrentSpieltag = dataLast.Group.Id;

                return spieltagInfo;
            }
            if (dataLast == null || (dataLast.Group.Id > dataNext.Group.Id)) //e.g. last season
            {
                spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = dataNext.Group.Id;

                return spieltagInfo;
            }
            if ((dataNext.Group.Id > dataLast.Group.Id) && (dataNext.Group.Id > SportsdataConfigInfo.Current.EndSpieltag))
            {
                spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = SportsdataConfigInfo.Current.EndSpieltag;
                spieltagInfo.IsCompleted = true;

                return spieltagInfo;
            }

            if (dataNext.Group.Id > dataLast.Group.Id)
            {
                spieltagInfo.CurrentSpieltag = dataLast.Group.Id;
                spieltagInfo.TippSpieltag = dataNext.Group.Id;
            }
            else {
                spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = dataLast.Group.Id;
                spieltagInfo.IsCompleted = true;
            }

            return  spieltagInfo;
        }

        public static MatchInfoModel Create(MatchDataModel match)
        {
            var matchModelObj = new MatchInfoModel
            {
                MatchId = match.MatchId,
                KickoffTime = match.KickoffTime,
                KickoffTimeUtc = match.KickoffTimeUTC,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                HomeTeamIcon = match.HomeTeam.IconUrl,
                AwayTeamIcon = match.AwayTeam.IconUrl,
                HomeTeam = match.HomeTeam.ShortName,
                AwayTeam = match.AwayTeam.ShortName,
                IsFinished = match.IsFinished
            };

            if (match.MatchResults != null && match.MatchResults.Any())
            {
                var result = (from r in match.MatchResults orderby r.ResultOrderId descending select r).FirstOrDefault();

                if (result == null) return matchModelObj;

                matchModelObj.HomeTeamScore = result.HomeTeamScore;
                matchModelObj.AwayTeamScore = result.AwayTeamScore;

                if (match.MatchId == 64153)
                {
                    matchModelObj.HomeTeamScore = 0;
                    matchModelObj.AwayTeamScore = 3;
                }
            }
            else if (match.MatchId == 64154)
            {
                matchModelObj.HomeTeamScore = 2;
                matchModelObj.AwayTeamScore = 2;
            }

            return matchModelObj;
        }
    }
}