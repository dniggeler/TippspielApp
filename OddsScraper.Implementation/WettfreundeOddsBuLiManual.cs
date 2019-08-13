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
                    HomeTeam = teamBayern,
                    AwayTeam = teamHertha,
                    HomeTeamSearch = teamBayernSearch,
                    AwayTeamSearch = teamHerthaSearch,
                    WinOdds = 1.20,
                    DrawOdds =6.8,
                    LossOdds = 13.0,
                },

                new OddsInfoModel
                {
                    HomeTeam = teamLeverkusen,
                    AwayTeam = teamPaderborn,
                    HomeTeamSearch = teamLeverkusenSearch,
                    AwayTeamSearch = teamPaderbornSearch,
                    WinOdds = 1.36,
                    DrawOdds = 5.3,
                    LossOdds = 7.9,
                },

                new OddsInfoModel
                {
                    HomeTeam = teamWolfsburg,
                    AwayTeam = teamKoeln,
                    HomeTeamSearch = teamWolfsburgSearch,
                    AwayTeamSearch = teamKoelnSearch,
                    WinOdds = 1.90,
                    DrawOdds = 3.75,
                    LossOdds = 3.90,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamDortmund,
                    AwayTeam = teamAugsburg,
                    HomeTeamSearch = teamDortmundSearch,
                    AwayTeamSearch = teamAugsburgSearch,
                    WinOdds = 1.26,
                    DrawOdds = 6.1,
                    LossOdds = 11.0,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamBremen,
                    AwayTeam = teamDusseldorf,
                    HomeTeamSearch = teamBremenSearch,
                    AwayTeamSearch = teamDusseldorfSearch,
                    WinOdds = 1.72,
                    DrawOdds = 3.9,
                    LossOdds = 4.25,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamFreiburg,
                    AwayTeam = teamMainz,
                    HomeTeamSearch = teamFreiburgSearch,
                    AwayTeamSearch = teamMainzSearch,
                    WinOdds = 2.15,
                    DrawOdds = 3.4,
                    LossOdds = 3.35,
                },

                new OddsInfoModel
                {
                    HomeTeam = teamMonchengladbach,
                    AwayTeam = teamSchalke,
                    HomeTeamSearch = teamMonchengladbachSearch,
                    AwayTeamSearch = teamSchalkeSearch,
                    WinOdds = 2.1,
                    DrawOdds = 3.55,
                    LossOdds = 3.45,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamFrankfurt,
                    AwayTeam = teamHoffenheim,
                    HomeTeamSearch = teamFrankfurtSearch,
                    AwayTeamSearch = teamHoffenheimSearch,
                    WinOdds = 2.5,
                    DrawOdds = 3.65,
                    LossOdds = 2.65,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamBerlin,
                    AwayTeam = teamLeipzig,
                    HomeTeamSearch = teamBerlinSearch,
                    AwayTeamSearch = teamLeipzigSearch,
                    WinOdds = 4.45,
                    DrawOdds = 3.95,
                    LossOdds = 1.75,
                },
            };


            return oddsList;
        }

    }
}
