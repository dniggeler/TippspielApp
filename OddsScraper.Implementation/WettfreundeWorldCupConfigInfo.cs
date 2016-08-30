﻿using OddsScraper.Contract;
using Tippspiel.Helpers;

namespace OddsScraper
{
    public class WettfreundeWorldCupConfigInfo : AppInfo<WettfreundeWorldCupConfigInfo>,IOddsScraperConfig
    {
        public string BaseUrl { get; set; }
        public string OddsLink { get; set; }

        public WettfreundeWorldCupConfigInfo()
        {
            BaseUrl = @"http://www.wettfreunde.net";
            OddsLink = @"/wm-2018-spielplan";
        }
    }
}