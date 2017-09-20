namespace Tippspiel.Implementation
{
    public class WettfreundeConfigInfo : AppInfo<WettfreundeConfigInfo>
    {
        public string BaseURL { get; set; }
        public string OddsLink { get; set; }

        public WettfreundeConfigInfo()
        {
            BaseURL = @"https://www.wettfreunde.net";
            OddsLink = @"/bundesliga-spielplan/";
        }
    }
}