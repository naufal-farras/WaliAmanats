namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewFieldTransaksiKejadian : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiKejadians", "IdPer", c => c.Long());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransaksiKejadians", "IdPer");
        }
    }
}
