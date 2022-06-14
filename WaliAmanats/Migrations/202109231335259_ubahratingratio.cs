namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ubahratingratio : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailRatios", "Ratio_Id", "dbo.Ratios");
            DropForeignKey("dbo.TransaksiRatios", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.TransaksiRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropForeignKey("dbo.DetailRatios", "TransaksiRatio_Id", "dbo.TransaksiRatios");
            DropIndex("dbo.DetailRatios", new[] { "TransaksiRatio_Id" });
            DropIndex("dbo.DetailRatios", new[] { "Ratio_Id" });
            DropIndex("dbo.TransaksiRatios", new[] { "TransaksiLaporan_Id" });
            DropIndex("dbo.TransaksiRatios", new[] { "StatusCetak_Id" });
            CreateTable(
                "dbo.DetailPerusahaans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Perusahaan_Id = c.Long(nullable: false),
                        Ratio_Id = c.Long(nullable: false),
                        Measurement_Id = c.Long(nullable: false),
                        Target = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Measurements", t => t.Measurement_Id, cascadeDelete: true)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: true)
                .ForeignKey("dbo.Ratios", t => t.Ratio_Id, cascadeDelete: true)
                .Index(t => t.Perusahaan_Id)
                .Index(t => t.Ratio_Id)
                .Index(t => t.Measurement_Id);
            
            CreateTable(
                "dbo.Measurements",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Matchings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TransaksiRatings", "Matching_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiRatings", "Matching_Id");
            AddForeignKey("dbo.TransaksiRatings", "Matching_Id", "dbo.Matchings", "Id", cascadeDelete: true);
            DropColumn("dbo.Ratings", "Persentase");
            DropColumn("dbo.Ratios", "Persentase");
            DropColumn("dbo.TransaksiRatings", "Keterangan");
            DropColumn("dbo.TransaksiRatings", "Batas");
            DropTable("dbo.DetailRatios");
            DropTable("dbo.TransaksiRatios");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TransaksiRatios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiLaporan_Id = c.Long(nullable: false),
                        NoSurat = c.String(),
                        StatusCetak_Id = c.Long(nullable: false),
                        TanggalSurat = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetailRatios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiRatio_Id = c.Long(nullable: false),
                        Realisasi = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Ratio_Id = c.Long(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Batas = c.String(),
                        Keterangan = c.String(),
                        HasilAudited = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TransaksiRatings", "Batas", c => c.String());
            AddColumn("dbo.TransaksiRatings", "Keterangan", c => c.String());
            AddColumn("dbo.Ratios", "Persentase", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Ratings", "Persentase", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.TransaksiRatings", "Matching_Id", "dbo.Matchings");
            DropForeignKey("dbo.DetailPerusahaans", "Ratio_Id", "dbo.Ratios");
            DropForeignKey("dbo.DetailPerusahaans", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.DetailPerusahaans", "Measurement_Id", "dbo.Measurements");
            DropIndex("dbo.TransaksiRatings", new[] { "Matching_Id" });
            DropIndex("dbo.DetailPerusahaans", new[] { "Measurement_Id" });
            DropIndex("dbo.DetailPerusahaans", new[] { "Ratio_Id" });
            DropIndex("dbo.DetailPerusahaans", new[] { "Perusahaan_Id" });
            DropColumn("dbo.TransaksiRatings", "Matching_Id");
            DropTable("dbo.Matchings");
            DropTable("dbo.Measurements");
            DropTable("dbo.DetailPerusahaans");
            CreateIndex("dbo.TransaksiRatios", "StatusCetak_Id");
            CreateIndex("dbo.TransaksiRatios", "TransaksiLaporan_Id");
            CreateIndex("dbo.DetailRatios", "Ratio_Id");
            CreateIndex("dbo.DetailRatios", "TransaksiRatio_Id");
            AddForeignKey("dbo.DetailRatios", "TransaksiRatio_Id", "dbo.TransaksiRatios", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiRatios", "StatusCetak_Id", "dbo.StatusCetaks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DetailRatios", "Ratio_Id", "dbo.Ratios", "Id", cascadeDelete: true);
        }
    }
}
