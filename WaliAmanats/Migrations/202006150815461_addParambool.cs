namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addParambool : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Jabatan", "Jenis", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jabatan", "Jenis", c => c.String());
        }
    }
}
