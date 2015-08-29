namespace Mzayad.Data.Migrations
{
    using OrangeJetpack.Base.Core.Formatting;
    using System;
    using System.Data.Entity.Migrations;

    public partial class IslamicCalendar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IslamicCalendars",
                c => new
                    {
                        IslamicCalendarId = c.Int(nullable: false, identity: true),
                        HijriYear = c.Int(nullable: false),
                        NewYear = c.DateTime(nullable: false),
                        EidFetrFrom = c.DateTime(nullable: false),
                        EidFetrTo = c.DateTime(nullable: false),
                        EidAdhaFrom = c.DateTime(nullable: false),
                        EidAdhaTo = c.DateTime(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.IslamicCalendarId);

            Mzayad.Models.IslamicCalendar islamicCalendar;

            var insertIslamicCalendar = "insert into IslamicCalendars(HijriYear,NewYear,EidFetrFrom,EidFetrTo,EidAdhaFrom,EidAdhaTo,CreatedUtc)"
                            + "values ({HijriYear}, '{NewYear}','{EidFetrFrom}','{EidFetrTo}','{EidAdhaFrom}','{EidAdhaTo}','{CreatedUtc}')";

            for (var year = 1436; year < 1536; year++)
            {
                islamicCalendar = Mzayad.Models.IslamicCalendar.CreateInstance(year);
                islamicCalendar.CreatedUtc = DateTime.UtcNow;
                Sql(StringFormatter.ObjectFormat(insertIslamicCalendar, islamicCalendar));
            }
        }
        
        public override void Down()
        {
            DropTable("dbo.IslamicCalendars");
        }
    }
}
