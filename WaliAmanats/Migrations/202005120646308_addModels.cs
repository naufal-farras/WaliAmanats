namespace WaliAmanats.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HariLibur",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        TanggalLibur = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Jabatan",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        NamaJabatan = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.JenisTugas",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MataUang",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        Initial = c.String(),
                        Satuan = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Perusahaan",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        Jalan = c.String(),
                        Kota = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PerwakilanPerusahaan",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Perusahaan_Id = c.Long(nullable: false),
                        Nama = c.String(),
                        Gender = c.String(),
                        Jabatan_Id = c.Long(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Jabatan", t => t.Jabatan_Id, cascadeDelete: true)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: true)
                .Index(t => t.Perusahaan_Id)
                .Index(t => t.Jabatan_Id);
            
            CreateTable(
                "dbo.Produk",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        Initial = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.StatusCetaks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        Warna = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransaksiDetails",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Trans_Id = c.Long(nullable: false),
                        Seri = c.String(),
                        Nominal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TglTerbit = c.DateTime(nullable: false),
                        Bunga = c.Double(nullable: false),
                        TglJatuhTempo = c.DateTime(nullable: false),
                        Periode = c.Int(nullable: false),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransaksiLaporans", t => t.Trans_Id, cascadeDelete: true)
                .Index(t => t.Trans_Id);
            
            CreateTable(
                "dbo.TransaksiLaporans",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Perusahaan_Id = c.Long(nullable: false),
                        NamaPerwakilan_Id = c.Long(nullable: false),
                        Jabatan_Id = c.Long(nullable: false),
                        Produk_Id = c.Long(nullable: false),
                        NamaProduk = c.String(),
                        JenisPembayaran = c.String(),
                        JenisTugas_Id = c.Long(nullable: false),
                        MataUang_Id = c.Long(nullable: false),
                        Fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NoRef = c.String(),
                        Status_Id = c.Long(nullable: false),
                        StatusCetak_Id = c.Long(nullable: false),
                        JenisPengiriman = c.String(),
                        NamaBank = c.String(),
                        NamaPenerima = c.String(),
                        NoRekening = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Jabatan", t => t.Jabatan_Id, cascadeDelete: true)
                .ForeignKey("dbo.JenisTugas", t => t.JenisTugas_Id, cascadeDelete: true)
                .ForeignKey("dbo.MataUang", t => t.MataUang_Id, cascadeDelete: true)
                .ForeignKey("dbo.Perusahaan", t => t.Perusahaan_Id, cascadeDelete: true)
                .ForeignKey("dbo.PerwakilanPerusahaan", t => t.NamaPerwakilan_Id, cascadeDelete: false)
                .ForeignKey("dbo.Produk", t => t.Produk_Id, cascadeDelete: true)
                .ForeignKey("dbo.Status", t => t.Status_Id, cascadeDelete: true)
                .ForeignKey("dbo.StatusCetaks", t => t.StatusCetak_Id, cascadeDelete: true)
                .Index(t => t.Perusahaan_Id)
                .Index(t => t.NamaPerwakilan_Id)
                .Index(t => t.Jabatan_Id)
                .Index(t => t.Produk_Id)
                .Index(t => t.JenisTugas_Id)
                .Index(t => t.MataUang_Id)
                .Index(t => t.Status_Id)
                .Index(t => t.StatusCetak_Id);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Nama = c.String(),
                        Warna = c.String(),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransaksiTanggals",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Detail_Id = c.Long(nullable: false),
                        TglSuratBunga = c.DateTime(nullable: false),
                        NoKupon = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        NilaiBunga = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ModifiedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TransaksiDetails", t => t.Detail_Id, cascadeDelete: true)
                .Index(t => t.Detail_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nama = c.String(nullable: false),
                        NPP = c.String(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TransaksiTanggals", "Detail_Id", "dbo.TransaksiDetails");
            DropForeignKey("dbo.TransaksiDetails", "Trans_Id", "dbo.TransaksiLaporans");
            DropForeignKey("dbo.TransaksiLaporans", "StatusCetak_Id", "dbo.StatusCetaks");
            DropForeignKey("dbo.TransaksiLaporans", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.TransaksiLaporans", "Produk_Id", "dbo.Produk");
            DropForeignKey("dbo.TransaksiLaporans", "NamaPerwakilan_Id", "dbo.PerwakilanPerusahaan");
            DropForeignKey("dbo.TransaksiLaporans", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.TransaksiLaporans", "MataUang_Id", "dbo.MataUang");
            DropForeignKey("dbo.TransaksiLaporans", "JenisTugas_Id", "dbo.JenisTugas");
            DropForeignKey("dbo.TransaksiLaporans", "Jabatan_Id", "dbo.Jabatan");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PerwakilanPerusahaan", "Perusahaan_Id", "dbo.Perusahaan");
            DropForeignKey("dbo.PerwakilanPerusahaan", "Jabatan_Id", "dbo.Jabatan");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.TransaksiTanggals", new[] { "Detail_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "StatusCetak_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "Status_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "MataUang_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "JenisTugas_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "Produk_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "Jabatan_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "NamaPerwakilan_Id" });
            DropIndex("dbo.TransaksiLaporans", new[] { "Perusahaan_Id" });
            DropIndex("dbo.TransaksiDetails", new[] { "Trans_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.PerwakilanPerusahaan", new[] { "Jabatan_Id" });
            DropIndex("dbo.PerwakilanPerusahaan", new[] { "Perusahaan_Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TransaksiTanggals");
            DropTable("dbo.Status");
            DropTable("dbo.TransaksiLaporans");
            DropTable("dbo.TransaksiDetails");
            DropTable("dbo.StatusCetaks");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Produk");
            DropTable("dbo.PerwakilanPerusahaan");
            DropTable("dbo.Perusahaan");
            DropTable("dbo.MataUang");
            DropTable("dbo.JenisTugas");
            DropTable("dbo.Jabatan");
            DropTable("dbo.HariLibur");
        }
    }
}
