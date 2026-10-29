using Microsoft.EntityFrameworkCore;

namespace BakkalApp
{
    public class BakkalDbContext : DbContext
    {
        public BakkalDbContext(DbContextOptions<BakkalDbContext> options) : base(options)
        // Veritabanı bağlantı ayarlarını (Connection String) dışarıdan (Program.cs'ten) almak için bu kurucu metodu (Constructor) yazdık.
        // kısaca diger parametereleri alıp base class'a gönderiyoruz.
        {
        }

        // SQL veritabanımızda 'Urunler' adında bir tablo olusturucaz
        public DbSet<Urun> Urunler { get; set; }
    }
}
//*** veri tabanını fiziksel olarak olusturmak için araclar NuGet paket yöneticisi ardından paket yömneticsi konsolu açıp aşağıdaki komutu çalıştırıyoruz=
//*** Add-Migration IlkBakkalTablosu yaazıp enter tuşuna basıyoruz ardından Update-Database yazıp enter tuşuna basıyoruz. Bu sayede veri tabanımız fiziksel olarak olusturulmuş oluyoruz.