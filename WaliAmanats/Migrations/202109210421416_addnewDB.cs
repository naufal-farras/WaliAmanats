namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        Persentase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsDelete = c.Boolean(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetailRatios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Realisasi = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perusahaan_Id = c.Long(nullable: false),
                        Ratio_Id = c.Long(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: true)
                .ForeignKey("dbo.Ratios", t => t.Ratio_Id, cascadeDelete: true)
                .Index(t => t.Perusahaan_Id)
                .Index(t => t.Ratio_Id);
            
            CreateTable(
                "dbo.Ratios",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        Persentase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsDelete = c.Boolean(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Jaminans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        IsDelete = c.Boolean(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransaksiKeuangans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Perusahaan_Id = c.Long(nullable: false),
                        PerwakilanPerusahaan_Id = c.Long(nullable: false),
                        JenisTugas_Id = c.Long(nullable: false),
                        Produk_Id = c.Long(nullable: false),
                        TransaksiLaporan_Id = c.Long(nullable: false),
                        TanggalInput = c.DateTime(nullable: false),
                        Keterangan = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JenisTugas", t => t.JenisTugas_Id, cascadeDelete: false)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: false)
                .ForeignKey("dbo.PerwakilanPerusahaan", t => t.PerwakilanPerusahaan_Id, cascadeDelete: false)
                .ForeignKey("dbo.Produk", t => t.Produk_Id, cascadeDelete: false)
                .ForeignKey("dbo.TransaksiLaporans", t => t.TransaksiLaporan_Id, cascadeDelete: false)
                .Index(t => t.Perusahaan_Id)
                .Index(t => t.PerwakilanPerusahaan_Id)
                .Index(t => t.JenisTugas_Id)
                .Index(t => t.Produk_Id)
                .Index(t => t.TransaksiLaporan_Id);
            
            CreateTable(
                "dbo.TransaksiRatings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Perusahaan_Id = c.Long(nullable: false),
                        PerwakilanPerusahaan_Id = c.Long(nullable: false),
                        TransaksiLaporan_Id = c.Long(nullable: false),
                        Produk_Id = c.Long(nullable: false),
                        JenisTugas_Id = c.Long(nullable: false),
                        TanggalInput = c.DateTime(nullable: false),
                        Status_Id = c.Long(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        Keterangan = c.String(),
                        Batas = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.JenisTugas", t => t.JenisTugas_Id, cascadeDelete: false)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: false)
                .ForeignKey("dbo.PerwakilanPerusahaan", t => t.PerwakilanPerusahaan_Id, cascadeDelete: false)
                .ForeignKey("dbo.Produk", t => t.Produk_Id, cascadeDelete: false)
                .ForeignKey("dbo.Status", t => t.Status_Id, cascadeDelete: false)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: false)
                .ForeignKey("dbo.TransaksiLaporans", t => t.TransaksiLaporan_Id, cascadeDelete: false)
                .Index(t => t.Perusahaan_Id)
                .Index(t => t.PerwakilanPerusahaan_Id)
                .Index(t => t.TransaksiLaporan_Id)
                .Index(t => t.Produk_Id)
                .Index(t => t.JenisTugas_Id)
                .Index(t => t.Status_Id)
                .Index(t => t.StatusCetak_Id);
            
            AddColumn("dbo.Perusahaan", "Rating_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.Perusahaan", "Rating_Id");
            AddForeignKey("dbo.Perusahaan", "Rating_Id", "dbo.Ratings", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransaksiRatings", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropForeignKey("dbo.TransaksiRatings", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.TransaksiRatings", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.TransaksiRatings", "Produk_Id", "dbo.Produk");
            DropForeignKey("dbo.TransaksiRatings", "PerwakilanPerusahaan_Id", "dbo.PerwakilanPerusahaan");
            DropForeignKey("dbo.TransaksiRatings", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.TransaksiRatings", "JenisTugas_Id", "dbo.JenisTugas");
            DropForeignKey("dbo.TransaksiKeuangans", "TransaksiLaporan_Id", "dbo.TransaksiLaporans");
            DropForeignKey("dbo.TransaksiKeuangans", "Produk_Id", "dbo.Produk");
            DropForeignKey("dbo.TransaksiKeuangans", "PerwakilanPerusahaan_Id", "dbo.PerwakilanPerusahaan");
            DropForeignKey("dbo.TransaksiKeuangans", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.TransaksiKeuangans", "JenisTugas_Id", "dbo.JenisTugas");
            DropForeignKey("dbo.DetailRatios", "Ratio_Id", "dbo.Ratios");
            DropForeignKey("dbo.DetailRatios", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.Perusahaan", "Rating_Id", "dbo.Ratings");
            DropIndex("dbo.TransaksiRatings", new[] { "StatusCetak_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "Status_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "JenisTugas_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "Produk_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "TransaksiLaporan_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "PerwakilanPerusahaan_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "Perusahaan_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "TransaksiLaporan_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "Produk_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "JenisTugas_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "PerwakilanPerusahaan_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "Perusahaan_Id" });
            DropIndex("dbo.DetailRatios", new[] { "Ratio_Id" });
            DropIndex("dbo.DetailRatios", new[] { "Perusahaan_Id" });
            DropIndex("dbo.Perusahaan", new[] { "Rating_Id" });
            DropColumn("dbo.Perusahaan", "Rating_Id");
            DropTable("dbo.TransaksiRatings");
            DropTable("dbo.TransaksiKeuangans");
            DropTable("dbo.Jaminans");
            DropTable("dbo.Ratios");
            DropTable("dbo.DetailRatios");
            DropTable("dbo.Ratings");
        }
    }
}
