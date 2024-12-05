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
            string teamHeidenheim = "Heidenheim";
            string teamHeidenheimSearch = "Heidenheim";

            string teamDortmund = "Borussia Dortmund";
            string teamDortmundSearch = "Dortmund";
            string teamAugsburg = "Augsburg";
            string teamAugsburgSearch = "Augsburg";

            string teamLeverkusen = "Bayer Leverkusen";
            string teamLeverkusenSearch = "Leverkusen";
            string teamPauli = "Sankt Pauli";
            string teamPauliSearch = "Pauli";

            string teamWolfsburg = "VfL Wolfsburg";
            string teamWolfsburgSearch = "Wolfsburg";
            string teamKiel = "Holstein Kiel";
            string teamKielSearch = "Kiel";

            string teamBremen = "Werder Bremen";
            string teamBremenSearch = "Werder Bremen";
            string teamDusseldorf = "Fortuna Düsseldorf";
            string teamDusseldorfSearch = "Fortuna Düsseldorf";

            string teamFreiburg = "SC Freiburg";
            string teamFreiburgSearch = "Freiburg";
            string teamMainz = "FSV Mainz 05";
            string teamMainzSearch = "Mainz";

            string teamMonchengladbach = "Borussia Mönchengladbach";
            string teamMonchengladbachSearch = "Gladbach";
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

            string teamStuttgart = "VfB Stuttgart";
            string teamStuttgartSearch = "Stuttgart";

            string teamBochum = "VfL Bochum";
            string teamBochumSearch = "Bochum";

            var oddsList = new List<OddsInfoModel>
            {
                new OddsInfoModel
                {
                    HomeTeam = teamStuttgart,
                    HomeTeamSearch = teamStuttgartSearch,
                    AwayTeam = teamBerlin,
                    AwayTeamSearch = teamBerlinSearch,
                    WinOdds = 1.69,
                    DrawOdds = 3.6,
                    LossOdds = 5,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamLeverkusen,
                    HomeTeamSearch = teamLeverkusenSearch,
                    AwayTeam = teamPauli,
                    AwayTeamSearch = teamPauliSearch,
                    WinOdds = 1.19,
                    DrawOdds = 7,
                    LossOdds = 11,
                },

                new OddsInfoModel
                {
                    HomeTeam = teamBayern,
                    HomeTeamSearch = teamBayernSearch,
                    AwayTeam = teamHeidenheim,
                    AwayTeamSearch = teamHeidenheimSearch,
                    WinOdds = 1.07,
                    DrawOdds = 9,
                    LossOdds = 26,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamFrankfurt,
                    HomeTeamSearch = teamFrankfurtSearch,
                    AwayTeam = teamAugsburg,
                    AwayTeamSearch = teamAugsburgSearch,
                    WinOdds = 1.54,
                    DrawOdds = 4,
                    LossOdds = 6,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamKiel,
                    HomeTeamSearch = teamKielSearch,
                    AwayTeam = teamLeipzig,
                    AwayTeamSearch = teamLeipzigSearch,
                    WinOdds = 5.5,
                    DrawOdds = 4,
                    LossOdds = 1.58,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamBochum,
                    HomeTeamSearch = teamBochumSearch,
                    AwayTeam = teamBremen,
                    AwayTeamSearch = teamBremenSearch,
                    WinOdds = 3.1,
                    DrawOdds = 3.5,
                    LossOdds = 2.25,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamMonchengladbach,
                    HomeTeamSearch = teamMonchengladbachSearch,
                    AwayTeam = teamWolfsburg,
                    AwayTeamSearch = teamWolfsburgSearch,
                    WinOdds = 3.4,
                    DrawOdds = 3.6,
                    LossOdds = 2.05,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamWolfsburg,
                    HomeTeamSearch = teamWolfsburgSearch,
                    AwayTeam = teamMainz,
                    AwayTeamSearch = teamMainzSearch,
                    WinOdds = 2.1,
                    DrawOdds = 3.30,
                    LossOdds = 3.60,
                },
                new OddsInfoModel
                {
                    HomeTeam = teamHoffenheim,
                    HomeTeamSearch = teamHoffenheimSearch,
                    AwayTeam = teamFreiburg,
                    AwayTeamSearch = teamFreiburgSearch,
                    WinOdds = 2.7,
                    DrawOdds = 3.25,
                    LossOdds = 2.62,
                }
            };
            
            return oddsList;
        }
    }
}
