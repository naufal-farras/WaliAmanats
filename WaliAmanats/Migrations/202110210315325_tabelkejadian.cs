namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tabelkejadian : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KejadianPentings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransaksiKejadians",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TransaksiDetail_Id = c.Long(nullable: false),
                        TransaksiTanggal_Id = c.Long(nullable: false),
                        TanggalInput = c.DateTime(nullable: false),
                        TanggalCetak = c.DateTime(nullable: false),
                        Status = c.Boolean(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        Path = c.String(),
                        Keterangan = c.String(),
                        Informasi = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: true)
                .ForeignKey("dbo.TransaksiDetails", t => t.TransaksiDetail_Id, cascadeDelete: false)
                .ForeignKey("dbo.TransaksiTanggals", t => t.TransaksiTanggal_Id, cascadeDelete: false)
                .Index(t => t.TransaksiDetail_Id)
                .Index(t => t.TransaksiTanggal_Id)
                .Index(t => t.StatusCetak_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransaksiKejadians", "TransaksiTanggal_Id", "dbo.TransaksiTanggals");
            DropForeignKey("dbo.TransaksiKejadians", "TransaksiDetail_Id", "dbo.TransaksiDetails");
            DropForeignKey("dbo.TransaksiKejadians", "StatusCetak_Id", "dbo.StatusCetaks");
            DropIndex("dbo.TransaksiKejadians", new[] { "StatusCetak_Id" });
            DropIndex("dbo.TransaksiKejadians", new[] { "TransaksiTanggal_Id" });
            DropIndex("dbo.TransaksiKejadians", new[] { "TransaksiDetail_Id" });
            DropTable("dbo.TransaksiKejadians");
            DropTable("dbo.KejadianPentings");
        }
    }
}
