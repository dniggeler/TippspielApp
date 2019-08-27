using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using OddsScraper.Contract;
using OddsScraper.Contract.Model;

namespace OddsScraper
{
    /// <summary>
    /// Helper class to extracts odds from wettfreunde.net site
    /// </summary>
    /// <seealso cref="OddsScraper.Contract.IOddsScraper" />
    public class WettfreundeOddsNewBuLiScraper : IOddsScraper
    {
        public List<OddsInfoModel> GetOdds(string oddsAsHtmlStr, string roundTag)
        {
            // fix html errors
            var cleanHtmlSrc = FixOpenTags(oddsAsHtmlStr);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(cleanHtmlSrc);

            return GetOdds(doc,roundTag);
        }

        public List<OddsInfoModel> LoadOdds(string url, string roundTag)
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            return GetOdds(doc,roundTag);
        }

        private string FixOpenTags(string oddsAsHtmlStr)
        {
            string errorPatternStr = @"</tr\n";
            string correctedPatternStr = @"</tr>\n";

            var regex = new Regex(errorPatternStr);

            return regex.Replace(oddsAsHtmlStr, correctedPatternStr);
        }

        /// <summary>
        /// Gets the odds.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="roundTag">The round tag.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException">Spieltag nicht gefunden.</exception>
        private List<OddsInfoModel> GetOdds(HtmlDocument doc, string roundTag)
        {
            int spieltag = int.Parse(roundTag);

            var oddsList = new List<OddsInfoModel>();
            {
                // remove all unnecessary html

                ScrubHelper.ScrubHtml(doc);

                var nodes = doc.DocumentNode.SelectNodes("//comment()");
                if (nodes != null)
                {
                    foreach (HtmlNode comment in nodes)
                    {
                        comment.ParentNode.RemoveChild(comment);
                    }
                }

                // Odds
                for (int ii = 1; ii <= 9; ii++)
                {
                    var sectionNode = GetSection(doc, ii);
                    Debug.WriteLine(sectionNode.InnerHtml);
                    var model = new OddsInfoModel();

                    var teams = GetTeams(sectionNode);


                    model.HomeTeam = teams.Item1;
                    model.HomeTeamSearch = model.HomeTeam.ToUpper();

                    model.AwayTeam = teams.Item2;
                    model.AwayTeamSearch = model.AwayTeam.ToUpper();

                    var odds = GetGameOdds(sectionNode);
                    model.WinOdds = odds.Item1;

                    oddsList.Add(model);
                    
                }
            }

            return oddsList;
        }

        private Tuple<double?,double?,double?> GetGameOdds(HtmlNode sectionNode)
        {
            return new Tuple<double?, double?, double?>(0.2,0.4,0.4);
        }

        private Tuple<string, string> GetTeams(HtmlNode sectionNode)
        {
            var split = sectionNode.InnerText.Split(new []{"Wettquoten"},StringSplitOptions.RemoveEmptyEntries);
            var split2 = split[0].Split(new []{":"},StringSplitOptions.RemoveEmptyEntries);

            var splitTeams = split2[0].Split(new[] { "-"}, StringSplitOptions.RemoveEmptyEntries);

            return new Tuple<string, string>(splitTeams[0].Trim(new []{' '}),splitTeams[1].Trim(new[] { ' ' }));
        }

        private static HtmlNode GetSection(HtmlDocument doc, int sectionNumber)
        {
            var sectionNode = doc.GetElementbyId($"section-{sectionNumber}");

            if (sectionNode == null)
            {
                return null;
            }

            foreach (HtmlNode node in sectionNode.SelectNodes(".//h2"))
            {
                string cleanStr = node.InnerText.Trim('\n', ' ');

                if (cleanStr.Contains("Wettquoten"))
                {
                    return node;
                }
            }

            return null;
        }

  }
}
