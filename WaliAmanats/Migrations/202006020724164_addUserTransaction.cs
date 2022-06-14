namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUserTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiLaporans", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.TransaksiLaporans", "UserId");
            AddForeignKey("dbo.TransaksiLaporans", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransaksiLaporans", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.TransaksiLaporans", new[] { "UserId" });
            DropColumn("dbo.TransaksiLaporans", "UserId");
        }
    }
}
