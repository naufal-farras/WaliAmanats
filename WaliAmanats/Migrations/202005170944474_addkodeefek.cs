namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addkodeefek : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiDetails", "KodeEfek", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransaksiDetails", "KodeEfek");
        }
    }
}
