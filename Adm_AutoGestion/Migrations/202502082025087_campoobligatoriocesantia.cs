namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoobligatoriocesantia : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SolicitudCesantias", "FondoCesantiasId", "dbo.FondoCesantias");
            DropIndex("dbo.SolicitudCesantias", new[] { "FondoCesantiasId" });
            AlterColumn("dbo.SolicitudCesantias", "FondoCesantiasId", c => c.Int(nullable: false));
            CreateIndex("dbo.SolicitudCesantias", "FondoCesantiasId");
            AddForeignKey("dbo.SolicitudCesantias", "FondoCesantiasId", "dbo.FondoCesantias", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SolicitudCesantias", "FondoCesantiasId", "dbo.FondoCesantias");
            DropIndex("dbo.SolicitudCesantias", new[] { "FondoCesantiasId" });
            AlterColumn("dbo.SolicitudCesantias", "FondoCesantiasId", c => c.Int());
            CreateIndex("dbo.SolicitudCesantias", "FondoCesantiasId");
            AddForeignKey("dbo.SolicitudCesantias", "FondoCesantiasId", "dbo.FondoCesantias", "Id");
        }
    }
}
