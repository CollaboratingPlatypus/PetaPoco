using System;

namespace PetaPoco.Tests.Integration.Documentation.Pocos
{
    public class Note
    {
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }
    }
}