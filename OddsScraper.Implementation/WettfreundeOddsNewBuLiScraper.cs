using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    if (sectionNode == null)
                    {
                        continue;
                    }

                    Debug.WriteLine(sectionNode.InnerHtml);
                    var model = new OddsInfoModel();

                    var teams = GetTeams(sectionNode);

                    if (teams == null)
                    {
                        continue;
                    }

                    model.HomeTeam = teams.Item1;
                    model.HomeTeamSearch = model.HomeTeam.ToUpper();

                    model.AwayTeam = teams.Item2;
                    model.AwayTeamSearch = model.AwayTeam.ToUpper();

                    var odds = GetGameOdds(sectionNode, teams.Item1, roundTag);
                    model.WinOdds = odds.Item1;
                    model.DrawOdds = odds.Item2;
                    model.LossOdds = odds.Item3;

                    if (roundTag == "31" && teams.Item1.Contains("Hertha"))
                    {
                        model.AwayTeam = "Stuttgart";
                        model.AwayTeamSearch = "Stuttgart".ToUpper();
                    }

                    oddsList.Add(model);
                }
            }

            return oddsList;
        }

        private Tuple<double?,double?,double?> GetGameOdds(HtmlNode sectionNode)
        {
            var selectedNodes = sectionNode
                .ParentNode
                .SelectNodes(".//tr[@class='removable']");

            if (selectedNodes == null)
            {
                return new Tuple<double?, double?, double?>(0.0, 0.0, 0.0);
            }

            var betProviderNode = selectedNodes.FirstOrDefault();

            var oddsNodes = betProviderNode.SelectNodes(".//span[@data-odds]");

            var winOddValue = Convert.ToDouble(oddsNodes[0].InnerText.Trim('\n', ' ', '\r'));
            var drawOddValue = Convert.ToDouble(oddsNodes[1].InnerText.Trim('\n', ' ', '\r'));
            var lossOddValue = Convert.ToDouble(oddsNodes[2].InnerText.Trim('\n', ' ', '\r'));

            return new Tuple<double?, double?, double?>(winOddValue,drawOddValue,lossOddValue);
        }

        private Tuple<double?, double?, double?> GetGameOdds(HtmlNode sectionNode, string team1, string roundTag)
        {
            if (roundTag == "0")
            {
                Dictionary<string, Tuple<double, double, double>> quotes =
                    new Dictionary<string, Tuple<double, double, double>>
                    {
                        { "Frankfurt", new Tuple<double, double, double>(2.75, 3.75, 2.37) },
                        { "Hertha BSC", new Tuple<double, double, double>(1.72, 4, 4.5) },
                        { "Bremen", new Tuple<double, double, double>(5.5, 4.5, 1.55) },
                        { "Stuttgart", new Tuple<double, double, double>(2.05, 3.75, 3.4) },
                        { "Schalke 04", new Tuple<double, double, double>(2.87, 3.5, 2.37) },
                        { "Bayern", new Tuple<double, double, double>(1.33, 5.5, 8.5) },
                        { "Hoffenheim", new Tuple<double, double, double>(5, 4.5, 1.6) },
                        { "Köln", new Tuple<double, double, double>(5.25, 4.2, 1.6) },
                        { "Bielefeld", new Tuple<double, double, double>(2.8, 3.3, 2.55) },
                    };

                double winOddValue = 0;
                double drawOddValue = 0;
                double lossOddValue = 0;

                if (quotes.TryGetValue(team1, out var quote))
                {
                    winOddValue = quote.Item1;
                    drawOddValue = quote.Item2;
                    lossOddValue = quote.Item3;
                }

                return new Tuple<double?, double?, double?>(winOddValue, drawOddValue, lossOddValue);
            }

            return GetGameOdds(sectionNode);
        }

        private Tuple<string, string> GetTeams(HtmlNode sectionNode)
        {
            var split2 = sectionNode.InnerText.Split(new []{":"},StringSplitOptions.RemoveEmptyEntries);

            var splitTeams = split2[0].Split(new[] { "-","&#8211;"}, StringSplitOptions.RemoveEmptyEntries);

            if (splitTeams.Length != 2)
            {
                return null;
            }

            return new Tuple<string, string>(splitTeams[0].Trim(' '),splitTeams[1].Trim(' '));
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
                return node;
            }

            return null;
        }

  }
}
