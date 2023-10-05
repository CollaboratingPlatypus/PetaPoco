using System;

namespace PetaPoco.Tests.Integration.Models.SqlServer
{
    [TableName("store.People")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class StorePerson
    {
        [Column]
        public Guid Id { get; set; }

        [Column(Name = "FullName")]
        public string Name { get; set; }

        [Column]
        public long Age { get; set; }
    }
}
