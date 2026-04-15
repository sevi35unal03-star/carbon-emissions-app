<%@ Page Title="Karbon Ayak İzi Hesapları" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="AdminKarbonAyakİzi.aspx.cs" Inherits="iztekadmin.AdminKarbonAyakİzi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .content-wrapper { padding: 30px; background: #f8fafc; min-height: 100vh; }
        .card { border-radius: 12px; border: 1px solid #e2e8f0; }
        .table th { font-size: 12px; text-transform: uppercase; letter-spacing: .05em; color: #64748b; font-weight: 600; }
        .table td { font-size: 13px; vertical-align: middle; }
        
        /* Detay Paneli Stilleri */
        .detay-panel { display: none; }
        .detay-panel.active { display: block; animation: fadeIn 0.3s ease; }
        
        .cevap-row { 
            background: #f8fafc; 
            border: 1px solid #e2e8f0; 
            border-radius: 8px; 
            padding: 12px 16px; 
            margin-bottom: 8px; 
            transition: transform 0.2s;
        }
        .cevap-row:hover { transform: translateX(5px); background: #f1f5f9; }
        .cevap-soru { font-size: 13px; color: #64748b; margin-bottom: 4px; }
        .cevap-cevap { font-size: 14px; font-weight: 500; color: #1e293b; }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(10px); }
            to { opacity: 1; transform: translateY(0); }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-wrapper">
        
        <div id="listePaneli">
            <div class="card shadow-sm p-4">
                <div class="d-flex justify-content-between align-items-center mb-4">
                    <div>
                        <h4 class="fw-bold mb-1">Karbon Ayak İzi Hesapları</h4>
                        <p class="text-muted small mb-0">Kullanıcıların yaptığı genel karbon ayak izi test sonuçları.</p>
                    </div>
                    <div class="d-flex gap-2">
                        <div class="input-group input-group-sm" style="width: 250px;">
                            <span class="input-group-text bg-white border-end-0"><i class="bi bi-search text-muted"></i></span>
                            <input type="text" class="form-control border-start-0" placeholder="Kullanıcı ara..." oninput="filtrele(this.value)">
                        </div>
                    </div>
                </div>

                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead class="table-light">
                            <tr>
                                <th>Kullanıcı Adı</th>
                                <th>Hesaplama Tarihi</th>
                                <th>Toplam Karbon Puanı</th>
                                <th>Durum / Seviye</th>
                                <th style="width: 50px;"></th>
                            </tr>
                        </thead>
                        <tbody id="tabloBody">
                            </tbody>
                    </table>
                </div>
            </div>
        </div>

        <div id="detayPaneli" class="detay-panel">
            <button type="button" class="btn btn-sm btn-outline-secondary mb-3 shadow-sm" onclick="geriDon()">
                <i class="bi bi-arrow-left"></i> Listeye Dön
            </button>
            
            <div class="card shadow-sm p-4">
                <div class="d-flex justify-content-between align-items-start mb-4">
                    <div class="d-flex align-items-center gap-3">
                        <div class="bg-light p-3 rounded-circle">
                            <i class="bi bi-person-bounding-box fs-3 text-primary"></i>
                        </div>
                        <div>
                            <h4 class="fw-bold mb-0" id="detayKullanici"></h4>
                            <small class="text-muted" id="detayTarih"></small>
                        </div>
                    </div>
                    <div class="text-end">
                        <div class="fs-3 fw-bold text-success" id="detayPuan"></div>
                        <span class="badge px-3 py-2" id="detaySeviye"></span>
                    </div>
                </div>
                
                <div class="alert alert-info border-0 bg-light mb-4">
                    <i class="bi bi-info-circle me-2"></i> Bu hesaplama, kullanıcının kayıtlı olan en son test verilerine dayanmaktadır.
                </div>

                <h6 class="fw-bold text-uppercase small letter-spacing-1 mb-3">Test Soruları ve Yanıtlar</h6>
                <div id="detayCevaplar"></div>
            </div>
        </div>

    </div>

    <script>
        const kayitlar = [
            {
                kullanici: "ahmet_yilmaz", tarih: "2025-04-13", puan: 42,
                cevaplar: [
                    { soru: "Günlük kaç km araç kullanıyorsunuz?", cevap: "10-30 km", puan: 15 },
                    { soru: "Evde kaç kişisiniz?", cevap: "4 kişi", puan: 8 },
                    { soru: "Aylık uçuş sayınız?", cevap: "0", puan: 0 },
                    { soru: "Et tüketim sıklığınız?", cevap: "Haftada 2-3 kez", puan: 12 },
                    { soru: "Geri dönüşüm yapıyor musunuz?", cevap: "Evet", puan: 7 }
                ]
            },
            {
                kullanici: "fatma_kaya", tarih: "2025-04-13", puan: 28,
                cevaplar: [
                    { soru: "Günlük kaç km araç kullanıyorsunuz?", cevap: "0-10 km", puan: 5 },
                    { soru: "Evde kaç kişisiniz?", cevap: "2 kişi", puan: 4 },
                    { soru: "Aylık uçuş sayınız?", cevap: "0", puan: 0 },
                    { soru: "Et tüketim sıklığınız?", cevap: "Hiç etmiyorum", puan: 0 },
                    { soru: "Geri dönüşüm yapıyor musunuz?", cevap: "Evet", puan: 7 }
                ]
            },
            {
                kullanici: "mehmet_celik", tarih: "2025-04-12", puan: 65,
                cevaplar: [
                    { soru: "Günlük kaç km araç kullanıyorsunuz?", cevap: "30+ km", puan: 25 },
                    { soru: "Evde kaç kişisiniz?", cevap: "3 kişi", puan: 6 },
                    { soru: "Aylık uçuş sayınız?", cevap: "1-2", puan: 20 },
                    { soru: "Et tüketim sıklığınız?", cevap: "Her gün", puan: 14 },
                    { soru: "Geri dönüşüm yapıyor musunuz?", cevap: "Hayır", puan: 0 }
                ]
            }
        ];

        function getSeviye(puan) {
            if (puan < 30) return { label: 'Düşük (İyi)', cls: 'bg-success' };
            if (puan < 55) return { label: 'Orta', cls: 'bg-warning text-dark' };
            return { label: 'Yüksek (Kritik)', cls: 'bg-danger' };
        }

        function render(liste) {
            const body = document.getElementById('tabloBody');
            if (liste.length === 0) {
                body.innerHTML = '<tr><td colspan="5" class="text-center py-4 text-muted">Kayıt bulunamadı.</td></tr>';
                return;
            }

            body.innerHTML = liste.map((k, i) => {
                const s = getSeviye(k.puan);
                // Orijinal dizideki indexi bulmak için kullanıyoruz
                const originalIndex = kayitlar.findIndex(item => item.kullanici === k.kullanici);
                return `
                <tr style="cursor:pointer" onclick="detayAc(${originalIndex})">
                    <td><i class="bi bi-person-circle text-primary me-2"></i><strong>${k.kullanici}</strong></td>
                    <td>${k.tarih}</td>
                    <td><span class="fw-bold">${k.puan}</span> <small class="text-muted">kg CO₂</small></td>
                    <td><span class="badge ${s.cls}">${s.label}</span></td>
                    <td class="text-end"><i class="bi bi-chevron-right text-muted"></i></td>
                </tr>`;
            }).join('');
        }

        function filtrele(ara) {
            const filtrelenmis = kayitlar.filter(k => k.kullanici.toLowerCase().includes(ara.toLowerCase()));
            render(filtrelenmis);
        }

        function detayAc(index) {
            const k = kayitlar[index];
            const s = getSeviye(k.puan);
            
            document.getElementById('detayKullanici').textContent = "@" + k.kullanici;
            document.getElementById('detayTarih').textContent = "Hesaplama Tarihi: " + k.tarih;
            document.getElementById('detayPuan').textContent = k.puan + ' kg CO₂';
            
            const sevEl = document.getElementById('detaySeviye');
            sevEl.textContent = s.label + ' Karbon Ayak İzi';
            sevEl.className = 'badge ' + s.cls;
            
            document.getElementById('detayCevaplar').innerHTML = k.cevaplar.map(c => `
                <div class="cevap-row d-flex justify-content-between align-items-center shadow-sm">
                    <div>
                        <div class="cevap-soru"><i class="bi bi-question-circle me-1"></i> ${c.soru}</div>
                        <div class="cevap-cevap">${c.cevap}</div>
                    </div>
                    <span class="badge rounded-pill bg-white text-dark border px-3">${c.puan} Puan</span>
                </div>`).join('');
            
            document.getElementById('listePaneli').style.display = 'none';
            document.getElementById('detayPaneli').classList.add('active');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        function geriDon() {
            document.getElementById('detayPaneli').classList.remove('active');
            document.getElementById('listePaneli').style.display = 'block';
        }

        // Başlangıçta listeyi yükle
        window.addEventListener('load', () => render(kayitlar));
    </script>
</asp:Content>