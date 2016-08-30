using FussballTippApp.Models;
using Mvc.Mailer;

namespace TippSpiel.Mailers
{ 
    public interface ITippMailer
    {
        MvcMailMessage EmailDailyWinner(string email, DailyWinnerInfoModel model, int spieltag);
	}
}