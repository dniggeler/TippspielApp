namespace Tippspiel.Implementation
{
    public class SportsdataConfigInfo : AppInfo<SportsdataConfigInfo>
    {
        // Addition shared info
        public int LeagueId { get; set; } = 4608;
        public string LeagueShortcut { get; set; } = "bl1";
        public string LeagueSaison { get; set; } = "2023";
        public int StartSpieltag { get; set; } = 1;
        public int EndSpieltag { get; set; } = 17;
    }
}
