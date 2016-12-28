namespace Tippspiel.Helpers
{
    public class WettfreundeConfigInfo : AppInfo<WettfreundeConfigInfo>
    {
        public string BaseURL { get; set; }
        public string OddsLink { get; set; }

        public WettfreundeConfigInfo()
        {
            BaseURL = @"http://www.wettfreunde.net";
            OddsLink = @"/bundesliga-spielplan/";
        }
    }
}