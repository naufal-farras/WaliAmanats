namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Perusahaan", "PersentaseKredit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Perusahaan", "PersentaseKredit");
        }
    }
}
