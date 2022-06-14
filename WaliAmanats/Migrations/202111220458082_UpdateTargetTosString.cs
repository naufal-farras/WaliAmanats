namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTargetTosString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SettingRatios", "Target", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SettingRatios", "Target", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
