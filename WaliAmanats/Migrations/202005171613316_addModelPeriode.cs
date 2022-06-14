namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addModelPeriode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Periodes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Hari = c.Int(nullable: false),
                        Penyebut = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TransaksiDetails", "Periode_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.TransaksiDetails", "Periode_Id");
            AddForeignKey("dbo.TransaksiDetails", "Periode_Id", "dbo.Periodes", "Id", cascadeDelete: true);
            DropColumn("dbo.TransaksiDetails", "Periode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiDetails", "Periode", c => c.Int(nullable: false));
            DropForeignKey("dbo.TransaksiDetails", "Periode_Id", "dbo.Periodes");
            DropIndex("dbo.TransaksiDetails", new[] { "Periode_Id" });
            DropColumn("dbo.TransaksiDetails", "Periode_Id");
            DropTable("dbo.Periodes");
        }
    }
}
