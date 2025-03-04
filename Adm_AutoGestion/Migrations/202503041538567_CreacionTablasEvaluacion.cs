namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreacionTablasEvaluacion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EvaluacionCriterios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EvaluacionDetalles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EvaluacionId = c.Int(nullable: false),
                        IndicadorId = c.Int(nullable: false),
                        BaseNumerador = c.Int(),
                        BaseDenominador = c.Int(),
                        IndicadorNumerador = c.Int(),
                        IndicadorDenominador = c.Int(),
                        Porcentaje = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EvaluacionEncabezadoes", t => t.EvaluacionId, cascadeDelete: true)
                .ForeignKey("dbo.EvaluacionIndicadors", t => t.IndicadorId, cascadeDelete: true)
                .Index(t => t.EvaluacionId)
                .Index(t => t.IndicadorId);
            
            CreateTable(
                "dbo.EvaluacionEncabezadoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpleadoId = c.Int(nullable: false),
                        EvaluadorId = c.Int(nullable: false),
                        PeriodoEvaluacion = c.String(),
                        FechaRegistro = c.DateTime(nullable: false),
                        Retroalimentacion = c.String(),
                        PlandeMejora = c.String(),
                        PuntajeFinal = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EvaluacionIndicadors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        Evidencia = c.String(),
                        UnidadOrganizativa = c.String(),
                        CriterioId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EvaluacionCriterios", t => t.CriterioId, cascadeDelete: true)
                .Index(t => t.CriterioId);
            
            CreateTable(
                "dbo.EvaluacionPeriodoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Periodo = c.String(),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EvaluacionSubAreas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        UnidadOrganizativa = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EvaluacionDetalles", "IndicadorId", "dbo.EvaluacionIndicadors");
            DropForeignKey("dbo.EvaluacionIndicadors", "CriterioId", "dbo.EvaluacionCriterios");
            DropForeignKey("dbo.EvaluacionDetalles", "EvaluacionId", "dbo.EvaluacionEncabezadoes");
            DropIndex("dbo.EvaluacionIndicadors", new[] { "CriterioId" });
            DropIndex("dbo.EvaluacionDetalles", new[] { "IndicadorId" });
            DropIndex("dbo.EvaluacionDetalles", new[] { "EvaluacionId" });
            DropTable("dbo.EvaluacionSubAreas");
            DropTable("dbo.EvaluacionPeriodoes");
            DropTable("dbo.EvaluacionIndicadors");
            DropTable("dbo.EvaluacionEncabezadoes");
            DropTable("dbo.EvaluacionDetalles");
            DropTable("dbo.EvaluacionCriterios");
        }
    }
}
