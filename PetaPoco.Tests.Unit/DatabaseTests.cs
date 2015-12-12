// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/12</date>

using System;
using PetaPoco.Tests.Unit.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit
{
    public class DatabaseTests
    {
        private IDatabase DB { get; set; }

        public DatabaseTests()
        {
            DB = new Database("mssql");
        }

        [Fact]
        public void Update_GivenInvalidArguments_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person(), 1));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person(), 1));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", null, 1));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person(), 1, null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person(), 1, null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", null, 1, null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", (Person) null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, "primaryKeyName", new Person(), null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", null, new Person(), null));
            Should.Throw<ArgumentNullException>(() => DB.Update("tableName", "primaryKeyName", null, null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, (object) null));

            Should.Throw<ArgumentNullException>(() => DB.Update(null, (object) null, null));

            Should.Throw<ArgumentNullException>(() => DB.Update<Person>((string) null));

            Should.Throw<ArgumentNullException>(() => DB.Update<Person>((Sql) null));
        }

        [Fact]
        public void Insert_GivenInvalidArguments_ShouldThrow()
        {
            Should.Throw<ArgumentNullException>(() => DB.Insert(null));
            Should.Throw<ArgumentNullException>(() => DB.Insert(null, "SomeColumn", new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Insert("SomeTable", null, new Person()));
            Should.Throw<ArgumentNullException>(() => DB.Insert("SomeTable", "SomeColumn", null));
        }
    }
}