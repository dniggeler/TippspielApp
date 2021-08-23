namespace Tippspiel.Implementation
{
    public class TippspielConfigInfo : AppInfo<TippspielConfigInfo>
    {
        public string EmailFrom { get; set; }
        public string EmailProviderUser { get; set; }

        public TippspielConfigInfo()
        {
            EmailFrom = @"dniggeler@bhfs.ch";
            EmailProviderUser = @"config@bhfs.ch";
        }
    }
}