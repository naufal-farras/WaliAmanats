namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subdetailperusahaan : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransaksiKeuangans", "JenisTugas_Id", "dbo.JenisTugas");
            DropForeignKey("dbo.TransaksiKeuangans", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.TransaksiKeuangans", "PerwakilanPerusahaan_Id", "dbo.PerwakilanPerusahaan");
            DropForeignKey("dbo.TransaksiKeuangans", "Produk_Id", "dbo.Produk");
            DropIndex("dbo.TransaksiKeuangans", new[] { "Perusahaan_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "PerwakilanPerusahaan_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "JenisTugas_Id" });
            DropIndex("dbo.TransaksiKeuangans", new[] { "Produk_Id" });
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DetailPerusahaans", t => t.DetailPerusahaan_Id, cascadeDelete: true)
                .ForeignKey("dbo.Matchings", t => t.Matching_Id, cascadeDelete: false)
                .ForeignKey("dbo.Measurements", t => t.Measurement_Id, cascadeDelete: false)
                .ForeignKey("dbo.Ratios", t => t.Ratio_Id, cascadeDelete: false)
                .Index(t => t.DetailPerusahaan_Id)
                .Index(t => t.Ratio_Id)
                .Index(t => t.Measurement_Id)
                .Index(t => t.Matching_Id);
            
            AddColumn("dbo.TransaksiKeuangans", "JatuhTempo", c => c.DateTime(nullable: false));
            DropColumn("dbo.TransaksiKeuangans", "Perusahaan_Id");
            DropColumn("dbo.TransaksiKeuangans", "PerwakilanPerusahaan_Id");
            DropColumn("dbo.TransaksiKeuangans", "JenisTugas_Id");
            DropColumn("dbo.TransaksiKeuangans", "Produk_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiKeuangans", "Produk_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiKeuangans", "JenisTugas_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiKeuangans", "PerwakilanPerusahaan_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiKeuangans", "Perusahaan_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.SubDetailPerusahaans", "Ratio_Id", "dbo.Ratios");
            DropForeignKey("dbo.SubDetailPerusahaans", "Measurement_Id", "dbo.Measurements");
            DropForeignKey("dbo.SubDetailPerusahaans", "Matching_Id", "dbo.Matchings");
            DropForeignKey("dbo.SubDetailPerusahaans", "DetailPerusahaan_Id", "dbo.DetailPerusahaans");
            DropIndex("dbo.SubDetailPerusahaans", new[] { "Matching_Id" });
            DropIndex("dbo.SubDetailPerusahaans", new[] { "Measurement_Id" });
            DropIndex("dbo.SubDetailPerusahaans", new[] { "Ratio_Id" });
            DropIndex("dbo.SubDetailPerusahaans", new[] { "DetailPerusahaan_Id" });
            DropColumn("dbo.TransaksiKeuangans", "JatuhTempo");
            DropTable("dbo.SubDetailPerusahaans");
            CreateIndex("dbo.TransaksiKeuangans", "Produk_Id");
            CreateIndex("dbo.TransaksiKeuangans", "JenisTugas_Id");
            CreateIndex("dbo.TransaksiKeuangans", "PerwakilanPerusahaan_Id");
            CreateIndex("dbo.TransaksiKeuangans", "Perusahaan_Id");
            AddForeignKey("dbo.TransaksiKeuangans", "Produk_Id", "dbo.Produk", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiKeuangans", "PerwakilanPerusahaan_Id", "dbo.PerwakilanPerusahaan", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiKeuangans", "Perusahaan_Id", "dbo.Perusahaan", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiKeuangans", "JenisTugas_Id", "dbo.JenisTugas", "Id", cascadeDelete: true);
        }
    }
}
