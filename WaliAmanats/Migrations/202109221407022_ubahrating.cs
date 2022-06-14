namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ubahrating : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailRatings", "Rating_Id", "dbo.Ratings");
            DropForeignKey("dbo.DetailRatings", "TransaksiRating_Id", "dbo.TransaksiRatings");
            DropIndex("dbo.DetailRatings", new[] { "TransaksiRating_Id" });
            DropIndex("dbo.DetailRatings", new[] { "Rating_Id" });
            AddColumn("dbo.TransaksiRatings", "Rating_Id", c => c.Long(nullable: false));
            AddColumn("dbo.TransaksiRatings", "Keterangan", c => c.String());
            AddColumn("dbo.TransaksiRatings", "Batas", c => c.String());
            CreateIndex("dbo.TransaksiRatings", "Rating_Id");
            AddForeignKey("dbo.TransaksiRatings", "Rating_Id", "dbo.Ratings", "Id", cascadeDelete: true);
            DropTable("dbo.DetailRatings");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.TransaksiRatings", "Rating_Id", "dbo.Ratings");
            DropIndex("dbo.TransaksiRatings", new[] { "Rating_Id" });
            DropColumn("dbo.TransaksiRatings", "Batas");
            DropColumn("dbo.TransaksiRatings", "Keterangan");
            DropColumn("dbo.TransaksiRatings", "Rating_Id");
            CreateIndex("dbo.DetailRatings", "Rating_Id");
            CreateIndex("dbo.DetailRatings", "TransaksiRating_Id");
            AddForeignKey("dbo.DetailRatings", "TransaksiRating_Id", "dbo.TransaksiRatings", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DetailRatings", "Rating_Id", "dbo.Ratings", "Id", cascadeDelete: true);
        }
    }
}
