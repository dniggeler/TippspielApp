using System.Collections.Generic;
using OddsScraper.Contract;
using OddsScraper.Contract.Model;

namespace OddsScraper
{
    /// <summary>
    /// Helper class to extracts odds from wettfreunde.net site
    /// </summary>
    /// <seealso cref="OddsScraper.Contract.IOddsScraper" />
    public class WettfreundeOddsBuLiManual : IOddsScraper
    {
        public List<OddsInfoModel> GetOdds(string oddsAsHtmlStr, string roundTag)
        {
            return GetOdds(roundTag);
        }

        public List<OddsInfoModel> LoadOdds(string url, string roundTag)
        {
            return new List<OddsInfoModel>();
        }

        /// <summary>
        /// Gets the odds.
        /// </summary>
        /// <param name="roundTag">The round tag.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">Spieltag nicht gefunden.</exception>
        private List<OddsInfoModel> GetOdds(string roundTag)
        {
            int spieltag = int.Parse(roundTag);

            string teamBayern = "Bayern München";
            string teamBayernSearch = "Bayern";
            string teamHertha = "Hertha BSC";
            string teamHerthaSearch = "Hertha";

            string teamDortmund = "Borussia Dortmund";
            string teamDortmundSearch = "Dortmund";
            string teamAugsburg = "Augsburg";
            string teamAugsburgSearch = "Augsburg";

            string teamLeverkusen = "Bayer Leverkusen";
            string teamLeverkusenSearch = "Leverkusen";
            string teamPaderborn = "SC Paderborn";
            string teamPaderbornSearch = "Paderborn";

            string teamWolfsburg = "VfL Wolfsburg";
            string teamWolfsburgSearch = "Wolfsburg";
            string teamKoeln = "1. FC Köln";
            string teamKoelnSearch = "Köln";

            string teamBremen = "Werder Bremen";
            string teamBremenSearch = "Werder Bremen";
            string teamDusseldorf = "Fortuna Düsseldorf";
            string teamDusseldorfSearch = "Fortuna Düsseldorf";

            string teamFreiburg = "SC Freiburg";
            string teamFreiburgSearch = "Freiburg";
            string teamMainz = "FSV Mainz 05";
            string teamMainzSearch = "Mainz";

            string teamMonchengladbach = "Mönchengladbach";
            string teamMonchengladbachSearch = "Mönchengladbach";
            string teamSchalke = "FC Schalke 04";
            string teamSchalkeSearch = "Schalke";

            string teamFrankfurt = "Eintracht Frankfurt";
            string teamFrankfurtSearch = "Frankfurt";
            string teamHoffenheim = "1899 Hoffenheim";
            string teamHoffenheimSearch = "Hoffenheim";

            string teamBerlin = "Union Berlin";
            string teamBerlinSearch = "Union";
            string teamLeipzig = "RB Leipzig";
            string teamLeipzigSearch = "Leipzig";

            var oddsList = new List<OddsInfoModel>
            {
                new OddsInfoModel
                {
                    HomeTeam = teamKoeln,
                    HomeTeamSearch = teamKoelnSearch,
                    AwayTeam = teamDortmund,
                    AwayTeamSearch = teamDortmundSearch,
                    WinOdds = 6.4,
                    DrawOdds = 4.95,
                    LossOdds = 1.45,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamDusseldorf,
                    HomeTeamSearch = teamDusseldorfSearch,
                    AwayTeam = teamLeverkusen,
                    AwayTeamSearch = teamLeverkusenSearch,
                    WinOdds = 4,
                    DrawOdds =4.15,
                    LossOdds = 1.77,
                },

                new OddsInfoModel
                {
                    HomeTeam = teamMainz,
                    HomeTeamSearch = teamMainzSearch,
                    AwayTeam = teamMonchengladbach,
                    AwayTeamSearch = teamMonchengladbach,
                    WinOdds = 3.1,
                    DrawOdds = 3.5,
                    LossOdds = 2.25,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamAugsburg,
                    HomeTeamSearch = teamAugsburgSearch,
                    AwayTeam = teamBerlin,
                    AwayTeamSearch = teamBerlinSearch,
                    WinOdds = 2.15,
                    DrawOdds = 3.4,
                    LossOdds = 3.4,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamPaderborn,
                    HomeTeamSearch = teamPaderbornSearch,
                    AwayTeam = teamFreiburg,
                    AwayTeamSearch = teamFreiburg,
                    WinOdds = 2.55,
                    DrawOdds = 3.55,
                    LossOdds = 2.65,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamHoffenheim,
                    HomeTeamSearch = teamHoffenheimSearch,
                    AwayTeam = teamBremen,
                    AwayTeamSearch = teamBremenSearch,
                    WinOdds = 1.97,
                    DrawOdds = 3.9,
                    LossOdds = 3.5,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamSchalke,
                    HomeTeamSearch = teamSchalkeSearch,
                    AwayTeam = teamBayern,
                    AwayTeamSearch = teamBayernSearch,
                    WinOdds = 7.5,
                    DrawOdds = 5.1,
                    LossOdds = 1.38,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamLeipzig,
                    HomeTeamSearch = teamLeipzigSearch,
                    AwayTeam = teamFrankfurt,
                    AwayTeamSearch = teamFrankfurtSearch,
                    WinOdds = 1.63,
                    DrawOdds = 4.30,
                    LossOdds = 4.90,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamHertha,
                    HomeTeamSearch = teamHerthaSearch,
                    AwayTeam = teamWolfsburg,
                    AwayTeamSearch = teamWolfsburgSearch,
                    WinOdds = 2.4,
                    DrawOdds = 3.55,
                    LossOdds = 2.85,
                },
            };


            return oddsList;
        }

    }
}
