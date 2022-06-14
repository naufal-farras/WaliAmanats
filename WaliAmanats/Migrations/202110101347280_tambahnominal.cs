namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tambahnominal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiJaminans", "Nominal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TransaksiJaminans", "Persentase", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.KopSurats", "TanggalSurat", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.KopSurats", "TanggalSurat", c => c.DateTime(nullable: false));
            DropColumn("dbo.TransaksiJaminans", "Persentase");
            DropColumn("dbo.TransaksiJaminans", "Nominal");
        }
    }
}
