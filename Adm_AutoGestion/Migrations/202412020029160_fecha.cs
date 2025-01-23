namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fecha : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HorasExtras", "FechaPago", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HorasExtras", "FechaPago", c => c.DateTime(nullable: false));
        }
    }
}
