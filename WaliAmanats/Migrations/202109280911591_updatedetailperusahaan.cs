namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedetailperusahaan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetailPerusahaans", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            AddColumn("dbo.DetailPerusahaans", "NoSurat", c => c.String());
            AddColumn("dbo.DetailPerusahaans", "TanggalSurat", c => c.DateTime(nullable: false));
            AddColumn("dbo.DetailPerusahaans", "Matching_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.DetailPerusahaans", "TransaksiLaporan_Id");
            CreateIndex("dbo.DetailPerusahaans", "Matching_Id");
            AddForeignKey("dbo.DetailPerusahaans", "Matching_Id", "dbo.Matchings", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DetailPerusahaans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetailPerusahaans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropForeignKey("dbo.DetailPerusahaans", "Matching_Id", "dbo.Matchings");
            DropIndex("dbo.DetailPerusahaans", new[] { "Matching_Id" });
            DropIndex("dbo.DetailPerusahaans", new[] { "TransaksiLaporan_Id" });
            DropColumn("dbo.DetailPerusahaans", "Matching_Id");
            DropColumn("dbo.DetailPerusahaans", "TanggalSurat");
            DropColumn("dbo.DetailPerusahaans", "NoSurat");
            DropColumn("dbo.DetailPerusahaans", "TransaksiLaporan_Id");
        }
    }
}
