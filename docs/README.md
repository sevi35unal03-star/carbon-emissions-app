# Karbon Ayak İzi API Dokümantasyonu

**Base URL:** `https://izt0001.izmirteknoloji.com.tr`  
**API Version:** `v1`  
**Auth:** JWT Bearer Token

---

## İçindekiler

- [Auth](#auth)
- [Ana Sayfa](#ana-sayfa)
- [Liderlik Tablosu](#liderlik-tablosu)
- [Günlük Aktiviteler](#günlük-aktiviteler)
- [Karbon Hesapla (Anket)](#karbon-hesapla-anket)
- [Faydalı Bilgiler](#faydalı-bilgiler)
- [Profil](#profil)
- [Hedefler (Admin)](#hedefler-admin)

---

## Genel Kurallar

### Authentication
Tüm korumalı endpoint'ler `Authorization: Bearer <token>` header'ı gerektirir.

### Response Formatı
```json
{
  "isSuccess": true,
  "data": { },
  "errors": []
}
```

### HTTP Status Kodları
| Kod | Açıklama |
|-----|----------|
| 200 | Başarılı |
| 201 | Oluşturuldu |
| 400 | Geçersiz istek |
| 401 | Yetkisiz |
| 403 | Erişim engellendi |
| 404 | Bulunamadı |
| 409 | Çakışma |
| 500 | Sunucu hatası |
