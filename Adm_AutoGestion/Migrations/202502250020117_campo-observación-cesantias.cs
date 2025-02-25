namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoobservacióncesantias : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SolicitudCesantias", "Observacion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SolicitudCesantias", "Observacion");
        }
    }
}
