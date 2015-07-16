using System;
using System.Collections;
using System.Collections.Generic;
using BankModel.Entities.Credits;

namespace BankModel.Entities
{
    public static class BankCreditTypes
    {
        public static List<Credit> Credits { get; set; }

        public static void Initialize()
        {
            Credits = new List<Credit>
            {
                new Credit(5, "House", new DateTime(10)),
                new Credit(7, "Car", new DateTime(7)),
                new Credit(4, "Education", new DateTime(5)),
                new Credit(10, "Travelling", new DateTime(1))
            };
        }
    };
}