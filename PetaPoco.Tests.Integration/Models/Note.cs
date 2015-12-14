// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/14</date>

using System;
using Shouldly;

namespace PetaPoco.Tests.Integration.Models
{
    public class Note
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }

        public void ShouldBe(Note other)
        {
            Id.ShouldBe(other.Id);
            CreatedOn.ShouldBe(other.CreatedOn);
            Text.ShouldBe(other.Text);
        }

        public void ShouldNotBe(Note other, bool sameIds)
        {
            if (sameIds)
                Id.ShouldBe(other.Id);
            else
                Id.ShouldNotBe(other.Id);

            CreatedOn.ShouldNotBe(other.CreatedOn);
            Text.ShouldNotBe(other.Text);
        }
    }
}