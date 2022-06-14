namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ubahjaminan : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransaksiJaminans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.TransaksiJaminans", new[] { "TransaksiLaporan_Id" });
            AddColumn("dbo.TransaksiJaminans", "Perusahaan_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiJaminans", "Perusahaan_Id");
            AddForeignKey("dbo.TransaksiJaminans", "Perusahaan_Id", "dbo.Perusahaan", "Id", cascadeDelete: true);
            DropColumn("dbo.TransaksiJaminans", "TransaksiLaporan_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiJaminans", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.TransaksiJaminans", "Perusahaan_Id", "dbo.Perusahaan");
            DropIndex("dbo.TransaksiJaminans", new[] { "Perusahaan_Id" });
            DropColumn("dbo.TransaksiJaminans", "Perusahaan_Id");
            CreateIndex("dbo.TransaksiJaminans", "TransaksiLaporan_Id");
            AddForeignKey("dbo.TransaksiJaminans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
        }
    }
}
