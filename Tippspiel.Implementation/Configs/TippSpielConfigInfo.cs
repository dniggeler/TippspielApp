namespace Tippspiel.Implementation
{
    public class TippspielConfigInfo : AppInfo<TippspielConfigInfo>
    {
        public string EmailFrom { get; set; }
        public string EmailProviderUser { get; set; }
        public string EmailApiKey { get; set; }
        public string EmailHost { get; set; }

        public TippspielConfigInfo()
        {
            EmailFrom = @"dniggeler@bhfs.ch";
            EmailProviderUser = @"config@bhfs.ch";
            EmailApiKey = @"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";
            EmailHost = @"mail.netzone.ch";
        }
    }
}