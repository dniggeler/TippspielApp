using FluentAssertions;
using OddsScraper.Contract;
using OddsScraper;
using Xunit;
using Xunit.Abstractions;
using OddsScraper.Tests.Properties;

namespace WettfreundeScraper.Tests
{
    [Trait("Wettfreunde Scraper", "New BuLi Test")]
    public class WettfreundeNewScraperBuLiTest
    {
        private readonly ITestOutputHelper _output;
        private readonly IOddsScraper _oddsScraper = new WettfreundeOddsNewBuLiScraper();

        public WettfreundeNewScraperBuLiTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "2019")]
        public void GetSpieltagTest()
        {
            // given
            string spieltag = "2";

            // when
            var result = _oddsScraper.GetOdds(Resources.spieltag2019_2, spieltag);
            _output.WriteLine(result.Count.ToString());

            // then
            result.Should().NotBeNull().And.Subject.Should().HaveCount(9);
        }

    }
}
