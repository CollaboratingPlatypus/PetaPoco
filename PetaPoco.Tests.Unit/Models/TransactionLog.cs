// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;

namespace PetaPoco.Tests.Unit.Models
{
    [TableName("TransactionLogs")]
    public class TransactionLog
    {
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}