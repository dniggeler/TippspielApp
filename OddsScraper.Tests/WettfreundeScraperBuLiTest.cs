using FluentAssertions;
using OddsScraper.Contract;
using OddsScraper;
using Xunit;
using Xunit.Abstractions;
using OddsScraper.Tests.Properties;

namespace WettfreundeScraper.Tests
{
    [Trait("Wettfreunde Scraper", "BuLi Test")]
    public class WettfreundeScraperBuLiTest
    {
        private readonly ITestOutputHelper _output;
        private readonly IOddsScraper _oddsScraper = new WettfreundeOddsBuLiScraper();

        public WettfreundeScraperBuLiTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "2017 14. Spieltag")]
        public void GetSpieltagTest()
        {
            // given
            string spieltag = "14";

            // when
            var result = _oddsScraper.GetOdds(Resources.spieltag14, spieltag);
            _output.WriteLine("{0}", result.Count);

            // then
            result.Should().NotBeNull().And.Subject.Should().HaveCount(9);

        }

        [Fact(DisplayName = "2016 3. Spieltag")]
        public void GetSpieltag201603Test()
        {
            // given
            string spieltag = "3";

            // when
            var result = _oddsScraper.GetOdds(Resources.Spieltag3Html, spieltag);
            _output.WriteLine("{0}", result.Count);

            // then
            result.Should().NotBeNull().And.Subject.Should().HaveCount(9);

        }
    }
}
