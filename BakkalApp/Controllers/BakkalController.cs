using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BakkalApp.Controllers
{
    // [ApiController]: Bu sınıfın bir Web API Controller'ı olduğunu .NET'e bildirir.
    // [Route]: Dışarıdan bu controller'a nasıl ulaşılacağını belirler (http://localhost:5169/api/bakkal).
    [ApiController]
    [Route("api/[controller]")]
    public class BakkalController : ControllerBase
    {
        // SQL Veritabanı ile haberleşmemizi sağlayan DbContext nesnemiz.
        private readonly BakkalDbContext _context;

        // Dependency Injection (Bağımlılık Enjeksiyonu): 
        // Program.cs'te tanımladığımız BakkalDbContext'i buraya otomatik olarak çağırıyoruz.
        public BakkalController(BakkalDbContext context)
        {
            _context = context;
        }

        // 1. RAFTAKİ / VERİTABANINDAKİ TÜM ÜRÜNLERİ GETİR
        [HttpGet]
        public async Task<IActionResult> TumUrunleriGetir()
        {
            // SQL'deki "Urunler" tablosuna gider ve tüm kayıtları liste olarak çeker (SELECT * FROM Urunler)
            var urunler = await _context.Urunler.ToListAsync();
            return Ok(urunler);
        }

        // 2. VERİTABANINA YENİ ÜRÜN EKLE
        [HttpPost]
        public async Task<IActionResult> UrunEkle(Urun yeniUrun)
        {
            // Gelen ürünü veritabanı hazinesine ekler
            _context.Urunler.Add(yeniUrun);

            // Yapılan değişikliği SQL veritabanına fiziksel olarak kaydeder (COMMIT)
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Ürün veritabanına başarıyla eklendi!", urun = yeniUrun });
        }

        // 3. SATIŞ YAP (VERİTABANINDAKİ STOKTAN DÜŞ)
        [HttpPut("{id}/satis-yap")]
        public async Task<IActionResult> SatisYap(int id, int adet)
        {
            // Gönderilen ID'ye sahip ürünü SQL veritabanında arar
            var urunOgesi = await _context.Urunler.FindAsync(id);

            if (urunOgesi == null)
                return NotFound("Ürün bulunamadı!");

            if (urunOgesi.Stok < adet)
                return BadRequest("Yetersiz stok!");

            // Stok miktarını düşürür
            urunOgesi.Stok -= adet;

            // Güncellenen yeni stoku SQL veritabanına kaydeder
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = $"{adet} adet {urunOgesi.Ad} satıldı. Kalan stok: {urunOgesi.Stok}" });
        }

        // 4. VERİTABANINDAN ÜRÜN SİL
        [HttpDelete("{id}")]
        public async Task<IActionResult> UrunSil(int id)
        {
            // Silinecek ürünü ID ile veritabanında arıyoruz
            var urunOgesi = await _context.Urunler.FindAsync(id);

            // Ürün veritabanında yoksa 404 Not Found dönüyoruz
            if (urunOgesi == null)
                return NotFound("Silinecek ürün bulunamadı!");

            // Ürünü veritabanı hazinesinden kaldırıyoruz
            _context.Urunler.Remove(urunOgesi);

            // Değişikliği SQL veritabanına fiziksel olarak işliyoruz
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = $"{urunOgesi.Ad} ürünü veritabanından başarıyla silindi!" });
        }

        // 5. ÜRÜN FİYATI GÜNCELLE
        [HttpPut("{id}/fiyat-guncelle")]
        public async Task<IActionResult> FiyatGuncelle(int id, decimal yeniFiyat)
        {
            // Fiyatı değişecek ürünü veritabanında buluyoruz
            var urunOgesi = await _context.Urunler.FindAsync(id);

            if (urunOgesi == null)
                return NotFound("Ürün bulunamadı!");

            if (yeniFiyat <= 0)
                return BadRequest("Fiyat 0 veya negatif olamaz!");

            // Eski fiyatı unutmamak için kenara not alıp yeni fiyatı atıyoruz
            decimal eskiFiyat = urunOgesi.Fiyat;
            urunOgesi.Fiyat = yeniFiyat;

            // Değişikliği veritabanına kaydediyoruz
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = $"{urunOgesi.Ad} ürününün fiyatı {eskiFiyat} TL yerine {yeniFiyat} TL olarak güncellendi." });
        }
    }
}