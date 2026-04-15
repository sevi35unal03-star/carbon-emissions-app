<%@ Page Title="Günlük Cevaplar" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="AdminGunlukCevaplar.aspx.cs" Inherits="iztekadmin.AdminGunlukCevaplar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .content-wrapper { padding: 30px; background: #f8fafc; min-height: 100vh; }
        .card { border-radius: 12px; border: 1px solid #e2e8f0; }
        .table th { font-size: 12px; text-transform: uppercase; letter-spacing: .05em; color: #64748b; font-weight: 600; }
        .table td { font-size: 13px; vertical-align: middle; }
        .detay-link { color: #0ea5e9; cursor: pointer; text-decoration: underline; font-size: 13px; }

        /* Takvim Modal */
        .modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,.5); display: none; align-items: center; justify-content: center; z-index: 1050; }
        .modal-overlay.open { display: flex; }
        .modal-box { background: white; border-radius: 14px; padding: 24px; width: 700px; max-width: 95vw; max-height: 90vh; overflow-y: auto; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1); }

        /* Takvim */
        .cal-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; }
        .cal-grid { display: grid; grid-template-columns: repeat(7, 1fr); gap: 4px; }
        .cal-day-name { text-align: center; font-size: 11px; font-weight: 600; color: #94a3b8; padding: 4px 0; }
        .cal-day { text-align: center; padding: 8px 4px; border-radius: 8px; font-size: 13px; cursor: pointer; border: 1px solid transparent; transition: all .15s; min-height: 38px; }
        .cal-day:hover { background: #f0f9ff; border-color: #bae6fd; }
        .cal-day.has-data { background: #dcfce7; color: #166534; font-weight: 600; }
        .cal-day.selected { background: #0ea5e9 !important; color: white !important; }

        /* Yıllık görünüm */
        .year-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 12px; }
        .month-card { border: 1px solid #e2e8f0; border-radius: 10px; padding: 12px; cursor: pointer; }
        .month-card:hover { border-color: #0ea5e9; background: #f8fafc; }
        .month-mini { display: grid; grid-template-columns: repeat(7, 1fr); gap: 1px; }
        .month-mini-day { width: 100%; aspect-ratio: 1; border-radius: 2px; font-size: 8px; display: flex; align-items: center; justify-content: center; color: #94a3b8; }
        .month-mini-day.has-data { background: #dcfce7; color: #166534; }

        .cevap-row { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 12px 16px; margin-bottom: 8px; display: flex; justify-content: space-between; align-items: center; }
        .detay-panel { display: none; }
        .detay-panel.open { display: block; }
        
        @media (max-width: 768px) { .year-grid { grid-template-columns: 1fr 1fr; } }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-wrapper">
        <div class="card shadow-sm p-4">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h4 class="fw-bold mb-1">Günlük Cevaplar</h4>
                    <p class="text-muted small mb-0">Kullanıcıların günlük aktivite ve puan takibini buradan yapabilirsiniz.</p>
                </div>
                <div class="d-flex gap-2">
                    <input type="text" class="form-control form-control-sm" style="width:200px" placeholder="Kullanıcı ara..." oninput="filtrele(this.value)">
                </div>
            </div>

            <div class="table-responsive">
                <table class="table table-hover border-top">
                    <thead>
                        <tr>
                            <th>Kullanıcı</th>
                            <th>Son Giriş</th>
                            <th>Karbon Ayak İzi</th>
                            <th>Aktiviteler</th>
                            <th>Toplam Puan</th>
                            <th>Ağaç Bağışı</th>
                        </tr>
                    </thead>
                    <tbody id="tabloBody"></tbody>
                </table>
            </div>
        </div>
    </div>

    <div class="modal-overlay" id="calModal">
        <div class="modal-box">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                    <h5 class="fw-bold mb-0" id="calKullanici"></h5>
                    <span class="badge bg-light text-dark border">Aktivite Geçmişi</span>
                </div>
                <button type="button" class="btn-close" onclick="closeModal()"></button>
            </div>

            <div class="d-flex gap-2 align-items-center mb-3 flex-wrap bg-light p-2 rounded">
                <div class="btn-group btn-group-sm">
                    <button type="button" class="btn btn-primary" id="btnAylik" onclick="setView('monthly')">Aylık</button>
                    <button type="button" class="btn btn-outline-primary" id="btnYillik" onclick="setView('yearly')">Yıllık</button>
                </div>
                <div class="d-flex align-items-center gap-1 ms-auto">
                    <input type="date" class="form-control form-control-sm" id="filtreBaslangic" onchange="renderCal()">
                    <span class="small">-</span>
                    <input type="date" class="form-control form-control-sm" id="filtreBitis" onchange="renderCal()">
                    <button type="button" class="btn btn-sm btn-link text-decoration-none" onclick="clearFilter()">Sıfırla</button>
                </div>
            </div>

            <div id="calArea"></div>

            <div class="detay-panel mt-4" id="gunDetay">
                <div class="d-flex justify-content-between align-items-center p-2 border-bottom mb-3">
                    <h6 class="fw-bold mb-0" id="gunBaslik"></h6>
                    <span class="badge bg-primary" id="gunToplamPuan"></span>
                </div>
                <div id="gunCevaplar"></div>
            </div>
        </div>
    </div>

    <script>
        const AYLAR = ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"];
        const GUNLER = ["Pzt", "Sal", "Çar", "Per", "Cum", "Cmt", "Paz"];

        // Örnek Veri
        const kullanicilar = [
            {
                kullanici: "ahmet_yilmaz", giris: "2025-04-13", karbon: 42, toplamPuan: 320, agac: 15, agacPuan: 750,
                aktiviteler: {
                    "2025-04-13": [{ soru: "Toplu taşıma kullandınız mı?", cevap: "Evet", puan: 10 }, { soru: "Geri dönüşüm yaptınız mı?", cevap: "Evet", puan: 8 }],
                    "2025-04-12": [{ soru: "Bisiklet kullandınız mı?", cevap: "Hayır", puan: 0 }],
                    "2025-04-10": [{ soru: "Su tasarrufu yaptınız mı?", cevap: "Evet", puan: 6 }]
                }
            },
            {
                kullanici: "fatma_kaya", giris: "2025-04-13", karbon: 28, toplamPuan: 510, agac: 25, agacPuan: 1250,
                aktiviteler: {
                    "2025-04-13": [{ soru: "Vejetaryen beslendiniz mi?", cevap: "Evet", puan: 12 }],
                    "2025-04-11": [{ soru: "Bisiklet kullandınız mı?", cevap: "Evet", puan: 10 }]
                }
            }
        ];

        let aktifKullanici = null;
        let calView = 'monthly';
        let calYear = 2026; // Güncel yıl
        let calMonth = 3; 

        function filtrele(ara) {
            const liste = kullanicilar.filter(k => k.kullanici.toLowerCase().includes(ara.toLowerCase()));
            renderTablo(liste);
        }

        function renderTablo(liste) {
            document.getElementById('tabloBody').innerHTML = liste.map(k => `
                <tr>
                    <td><strong>${k.kullanici}</strong></td>
                    <td>${k.giris}</td>
                    <td><span class="badge ${k.karbon < 30 ? 'bg-success' : 'bg-warning text-dark'}">${k.karbon} kg CO₂</span></td>
                    <td><span class="detay-link" onclick="openModal('${k.kullanici}')"><i class="bi bi-calendar3"></i> Detay</span></td>
                    <td><span class="fw-bold text-primary">${k.toplamPuan}</span></td>
                    <td><span class="text-success"><i class="bi bi-tree-fill"></i> ${k.agac}</span> <small class="text-muted">(${k.agacPuan}p)</small></td>
                </tr>`).join('');
        }

        function openModal(kullanici) {
            aktifKullanici = kullanicilar.find(k => k.kullanici === kullanici);
            document.getElementById('calKullanici').textContent = "@" + aktifKullanici.kullanici;
            document.getElementById('gunDetay').classList.remove('open');
            setView('monthly');
            document.getElementById('calModal').classList.add('open');
        }

        function closeModal() { document.getElementById('calModal').classList.remove('open'); }

        function setView(v) {
            calView = v;
            document.getElementById('btnAylik').className = v === 'monthly' ? 'btn btn-primary' : 'btn btn-outline-primary';
            document.getElementById('btnYillik').className = v === 'yearly' ? 'btn btn-primary' : 'btn btn-outline-primary';
            renderCal();
        }

        function renderCal() {
            calView === 'monthly' ? renderMonthly() : renderYearly();
        }

        function renderMonthly() {
            const aktiviteler = aktifKullanici.aktiviteler;
            let html = `
                <div class="cal-header">
                    <button type="button" class="btn btn-sm btn-light border" onclick="degisAy(-1)"><i class="bi bi-chevron-left"></i></button>
                    <strong class="fs-5">${AYLAR[calMonth]} ${calYear}</strong>
                    <button type="button" class="btn btn-sm btn-light border" onclick="degisAy(1)"><i class="bi bi-chevron-right"></i></button>
                </div>
                <div class="cal-grid">
                    ${GUNLER.map(g => `<div class="cal-day-name">${g}</div>`).join('')}`;

            const ilk = new Date(calYear, calMonth, 1);
            const sonGun = new Date(calYear, calMonth + 1, 0).getDate();
            let bosluk = (ilk.getDay() + 6) % 7;
            
            for (let i = 0; i < bosluk; i++) html += `<div class="cal-day empty"></div>`;
            for (let g = 1; g <= sonGun; g++) {
                const dateStr = `${calYear}-${String(calMonth+1).padStart(2,'0')}-${String(g).padStart(2,'0')}`;
                const hasData = aktiviteler[dateStr];
                html += `<div class="cal-day ${hasData ? 'has-data' : ''}" onclick="gunSec('${dateStr}')">${g}</div>`;
            }
            html += `</div>`;
            document.getElementById('calArea').innerHTML = html;
        }

        function degisAy(n) {
            calMonth += n;
            if(calMonth < 0) { calMonth = 11; calYear--; }
            if(calMonth > 11) { calMonth = 0; calYear++; }
            renderCal();
        }

        function gunSec(dateStr) {
            const aktiviteler = aktifKullanici.aktiviteler;
            const panel = document.getElementById('gunDetay');
            if (!aktiviteler[dateStr]) { panel.classList.remove('open'); return; }

            const veriler = aktiviteler[dateStr];
            const toplam = veriler.reduce((t, c) => t + c.puan, 0);
            document.getElementById('gunBaslik').textContent = dateStr;
            document.getElementById('gunToplamPuan').textContent = toplam + " Puan";
            document.getElementById('gunCevaplar').innerHTML = veriler.map(c => `
                <div class="cevap-row">
                    <div><div class="small text-muted">${c.soru}</div><div class="fw-bold">${c.cevap}</div></div>
                    <span class="badge ${c.puan >= 0 ? 'bg-success' : 'bg-danger'}">${c.puan > 0 ? '+' : ''}${c.puan}</span>
                </div>`).join('');
            panel.classList.add('open');
        }

        function clearFilter() {
            document.getElementById('filtreBaslangic').value = '';
            document.getElementById('filtreBitis').value = '';
            renderCal();
        }

        window.addEventListener('load', () => renderTablo(kullanicilar));
    </script>
</asp:Content>