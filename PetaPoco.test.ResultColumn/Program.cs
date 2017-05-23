using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;
namespace PetaPoco.test.ResultColumn
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "localhost";
            string database = "petapoco_test";//To file for "sql.txt"
            string uid = "tiomer"; //mysql uid
            string pwd = "dingdang"; //mysql pwd
            string connStr = $"Server={server};Port=3306;Database={database};Uid={uid};Pwd={pwd}";
            
            Database db = new Database(connStr, "MySql");
            Logs obj = new Logs
            {
                UserId = 1,
                //IpAddress = "127.0.0.1"  is Error,I don't know if this is a bug.
                IpAddress = "'127.0.0.1'"
            };
            var id = db.Insert(obj);

            //Before fixing the bug.

            var result = db.FirstOrDefault<Logs>("select * from Logs where LoginId=@0", id);         //This is Ok! LoginTime=[datetime]
            Console.WriteLine("sqlString for: select * from Logs where LoginId=@0");
            Console.WriteLine("LoginId:"+ result.LoginId);
            Console.WriteLine("UserId:"+ result.UserId);
            Console.WriteLine("LoginTime:"+ result.LoginTime);
            Console.WriteLine("IpAddress:"+ result.IpAddress);
            Console.WriteLine("-----------------------------------------");
            result = db.FirstOrDefault<Logs>("where LoginId=@0", id);                       // LoginTime=null,is error
            Console.WriteLine("sqlString for: where LoginId=@0");
            Console.WriteLine("LoginId:"+result.LoginId);
            Console.WriteLine("UserId:"+ result.UserId);
            Console.WriteLine("LoginTime:"+ result.LoginTime);
            Console.WriteLine("IpAddress:"+ result.IpAddress);
            Console.WriteLine("-----------------------------------------");
            Console.ReadKey();

            //After repair,The above two expressions are equivalent.

        }

        /// <summary>
        /// This is Table
        /// </summary>
        [TableName("Logs")]
        [PrimaryKey("LoginId", AutoIncrement = true)]
        public class Logs
        {
            [Column]
            public int LoginId { get; set; }
            [Column]
            public int UserId { get; set; }
            [ResultColumn]
            public DateTime? LoginTime { get; set; }
            [Column]
            public string IpAddress { get; set; }
        }
    }
}
