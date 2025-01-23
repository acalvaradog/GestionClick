namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjustesCapacitacionesCesantias : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Certificados",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpleadoId = c.Int(nullable: false),
                        Titulo = c.String(),
                        IdCursoNormativo = c.Int(nullable: false),
                        Estado = c.String(),
                        Archivo = c.String(),
                        Observacion = c.String(),
                        FechaCaducidad = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CursosNormativas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Titulo = c.String(),
                        Estado = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DestinoCesantias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SoporteDestinoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        DestinoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DestinoCesantias", t => t.DestinoId, cascadeDelete: true)
                .Index(t => t.DestinoId);
            
            CreateTable(
                "dbo.Dosimetrias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpleadoId = c.Int(nullable: false),
                        Anio = c.Int(nullable: false),
                        Mes = c.Int(nullable: false),
                        ValorHp10 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ValorHp3 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SedeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Empleadoes", t => t.EmpleadoId, cascadeDelete: true)
                .ForeignKey("dbo.Sedes", t => t.SedeId, cascadeDelete: true)
                .Index(t => t.EmpleadoId)
                .Index(t => t.SedeId);
            
            CreateTable(
                "dbo.Empresas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EstadoCesantias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LogSolicitudCesantias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SolicitudCesantiaId = c.Int(nullable: false),
                        Usuario = c.String(),
                        Accion = c.String(),
                        FechaHora = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SolicitudCesantias", t => t.SolicitudCesantiaId, cascadeDelete: true)
                .Index(t => t.SolicitudCesantiaId);
            
            CreateTable(
                "dbo.SolicitudCesantias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmpleadoId = c.Int(nullable: false),
                        FechaRegistro = c.DateTime(nullable: false),
                        ValorRetiro = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DestinoId = c.Int(nullable: false),
                        EstadoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DestinoCesantias", t => t.DestinoId, cascadeDelete: true)
                .ForeignKey("dbo.Empleadoes", t => t.EmpleadoId, cascadeDelete: true)
                .ForeignKey("dbo.EstadoCesantias", t => t.EstadoId, cascadeDelete: true)
                .Index(t => t.EmpleadoId)
                .Index(t => t.DestinoId)
                .Index(t => t.EstadoId);
            
            CreateTable(
                "dbo.SoporteCesantias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SolicitudCesantiaId = c.Int(nullable: false),
                        NombreSoporte = c.String(),
                        UrlSoporte = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SolicitudCesantias", t => t.SolicitudCesantiaId, cascadeDelete: true)
                .Index(t => t.SolicitudCesantiaId);
            
            CreateTable(
                "dbo.LugaresCapacitacions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NoticiaImagens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImagenUrl = c.String(),
                        NoticiaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Noticias", t => t.NoticiaId, cascadeDelete: true)
                .Index(t => t.NoticiaId);
            
            AddColumn("dbo.Capacitacions", "Ciudad", c => c.String());
            AddColumn("dbo.Capacitacions", "Lugar", c => c.String());
            AddColumn("dbo.Capacitacions", "EvaluacionConocimiento", c => c.String());
            AddColumn("dbo.Capacitacions", "Mes", c => c.DateTime(nullable: false));
            AddColumn("dbo.Capacitacions", "EncuestaSatisfaccion", c => c.String());
            AddColumn("dbo.Capacitacions", "DirigidoASelect", c => c.String());
            AddColumn("dbo.Capacitacions", "Modalidad", c => c.String());
            AddColumn("dbo.Capacitacions", "CursoId", c => c.String());
            AddColumn("dbo.Capacitacions", "IdentificadorRelacion", c => c.Guid(nullable: false));
            AddColumn("dbo.Capacitacions", "Docente", c => c.String());
            AddColumn("dbo.Capacitacions", "IdCursoNormativo", c => c.Int(nullable: false));
            AddColumn("dbo.Sedes", "SociedadId", c => c.Int());
            AddColumn("dbo.Terceroes", "Universidad", c => c.String());
            AlterColumn("dbo.Capacitacions", "TotalHoras", c => c.String());
            CreateIndex("dbo.Sedes", "SociedadId");
            AddForeignKey("dbo.Sedes", "SociedadId", "dbo.Sociedads", "Id");
            DropColumn("dbo.Capacitacions", "LugarlCiudad");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Capacitacions", "LugarlCiudad", c => c.String(nullable: false));
            DropForeignKey("dbo.NoticiaImagens", "NoticiaId", "dbo.Noticias");
            DropForeignKey("dbo.SoporteCesantias", "SolicitudCesantiaId", "dbo.SolicitudCesantias");
            DropForeignKey("dbo.LogSolicitudCesantias", "SolicitudCesantiaId", "dbo.SolicitudCesantias");
            DropForeignKey("dbo.SolicitudCesantias", "EstadoId", "dbo.EstadoCesantias");
            DropForeignKey("dbo.SolicitudCesantias", "EmpleadoId", "dbo.Empleadoes");
            DropForeignKey("dbo.SolicitudCesantias", "DestinoId", "dbo.DestinoCesantias");
            DropForeignKey("dbo.Dosimetrias", "SedeId", "dbo.Sedes");
            DropForeignKey("dbo.Dosimetrias", "EmpleadoId", "dbo.Empleadoes");
            DropForeignKey("dbo.SoporteDestinoes", "DestinoId", "dbo.DestinoCesantias");
            DropForeignKey("dbo.Sedes", "SociedadId", "dbo.Sociedads");
            DropIndex("dbo.NoticiaImagens", new[] { "NoticiaId" });
            DropIndex("dbo.SoporteCesantias", new[] { "SolicitudCesantiaId" });
            DropIndex("dbo.SolicitudCesantias", new[] { "EstadoId" });
            DropIndex("dbo.SolicitudCesantias", new[] { "DestinoId" });
            DropIndex("dbo.SolicitudCesantias", new[] { "EmpleadoId" });
            DropIndex("dbo.LogSolicitudCesantias", new[] { "SolicitudCesantiaId" });
            DropIndex("dbo.Dosimetrias", new[] { "SedeId" });
            DropIndex("dbo.Dosimetrias", new[] { "EmpleadoId" });
            DropIndex("dbo.SoporteDestinoes", new[] { "DestinoId" });
            DropIndex("dbo.Sedes", new[] { "SociedadId" });
            AlterColumn("dbo.Capacitacions", "TotalHoras", c => c.Int(nullable: false));
            DropColumn("dbo.Terceroes", "Universidad");
            DropColumn("dbo.Sedes", "SociedadId");
            DropColumn("dbo.Capacitacions", "IdCursoNormativo");
            DropColumn("dbo.Capacitacions", "Docente");
            DropColumn("dbo.Capacitacions", "IdentificadorRelacion");
            DropColumn("dbo.Capacitacions", "CursoId");
            DropColumn("dbo.Capacitacions", "Modalidad");
            DropColumn("dbo.Capacitacions", "DirigidoASelect");
            DropColumn("dbo.Capacitacions", "EncuestaSatisfaccion");
            DropColumn("dbo.Capacitacions", "Mes");
            DropColumn("dbo.Capacitacions", "EvaluacionConocimiento");
            DropColumn("dbo.Capacitacions", "Lugar");
            DropColumn("dbo.Capacitacions", "Ciudad");
            DropTable("dbo.NoticiaImagens");
            DropTable("dbo.LugaresCapacitacions");
            DropTable("dbo.SoporteCesantias");
            DropTable("dbo.SolicitudCesantias");
            DropTable("dbo.LogSolicitudCesantias");
            DropTable("dbo.EstadoCesantias");
            DropTable("dbo.Empresas");
            DropTable("dbo.Dosimetrias");
            DropTable("dbo.SoporteDestinoes");
            DropTable("dbo.DestinoCesantias");
            DropTable("dbo.CursosNormativas");
            DropTable("dbo.Certificados");
        }
    }
}
