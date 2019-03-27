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