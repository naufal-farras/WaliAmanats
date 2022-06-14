namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailCetaks", "TransaksiTanggal_Id", "dbo.TransaksiTanggals");
            DropIndex("dbo.DetailCetaks", new[] { "TransaksiTanggal_Id" });
            AddColumn("dbo.DetailCetaks", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.DetailCetaks", "TransaksiLaporan_Id");
            AddForeignKey("dbo.DetailCetaks", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
            DropColumn("dbo.DetailCetaks", "TransaksiTanggal_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DetailCetaks", "TransaksiTanggal_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.DetailCetaks", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.DetailCetaks", new[] { "TransaksiLaporan_Id" });
            DropColumn("dbo.DetailCetaks", "TransaksiLaporan_Id");
            CreateIndex("dbo.DetailCetaks", "TransaksiTanggal_Id");
            AddForeignKey("dbo.DetailCetaks", "TransaksiTanggal_Id", "dbo.TransaksiTanggals", "Id", cascadeDelete: true);
        }
    }
}
