namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRealisasi : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransaksiRatios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiLaporan_Id = c.Long(nullable: false),
                        NoSurat = c.String(),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusCetak_Id = c.Long(nullable: false),
                        TanggalSurat = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: false)
                .ForeignKey("dbo.TransaksiLaporans", t => t.TransaksiLaporan_Id, cascadeDelete: false)
                .Index(t => t.TransaksiLaporan_Id)
                .Index(t => t.StatusCetak_Id);
            
            AddColumn("dbo.DetailPerusahaans", "Realisasi", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DetailPerusahaans", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransaksiRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropForeignKey("dbo.TransaksiRatios", "StatusCetak_Id", "dbo.StatusCetaks");
            DropIndex("dbo.TransaksiRatios", new[] { "StatusCetak_Id" });
            DropIndex("dbo.TransaksiRatios", new[] { "TransaksiLaporan_Id" });
            AlterColumn("dbo.DetailPerusahaans", "Target", c => c.String());
            DropColumn("dbo.DetailPerusahaans", "Realisasi");
            DropTable("dbo.TransaksiRatios");
        }
    }
}
