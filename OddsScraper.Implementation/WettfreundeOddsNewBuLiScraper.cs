using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(oddsAsHtmlStr);

            return GetOdds(doc,roundTag);
        }

        public List<OddsInfoModel> LoadOdds(string url, string roundTag)
        {
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(url);

            return GetOdds(doc,roundTag);
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
                    model.DrawOdds = odds.Item2;
                    model.LossOdds = odds.Item3;

                    oddsList.Add(model);
                }
            }

            return oddsList;
        }

        private Tuple<double?,double?,double?> GetGameOdds(HtmlNode sectionNode)
        {
            var betProviderNode = sectionNode
                .ParentNode
                .SelectNodes(".//div[@class='bbo_table_row']")
                .FirstOrDefault();

            var winOddsNode = betProviderNode.SelectSingleNode(".//div[@class='bbo_odds_1 ']");
            if (winOddsNode == null)
            {
                winOddsNode = betProviderNode.SelectSingleNode(".//div[@class='bbo_odds_1 hl']");
            }

            var drawOddsNode = betProviderNode.SelectSingleNode(".//div[@class='bbo_odds_x ']");
            if (drawOddsNode == null)
            {
                drawOddsNode = betProviderNode.SelectSingleNode(".//div[@class='bbo_odds_x hl']");
            }

            var lossOddsNode = betProviderNode.SelectSingleNode(".//div[@class='bbo_odds_2 ']");
            if (lossOddsNode == null)
            {
                lossOddsNode = betProviderNode.SelectSingleNode(".//div[@class='bbo_odds_2 hl']");
            }

            var winOddValue = Convert.ToDouble(winOddsNode.InnerText.Trim('\n', ' ', '\r'));
            var drawOddValue = Convert.ToDouble(drawOddsNode.InnerText.Trim('\n', ' ', '\r'));
            var lossOddValue = Convert.ToDouble(lossOddsNode.InnerText.Trim('\n', ' ', '\r'));

            return new Tuple<double?, double?, double?>(winOddValue,drawOddValue,lossOddValue);
        }

        private Tuple<string, string> GetTeams(HtmlNode sectionNode)
        {
            var split = sectionNode.InnerText.Trim(' ').Split(new []{"Wettquoten"},StringSplitOptions.RemoveEmptyEntries);
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
