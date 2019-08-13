using FluentAssertions;
using OddsScraper.Contract;
using OddsScraper;
using Xunit;
using Xunit.Abstractions;
using OddsScraper.Tests.Properties;

namespace WettfreundeScraper.Tests
{
    [Trait("Wettfreunde Manual", "BuLi Test")]
    public class WettfreundeManualBuLiTest
    {
        private readonly ITestOutputHelper _output;
        private readonly IOddsScraper _oddsScraper = new WettfreundeOddsBuLiManual();

        public WettfreundeManualBuLiTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact(DisplayName = "2019 1. Spieltag")]
        public void GetSpieltag2019OneTest()
        {
            // given
            string spieltag = "1";

            // when
            var result = _oddsScraper.GetOdds(null, spieltag);
            _output.WriteLine("{0}", result.Count);

            // then
            result.Should().NotBeNull().And.Subject.Should().HaveCount(9);

        }
    }
}
