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

                HtmlNode spieltagNode = GetSpieltagHtml(doc, spieltag);
                if (spieltagNode == null) throw new ApplicationException("Spieltag nicht gefunden.");

                HtmlNode oddsTableNode = spieltagNode
                    .ParentNode
                    .ParentNode
                    .ParentNode
                    .NextSibling;

                // Odds
                {
                    var xpath = "./td";
                    bool insideSpecialRule = false;
                    HtmlNode trNode = oddsTableNode.NextSibling.NextSibling.NextSibling;

                    for (int ii = 3; ii < 12; ii++)
                    {
                        Debug.WriteLine(trNode.InnerHtml);
                        var model = new OddsInfoModel();

                        int kk = 0;
                        foreach (var tdNode in trNode.SelectNodes(xpath))
                        {
                            switch (kk)
                            {
                                case 1:
                                    model.HomeTeam = tdNode.InnerText;
                                    model.HomeTeamSearch = model.HomeTeam.ToUpper();
                                    break;
                                case 2:
                                    model.AwayTeam = tdNode.InnerText;
                                    model.AwayTeamSearch = model.AwayTeam.ToUpper();
                                    break;
                                case 4:
                                {
                                    var oddsNode = tdNode.SelectSingleNode(".//strong");
                                    model.WinOdds = ScrubHelper.ConvertToDouble(oddsNode);
                                }
                                    break;
                                case 5:
                                {
                                    var oddsNode = tdNode.SelectSingleNode(".//strong");
                                    model.DrawOdds = ScrubHelper.ConvertToDouble(oddsNode);
                                }
                                    break;
                                case 6:
                                {
                                    var oddsNode = tdNode.SelectSingleNode(".//strong");
                                    model.LossOdds = ScrubHelper.ConvertToDouble(oddsNode);
                                }
                                    break;
                            }

                            kk++;
                        }

                        var potentialNextNode = trNode.NextSibling.NextSibling;

                        if (potentialNextNode != null && potentialNextNode.Name != "tr")
                        {
                            insideSpecialRule = true;
                            trNode = trNode.SelectSingleNode("following-sibling::tr[1]");
                            xpath = "preceding-sibling::td";

                        }
                        else if (insideSpecialRule)
                        {
                            xpath = "./td";
                            insideSpecialRule = false;
                        }
                        else
                        {
                            trNode = trNode.NextSibling.NextSibling;
                            xpath = "./td";
                            insideSpecialRule = false;
                        }

                        oddsList.Add(model);
                    }
                }
            }

            return oddsList;
        }

        private static HtmlNode GetSpieltagHtml(HtmlDocument doc, int spieltag)
        {
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//h2"))
            {
                string cleanStr = node.InnerText.Trim(new char[] { '\n', ' ' });

                if (cleanStr.ToUpper().Contains($"{spieltag}. SPIELTAG"))
                {
                    return node;
                }
            }

            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//table/tbody/tr/td/span/strong"))
            {
                string cleanStr = node.InnerText.Trim(new char[] { '\n', ' ' });

                if (cleanStr.ToUpper().Equals("SPIELTAG " + spieltag))
                {
                    return node;
                }
            }

            // fall back, none found so far
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//table[3]/tbody/tr/td/span/strong"))
            {
                string cleanStr = node.InnerText.Trim(new char[] { '\n', ' ' });

                if (cleanStr.ToUpper().Equals("SPIELTAG " + spieltag))
                {
                    return node;
                }
            }

            return null;
        }
    }
}
