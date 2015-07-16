namespace JustBank.Messaging
{
    public static class Message
    {
        public static string OperationSuccessed = "Operation successed.";
        public static string FileLoaded = "File loaded successfully."; 
        public static string WrongSum = "Entered wrong sum.";
        public static string WrongSumForCredit = "Entered wrong sum for credit.";
        public static string WrongId = "No such Id.";
        public static string WrongFields = "Some fields are wrong.";
        public static string WrongField = "field is wrong.";
        public static string NotEnoughBankMoney = "Not enough money on your BankId.";
        public static string NotEnoughCardMoney = "Not enough money on your CardId.";
        public static string EmailIsUsed = "Such email is already used.";
        public static string SumIntervalMismatch = "Sum must be between 0 and 1000000.";
        public static string ClientsNotFound = "No clients found.";
        public static string OperationsNotFound = "No operations found.";
        public static string CreditsNotFound = "No credits found.";
    }
}
