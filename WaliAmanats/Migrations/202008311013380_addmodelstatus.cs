namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmodelstatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmitenNotes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        notes = c.String(),
                        Perusahaan_Id = c.Long(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: true)
                .Index(t => t.Perusahaan_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmitenNotes", "Perusahaan_Id", "dbo.Perusahaan");
            DropIndex("dbo.EmitenNotes", new[] { "Perusahaan_Id" });
            DropTable("dbo.EmitenNotes");
        }
    }
}
