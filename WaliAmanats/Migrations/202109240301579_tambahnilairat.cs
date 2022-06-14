namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tambahnilairat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Perusahaan", "Nilai", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TransaksiRatings", "Nilai", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TransaksiRatings", "Nilai");
            DropColumn("dbo.Perusahaan", "Nilai");
        }
    }
}
