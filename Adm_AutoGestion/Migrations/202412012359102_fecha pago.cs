﻿namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fechapago : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HorasExtras", "FechaPago", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HorasExtras", "FechaPago");
        }
    }
}
