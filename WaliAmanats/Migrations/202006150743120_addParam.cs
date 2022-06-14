namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addParam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jabatan", "Jenis", c => c.String());
            AddColumn("dbo.Perusahaan", "Gedung", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Perusahaan", "Gedung");
            DropColumn("dbo.Jabatan", "Jenis");
        }
    }
}
