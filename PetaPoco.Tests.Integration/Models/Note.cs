using System;
using Shouldly;

namespace PetaPoco.Tests.Integration.Models
{
    public class Note
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime CreatedOn { get; set; }

        public void ShouldBe(Note other)
        {
            Id.ShouldBe(other.Id);
            Text.ShouldBe(other.Text);
            CreatedOn.ShouldBe(other.CreatedOn);
        }

        public void ShouldNotBe(Note other, bool sameIds)
        {
            if (sameIds)
                Id.ShouldBe(other.Id);
            else
                Id.ShouldNotBe(other.Id);

            Text.ShouldNotBe(other.Text);
            CreatedOn.ShouldNotBe(other.CreatedOn);
        }
    }
}
