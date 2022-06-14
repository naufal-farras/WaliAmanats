namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewString2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DetailPerusahaans", "Target", c => c.String());
            AlterColumn("dbo.DetailPerusahaans", "Realisasi", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DetailPerusahaans", "Realisasi", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DetailPerusahaans", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
