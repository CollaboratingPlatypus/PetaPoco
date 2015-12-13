// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;

namespace PetaPoco.Tests.Unit.Models
{
    public class Note
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }
    }
}