namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtglbunga : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiCetaks", "TglJatuhTempo", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransaksiCetaks", "TglJatuhTempo");
        }
    }
}
