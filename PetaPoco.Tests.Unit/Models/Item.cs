// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2020/04/03</date>

using System;
using Shouldly;

namespace PetaPoco.Tests.Unit.Models
{
    [TableName("Item")]
    [PrimaryKey("UserId", "Index")]
    public class Item
    {
        public int UserId { get; set; }

        public int Index { get; set; }

        public int Type { get; set; }
    }
}