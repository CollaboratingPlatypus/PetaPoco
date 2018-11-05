// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;


namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseStoredProcTests: BaseDatabase
    {
        protected BaseStoredProcTests(DBTestProvider provider)
            : base(provider)
        {
            AddPeople(6);
        }

        protected void AddPeople(int peopleToAdd)
        {
            for (var i = 0; i < peopleToAdd; i++)
            {
                var p = new Person
                {
                    Id = Guid.NewGuid(),
                    Name = "Peta" + i,
                    Age = 18 + i,
                    Dob = new DateTime(1980 - (18 + i), 1, 1, 1, 1, 1, DateTimeKind.Utc),
                };
                DB.Insert(p);
            }
        }

        protected abstract Type DataParameterType { get; }

        private IDataParameter GetDataParameter()
        {
            var param = Activator.CreateInstance(DataParameterType) as IDataParameter;
            param.ParameterName = "age";
            param.Value = 20;
            return param;
        }

        [Fact]
        public void FetchProc_NoParam_ShouldReturnAll()
        {
            var results = DB.FetchProc<Person>("SelectPeople");
            results.Count().ShouldBe(6);
        }

        [Fact]
        public void FetchProc_WithParam_ShouldReturnSome()
        {
            var results = DB.FetchProc<Person>("SelectPeopleWithParam", new { age = 20 });
            results.Count().ShouldBe(3);
        }

        [Fact]
        public void FetchProc_WithDbParam_ShouldReturnSome()
        {
            var results = DB.FetchProc<Person>("SelectPeopleWithParam", GetDataParameter());
            results.Count().ShouldBe(3);
        }

        [Fact]
        public void ScalarProc_NoParam_ShouldReturnAll()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeople");
            count.ShouldBe(6);
        }

        [Fact]
        public void ScalarProc_WithParam_ShouldReturnSome()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeopleWithParam", new { age = 20 });
            count.ShouldBe(3);
        }

        [Fact]
        public void ScalarProc_WithDbParam_ShouldReturnSome()
        {
            var count = DB.ExecuteScalarProc<int>("CountPeopleWithParam", GetDataParameter());
            count.ShouldBe(3);
        }

        [Fact]
        public void NonQueryProc_NoParam_ShouldUpdateAll()
        {
            DB.ExecuteNonQueryProc("UpdatePeople");
            DB.Query<Person>("WHERE FullName='Updated'").Count().ShouldBe(6);
        }

        [Fact]
        public void NonQueryProc_WithParam_ShouldUpdateSome()
        {
            var count = DB.ExecuteNonQueryProc("UpdatePeopleWithParam", new { age = 20 });
            DB.Query<Person>("WHERE FullName='Updated'").Count().ShouldBe(3);
        }

        [Fact]
        public void NonQueryProc_WithDbParam_ShouldUpdateSome()
        {
            var count = DB.ExecuteNonQueryProc("UpdatePeopleWithParam", GetDataParameter());
            DB.Query<Person>("WHERE FullName='Updated'").Count().ShouldBe(3);
        }
    }
}
