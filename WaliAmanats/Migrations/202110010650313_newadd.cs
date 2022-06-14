namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newadd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransaksiRatings", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.TransaksiRatings", new[] { "TransaksiLaporan_Id" });
            AddColumn("dbo.TransaksiKeuangans", "TanggalTerbit", c => c.DateTime());
            AddColumn("dbo.TransaksiRatings", "Perusahaan_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiRatings", "Perusahaan_Id");
            AddForeignKey("dbo.TransaksiRatings", "Perusahaan_Id", "dbo.Perusahaan", "Id", cascadeDelete: false);
            DropColumn("dbo.TransaksiRatings", "TransaksiLaporan_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiRatings", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.TransaksiRatings", "Perusahaan_Id", "dbo.Perusahaan");
            DropIndex("dbo.TransaksiRatings", new[] { "Perusahaan_Id" });
            DropColumn("dbo.TransaksiRatings", "Perusahaan_Id");
            DropColumn("dbo.TransaksiKeuangans", "TanggalTerbit");
            CreateIndex("dbo.TransaksiRatings", "TransaksiLaporan_Id");
            AddForeignKey("dbo.TransaksiRatings", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
        }
    }
}
