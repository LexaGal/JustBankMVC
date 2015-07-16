using System;

namespace BankModel.Entities.Errors
{
    public class ErrorNotifier
    {
        public string Source { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public Tuple<string, string> ActionLink { get; set; }

        public void SetDescription()
        {
            switch (Source)
            {
                case "take":
                    Message = "You cannot take credit.";
                    Description =
                        "You must have Premium type of accout to take/pay credits. To change it, link to";
                    break;
                case "pay":
                    Message = "You cannot pay credit.";
                    Description =
                        "You must have Premium type of accout to take/pay credits. To change it, link to";
                    break;
                case "transfer":
                    Message = "You cannot transfer your money.";
                    Description =
                        "You must have Premium type of accout to transfer your money from BankId to BankId. To change it, link to";
                    break;
                case "updateCredits":
                    Message = "You cannot update credit sum.";
                    Description = 
                        "Client must have Premium type of accout to take credits and a credit to update.";
                    break;
                case "updateBank":
                    Message = "You cannot update bank sum.";
                    Description = "Bank sum is 0.";
                    break;
                case "takeCredit":
                    Message = "You cannot take credit.";
                    Description = "You have already taken a credit of such type.";
                    break;
            }
        }
    }
}
