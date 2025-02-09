namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fondocesantiassolicitud : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SolicitudCesantias", "FondoCesantiasId", c => c.Int());
            CreateIndex("dbo.SolicitudCesantias", "FondoCesantiasId");
            AddForeignKey("dbo.SolicitudCesantias", "FondoCesantiasId", "dbo.FondoCesantias", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SolicitudCesantias", "FondoCesantiasId", "dbo.FondoCesantias");
            DropIndex("dbo.SolicitudCesantias", new[] { "FondoCesantiasId" });
            DropColumn("dbo.SolicitudCesantias", "FondoCesantiasId");
        }
    }
}
