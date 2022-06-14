namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetailCetaks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiCetak_Id = c.Long(nullable: false),
                        TransaksiTanggal_Id = c.Long(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransaksiCetaks", t => t.TransaksiCetak_Id, cascadeDelete: true)
                .ForeignKey("dbo.TransaksiTanggals", t => t.TransaksiTanggal_Id, cascadeDelete: true)
                .Index(t => t.TransaksiCetak_Id)
                .Index(t => t.TransaksiTanggal_Id);
            
            CreateTable(
                "dbo.TransaksiCetaks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Status = c.Boolean(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        TglSurat = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: false)
                .Index(t => t.StatusCetak_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetailCetaks", "TransaksiTanggal_Id", "dbo.TransaksiTanggals");
            DropForeignKey("dbo.DetailCetaks", "TransaksiCetak_Id", "dbo.TransaksiCetaks");
            DropForeignKey("dbo.TransaksiCetaks", "StatusCetak_Id", "dbo.StatusCetaks");
            DropIndex("dbo.TransaksiCetaks", new[] { "StatusCetak_Id" });
            DropIndex("dbo.DetailCetaks", new[] { "TransaksiTanggal_Id" });
            DropIndex("dbo.DetailCetaks", new[] { "TransaksiCetak_Id" });
            DropTable("dbo.TransaksiCetaks");
            DropTable("dbo.DetailCetaks");
        }
    }
}
