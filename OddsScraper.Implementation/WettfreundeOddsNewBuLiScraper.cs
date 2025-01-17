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

                    // remove "Wettquote" from team name
                    string spamString = "Wettquoten";
                    if (teams.Item1.Contains(spamString))
                    {
                        model.HomeTeam = model.HomeTeam.Replace(spamString, "").Trim(' ');
                        model.HomeTeamSearch = model.HomeTeam.ToUpper();
                    }

                    if (teams.Item2.Contains(spamString))
                    {
                        model.AwayTeam = model.AwayTeam.Replace(spamString, "").Trim(' ');
                        model.AwayTeamSearch = model.AwayTeam.ToUpper();
                    }

                    if (teams.Item1.Contains("BVB"))
                    {
                        model.HomeTeam = "Dortmund";
                        model.HomeTeamSearch = "Dortmund".ToUpper();
                    }

                    if (teams.Item2.Contains("BVB"))
                    {
                        model.AwayTeam = "Dortmund";
                        model.AwayTeamSearch = "Dortmund".ToUpper();
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
            if (roundTag == "35")
            {
                string teamClean = team1.Replace("Wettquoten","").Trim(' ');
                Dictionary<string, Tuple<double, double, double>> quotes =
                    new Dictionary<string, Tuple<double, double, double>>
                    {
                        { "Stuttgart", new Tuple<double, double, double>(1.69, 3.60, 5.00) },
                        { "Leverkusen", new Tuple<double, double, double>(1.19, 7, 11) },
                        { "Bayern", new Tuple<double, double, double>(1.07, 9, 26) },
                        { "Frankfurt", new Tuple<double, double, double>(1.54, 4, 6) },
                        { "Kiel", new Tuple<double, double, double>(5.5, 4, 1.58) },
                        { "Bochum", new Tuple<double, double, double>(3.1, 3.5, 2.25) },
                        { "Gladbach", new Tuple<double, double, double>(3.4, 3.6, 2.05) },
                        { "Wolfsburg", new Tuple<double, double, double>(2.1, 3.3, 3.6) },
                        { "Hoffenheim", new Tuple<double, double, double>(2.7, 3.25, 2.62) },
                    };

                double winOddValue = 0;
                double drawOddValue = 0;
                double lossOddValue = 0;

                if (quotes.TryGetValue(teamClean, out var quote))
                {
                    winOddValue = quote.Item1;
                    drawOddValue = quote.Item2;
                    lossOddValue = quote.Item3;
                }
                else
                {
                    return GetGameOdds(sectionNode);
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
