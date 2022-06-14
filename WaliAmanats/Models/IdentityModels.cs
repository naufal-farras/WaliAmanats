using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WaliAmanats.Models.Master;
using WaliAmanats.Models.Transaksi;
using System.ComponentModel.DataAnnotations;

namespace WaliAmanats.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Nama Lengkap")]
        public string Nama { get; set; }

        [Required]
        [Display(Name = "NPP")]
        public string NPP { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
   
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("WaliAmanatApps", throwIfV1Schema: false)
        {
        }
        public DbSet<Perusahaan> Perusahaan { get; set; } 
        public DbSet<PerwakilanPerusahaan> Perwakilan { get; set; }
        public DbSet<MataUang> MataUang { get; set; }
        public DbSet<Produk> Produk { get; set; }
        public DbSet<JenisTugas> JenisTugas { get; set; }
        public DbSet<HariLibur> HariLibur { get; set; }
        public DbSet<TransaksiLaporan> TransaksiLaporan { get; set; }
        public DbSet<TransaksiTanggal> TransaksiTanggal { get; set; }
        public DbSet<Jabatan> Jabatan { get; set; }
        public DbSet<TransaksiDetail> TransaksiDetail { get; set; }
        public DbSet<StatusCetak> StatusCetak { get; set; }
        public DbSet<TransaksiCetak> TransaksiCetak { get; set; }
        public DbSet<DetailCetak> DetailCetak { get; set; }
        public DbSet<Periode> Periode { get; set; }
        public DbSet<SubDetailCetak> SubDetailCetak { get; set; }
        public DbSet<SubDetailTanggal> SubDetailTanggal { get; set; }
        public DbSet<OJK> OJK { get; set; }
        public DbSet<AturanOJK> AturanOJK { get; set; }
        public DbSet<EmitenNotes> EmitenNotes { get; set; }
        public DbSet<Jaminan> Jaminan { get; set; }
        public DbSet<Rating> Rating { get; set; }
        public DbSet<Ratio> Ratio { get; set; }      
        public DbSet<Measurement> Measurement { get; set; }
        public DbSet<Matching> Matching { get; set; }
        public DbSet<TransaksiRating> TransaksiRating { get; set; }
        public DbSet<TransaksiJaminan> TransaksiJaminan { get; set; }
        public DbSet<TransaksiKeuangan> TransaksiKeuangan { get; set; }
        public DbSet<DetailKeuangan> DetailKeuangan { get; set; }
        public DbSet<DetailPerusahaan> DetailPerusahaan { get; set; }
        public DbSet<KopSurat> KopSurat { get; set; }
        public DbSet<SettingRatio> SettingRatio { get; set; }
        public DbSet<TransaksiPenggunaanDana> TransaksiPenggunaanDana { get; set; }
        public DbSet<KejadianPenting> KejadianPenting { get; set; }
        public DbSet<TransaksiKejadian> TransaksiKejadian { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}