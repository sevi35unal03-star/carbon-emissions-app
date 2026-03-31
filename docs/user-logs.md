# User Logs (Admin)

## İş Kuralları
- Yalnızca admin erişebilir
- Tüm CRUD işlemleri otomatik loglanır
- Sayfalama zorunludur
- Kayıtlar en yeniden eskiye sıralanır

---

## Audit Logları Getir

**GET** `/api/v1/user-logs`  
🔒 Admin

### Query Parameters
| Parametre | Tip | Zorunlu | Açıklama |
|-----------|-----|---------|----------|
| pageNumber | int | Evet | Sayfa numarası (başlangıç: 1) |
| pageSize | int | Evet | Sayfa başına kayıt sayısı |

### Response `200`
```json
{
  "isSuccess": true,
  "data": [
    {
      "userName": "admin@iztek.com",
      "operation": "Ekleme",
      "tableName": "PollSets",
      "createdAt": "2024-01-15T10:30:00Z",
      "details": "Eski: {} -> Yeni: {\"Name\": \"Ocak 2024 Anketi\"}"
    },
    {
      "userName": "kullanici@ornek.com",
      "operation": "Güncelleme",
      "tableName": "Users",
      "createdAt": "2024-01-14T09:15:00Z",
      "details": "Eski: {\"Name\": \"Ali\"} -> Yeni: {\"Name\": \"Ali Can\"}"
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 10
}
```
