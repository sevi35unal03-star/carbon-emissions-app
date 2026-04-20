# Daily Activities — Flutter API Dokümantasyonu

Base URL: `https://<host>/api/v1/daily-activities`  
Auth: Tüm endpoint'ler `Bearer <access_token>` gerektirir.

---

## İş Kuralları

- Admin günde maksimum **50 soru** oluşturabilir
- Sorular kırılımlı yapıda olabilir — bir seçenek bir sonraki soruyu tetikler
- Kullanıcı bir soruyu cevapladıktan sonra o soru "Bugünün Soruları"ndan kalkar
- `previous-answers` sadece **en son cevaplanmış günü** döner
- Takvimde tüm geçmiş görülebilir

---

## Flutter Akışı

```
Ekran açılır
    ↓
GET /daily-activities/questions       → Bugünün cevaplanmamış soruları
GET /daily-activities/previous-answers → En son cevaplanmış günün cevapları
    ↓
Kullanıcı soruya tıklar
    ↓
POST /daily-activities/answers
    ↓
data.isFlowCompleted == false → data.nextQuestion'ı göster
data.isFlowCompleted == true  → "Puanı Al" ekranı, data.totalCarbonScore göster
    ↓
GET /daily-activities/questions       → Cevaplanmış soru artık gelmez
GET /daily-activities/previous-answers → Yeni cevap görünür
```

---

## 1. Bugünün Soruları

### `GET /daily-activities/questions`

Bugün aktif olan ve henüz cevaplanmamış **root soruları** döner.

**İş Kuralları:**
- Sadece `StartDate <= şimdi <= EndDate` olan sorular gelir
- Sadece hiçbir option'ın `NextQuestionId`'si olmayan sorular (root) gelir
- Kullanıcının bugün cevapladığı sorular listeden çıkar
- `remainingSeconds` → gece 00:00'a kalan süre (saniye)

**Response `200 OK`**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "text": "Bu sabah işe hangi ulaşım aracıyla gideceksiniz?",
      "displayOrder": 1,
      "options": [
        {
          "id": "opt-aaa",
          "text": "Toplu Ulaşım",
          "carbonValue": 25.0,
          "nextQuestionId": "4fa85f64-5717-4562-b3fc-2c963f66afa6"
        },
        {
          "id": "opt-bbb",
          "text": "Özel Araç",
          "carbonValue": -10.0,
          "nextQuestionId": null
        }
      ],
      "remainingSeconds": 57600
    }
  ]
}
```

**Tüm sorular cevaplanmışsa `200 OK` — boş liste**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": []
}
```

**Alan Açıklamaları**

| Alan | Tip | Açıklama |
|---|---|---|
| `id` | `Guid` | Soru ID'si |
| `text` | `string` | Soru metni |
| `displayOrder` | `int` | Gösterim sırası |
| `options[].id` | `Guid` | Seçenek ID'si |
| `options[].text` | `string` | Seçenek metni |
| `options[].carbonValue` | `double` | Karbon değeri |
| `options[].nextQuestionId` | `Guid?` | Sonraki soru ID'si — null ise flow biter |
| `remainingSeconds` | `long` | Gece 00:00'a kalan süre (saniye) |

---

## 2. Cevap Gönder

### `POST /daily-activities/answers`

Kullanıcının seçtiği option'ı kaydeder. Response'da sonraki soru veya flow tamamlandı bilgisi döner.

**İş Kuralları:**
- `nextQuestion != null` → kırılım devam ediyor, bir sonraki soruyu göster
- `nextQuestion == null` + `isFlowCompleted == true` → tüm akış tamamlandı
- `totalCarbonScore` → kullanıcının bugünkü toplam karbon skoru

**Request Body**
```json
{
  "questionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "selectedOptionId": "opt-aaa",
  "userId": "user-guid-buraya"
}
```

**Alan Açıklamaları — Request**

| Alan | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `questionId` | `Guid` | Evet | Cevaplanan sorunun ID'si |
| `selectedOptionId` | `Guid` | Evet | Seçilen option'ın ID'si |
| `userId` | `Guid` | Evet | Kullanıcı ID'si |

---

**Response `200 OK` — Akış devam ediyor**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": {
    "nextQuestion": {
      "id": "4fa85f64-5717-4562-b3fc-2c963f66afa6",
      "text": "Kullandığınız ulaşım aracını seçiniz.",
      "displayOrder": 2,
      "options": [
        {
          "id": "opt-ccc",
          "text": "Otobüs",
          "carbonValue": 0.3,
          "nextQuestionId": "5fa85f64-5717-4562-b3fc-2c963f66afa6"
        },
        {
          "id": "opt-ddd",
          "text": "Metro",
          "carbonValue": 0.1,
          "nextQuestionId": "5fa85f64-5717-4562-b3fc-2c963f66afa6"
        }
      ],
      "remainingSeconds": 57580
    },
    "totalCarbonScore": 25.0,
    "isFlowCompleted": false
  }
}
```

**Response `200 OK` — Akış tamamlandı**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": {
    "nextQuestion": null,
    "totalCarbonScore": 42.0,
    "isFlowCompleted": true
  }
}
```

**Alan Açıklamaları — Response**

| Alan | Tip | Açıklama |
|---|---|---|
| `nextQuestion` | `object?` | Sonraki soru — null ise akış bitti |
| `nextQuestion.id` | `Guid` | Sonraki sorunun ID'si |
| `nextQuestion.text` | `string` | Sonraki sorunun metni |
| `nextQuestion.options` | `array` | Sonraki sorunun seçenekleri |
| `nextQuestion.remainingSeconds` | `long` | Gece 00:00'a kalan süre |
| `totalCarbonScore` | `double` | Kullanıcının bugünkü toplam karbon skoru |
| `isFlowCompleted` | `bool` | true → "Puanı Al" ekranını göster |

---

## 3. Önceki Cevaplar

### `GET /daily-activities/previous-answers`

En son cevaplanmış günün cevaplarını döner.

**İş Kuralları:**
- Sadece **en son cevaplanmış 1 günün** cevapları döner
- `UserActivityLogs` tablosundan `ActivityDate`'e göre gruplandırılır
- Hiç cevap yoksa `404` döner

**Response `200 OK`**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": [
    {
      "date": "2026-04-15T00:00:00Z",
      "answers": [
        {
          "questionText": "Bu sabah işe hangi ulaşım aracıyla gideceksiniz?",
          "answerText": "Toplu Ulaşım",
          "score": 25.0,
          "date": "2026-04-15T08:30:00Z"
        },
        {
          "questionText": "Kullandığınız ulaşım aracını seçiniz.",
          "answerText": "Otobüs",
          "score": 10.0,
          "date": "2026-04-15T08:31:00Z"
        }
      ]
    }
  ]
}
```

**Response `404 Not Found` — Hiç cevap yok**
```json
{
  "isSuccessful": false,
  "statusCode": 404,
  "errors": [{ "code": "PreviousAnswersNotFound" }],
  "data": null
}
```

**Alan Açıklamaları**

| Alan | Tip | Açıklama |
|---|---|---|
| `date` | `DateTime` | Grubun tarihi (gün başı) |
| `answers[].questionText` | `string` | Sorunun metni |
| `answers[].answerText` | `string` | Seçilen option'ın metni |
| `answers[].score` | `double` | O cevap için karbon skoru |
| `answers[].date` | `DateTime` | Cevabın verildiği saat |

---

## 4. Bekleyen Sorular

### `GET /daily-activities?status=pending`

Kullanıcının bugün henüz cevaplamadığı soru sayısını döner.

**İş Kuralları:**
- Uygulama açılışında badge göstermek için çağrılır
- `hasPending == false` → bugün tüm sorular cevaplanmış

**Response `200 OK`**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": {
    "hasPending": true,
    "pendingCount": 2
  }
}
```

**Alan Açıklamaları**

| Alan | Tip | Açıklama |
|---|---|---|
| `hasPending` | `bool` | Cevaplanmamış soru var mı? |
| `pendingCount` | `int` | Cevaplanmamış soru sayısı |

---

## 5. Gün Detayı

### `GET /daily-activities?date=2026-04-15`

Takvimde seçilen güne ait cevap detaylarını döner.

**İş Kuralları:**
- `UserActivityLogs` tablosundan `ActivityDate`'e göre filtrelenir
- O gün hiç cevap yoksa `404` döner
- `totalScore` → o günün toplam karbon skoru

**Query Parameters**

| Parametre | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `date` | `DateTime` | Evet | `yyyy-MM-dd` formatında |

**Örnek:** `GET /daily-activities?date=2026-04-15`

**Response `200 OK`**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": {
    "date": "2026-04-15T00:00:00Z",
    "totalScore": 42.0,
    "activities": [
      {
        "questionText": "Bu sabah işe hangi ulaşım aracıyla gideceksiniz?",
        "selectedOptionText": "Toplu Ulaşım",
        "score": 25.0,
        "activityDate": "2026-04-15T08:30:00Z"
      },
      {
        "questionText": "Kullandığınız ulaşım aracını seçiniz.",
        "selectedOptionText": "Otobüs",
        "score": 17.0,
        "activityDate": "2026-04-15T08:31:00Z"
      }
    ]
  }
}
```

**Response `404 Not Found`**
```json
{
  "isSuccessful": false,
  "statusCode": 404,
  "errors": [{ "code": "ActivityNotFound" }],
  "data": null
}
```

**Alan Açıklamaları**

| Alan | Tip | Açıklama |
|---|---|---|
| `date` | `DateTime` | Seçilen gün |
| `totalScore` | `double` | Günün toplam karbon skoru |
| `activities[].questionText` | `string` | Soru metni |
| `activities[].selectedOptionText` | `string` | Seçilen option metni |
| `activities[].score` | `double` | O cevabın karbon skoru |
| `activities[].activityDate` | `DateTime` | Cevabın verildiği saat |

---

## 6. Takvim

### `GET /daily-activities/calendar`

Seçilen dönemdeki günlük karbon skorlarını döner.

**İş Kuralları:**
- `UserActivityAnswers` tablosundan `AnsweredAt`'e göre filtrelenir
- `month` girilmezse tüm yıl döner
- `period` girilirse: `1` = 1-15, `2` = 16-31 arası günler

**Query Parameters**

| Parametre | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `year` | `int` | Evet | Yıl |
| `month` | `int` | Hayır | Ay (1-12). Girilmezse tüm yıl döner |
| `period` | `int` | Hayır | `1` = 1-15, `2` = 16-31 |

**Örnek:** `GET /daily-activities/calendar?year=2026&month=4&period=1`

**Response `200 OK`**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": {
    "totalScore": 670.0,
    "items": [
      {
        "date": "2026-04-01T00:00:00Z",
        "score": 42.0,
        "hasDetails": true
      },
      {
        "date": "2026-04-03T00:00:00Z",
        "score": 18.5,
        "hasDetails": true
      }
    ]
  }
}
```

**Alan Açıklamaları**

| Alan | Tip | Açıklama |
|---|---|---|
| `totalScore` | `double` | Seçilen dönemin toplam skoru |
| `items[].date` | `DateTime` | Gün |
| `items[].score` | `double` | O günün toplam karbon skoru |
| `items[].hasDetails` | `bool` | Her zaman `true` — aktivite olan günler döner |

**Flutter akışı:**
- `hasDetails == true` olan günleri takvimde işaretle
- O güne tıklanınca `GET /daily-activities?date=...` çağır

---

## 7. Aylık Aktiviteler

### `GET /daily-activities/monthly`

"Tümünü Gör" ekranı için seçilen periyodun günlük skorlarını döner.

**İş Kuralları:**
- `period=1` → 1-15 arası günler
- `period=2` → 16-31 arası günler
- `totalMonthlyScore` → tüm ayın skoru (periyottan bağımsız)
- `totalPeriodScore` → sadece seçilen periyodun skoru
- O periyotta hiç kayıt yoksa `404` döner

**Query Parameters**

| Parametre | Tip | Zorunlu | Açıklama |
|---|---|---|---|
| `year` | `int` | Evet | Yıl |
| `month` | `int` | Evet | Ay (1-12) |
| `period` | `int` | Evet | `1` veya `2` |

**Örnek:** `GET /daily-activities/monthly?year=2026&month=4&period=1`

**Response `200 OK`**
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "errors": null,
  "data": {
    "totalMonthlyScore": 670.0,
    "totalPeriodScore": 335.0,
    "dailyScores": [
      {
        "date": "2026-04-01T00:00:00Z",
        "totalScore": 42.0
      },
      {
        "date": "2026-04-03T00:00:00Z",
        "totalScore": 18.5
      }
    ]
  }
}
```

**Response `404 Not Found` — Bu periyotta kayıt yok**
```json
{
  "isSuccessful": false,
  "statusCode": 404,
  "errors": [{ "code": "NoActivityFoundForPeriod" }],
  "data": null
}
```

**Alan Açıklamaları**

| Alan | Tip | Açıklama |
|---|---|---|
| `totalMonthlyScore` | `double` | Tüm ayın toplam skoru |
| `totalPeriodScore` | `double` | Seçilen periyodun skoru |
| `dailyScores[].date` | `DateTime` | Gün |
| `dailyScores[].totalScore` | `double` | O günün toplam skoru |

---

## Hata Kodları

| HTTP | Kod | Açıklama |
|---|---|---|
| `400` | `InvalidParameter` | Geçersiz veya eksik parametre |
| `401` | `Unauthorized` | Token yok veya geçersiz |
| `404` | `ActivityQuestionNotFound` | Soru bulunamadı |
| `404` | `ActivityNotFound` | O gün için aktivite yok |
| `404` | `PreviousAnswersNotFound` | Hiç cevap geçmişi yok |
| `404` | `NoActivityFoundForPeriod` | O periyotta kayıt yok |
| `500` | `SystemError` | Sunucu hatası |

**Hata response yapısı**
```json
{
  "isSuccessful": false,
  "statusCode": 404,
  "errors": [
    {
      "code": "ActivityNotFound",
      "message": "Bu gün için aktivite bulunamadı.",
      "isShow": true
    }
  ],
  "data": null
}
```

---

## Endpoint Özeti

| Endpoint | Method | Açıklama |
|---|---|---|
| `/daily-activities/questions` | GET | Bugünün cevaplanmamış soruları |
| `/daily-activities/answers` | POST | Cevap kaydet |
| `/daily-activities/previous-answers` | GET | En son cevaplanmış günün cevapları |
| `/daily-activities?status=pending` | GET | Bekleyen soru badge'i |
| `/daily-activities?date=yyyy-MM-dd` | GET | Seçilen günün detayı |
| `/daily-activities/calendar` | GET | Takvim — hangi günler dolu |
| `/daily-activities/monthly` | GET | Aylık periyod listesi |
