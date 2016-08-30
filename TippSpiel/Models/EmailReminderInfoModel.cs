using System;
using System.Collections.Generic;

namespace FussballTippApp.Models
{
    public class EmailReminderInfoModel
    {
        public Dictionary<string, bool?> EmailReminderDict { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public DateTime KickoffTime { get; set; }

        public EmailReminderInfoModel()
        {
            EmailReminderDict = new Dictionary<string, bool?>();
        }
    }
}
