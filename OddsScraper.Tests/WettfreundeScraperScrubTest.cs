﻿using FluentAssertions;
using HtmlAgilityPack;
using Xunit;
using Xunit.Abstractions;
using OddsScraper.Tests.Properties;
using OddsScraper;

namespace WettfreundeScraper.Tests
{
    [Trait("Wettfreunde Scraper", "Scrub Test")]
    public class WettfreundeScraperScrubTest
    {
        private readonly ITestOutputHelper _output;

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

    }
}
