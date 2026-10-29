namespace BakkalApp
{
    public class Urun
    {
        public int Id { get; set; }
        // her ürün içn bir kimlik nurarası oluşturuyoruz
        public string Ad { get; set; } = string.Empty;
        //ürün adı BOS gözükmesin diye boş string(metın) atadım
        public decimal Fiyat { get; set; }
        //c# da para ondalıklı para birimleri için decimal kullanılıyormuş
        public int Stok { get; set; }
        //adet miktarinı görmek için 
    }
}