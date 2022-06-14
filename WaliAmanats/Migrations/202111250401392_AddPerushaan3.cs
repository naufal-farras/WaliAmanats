namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPerushaan3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TransaksiKejadians", "Perusahaan_Id", c => c.Long());
            CreateIndex("dbo.TransaksiKejadians", "Perusahaan_Id");
            AddForeignKey("dbo.TransaksiKejadians", "Perusahaan_Id", "dbo.Perusahaan", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransaksiKejadians", "Perusahaan_Id", "dbo.Perusahaan");
            DropIndex("dbo.TransaksiKejadians", new[] { "Perusahaan_Id" });
            DropColumn("dbo.TransaksiKejadians", "Perusahaan_Id");
        }
    }
}
