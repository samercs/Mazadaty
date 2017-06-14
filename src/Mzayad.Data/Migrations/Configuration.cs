using OrangeJetpack.Base.Data;
using System.Data.Entity.Migrations;

namespace Mzayad.Data.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new MigrationSqlGenerator());
        }

        protected override void Seed(DataContext context)
        {
        }
    }
}
