namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class penambahantabelbaru : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailPerusahaans", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.SubDetailPerusahaans", "DetailPerusahaan_Id", "dbo.DetailPerusahaans");
            DropForeignKey("dbo.SubDetailPerusahaans", "Matching_Id", "dbo.Matchings");
            DropForeignKey("dbo.SubDetailPerusahaans", "Measurement_Id", "dbo.Measurements");
            DropForeignKey("dbo.SubDetailPerusahaans", "Ratio_Id", "dbo.Ratios");
            DropForeignKey("dbo.TransaksiRatios", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.TransaksiRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropIndex("dbo.DetailPerusahaans", new[] { "Perusahaan_Id" });
            DropIndex("dbo.SubDetailPerusahaans", new[] { "DetailPerusahaan_Id" });
            DropIndex("dbo.SubDetailPerusahaans", new[] { "Ratio_Id" });
            DropIndex("dbo.SubDetailPerusahaans", new[] { "Measurement_Id" });
            DropIndex("dbo.SubDetailPerusahaans", new[] { "Matching_Id" });
            DropIndex("dbo.TransaksiRatios", new[] { "TransaksiLaporan_Id" });
            DropIndex("dbo.TransaksiRatios", new[] { "StatusCetak_Id" });
            CreateTable(
                "dbo.KopSurats",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Perusahaan_Id = c.Long(nullable: false),
                        NoSurat = c.String(),
                        TanggalSurat = c.DateTime(nullable: false),
                        Periode = c.String(),
                        Status = c.Boolean(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        Path = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: true)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: true)
                .Index(t => t.Perusahaan_Id)
                .Index(t => t.StatusCetak_Id);
            
            CreateTable(
                "dbo.SettingRatios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Perusahaan_Id = c.Long(nullable: false),
                        Ratio_Id = c.Long(nullable: false),
                        Measurement_Id = c.Long(nullable: false),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                "dbo.TransaksiJaminans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiLaporan_Id = c.Long(nullable: false),
                        Jaminan_Id = c.Long(nullable: false),
                        TanggalInput = c.DateTime(nullable: false),
                        TanggalCetak = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        Path = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Jaminans", t => t.Jaminan_Id, cascadeDelete: true)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: true)
                .ForeignKey("dbo.TransaksiLaporans", t => t.TransaksiLaporan_Id, cascadeDelete: false)
                .Index(t => t.TransaksiLaporan_Id)
                .Index(t => t.Jaminan_Id)
                .Index(t => t.StatusCetak_Id);
            
            CreateTable(
                "dbo.TransaksiPenggunaanDanas",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiDetail_Id = c.Long(nullable: false),
                        TanggalInput = c.DateTime(nullable: false),
                        TanggalCetak = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        Path = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: true)
                .ForeignKey("dbo.TransaksiDetails", t => t.TransaksiDetail_Id, cascadeDelete: false)
                .Index(t => t.TransaksiDetail_Id)
                .Index(t => t.StatusCetak_Id);
            
            AddColumn("dbo.DetailKeuangans", "Path", c => c.String());
            AddColumn("dbo.DetailPerusahaans", "KopSurat_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiRatings", "Status", c => c.Boolean(nullable: false));
            AddColumn("dbo.TransaksiRatings", "Path", c => c.String());
            CreateIndex("dbo.DetailPerusahaans", "KopSurat_Id");
            AddForeignKey("dbo.DetailPerusahaans", "KopSurat_Id", "dbo.KopSurats", "Id", cascadeDelete: true);
            DropColumn("dbo.DetailPerusahaans", "Perusahaan_Id");
            DropColumn("dbo.DetailPerusahaans", "NoSurat");
            DropColumn("dbo.DetailPerusahaans", "TanggalSurat");
            DropTable("dbo.SubDetailPerusahaans");
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
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusCetak_Id = c.Long(nullable: false),
                        TanggalSurat = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubDetailPerusahaans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DetailPerusahaan_Id = c.Long(nullable: false),
                        Ratio_Id = c.Long(nullable: false),
                        Measurement_Id = c.Long(nullable: false),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Realisasi = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoSurat = c.String(),
                        TanggalSurat = c.DateTime(nullable: false),
                        Matching_Id = c.Long(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.DetailPerusahaans", "TanggalSurat", c => c.DateTime());
            AddColumn("dbo.DetailPerusahaans", "NoSurat", c => c.String());
            AddColumn("dbo.DetailPerusahaans", "Perusahaan_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.TransaksiPenggunaanDanas", "TransaksiDetail_Id", "dbo.TransaksiDetails");
            DropForeignKey("dbo.TransaksiPenggunaanDanas", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.TransaksiJaminans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropForeignKey("dbo.TransaksiJaminans", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.TransaksiJaminans", "Jaminan_Id", "dbo.Jaminans");
            DropForeignKey("dbo.SettingRatios", "Ratio_Id", "dbo.Ratios");
            DropForeignKey("dbo.SettingRatios", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.SettingRatios", "Measurement_Id", "dbo.Measurements");
            DropForeignKey("dbo.DetailPerusahaans", "KopSurat_Id", "dbo.KopSurats");
            DropForeignKey("dbo.KopSurats", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.KopSurats", "Perusahaan_Id", "dbo.Perusahaan");
            DropIndex("dbo.TransaksiPenggunaanDanas", new[] { "StatusCetak_Id" });
            DropIndex("dbo.TransaksiPenggunaanDanas", new[] { "TransaksiDetail_Id" });
            DropIndex("dbo.TransaksiJaminans", new[] { "StatusCetak_Id" });
            DropIndex("dbo.TransaksiJaminans", new[] { "Jaminan_Id" });
            DropIndex("dbo.TransaksiJaminans", new[] { "TransaksiLaporan_Id" });
            DropIndex("dbo.SettingRatios", new[] { "Measurement_Id" });
            DropIndex("dbo.SettingRatios", new[] { "Ratio_Id" });
            DropIndex("dbo.SettingRatios", new[] { "Perusahaan_Id" });
            DropIndex("dbo.KopSurats", new[] { "StatusCetak_Id" });
            DropIndex("dbo.KopSurats", new[] { "Perusahaan_Id" });
            DropIndex("dbo.DetailPerusahaans", new[] { "KopSurat_Id" });
            DropColumn("dbo.TransaksiRatings", "Path");
            DropColumn("dbo.TransaksiRatings", "Status");
            DropColumn("dbo.DetailPerusahaans", "KopSurat_Id");
            DropColumn("dbo.DetailKeuangans", "Path");
            DropTable("dbo.TransaksiPenggunaanDanas");
            DropTable("dbo.TransaksiJaminans");
            DropTable("dbo.SettingRatios");
            DropTable("dbo.KopSurats");
            CreateIndex("dbo.TransaksiRatios", "StatusCetak_Id");
            CreateIndex("dbo.TransaksiRatios", "TransaksiLaporan_Id");
            CreateIndex("dbo.SubDetailPerusahaans", "Matching_Id");
            CreateIndex("dbo.SubDetailPerusahaans", "Measurement_Id");
            CreateIndex("dbo.SubDetailPerusahaans", "Ratio_Id");
            CreateIndex("dbo.SubDetailPerusahaans", "DetailPerusahaan_Id");
            CreateIndex("dbo.DetailPerusahaans", "Perusahaan_Id");
            AddForeignKey("dbo.TransaksiRatios", "TransaksiLaporan_Id", "dbo.TransaksiLaporans", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiRatios", "StatusCetak_Id", "dbo.StatusCetaks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubDetailPerusahaans", "Ratio_Id", "dbo.Ratios", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubDetailPerusahaans", "Measurement_Id", "dbo.Measurements", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubDetailPerusahaans", "Matching_Id", "dbo.Matchings", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubDetailPerusahaans", "DetailPerusahaan_Id", "dbo.DetailPerusahaans", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DetailPerusahaans", "Perusahaan_Id", "dbo.Perusahaan", "Id", cascadeDelete: true);
        }
    }
}
