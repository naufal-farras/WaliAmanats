namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ubahtransdana : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.TransaksiPenggunaanDanas", new[] { "TransaksiLaporan_Id" });
            AddColumn("dbo.TransaksiPenggunaanDanas", "Perusahaan_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiPenggunaanDanas", "Perusahaan_Id");
            AddForeignKey("dbo.TransaksiPenggunaanDanas", "Perusahaan_Id", "dbo.Perusahaan", "Id", cascadeDelete: true);
            DropColumn("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.TransaksiPenggunaanDanas", "Perusahaan_Id", "dbo.Perusahaan");
            DropIndex("dbo.TransaksiPenggunaanDanas", new[] { "Perusahaan_Id" });
            DropColumn("dbo.TransaksiPenggunaanDanas", "Perusahaan_Id");
            CreateIndex("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id");
            AddForeignKey("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
        }
    }
}
