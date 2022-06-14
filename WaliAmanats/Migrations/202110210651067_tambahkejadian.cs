namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tambahkejadian : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiKejadians", "Kejadian_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiKejadians", "Kejadian_Id");
            AddForeignKey("dbo.TransaksiKejadians", "Kejadian_Id", "dbo.KejadianPentings", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransaksiKejadians", "Kejadian_Id", "dbo.KejadianPentings");
            DropIndex("dbo.TransaksiKejadians", new[] { "Kejadian_Id" });
            DropColumn("dbo.TransaksiKejadians", "Kejadian_Id");
        }
    }
}
