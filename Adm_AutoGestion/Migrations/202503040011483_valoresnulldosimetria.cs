namespace Adm_AutoGestion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class valoresnulldosimetria : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Dosimetrias", "ValorHp10", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Dosimetrias", "ValorHp3", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Dosimetrias", "ValorHp3", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Dosimetrias", "ValorHp10", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
