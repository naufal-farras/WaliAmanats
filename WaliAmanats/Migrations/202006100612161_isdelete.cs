namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isdelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiLaporans", "isDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransaksiLaporans", "isDelete");
        }
    }
}
