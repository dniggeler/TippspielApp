namespace Tippspiel.Helpers
{
    public class TippspielConfigInfo : AppInfo<TippspielConfigInfo>
    {
        public string EmailFrom { get; set; }
        public string EmailProviderUser { get; set; }
        public string EmailProviderPwd { get; set; }
        public string EmailHost { get; set; }

        public TippspielConfigInfo()
        {
            EmailFrom = @"dniggeler@bhfs.ch";
            EmailProviderUser = @"config@bhfs.ch";
            EmailProviderPwd = @"Hilbert1_";
            EmailHost = @"mail.netzone.ch";
        }
    }
}