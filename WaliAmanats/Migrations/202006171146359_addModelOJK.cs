namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addModelOJK : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AturanOJKs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Aturan = c.String(),
                        IsiAturan = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OJKs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Alamat1 = c.String(),
                        Alamat2 = c.String(),
                        Alamat3 = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OJKs");
            DropTable("dbo.AturanOJKs");
        }
    }
}
