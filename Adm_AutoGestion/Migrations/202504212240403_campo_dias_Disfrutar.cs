namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campo_dias_Disfrutar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PeriodoVacacionesEmpleadoes", "DiasporDisfrutar", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PeriodoVacacionesEmpleadoes", "DiasporDisfrutar");
        }
    }
}
