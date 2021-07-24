﻿namespace Tippspiel.Implementation
{
    public class SportsdataConfigInfo : AppInfo<SportsdataConfigInfo>
    {
        // Addition shared info
        public string LeagueShortcut { get; set; }
        public string LeagueSaison { get; set; }
        public int StartSpieltag { get; set; }
        public int EndSpieltag { get; set; }

        public SportsdataConfigInfo()
        {
            LeagueShortcut = "bl1";
            LeagueSaison = "2021";
            StartSpieltag = 1;
            EndSpieltag = 17;
        }
    }
}
