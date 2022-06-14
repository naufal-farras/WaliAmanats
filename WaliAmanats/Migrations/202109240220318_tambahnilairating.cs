namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tambahnilairating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ratings", "Nilai", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ratings", "Nilai");
        }
    }
}
