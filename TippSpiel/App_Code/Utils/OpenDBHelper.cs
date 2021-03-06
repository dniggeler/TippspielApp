﻿using System;
using System.Linq;
using log4net;
using FussballTippApp.Models;
using Tippspiel.Contracts;
using Tippspiel.Implementation;

namespace BhFS.Tippspiel.Utils
{
    public class OpenDBHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public static CompleteInfo IsSpieltagComplete(TippSpiel.SvcOpenData.SportsdataSoapClient client)
        {
            var dataNext = client.GetNextMatch(SportsdataConfigInfo.Current.LeagueShortcut);
            var dataLast = client.GetLastMatch(SportsdataConfigInfo.Current.LeagueShortcut);

            Log.Debug($"Last:{dataLast?.groupOrderID}, Next: {dataNext?.groupOrderID}");

            if (dataNext == null)
            {

                return new CompleteInfo()
                {
                    IsCompleted = true,
                    CompletedSince = dataLast.matchDateTime.AddHours(3)
                };
            }
            if (dataLast == null)
            {
                return new CompleteInfo()
                {
                    IsCompleted = false,
                    CompletedSince = null,
                };
            }
            var result = new CompleteInfo();

            if (dataLast.groupOrderID < dataNext.groupOrderID)
            {
                result.IsCompleted = true;
                // check if emails already sent the day before
                {
                    var lastMatchDate = dataLast.matchDateTime;
                    var yesterday = DateTime.Now.AddDays(-1);
                    if (lastMatchDate > yesterday)
                    {
                        result.IsCompletedRecently = true;
                    }
                }
                result.CompletedSince = dataLast.matchDateTime.AddHours(3);
            }

            return result;
        }

        public static SpieltagInfo GetSpieltagInfo(IFussballDataRepository repository)
        {
            var spieltagInfo = new SpieltagInfo();
            spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = 1;

            var dataNext = repository.GetNextMatch();
            var dataLast = repository.GetLastMatch();

            Log.Debug($"Last is null: {dataLast==null}, Next is null: {dataNext==null}, Last:{dataLast?.GroupOrderId}, Next: {dataNext?.GroupOrderId}");

            if (dataNext == null && dataLast == null)
            {
                return spieltagInfo;
            }
            if (dataNext == null || (dataNext.MatchId==-1)) //e.g. last season
            {
                spieltagInfo.CurrentSpieltag = dataLast.GroupId;

                return spieltagInfo;
            }
            if (dataLast == null || (dataLast.GroupId > dataNext.GroupId)) //e.g. last season
            {
                spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = dataNext.GroupId;

                return spieltagInfo;
            }
            if ((dataNext.GroupId > dataLast.GroupId) && (dataNext.GroupId > SportsdataConfigInfo.Current.EndSpieltag))
            {
                spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = SportsdataConfigInfo.Current.EndSpieltag;
                spieltagInfo.IsCompleted = true;

                return spieltagInfo;
            }

            if (dataNext.GroupId > dataLast.GroupId)
            {
                spieltagInfo.CurrentSpieltag = dataLast.GroupId;
                spieltagInfo.TippSpieltag = dataNext.GroupId;
            }
            else {
                spieltagInfo.CurrentSpieltag = spieltagInfo.TippSpieltag = dataLast.GroupId;
                spieltagInfo.IsCompleted = true;
            }

            return  spieltagInfo;
        }

        public static MatchInfoModel Create(TippSpiel.SvcOpenData.Matchdata match)
        {
            var matchModelObj = new MatchInfoModel
            {
                MatchId = match.matchID,
                KickoffTime = match.matchDateTime,
                KickoffTimeUtc = match.matchDateTimeUTC,
                HomeTeamScore = match.pointsTeam1,
                AwayTeamScore = match.pointsTeam2,
                HomeTeamIcon = match.iconUrlTeam1,
                AwayTeamIcon = match.iconUrlTeam2,
                HomeTeam = match.nameTeam1,
                AwayTeam = match.nameTeam2,
                IsFinished = match.matchIsFinished
            };

            if (match.matchResults != null && match.matchResults.Any())
            {
                var result = (from r in match.matchResults orderby r.resultTypeId descending select r).FirstOrDefault();

                if (result == null) return matchModelObj;

                matchModelObj.HomeTeamScore = result.pointsTeam1;
                matchModelObj.AwayTeamScore = result.pointsTeam2;
            }


            return matchModelObj;
        }
    }
}