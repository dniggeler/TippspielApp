namespace Tippspiel.Implementation
{
    public class SportsdataConfigInfo : AppInfo<SportsdataConfigInfo>
    {
        // Addition shared info
        public int LeagueId { get; set; } = 4821;
        public string LeagueShortcut { get; set; } = "bl1";
        public string LeagueSaison { get; set; } = "2025";
        public int StartSpieltag { get; set; } = 18;
        public int EndSpieltag { get; set; } = 34;
    }
}
