namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campocartacesantias : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SolicitudCesantias", "CartaFondo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SolicitudCesantias", "CartaFondo");
        }
    }
}
