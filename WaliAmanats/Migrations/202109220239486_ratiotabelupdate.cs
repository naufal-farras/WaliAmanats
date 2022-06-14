namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ratiotabelupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailRatios", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.DetailRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.DetailRatios", new[] { "TransaksiLaporan_Id" });
            DropIndex("dbo.DetailRatios", new[] { "Perusahaan_Id" });
            CreateTable(
                "dbo.TransaksiRatios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiLaporan_Id = c.Long(nullable: false),
                        NoSurat = c.String(),
                        TanggalSurat = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransaksiLaporans", t => t.TransaksiLaporan_Id, cascadeDelete: true)
                .Index(t => t.TransaksiLaporan_Id);
            
            AddColumn("dbo.DetailRatios", "TransaksiRatio_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.DetailRatios", "TransaksiRatio_Id");
            AddForeignKey("dbo.DetailRatios", "TransaksiRatio_Id", "dbo.TransaksiRatios", "Id", cascadeDelete: true);
            DropColumn("dbo.DetailRatios", "TransaksiLaporan_Id");
            DropColumn("dbo.DetailRatios", "Perusahaan_Id");
            DropColumn("dbo.DetailRatios", "NoSurat");
            DropColumn("dbo.DetailRatios", "TanggalSurat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DetailRatios", "TanggalSurat", c => c.DateTime(nullable: false));
            AddColumn("dbo.DetailRatios", "NoSurat", c => c.String());
            AddColumn("dbo.DetailRatios", "Perusahaan_Id", c => c.Long(nullable: false));
            AddColumn("dbo.DetailRatios", "TransaksiLaporan_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.DetailRatios", "TransaksiRatio_Id", "dbo.TransaksiRatios");
            DropForeignKey("dbo.TransaksiRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.TransaksiRatios", new[] { "TransaksiLaporan_Id" });
            DropIndex("dbo.DetailRatios", new[] { "TransaksiRatio_Id" });
            DropColumn("dbo.DetailRatios", "TransaksiRatio_Id");
            DropTable("dbo.TransaksiRatios");
            CreateIndex("dbo.DetailRatios", "Perusahaan_Id");
            CreateIndex("dbo.DetailRatios", "TransaksiLaporan_Id");
            AddForeignKey("dbo.DetailRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DetailRatios", "Perusahaan_Id", "dbo.Perusahaan", "Id", cascadeDelete: true);
        }
    }
}
