﻿using FluentAssertions;
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

        [Fact(DisplayName = "26. Spieltag")]
        public void GetSpieltagTest()
        {
            // given
            string spieltag = "26";

            // when
            var result = _oddsScraper.GetOdds(Resources.Spieltag26Html, spieltag);
            _output.WriteLine("{0}",result.Count);

            // then
            result.Should().NotBeNull().And.Subject.Should().HaveCount(9);

        }
    }
}
