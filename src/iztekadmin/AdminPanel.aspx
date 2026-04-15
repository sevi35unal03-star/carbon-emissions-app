<!DOCTYPE html>
<html lang="tr">
<head>
<meta charset="UTF-8">
<title>Admin Panel</title>

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/css/bootstrap.min.css" rel="stylesheet">
<link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">

<style>
body {
    background-color: #f5f7fa;
}

.sidebar {
    height: 100vh;
    background: #1e293b;
    color: white;
    padding: 20px;
}

.sidebar h5 {
    color: #38bdf8;
}

.sidebar a {
    color: #cbd5e1;
    text-decoration: none;
    display: block;
    padding: 8px;
    border-radius: 6px;
}

.sidebar a:hover {
    background: #334155;
    color: white;
}

.sub-menu {
    margin-left: 15px;
}

.content {
    padding: 30px;
}

.card {
    border-radius: 12px;
}
</style>

</head>

<body>
<div class="container-fluid">
    <div class="row">

       <!-- SIDEBAR -->
        <div class="col-md-3 col-lg-2 sidebar">

            <h5 class="mb-4">Admin Panel</h5>

            <!-- Tanımlamalar -->
            <a data-bs-toggle="collapse" href="#tanımlamalar">
                <i class="bi bi-gear"></i> Tanımlamalar
            </a>

            <div class="collapse show sub-menu" id="tanımlamalar">

                <a data-bs-toggle="collapse" href="#soru">
                    ➤ Soru Tanımlamaları
                </a>

                <div class="collapse show sub-menu" id="soru">
                    <a href="#">• Soru Ekle/Düzenle</a>
                    <a href="#">• Anket Ekle</a>
                </div>

                <a data-bs-toggle="collapse" href="#genel">
                    ➤ Genel Tanımlamalar
                </a>

                <div class="collapse sub-menu" id="genel">
                    <a href="#">• Puanlama Ayarları</a>
                </div>

                <a data-bs-toggle="collapse" href="#faydali">
                    ➤ Faydalı Bilgiler
                </a>

                <div class="collapse sub-menu" id="faydali">
                    <a href="#">• Bilgi Ekle/Düzenle</a>
                </div>

            </div>

            <hr class="text-secondary">

            <a href="#"><i class="bi bi-calendar-check"></i> Günlük Cevaplar</a>
            <a href="#"><i class="bi bi-leaf"></i> Karbon Ayak İzi Hesapları</a>
            <a href="#"><i class="bi bi-bar-chart"></i> Raporlar</a>
            <a href="#"><i class="bi bi-journal-text"></i> Log Kayıtları</a>

        </div>

        <!-- CONTENT -->
        <div class="col-md-9 col-lg-10 content">

            <div class="card shadow-sm p-4">

                <h4 class="mb-3">Hoş Geldiniz 👋</h4>

                <p>Sol menüden işlem seçebilirsiniz.</p>

                <!-- Örnek Dashboard Kartları -->
                <div class="row mt-4">

                    <div class="col-md-3">
                        <div class="card text-center p-3 shadow-sm">
                            <h6>Günlük Cevap</h6>
                            <h4>124</h4>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="card text-center p-3 shadow-sm">
                            <h6>Anket Sayısı</h6>
                            <h4>8</h4>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="card text-center p-3 shadow-sm">
                            <h6>Kullanıcı</h6>
                            <h4>56</h4>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="card text-center p-3 shadow-sm">
                            <h6>Raporlar</h6>
                            <h4>12</h4>
                        </div>
                    </div>

                </div>

            </div>

        </div>

    </div>
</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/js/bootstrap.bundle.min.js"></script>

</body>
</html>