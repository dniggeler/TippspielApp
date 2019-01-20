using System;
using System.Collections.Generic;

namespace FussballTippApp.Models
{
    public class DailyWinnerInfoModel
    {
        public List<RankingInfoModel> Ranking { get; set; }
        public Dictionary<string, List<MatchInfoModel>> AllTippInfoDict { get; set; }
        public List<MatchInfoModel> MatchInfo { get; set; }

        public DailyWinnerInfoModel()
        {
            Ranking = new List<RankingInfoModel>();
            AllTippInfoDict = new Dictionary<string, List<MatchInfoModel>>();
            MatchInfo = new List<MatchInfoModel>();
        }
    }

    public class OverallDailyWinnerInfoModel
    {
        public Dictionary<string, OverallDailyWinnerRankingItemModel> AllDailyWinnerMap { get; set; }

        public OverallDailyWinnerInfoModel()
        {
            AllDailyWinnerMap = new Dictionary<string, OverallDailyWinnerRankingItemModel>();
        }
    }

    public class OverallDailyWinnerRankingItemModel
    {
        public string User { get; set; }
        public string DisplayName { get; set; }
        public int Rang { get; set; }
        public int Wins { get; set; }        
    }

    public class RankingInfoModel
    {
        public string User { get; set; }
        public string DisplayName { get; set; }
        public int TippCount { get; set; }
        public int TippCountFavorite { get; set; }
        public int TippCountLongshot { get; set; }
        public double TotalPoints { get; set; }
        public int Rang { get; set; }
        public int RangDelta { get; set; }

        public double FavoriteRatio => (TippCount > 0) ? TippCountFavorite / TippCount : 0.0;

        public double LongshotRatio => (TippCount > 0) ? TippCountLongshot / TippCount : 0.0;

        public double PointAvg => (TippCount > 0) ? TotalPoints / TippCount : 0.0;
    }

    public class MatchInfoModel
    {
        public int MatchId { get; set; }
        public DateTime KickoffTime { get; set; }
        public DateTime KickoffTimeUtc { get; set; }
        public bool IsFinished { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeamIcon { get; set; }
        public string AwayTeamIcon { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }

        public double? HomeTeamOdds { get; set; }
        public double? AwayTeamOdds { get; set; }
        public double? DrawOdds { get; set; }

        public int? FavoriteTippIndex { get; set; }
        public int? LongshotTippIndex { get; set; }

        public int? MyTip { get; set; }
        public double? MyOdds { get; set; }
        public double? MyAmount { get; set; }

        public int? ResultType
        {
            get
            {
                if (HasStarted)
                {
                    return (HomeTeamScore > AwayTeamScore) ?
                                                    1 : (HomeTeamScore < AwayTeamScore) ?
                                                    2 : 0;
                }

                return null;
            }
        }

        public bool? IsMyTipCorrect
        {
            get
            {
                if (HasStarted == false || ResultType.HasValue== false) return null;

                if (MyTip.HasValue)
                {
                    return (ResultType.Value == MyTip.Value);
                }

                return false;
            }
        }

        public double? MyPoints
        {
            get
            {
                if (HasStarted &&
                    MyTip.HasValue &&
                    ResultType.HasValue)
                {
                    return (ResultType.Value == MyTip.Value) ? MyOdds * MyAmount : 0.0;
                }

                return null;
            }
        }

        public bool HasStarted => !(KickoffTimeUtc > DateTime.UtcNow);

        public MatchInfoModel()
        {
        }

    }

    public class SpieltagModel
    {
        public int Spieltag { get; set; }
        public bool IsSpieltagFinished { get; set; }
        public bool IsTippSpielFinished { get; set; }
        public List<MatchInfoModel> Matchdata { get; set; }

        public SpieltagModel()
        {
            Spieltag = 1;
            IsSpieltagFinished = true;
            IsSpieltagFinished = false;
            Matchdata = new List<MatchInfoModel>();
        }
    }
}