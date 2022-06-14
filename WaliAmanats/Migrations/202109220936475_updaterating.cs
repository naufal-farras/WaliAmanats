namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updaterating : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TransaksiRatings", "JenisTugas_Id", "dbo.JenisTugas");
            DropForeignKey("dbo.TransaksiRatings", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.TransaksiRatings", "PerwakilanPerusahaan_Id", "dbo.PerwakilanPerusahaan");
            DropForeignKey("dbo.TransaksiRatings", "Produk_Id", "dbo.Produk");
            DropForeignKey("dbo.TransaksiRatings", "Status_Id", "dbo.Status");
            DropIndex("dbo.TransaksiRatings", new[] { "Perusahaan_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "PerwakilanPerusahaan_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "Produk_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "JenisTugas_Id" });
            DropIndex("dbo.TransaksiRatings", new[] { "Status_Id" });
            CreateTable(
                "dbo.DetailRatings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiRating_Id = c.Long(nullable: false),
                        Rating_Id = c.Long(nullable: false),
                        Keterangan = c.String(),
                        Batas = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ratings", t => t.Rating_Id, cascadeDelete: true)
                .ForeignKey("dbo.TransaksiRatings", t => t.TransaksiRating_Id, cascadeDelete: true)
                .Index(t => t.TransaksiRating_Id)
                .Index(t => t.Rating_Id);
            
            AddColumn("dbo.Ratios", "Initial", c => c.String());
            AddColumn("dbo.TransaksiRatios", "StatusCetak_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiRatios", "StatusCetak_Id");
            AddForeignKey("dbo.TransaksiRatios", "StatusCetak_Id", "dbo.StatusCetaks", "Id", cascadeDelete: false);
            DropColumn("dbo.TransaksiRatings", "Perusahaan_Id");
            DropColumn("dbo.TransaksiRatings", "PerwakilanPerusahaan_Id");
            DropColumn("dbo.TransaksiRatings", "Produk_Id");
            DropColumn("dbo.TransaksiRatings", "JenisTugas_Id");
            DropColumn("dbo.TransaksiRatings", "Status_Id");
            DropColumn("dbo.TransaksiRatings", "Keterangan");
            DropColumn("dbo.TransaksiRatings", "Batas");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiRatings", "Batas", c => c.String());
            AddColumn("dbo.TransaksiRatings", "Keterangan", c => c.String());
            AddColumn("dbo.TransaksiRatings", "Status_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiRatings", "JenisTugas_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiRatings", "Produk_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiRatings", "PerwakilanPerusahaan_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiRatings", "Perusahaan_Id", c => c.Long(nullable: false));
            DropForeignKey("dbo.TransaksiRatios", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.DetailRatings", "TransaksiRating_Id", "dbo.TransaksiRatings");
            DropForeignKey("dbo.DetailRatings", "Rating_Id", "dbo.Ratings");
            DropIndex("dbo.TransaksiRatios", new[] { "StatusCetak_Id" });
            DropIndex("dbo.DetailRatings", new[] { "Rating_Id" });
            DropIndex("dbo.DetailRatings", new[] { "TransaksiRating_Id" });
            DropColumn("dbo.TransaksiRatios", "StatusCetak_Id");
            DropColumn("dbo.Ratios", "Initial");
            DropTable("dbo.DetailRatings");
            CreateIndex("dbo.TransaksiRatings", "Status_Id");
            CreateIndex("dbo.TransaksiRatings", "JenisTugas_Id");
            CreateIndex("dbo.TransaksiRatings", "Produk_Id");
            CreateIndex("dbo.TransaksiRatings", "PerwakilanPerusahaan_Id");
            CreateIndex("dbo.TransaksiRatings", "Perusahaan_Id");
            AddForeignKey("dbo.TransaksiRatings", "Status_Id", "dbo.Status", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiRatings", "Produk_Id", "dbo.Produk", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiRatings", "PerwakilanPerusahaan_Id", "dbo.PerwakilanPerusahaan", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiRatings", "Perusahaan_Id", "dbo.Perusahaan", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TransaksiRatings", "JenisTugas_Id", "dbo.JenisTugas", "Id", cascadeDelete: true);
        }
    }
}
