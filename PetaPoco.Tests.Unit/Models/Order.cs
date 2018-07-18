﻿// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;
using Shouldly;

namespace PetaPoco.Tests.Unit.Models
{
    [ExplicitColumns]
    [TableName("Orders")]
    [PrimaryKey("Id")]
    public class Order
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public Guid PersonId { get; set; }

        [Column]
        public string PoNumber { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [Column]
        public string CreatedBy { get; set; }

        [Column("OrderStatus")]
        public OrderStatus Status { get; set; }

        public void ShouldBe(Order other)
        {
            Id.ShouldBe(other.Id);
            PersonId.ShouldBe(other.PersonId);
            PoNumber.ShouldBe(other.PoNumber);
            Status.ShouldBe(other.Status);
            CreatedOn.ShouldBe(other.CreatedOn);
            CreatedBy.ShouldBe(other.CreatedBy);
        }

        public void ShouldNotBe(Order other, bool sameIds)
        {
            if (sameIds)
            {
                Id.ShouldBe(other.Id);
                PersonId.ShouldBe(other.PersonId);
            }
            else
            {
                Id.ShouldNotBe(other.Id);
                PersonId.ShouldNotBe(other.PersonId);
            }

            PoNumber.ShouldNotBe(other.PoNumber);
            Status.ShouldNotBe(other.Status);
            CreatedOn.ShouldNotBe(other.CreatedOn);
            CreatedBy.ShouldNotBe(other.CreatedBy);
        }
    }
}