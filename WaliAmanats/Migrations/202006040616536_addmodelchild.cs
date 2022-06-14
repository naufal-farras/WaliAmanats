namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodelchild : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubDetailCetaks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DetailCetak_Id = c.Long(nullable: false),
                        TransDet_Id = c.Long(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DetailCetaks", t => t.DetailCetak_Id, cascadeDelete: true)
                .Index(t => t.DetailCetak_Id);
            
            CreateTable(
                "dbo.SubDetailTanggals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        SubDetail_Id = c.Long(nullable: false),
                        TransTanggal_Id = c.Long(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubDetailCetaks", t => t.SubDetail_Id, cascadeDelete: true)
                .Index(t => t.SubDetail_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubDetailTanggals", "SubDetail_Id", "dbo.SubDetailCetaks");
            DropForeignKey("dbo.SubDetailCetaks", "DetailCetak_Id", "dbo.DetailCetaks");
            DropIndex("dbo.SubDetailTanggals", new[] { "SubDetail_Id" });
            DropIndex("dbo.SubDetailCetaks", new[] { "DetailCetak_Id" });
            DropTable("dbo.SubDetailTanggals");
            DropTable("dbo.SubDetailCetaks");
        }
    }
}
