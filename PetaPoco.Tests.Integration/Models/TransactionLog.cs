using System;

namespace PetaPoco.Tests.Integration.Models
{
    [TableName("TransactionLogs")]
    public class TransactionLog
    {
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
