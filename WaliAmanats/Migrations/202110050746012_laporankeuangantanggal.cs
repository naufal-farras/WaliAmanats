namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class laporankeuangantanggal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetailKeuangans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiKeuangan_Id = c.Long(nullable: false),
                        TanggalSurat = c.DateTime(nullable: false),
                        TanggalCetak = c.DateTime(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        Status = c.Boolean(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: true)
                .ForeignKey("dbo.TransaksiKeuangans", t => t.TransaksiKeuangan_Id, cascadeDelete: true)
                .Index(t => t.TransaksiKeuangan_Id)
                .Index(t => t.StatusCetak_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetailKeuangans", "TransaksiKeuangan_Id", "dbo.TransaksiKeuangans");
            DropForeignKey("dbo.DetailKeuangans", "StatusCetak_Id", "dbo.StatusCetaks");
            DropIndex("dbo.DetailKeuangans", new[] { "StatusCetak_Id" });
            DropIndex("dbo.DetailKeuangans", new[] { "TransaksiKeuangan_Id" });
            DropTable("dbo.DetailKeuangans");
        }
    }
}
