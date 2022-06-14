namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropIdPer : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TransaksiKejadians", "IdPer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TransaksiKejadians", "IdPer", c => c.Long());
        }
    }
}
