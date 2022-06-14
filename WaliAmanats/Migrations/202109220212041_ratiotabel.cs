namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ratiotabel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetailRatios", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            AddColumn("dbo.DetailRatios", "Batas", c => c.String());
            AddColumn("dbo.DetailRatios", "Keterangan", c => c.String());
            AddColumn("dbo.DetailRatios", "HasilAudited", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.DetailRatios", "NoSurat", c => c.String());
            AddColumn("dbo.DetailRatios", "TanggalSurat", c => c.DateTime(nullable: false));
            CreateIndex("dbo.DetailRatios", "TransaksiLaporan_Id");
            AddForeignKey("dbo.DetailRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: false);
            DropColumn("dbo.DetailRatios", "Target");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DetailRatios", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.DetailRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.DetailRatios", new[] { "TransaksiLaporan_Id" });
            DropColumn("dbo.DetailRatios", "TanggalSurat");
            DropColumn("dbo.DetailRatios", "NoSurat");
            DropColumn("dbo.DetailRatios", "HasilAudited");
            DropColumn("dbo.DetailRatios", "Keterangan");
            DropColumn("dbo.DetailRatios", "Batas");
            DropColumn("dbo.DetailRatios", "TransaksiLaporan_Id");
        }
    }
}
