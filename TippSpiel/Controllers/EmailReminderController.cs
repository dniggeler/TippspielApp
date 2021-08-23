using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using FussballTippApp.Models;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using Tippspiel.Contracts;
using Tippspiel.Implementation;
using Tippspiel.Contracts.Models;

namespace FussballTippApp.Controllers
{
    public class EmailReminderController : Controller
    {
        readonly log4net.ILog _log;

        private IFussballDataRepository _matchDataRepository;

        public EmailReminderController(IFussballDataRepository repository, log4net.ILog logger)
        {
            _matchDataRepository = repository;
            _log = logger;
        }

        public ActionResult Index()
        {
            var model = GetEmailReminders();

            return View(model);

        }

        private EmailReminderInfoModel GetEmailReminders()
        {
            using (var ctxt = new UsersContext())
            {
                var reminderModel = new EmailReminderInfoModel();

                var nextMatch = _matchDataRepository.GetNextMatch();

                reminderModel.KickoffTime = nextMatch.KickoffTime;
                reminderModel.HomeTeam = nextMatch.HomeTeam;
                reminderModel.AwayTeam = nextMatch.AwayTeam;

                using (var tippContext = new TippSpielContext())
                {
                    var tippList = (from t in tippContext.TippMatchList
                                    where t.MatchId == nextMatch.MatchId
                                    select t);

                    foreach (var user in ctxt.UserProfiles)
                    {
                        var tippObj = (from userTipp in tippList
                                       where userTipp.User == user.UserName
                                       select userTipp)
                                      .FirstOrDefault();

                        reminderModel.EmailReminderDict.Add(user.UserName, IsSendEmailReminder(user, nextMatch, tippObj));
                    }
                }

                return reminderModel;

            }
        }

        private static bool IsSendEmailReminder(UserProfile user, MatchDataModel matchObj, TippMatchModel tippObj)
        {
            if (user.HasEmailReminder == false)
            {
                return false;
            }

            if ((tippObj == null || tippObj.MyTip.HasValue == false) &&
                (matchObj.KickoffTime > DateTime.Now) &&
                (DateTime.Now > matchObj.KickoffTime.AddDays(-1)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Send(string user)
        {
            using (var userContext = new UsersContext())
            {
                var userObj = userContext.UserProfiles.FirstOrDefault(u => u.UserName == user);

                if (userObj != null && userObj.HasEmailReminder == true)
                {
                    try
                    {
                        SendEmailBySendGrid( userObj.UserName, userObj.Email);
                    }
                    catch (Exception ex)
                    {
                        _log.ErrorFormat("Exception while email sent: " + ex.Message);
                    }
                }
            }

            return RedirectToAction("Index");
        }

        public JsonResult CheckAndSend()
        {
            _log.Debug("Begin CheckAndSend()");

            var model = GetEmailReminders();

            using (var ctxt = new UsersContext())
            {
                foreach (var kp in model.EmailReminderDict.Where(p => (p.Value == true)))
                {
                    var userObj = ctxt.UserProfiles.FirstOrDefault(u => u.UserName == kp.Key);
                    if (userObj != null)
                    {
                        SendEmailBySendGrid(kp.Key, userObj.Email);
                    }
                }

                var jsonResponse = new
                {
                    Success = true,
                    Receivers = model.EmailReminderDict.Where(p => (p.Value == true)).Select(p => p.Key).ToArray()
                };

                _log.Debug("End CheckAndSend()");

                return Json(jsonResponse, JsonRequestBehavior.AllowGet);
            }
        }

        private static bool IsSendEmailReminder(UserProfile user, MatchInfoModel matchObj, TippMatchModel tippObj)
        {
            if (user.HasEmailReminder == false)
            {
                return false;
            }

            if ((tippObj == null || tippObj.MyTip.HasValue == false) &&
                (matchObj.KickoffTimeUtc > DateTime.UtcNow) &&
                (DateTime.UtcNow > matchObj.KickoffTimeUtc.AddDays(-1)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SendEmailBySendGrid(string username, string email)
        {
            var client = new SendGridClient(ConfigurationManager.AppSettings["EmailApiKey"]);
            var from = new EmailAddress(TippspielConfigInfo.Current.EmailFrom, "Dieter Niggeler");
            var subject = "Buli-Tippspiel: Reminder";
            var to = new EmailAddress(email, email);
            var plainTextContent = "Hallo " + username + "," + Environment.NewLine + Environment.NewLine +
                                   @"Du hast für ein oder mehrere Spiele der aktuellen Buli-Runde, die in Kürze beginnen, noch Tipps offen." +
                                   Environment.NewLine + Environment.NewLine + "Beste Grüsse, Dieter";

            var htmlContent = "";
            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            Response response = client.SendEmailAsync(msg).Result;

            if (response.IsSuccessStatusCode)
            {
                _log.DebugFormat($"Reminder email sent to {email}");
            }
            else
            {
                _log.WarnFormat($"Sending email to {email} failed: {response.StatusCode}");
            }
        }
    }
}
