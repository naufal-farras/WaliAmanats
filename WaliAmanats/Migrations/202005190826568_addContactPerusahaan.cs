namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addContactPerusahaan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Perusahaan", "NoTelp", c => c.String());
            AddColumn("dbo.Perusahaan", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Perusahaan", "Email");
            DropColumn("dbo.Perusahaan", "NoTelp");
        }
    }
}
