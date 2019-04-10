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