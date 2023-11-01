using System;
using System.Linq;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Tests.Integration.Models;
using PetaPoco.Tests.Integration.Providers;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Oracle
{
    public abstract partial class OracleUpdateTests : UpdateTests
    {
        protected OracleUpdateTests(TestProvider provider)
            : base(provider)
        {
        }

        [Collection("Oracle.Delimited")]
        public class Delimited : OracleUpdateTests
        {
            public Delimited()
                : base(new OracleDelimitedTestProvider())
            {
            }
        }

        [Collection("Oracle.Ordinary")]
        public class Ordinary : OracleUpdateTests
        {
            public Ordinary()
                : base(new OracleOrdinaryTestProvider())
            {
            }

            [Fact]
            [Trait("Issue", "#667")]
            public override void Update_GivenTablePrimaryKeyNameAndDynamicType_ShouldNotThrow()
            {
                var pd = PocoData.ForType(typeof(Note), DB.DefaultMapper);
                var tblNote = DB.Provider.EscapeTableName(pd.TableInfo.TableName);

                DB.Insert(note);
                var entity = DB.Fetch<dynamic>($"SELECT * FROM {tblNote}").First();

#if !NETCOREAPP
                Should.NotThrow(() => DB.Update("NOTE", "ID", (object)entity));
#else
                Should.NotThrow(() => DB.Update("NOTE", "ID", entity));
#endif
            }

            [Fact]
            public override void Update_GivenTablePrimaryKeyNameAndDynamicType_ShouldUpdate()
            {
                var pd = PocoData.ForType(typeof(Note), DB.DefaultMapper);
                var tblNote = DB.Provider.EscapeTableName(pd.TableInfo.TableName);

                DB.Insert(note);
                var entity = DB.Fetch<dynamic>($"SELECT * FROM {tblNote}").First();

                entity.TEXT += " was updated";

#if !NETCOREAPP
                DB.Update("NOTE", "ID", (object)entity);
#else
                DB.Update("NOTE", "ID", entity);
#endif

                var entity2 = DB.Fetch<dynamic>($"SELECT * FROM {tblNote}").First();
                ((string)entity2.TEXT).ShouldContain("updated");
            }

            [Fact]
            [Trait("Issue", "#667")]
            public override async Task UpdateAsync_GivenTablePrimaryKeyNameAndDynamicType_ShouldNotThrow()
            {
                var pd = PocoData.ForType(typeof(Note), DB.DefaultMapper);
                var tblNote = DB.Provider.EscapeTableName(pd.TableInfo.TableName);

                await DB.InsertAsync(note);
                var entity = (await DB.FetchAsync<dynamic>($"SELECT * FROM {tblNote}")).First();

                // https://docs.shouldly.org/documentation/exceptions/throw#shouldthrowfuncoftask
#if !NETCOREAPP
                Should.NotThrow(async () => await Task.Run(() => DB.UpdateAsync("NOTE", "ID", (object)entity)));
#else
                await Should.NotThrowAsync(() => DB.UpdateAsync("NOTE", "ID", entity));
#endif
            }

            [Fact]
            public override async Task UpdateAsync_GivenTablePrimaryKeyNameAndDynamicType_ShouldUpdate()
            {
                var pd = PocoData.ForType(typeof(Note), DB.DefaultMapper);
                var tblNote = DB.Provider.EscapeTableName(pd.TableInfo.TableName);

                await DB.InsertAsync(note);
                var entity = (await DB.FetchAsync<dynamic>($"SELECT * FROM {tblNote}")).First();

                entity.TEXT += " was updated";

#if !NETCOREAPP
                await DB.UpdateAsync("NOTE", "ID", (object)entity);
#else
                await DB.UpdateAsync("NOTE", "ID", entity);
#endif

                var entity2 = (await DB.FetchAsync<dynamic>($"SELECT * FROM {tblNote}")).First();
                ((string)entity2.TEXT).ShouldContain("updated");
            }
        }
    }
}
