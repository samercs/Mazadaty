using OrangeJetpack.Base.Data;
using System.Data.Entity.Migrations;

namespace Mazadaty.Data.Migrations
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
