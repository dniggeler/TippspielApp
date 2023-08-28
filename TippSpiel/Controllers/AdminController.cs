using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMatrix.WebData;
using FussballTippApp.Models;
using System.Web.Security;
using FussballTippApp.Filters;
using OddsScraper.Contract.Model;
using BhFS.Tippspiel.Utils;
using Data.SqlClient;
using Tippspiel.Contracts;
using Tippspiel.Implementation;

namespace FussballTippApp.Controllers
{
    [InitializeSimpleMembership]
    public class AdminController : Controller
    {
        readonly log4net.ILog _log;

        private IFussballDataRepository _matchDataRepository;
        private readonly ICacheProvider _cache;
        private readonly WettfreundeScraper _oddsScraper;

        public AdminController(IFussballDataRepository repository, WettfreundeScraper oddsScraper, ICacheProvider cacheProvider, log4net.ILog logger)
        {
            _matchDataRepository = repository;
            _cache = cacheProvider;
            _log = logger;
            _oddsScraper = oddsScraper;
        }

        public async Task<ActionResult> Index(int? Spieltag)
        {
            int currSpieltag = Spieltag.HasValue
                ? Spieltag.Value
                : (await OpenDBHelper.GetSpieltagInfo(_matchDataRepository)).TippSpieltag;

            // build dropdown list data
            {
                var count = SportsdataConfigInfo.Current.EndSpieltag - SportsdataConfigInfo.Current.StartSpieltag + 1;
                var ddlSpieltageRange = (from e in Enumerable.Range(SportsdataConfigInfo.Current.StartSpieltag, count)
                    select new SelectListItem()
                    {
                        Value = e.ToString(),
                        Text = "Spieltag " + e,
                        Selected = (e == currSpieltag)
                    });

                ViewBag.Spieltag = ddlSpieltageRange;
            }

            var viewModel = await DailyWinnersInternalAsync(currSpieltag, true);

            return View(viewModel);
        }

        public ActionResult Clear()
        {
            // delete all tipper data
            using (var ctxt = new TippSpielContext())
            {
                foreach (var tipp in ctxt.TippMatchList)
                {
                    ctxt.Entry(tipp).State = EntityState.Deleted;
                }

                ctxt.SaveChanges();
            }

            // delete all users
            var users = GetUserList();

            foreach (var user in users)
            {
                var username = user.username;

                try
                {
                    if (WebSecurity.UserExists(username) == true)
                    {
                        Membership.DeleteUser(username);
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    _log.ErrorFormat("User {0} not created: {1}", user.username, e.Message);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult ClearCache()
        {
            _cache.Clear();

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> GetAllMatchesAsync(int spieltag)
        {
            var matches = await _matchDataRepository.GetMatchesByGroupAsync(spieltag);

            var leagueIdentifier = SportsdataConfigInfo.Current.LeagueShortcut;
            var seasonIdentifier = SportsdataConfigInfo.Current.LeagueSaison;

            using (var ctxt = new MatchInfoContext())
            {
                foreach (var m in matches)
                {
                    ctxt.Items.Add(new MatchInfoItem()
                    {
                        GroupOrderId = m.Group.Id,
                        MatchId = m.MatchId,
                        MatchNr = m.MatchNr,
                        HomeTeamId = m.HomeTeam.Id,
                        AwayTeamId = m.AwayTeam.Id,
                        HomeTeam = m.HomeTeam.ShortName,
                        AwayTeam = m.AwayTeam.ShortName,
                        HomeTeamScore = m.HomeTeamScore,
                        AwayTeamScore = m.AwayTeamScore,
                        HomeTeamIcon = m.HomeTeam.IconUrl,
                        AwayTeamIcon = m.AwayTeam.IconUrl,
                        KickoffTime = m.KickoffTime,
                        KickoffTimeUtc = m.KickoffTimeUTC,
                        IsFinished = m.IsFinished,
                        IsInProlongation = m.HasVerlaengerung,
                        LeagueIdentifier = leagueIdentifier,
                        SeasonIdentifier = seasonIdentifier
                    });
                }

                ctxt.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private List<dynamic> GetUserList()
        {
            var users = new[] {  
                new { username = "stefan_z", email= "stefan.zeisberger@bf.uzh.ch"},
                new { username = "remo", email= "remo.stoessel@bf.uzh.ch"},
                new { username = "dieter", email= "dniggeler@bhfs.ch"},
                new { username = "thorsten", email= "thorsten.hens@bf.uzh.ch"},
                new { username = "karl", email= "karl.schmedders@business.uzh.ch"},
                new { username = "dominik", email= "Dominik.Hlinka@web.de"},
                new { username = "mo", email= "mhl@agfif.com"},
                new { username = "eckart", email= "eckart.jaeger@bf.uzh.ch"},
                new { username = "stefan_r", email= "stefan.rehder@via-value.de"},
            };

            return users.ToList<dynamic>();
        }

        private async Task<DailyWinnerInfoModel> DailyWinnersInternalAsync(int currSpieltag, bool IsAdminView)
        {
            var viewModel = new DailyWinnerInfoModel();

            var matchesDB = await _matchDataRepository.GetMatchesByGroupAsync(currSpieltag);

            foreach (var m in matchesDB)
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
                foreach (var match in matchesDB)
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

                        if (matchModelObj.HasStarted || IsAdminView)
                        {
                            resultDict[tip.User].TippCount++;
                            resultDict[tip.User].TotalPoints +=
                                (matchModelObj.MyPoints.HasValue) ? matchModelObj.MyPoints.Value : 0.0;
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
