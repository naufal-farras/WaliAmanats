namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fktransdet : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.SubDetailCetaks", "TransDet_Id");
            AddForeignKey("dbo.SubDetailCetaks", "TransDet_Id", "dbo.TransaksiDetails", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubDetailCetaks", "TransDet_Id", "dbo.TransaksiDetails");
            DropIndex("dbo.SubDetailCetaks", new[] { "TransDet_Id" });
        }
    }
}
