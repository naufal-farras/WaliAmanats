namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fktranstgl : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.SubDetailTanggals", "TransTanggal_Id");
            AddForeignKey("dbo.SubDetailTanggals", "TransTanggal_Id", "dbo.TransaksiTanggals", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubDetailTanggals", "TransTanggal_Id", "dbo.TransaksiTanggals");
            DropIndex("dbo.SubDetailTanggals", new[] { "TransTanggal_Id" });
        }
    }
}
