namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ubahdbdetail : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailPerusahaans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.DetailPerusahaans", new[] { "TransaksiLaporan_Id" });
            AlterColumn("dbo.DetailPerusahaans", "TanggalSurat", c => c.DateTime());
            DropColumn("dbo.DetailPerusahaans", "TransaksiLaporan_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DetailPerusahaans", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.DetailPerusahaans", "TanggalSurat", c => c.DateTime(nullable: false));
            CreateIndex("dbo.DetailPerusahaans", "TransaksiLaporan_Id");
            AddForeignKey("dbo.DetailPerusahaans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
        }
    }
}
