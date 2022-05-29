using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetaPoco.Tests.Unit.Models
{
    public class Parent1
    {
        public virtual int ID { get; set; }
        public virtual string Ignored { get; set; }
    }

    public class Child1 : Parent1
    {
        [Column("Foo")]
        [FooValueConverter]
        public override int ID { get; set; }
        [Ignore]
        public override string Ignored { get; set; }
    }

    // -------

    public class Parent2
    {
        [Column("Foo")]
        [FooValueConverter]
        public virtual int ID { get; set; }
        [Ignore]
        public virtual string Ignore { get; set; }
    }

    public class Child2 : Parent2
    {
        public override int ID { get; set; }
        public override string Ignore { get; set; }
    }

    // ---------

    public class Parent3
    {
        [Column("Foo")]
        [FooValueConverter]
        public virtual int ID { get; set; }
        [Ignore]
        public virtual string Ignore { get; set; }
    }

    public class Child3 : Parent3
    {

    }

    // ---------

    public class Parent4
    {
        [Column("NOTFoo")]
        public virtual int ID { get; set; }
    }

    public class Child4 : Parent4
    {
        [Column("Foo")]
        [FooValueConverter]
        public override int ID { get; set; }
        [Ignore]
        public virtual string Ignore { get; set; }
    }

    public class FooValueConverterAttribute : ValueConverterAttribute
    {
        public override object ConvertFromDb(object value)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToDb(object value)
        {
            throw new NotImplementedException();
        }
    }
}
