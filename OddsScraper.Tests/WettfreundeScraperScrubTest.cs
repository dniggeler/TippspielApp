using System.Text.RegularExpressions;
using FluentAssertions;
using HtmlAgilityPack;
using OddsScraper.Contract;
using OddsScraper.Tests.Properties;
using Xunit;
using Xunit.Abstractions;

namespace OddsScraper.Tests
{
    [Trait("Wettfreunde Scraper", "Scrub Test")]
    public class WettfreundeScraperScrubTest
    {
        private readonly ITestOutputHelper _output;
        private readonly IOddsScraper _oddsScraper = new WettfreundeOddsBuLiScraper();

        public WettfreundeScraperScrubTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "Remove Unused Parts")]
        public void ScrupTest()
        {
            // given
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(Resources.Spieltag26Html);
            
            // when
            ScrubHelper.ScrubHtml(doc);
            var nodes = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object");

            // then
            nodes.Should().BeNull();
        }

        [Fact(DisplayName = "Fix Open Tags")]
        public void FixOpenTagTest()
        {
            // given
            var htmlSrc = Resources.Spieltag23Html;
            var regex = new Regex(@"</tr\r\n");

            var match = regex.Match(htmlSrc);

            // when
            var result = regex.Replace(htmlSrc, "</tr>\r\n");

            // then
            result.Should().NotBeNullOrEmpty();
            result.Contains(@"<tr\r\n").Should().BeFalse();
            match.Should().NotBeNull();
        }

    }
}
