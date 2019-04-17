using System;
using System.Data;
using System.Data.SqlClient;

namespace PetaPoco.Tests.Integration.Documentation.Pocos
{
    public class Note
    {
        public int Id { get; set; }

        [DateTime2ConverterAttribute]
        public DateTime CreatedOn { get; set; }

        public string Text { get; set; }
    }

    public class DateTime2ConverterAttribute : ValueConverterAttribute
    {
        public override object ConvertToDb(object value) =>
            new SqlParameter
            {
                DbType = DbType.DateTime2,
                Value = value
            };

        public override object ConvertFromDb(object value) => value;
    } 
}