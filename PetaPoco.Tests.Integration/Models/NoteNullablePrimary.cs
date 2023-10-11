using System;
using Shouldly;

namespace PetaPoco.Tests.Integration.Models
{
    [TableName("Note")]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class NoteNullablePrimary
    {
        public int? Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }

        public void ShouldBe(NoteNullablePrimary other)
        {
            Id.ShouldBe(other.Id);
            CreatedOn.ShouldBe(other.CreatedOn);
            Text.ShouldBe(other.Text);
        }

        public void ShouldNotBe(NoteNullablePrimary other, bool sameIds)
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
