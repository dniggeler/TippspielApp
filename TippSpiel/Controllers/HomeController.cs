using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Web.Mvc;
using BhFS.Tippspiel.Utils;
using FussballTippApp.Models;
using TippSpiel.Mailers;
using OddsScraper.Contract.Model;
using WebGrease.Css.Extensions;
using OddsScraper;
using TippSpiel.Properties;
using Tippspiel.Contracts;
using Tippspiel.Implementation;
using log4net;
using Tippspiel.Contracts.Models;

namespace FussballTippApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILog _log;
        private IFussballDataRepository _matchDataRepository;
        private ITippMailer _tippMailer = new TippMailer();
        private readonly WettfreundeScraper _oddsScraper;

        public HomeController(IFussballDataRepository repository, WettfreundeScraper oddsScraper, ILog logger)
        {
            _matchDataRepository = repository;
            _log = logger;
            _oddsScraper = oddsScraper;
        }

        public ITippMailer TippMailer
        {
            get { return _tippMailer; }
            set { _tippMailer = value; }
        }

        public IAccessStats MatchDBStats => _matchDataRepository as IAccessStats;

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public async Task<ActionResult> Index(int? Spieltag)
        {
            OpenDBHelper.SpieltagInfo spieltagInfo = await OpenDBHelper.GetSpieltagInfo(_matchDataRepository);

            int currentSpieltag = Spieltag ?? spieltagInfo.TippSpieltag;

            // build dropdown list data
            {
                var count = SportsdataConfigInfo.Current.EndSpieltag - SportsdataConfigInfo.Current.StartSpieltag + 1;
                var ddlSpieltageRange = (from e in Enumerable.Range(SportsdataConfigInfo.Current.StartSpieltag, count)
                    select new SelectListItem()
                        { Value = e.ToString(), Text = "Spieltag " + e.ToString(), Selected = (e == currentSpieltag) });

                ViewBag.Spieltag = ddlSpieltageRange;
            }

            var model = new SpieltagModel();
            model.Spieltag = currentSpieltag;

            if ((currentSpieltag == 34 || currentSpieltag == 0) && spieltagInfo.IsCompleted)
            {
                model.IsTippSpielFinished = true;
                return View(model);
            }

            List<OddsInfoModel> oddsList = null;

            try
            {
                //throw new ApplicationException("skip");
                oddsList = _oddsScraper.Scrap(currentSpieltag);
            }
            catch (Exception ex)
            {
                _log.Error("Error while scrab odds: " + ex.Message);
                string fixFilename = String.Format("Odds_{0}_{1}_{2}", SportsdataConfigInfo.Current.LeagueShortcut,
                    SportsdataConfigInfo.Current.LeagueSaison, currentSpieltag);
                _log.Info("Try fix: " + fixFilename);

                ResourceManager rm = new ResourceManager(typeof(Resources));
                var fixContent = rm.GetObject(fixFilename) as string;

                var oddScraper = new WettfreundeOddsNewBuLiScraper();

                oddsList = oddScraper.GetOdds(fixContent, currentSpieltag.ToString());
            }

            var matches = await _matchDataRepository.GetMatchesByGroupAsync(currentSpieltag);

            foreach (var m in matches)
            {
                var modelAllInfo = new MatchInfoModel
                {
                    MatchId = m.MatchId,
                    AwayTeam = m.AwayTeam.ShortName,
                    AwayTeamIcon = m.AwayTeam.IconUrl,
                    AwayTeamScore = m.AwayTeamScore,
                    HomeTeam = m.HomeTeam.ShortName,
                    HomeTeamIcon = m.HomeTeam.IconUrl,
                    HomeTeamScore = m.HomeTeamScore,
                    IsFinished = m.IsFinished,
                    KickoffTime = m.KickoffTime,
                    KickoffTimeUtc = m.KickoffTimeUTC
                };

                model.IsSpieltagFinished = (model.IsSpieltagFinished && modelAllInfo.IsFinished);

                // mixin odds quotes into match data
                {
                    MixinOddsQuotes(oddsList, modelAllInfo);
                }

                using (var ctxt = new TippSpielContext())
                {
                    var myTippObject = (from t in ctxt.TippMatchList
                            where t.MatchId == modelAllInfo.MatchId &&
                                  t.User == User.Identity.Name
                            select t)
                        .FirstOrDefault();

                    if (myTippObject != null)
                    {
                        modelAllInfo.MyOdds = myTippObject.MyOdds;
                        modelAllInfo.MyTip = myTippObject.MyTip;
                        modelAllInfo.MyAmount = myTippObject.MyAmount;
                    }
                }


                model.Matchdata.Add(modelAllInfo);
            }

            {
                _log.DebugFormat("Tipp data stats: remote hits={0}, cache hits={1}", MatchDBStats.GetRemoteHits(),
                    MatchDBStats.GetCacheHits());
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> OverallStanding()
        {

            using (var ctxt = new TippSpielContext())
            {
                var resultDict = new Dictionary<string, RankingInfoModel>();
                using (var userCtxt = new UsersContext())
                {
                    // init dict
                    {
                        foreach (var username in (from t in ctxt.TippMatchList select t.User).Distinct())
                        {
                            var m = new RankingInfoModel();
                            m.User = username;
                            m.DisplayName = (from u in userCtxt.UserProfiles
                                    where u.UserName == username
                                    select u.DisplayName)
                                .FirstOrDefault();
                            resultDict.Add(username, m);
                        }
                    }
                }

                foreach (var tip in ctxt.TippMatchList.Where(t => t.MyTip.HasValue))
                {
                    var rankingObj = resultDict[tip.User];

                    MatchDataModel matchInfo = await _matchDataRepository.GetMatchDataAsync(tip.MatchId);

                    var matchModelObj = new MatchInfoModel()
                    {
                        MatchId = matchInfo.MatchId,
                        AwayTeam = matchInfo.AwayTeam.ShortName,
                        AwayTeamIcon = matchInfo.HomeTeam.IconUrl,
                        AwayTeamScore = matchInfo.AwayTeamScore,
                        HomeTeam = matchInfo.HomeTeam.ShortName,
                        HomeTeamIcon = matchInfo.HomeTeam.IconUrl,
                        HomeTeamScore = matchInfo.HomeTeamScore,
                        IsFinished = matchInfo.IsFinished,
                        KickoffTime = matchInfo.KickoffTime,
                        KickoffTimeUtc = matchInfo.KickoffTimeUTC,
                        MyOdds = tip.MyOdds,
                        MyAmount = tip.MyAmount,
                        MyTip = tip.MyTip
                    };


                    if (tip.MyOdds.HasValue && tip.MyAmount.HasValue)
                    {
                        matchModelObj.MyOdds = tip.MyOdds;
                        matchModelObj.MyAmount = tip.MyAmount;
                        matchModelObj.MyTip = tip.MyTip;

                        if (matchModelObj.HasStarted == true)
                        {
                            rankingObj.TippCount++;
                            rankingObj.TotalPoints +=
                                (matchModelObj.MyPoints.HasValue) ? matchModelObj.MyPoints.Value : 0.0;
                        }

                        // count longshot and favorite
                        {
                            rankingObj.TippCountFavorite +=
                                (matchModelObj.FavoriteTippIndex == tip.MyTip.Value) ? 1 : 0;
                            rankingObj.TippCountLongshot +=
                                (matchModelObj.LongshotTippIndex == tip.MyTip.Value) ? 1 : 0;
                        }
                    }
                }

                var resultList = (from kp in resultDict select kp.Value).ToList();

                resultList = (from e in resultList
                        orderby e.TotalPoints descending, e.PointAvg, e.TippCount descending
                        select e)
                    .ToList();

                int counter = 1;
                resultList.ForEach(e => { e.Rang = counter++; });

                return View(resultList);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> DailyWinners(int? Spieltag)
        {
            _log.Debug("Begin DailyWinners");

            int currSpieltag = Spieltag.HasValue
                ? Spieltag.Value
                : (await OpenDBHelper.GetSpieltagInfo(_matchDataRepository)).CurrentSpieltag;


            // build dropdown list data
            {
                var count = SportsdataConfigInfo.Current.EndSpieltag - SportsdataConfigInfo.Current.StartSpieltag + 1;
                var ddlSpieltageRange = (from e in Enumerable.Range(SportsdataConfigInfo.Current.StartSpieltag, count)
                    select new SelectListItem()
                    {
                        Value = e.ToString(),
                        Text = "Spieltag " + e.ToString(),
                        Selected = (e == currSpieltag)
                    });

                ViewBag.Spieltag = ddlSpieltageRange;
            }

            var viewModel = await DailyWinnersInternalAsync(currSpieltag);

            _log.Debug("End DailyWinners");

            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<JsonResult> SendDailyWinnerEmail(string username)
        {
            _log.Debug("Begin SendDailyWinnerEmail()");

            // precondition to send daily winner email
            {
                var completeInfo = await OpenDBHelper.IsSpieltagComplete(_matchDataRepository, true);

                var jsonResponse = new
                {
                    Success = true,
                    Receivers = new List<string>()
                };

                if (completeInfo.IsCompleted == false)
                {
                    _log.DebugFormat("Spieltag is not yet completed");

                    return Json(jsonResponse, JsonRequestBehavior.AllowGet);
                }
                else if (completeInfo.IsCompletedRecently == false)
                {
                    _log.DebugFormat("Spieltag is out-dated");

                    return Json(jsonResponse, JsonRequestBehavior.AllowGet);
                }
            }

            var spieltag = (await OpenDBHelper.GetSpieltagInfo(_matchDataRepository)).CurrentSpieltag;

            var result = await DailyWinnersInternalAsync(spieltag);

            using (var ctxt = new UsersContext())
            {
                var userList = new List<string>();

                var profiles = ctxt.UserProfiles as IQueryable<UserProfile>;
                if (String.IsNullOrEmpty(username) == false)
                {
                    profiles = profiles.Where(p => p.UserName == username);
                }

                foreach (var user in profiles)
                {
                    if (user != null && !String.IsNullOrEmpty(user.Email))
                    {
                        TippMailer.EmailDailyWinner(user.Email, result, spieltag).Send();
                        _log.Debug("Email sent to " + user.Email);

                        userList.Add(user.UserName);
                    }
                }

                var jsonResponse = new
                {
                    Success = true,
                    Receivers = userList
                };

                _log.Debug("End SendDailyWinnerEmail()");

                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ReportDailyWinners()
        {
            _log.Debug("Begin All DailyWinners");

            int endSpieltag = (await OpenDBHelper.GetSpieltagInfo(_matchDataRepository)).CurrentSpieltag;
            int beginSpieltag = SportsdataConfigInfo.Current.StartSpieltag;

            var allWinnersModel = new OverallDailyWinnerInfoModel();

            using (var ctxt = new TippSpielContext())
            {
                using (var userCtxt = new UsersContext())
                {
                    var allUsers = (from t in ctxt.TippMatchList select t.User).Distinct().ToArray();
                    foreach (var user in allUsers)
                    {
                        var displayName =
                            (from u in userCtxt.UserProfiles where u.UserName == user select u.DisplayName)
                            .FirstOrDefault();

                        allWinnersModel.AllDailyWinnerMap.Add(user, new OverallDailyWinnerRankingItemModel()
                        {
                            User = user,
                            DisplayName = displayName,
                            Rang = -1,
                            Wins = 0
                        });
                    }
                }
            }


            for (int day = beginSpieltag; day <= endSpieltag; day++)
            {
                var result = await DailyWinnersInternalAsync(day);
                foreach (var item in result.Ranking.Where(r => r.Rang == 1))
                {
                    allWinnersModel.AllDailyWinnerMap[item.User].Wins += 1;
                }
            }

            var orderResult =
                (from kp in allWinnersModel.AllDailyWinnerMap orderby kp.Value.Wins descending select kp).ToDictionary(
                    p => p.Key, p => p.Value);
            int counter = 1;
            orderResult.ForEach(e => { e.Value.Rang = counter++; });


            allWinnersModel.AllDailyWinnerMap = orderResult;

            _log.Debug("End All DailyWinners");

            return View(allWinnersModel);
        }

        [HttpPost]
        public JsonResult SetMatchTipp(int id, int tip, double? odds)
        {
            try
            {
                var user = User.Identity.Name.ToLower();

                _log.DebugFormat("match id={0}, tip={1}, odds={2:0.00}, user={3}", id, tip, odds, user);

                using (var ctxt = new TippSpielContext())
                {
                    TippMatchModel matchObj = (from m in ctxt.TippMatchList 
                                    where m.MatchId == id &&
                                          m.User == user
                                    select m)
                                    .FirstOrDefault();

                    if (matchObj == null)
                    {
                        var newMatchObj = new TippMatchModel()
                        {
                            MatchId = id,
                            MyOdds = odds,
                            MyTip = tip,
                            MyAmount = 1.0,
                            User = user,
                            LastUpdated = DateTime.Now
                        };

                        ctxt.TippMatchList.Add(newMatchObj);
                    }
                    else
                    {
                        matchObj.LastUpdated = DateTime.Now;
                        matchObj.User = user;
                        matchObj.MyOdds = odds;
                        matchObj.MyTip = tip;
                        matchObj.MyAmount = 1.0;
                        matchObj.MyTip = tip;
                    }

                    ctxt.SaveChanges();

                    var result = new
                    {
                        Success = true,
                        MatchId = id,
                        MyOdds = $"{odds:0.00}"
                    };

                    return Json(result);
                }
            }
            catch (FormatException ex)
            {
                _log.ErrorFormat("Match id cannot be converted, id={0}.", id);
                _log.ErrorFormat("Exception message={0}.", ex.Message);

                return Json(new {
                    Success = false,
                    Error = ex.Message,
                    MatchId = id
                });
            }
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "BuLi Tippspiel 2019/2020";

            return View();
        }

        private static void MixinOddsQuotes(List<OddsInfoModel> oddsList, MatchInfoModel m)
        {
            var homeTeamUpper = m.HomeTeam.ToUpper();
            var awayTeamUpper = m.AwayTeam.ToUpper();

            var oddsMatchObj = (from o in oddsList
                             where (m.HomeTeam.IndexOf(o.HomeTeamSearch, StringComparison.OrdinalIgnoreCase) >=0 ||
                                    m.AwayTeam.IndexOf(o.AwayTeamSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                             select o)
                            .FirstOrDefault();

            if (oddsMatchObj == null)
            {
                foreach (var el in oddsList)
                {
                    {
                        var longestElement =
                            el.HomeTeamSearch.Split(new char[] { ' ' }, StringSplitOptions.None).OrderByDescending(i => i.Length).FirstOrDefault();

                        if (longestElement != null)
                        {
                            el.HomeTeamSearch = longestElement;
                        }
                    }

                    {
                        var longestElement =
                            el.AwayTeamSearch.Split(new char[] { ' ' }, StringSplitOptions.None).OrderByDescending(i => i.Length).FirstOrDefault();

                        if (longestElement != null)
                        {
                            el.AwayTeamSearch = longestElement;
                        }
                    }

                }

                oddsMatchObj = (from o in oddsList
                                where (homeTeamUpper.Contains(o.HomeTeamSearch.ToUpper()) || awayTeamUpper.Contains(o.AwayTeamSearch.ToUpper()))
                                select o)
                    .DefaultIfEmpty(new OddsInfoModel
                    {
                        HomeTeam = m.HomeTeam,
                        HomeTeamSearch = homeTeamUpper,
                        AwayTeam = m.AwayTeam,
                        AwayTeamSearch = awayTeamUpper,
                        })
                                .First();
            }

            var oddsMatch = oddsMatchObj;

            if (m.MatchId == 66738)
            {
                m.HomeTeamOdds = 1.15;
                m.AwayTeamOdds = 17.0;
                m.DrawOdds = 7.0;
            }
            else if (m.MatchId == 66787)
            {
                m.HomeTeamOdds = null;
                m.AwayTeamOdds = null;
                m.DrawOdds = null;
            }
            else
            {
                m.HomeTeamOdds = oddsMatch.WinOdds;
                m.AwayTeamOdds = oddsMatch.LossOdds;
                m.DrawOdds = oddsMatch.DrawOdds;
            }

            // find favorite and longshot tipp
            if (m.DrawOdds.HasValue &&
                m.HomeTeamOdds.HasValue &&
                m.AwayTeamOdds.HasValue)
            {
                double[] odds = new double[]{
                                    m.DrawOdds.Value,
                                    m.HomeTeamOdds.Value,
                                    m.AwayTeamOdds.Value
                                };

                var favoriteOdds = odds.Min();
                var longshotOdds = odds.Max();

                if (odds[1] == favoriteOdds)
                {
                    m.FavoriteTippIndex = 1;
                }
                else if (odds[2] == favoriteOdds)
                {
                    m.FavoriteTippIndex = 2;
                }
                else if (odds[0] == favoriteOdds)
                {
                    m.FavoriteTippIndex = 0;
                }

                if (odds[1] == longshotOdds)
                {
                    m.LongshotTippIndex = 1;
                }
                else if (odds[2] == longshotOdds)
                {
                    m.LongshotTippIndex = 2;
                }
                else if (odds[0] == longshotOdds)
                {
                    m.LongshotTippIndex = 0;
                }
            }
        }

        private async Task<DailyWinnerInfoModel> DailyWinnersInternalAsync(int currSpieltag)
        {
            _log.Debug("Current spieltag="+currSpieltag.ToString());

            var viewModel = new DailyWinnerInfoModel();

            var matchesDb = await _matchDataRepository.GetMatchesByGroupAsync(currSpieltag);

            foreach (var m in matchesDb)
            {
                viewModel.MatchInfo.Add(OpenDBHelper.Create(m));
            }

            using (var ctxt = new TippSpielContext())
            {
                var resultDict = new Dictionary<string, RankingInfoModel>();
                using (var userCtxt = new UsersContext())
                {
                    // init result dict
                    {
                        foreach (var username in (from t in ctxt.TippMatchList select t.User).Distinct())
                        {
                            var m = new RankingInfoModel();
                            m.User = username;
                            m.DisplayName = (from u in userCtxt.UserProfiles
                                    where u.UserName == username
                                    select u.DisplayName)
                                .FirstOrDefault();

                            resultDict.Add(username, m);
                            viewModel.AllTippInfoDict.Add(username, new List<MatchInfoModel>());
                        }
                    }
                }


                // 1. get all tipps for match with id
                foreach (var match in matchesDb)
                {
                    var tippSet = (from t in ctxt.TippMatchList
                        where t.MatchId == match.MatchId &&
                              t.MyTip.HasValue &&
                              t.MyAmount.HasValue &&
                              t.MyOdds.HasValue
                        select t);
                    foreach (var tip in tippSet)
                    {
                        var matchModelObj = OpenDBHelper.Create(match);
                        matchModelObj.MyOdds = tip.MyOdds;
                        matchModelObj.MyAmount = tip.MyAmount;
                        matchModelObj.MyTip = tip.MyTip;

                        if (matchModelObj.HasStarted == true)
                        {
                            resultDict[tip.User].TippCount++;
                            resultDict[tip.User].TotalPoints +=
                                (matchModelObj.MyPoints.HasValue) ? matchModelObj.MyPoints.Value : 0.0;
                        }

                        if (matchModelObj.HasStarted == true)
                        {
                            viewModel.AllTippInfoDict[tip.User].Add(matchModelObj);
                        }
                    }
                }

                var resultList = (from kp in resultDict select kp.Value).ToList();

                viewModel.Ranking = (from e in resultList
                        orderby e.TotalPoints descending, e.PointAvg, e.TippCount descending
                        select e)
                    .ToList();

                int counter = 1;
                viewModel.Ranking.ForEach(e => { e.Rang = counter++; });

                return viewModel;
            }
        }

    }
}
