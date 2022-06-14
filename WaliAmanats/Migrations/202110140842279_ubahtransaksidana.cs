namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ubahtransaksidana : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransaksiPenggunaanDanas", "TransaksiDetail_Id", "dbo.TransaksiDetails");
            DropIndex("dbo.TransaksiPenggunaanDanas", new[] { "TransaksiDetail_Id" });
            AddColumn("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id");
            AddForeignKey("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: false);
            DropColumn("dbo.TransaksiPenggunaanDanas", "TransaksiDetail_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiPenggunaanDanas", "TransaksiDetail_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.TransaksiPenggunaanDanas", new[] { "TransaksiLaporan_Id" });
            DropColumn("dbo.TransaksiPenggunaanDanas", "TransaksiLaporan_Id");
            CreateIndex("dbo.TransaksiPenggunaanDanas", "TransaksiDetail_Id");
            AddForeignKey("dbo.TransaksiPenggunaanDanas", "TransaksiDetail_Id", "dbo.TransaksiDetails", "Id", cascadeDelete: true);
        }
    }
}
