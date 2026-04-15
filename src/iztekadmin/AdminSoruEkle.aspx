<%@ Page Title="Soru Yönetimi" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="AdminSoruEkle.aspx.cs" Inherits="iztekadmin.AdminSoruEkle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .content-wrapper { padding: 30px; }
        .form-panel { 
            background: white; 
            border: 1px solid #e2e8f0; 
            border-radius: 12px; 
            padding: 24px; 
            margin-bottom: 20px; 
            display: none; 
            box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);
        }
        .form-panel.active { display: block; animation: slideDown 0.3s ease-out; }
        .soru-item { 
            background: white; 
            border: 1px solid #e2e8f0; 
            border-radius: 10px; 
            padding: 16px; 
            margin-bottom: 12px; 
            transition: all 0.2s;
        }
        .soru-item:hover { border-color: #38bdf8; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
        .soru-text { font-weight: 600; color: #1e293b; margin-bottom: 8px; font-size: 15px; }
        .secenekler { font-size: 13px; color: #64748b; }
        .secenek-row { display: flex; gap: 8px; align-items: center; margin-bottom: 10px; }
        
        @keyframes slideDown {
            from { opacity: 0; transform: translateY(-10px); }
            to { opacity: 1; transform: translateY(0); }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-wrapper">
        
        <div class="form-panel" id="soruForm">
            <div class="d-flex justify-content-between align-items-center mb-4 pb-2 border-bottom">
                <h5 class="fw-bold mb-0 text-primary" id="formBaslik">Yeni Soru Tanımla</h5>
                <button type="button" class="btn-close" onclick="formKapat()"></button>
            </div>

            <input type="hidden" id="duzenleIndex">

            <div class="mb-4">
                <label class="form-label fw-bold small text-uppercase text-muted">Soru Metni</label>
                <textarea class="form-control border-2" id="soruMetin" rows="2" placeholder="Kullanıcıya sorulacak soruyu giriniz..."></textarea>
            </div>

            <div class="mb-4">
                <label class="form-label fw-bold small text-uppercase text-muted">Seçenekler ve Puanlama</label>
                <div id="seceneklerListesi"></div>
                <button type="button" class="btn btn-sm btn-outline-primary mt-2" onclick="secenekEkle()">
                    <i class="bi bi-plus-circle me-1"></i> Seçenek Ekle
                </button>
            </div>

            <hr class="my-4">

            <div class="row g-3">
                <div class="col-md-2">
                    <label class="form-label fw-bold small text-muted">SIRA NO</label>
                    <input type="number" class="form-control" id="siraNo">
                </div>
                <div class="col-md-3">
                    <label class="form-label fw-bold small text-muted">BAŞLANGIÇ</label>
                    <input type="date" class="form-control" id="basTarih">
                </div>
                <div class="col-md-3">
                    <label class="form-label fw-bold small text-muted">BİTİŞ</label>
                    <input type="date" class="form-control" id="bitTarih">
                </div>
                <div class="col-md-2">
                    <label class="form-label fw-bold small text-muted">YAYIN SAATİ</label>
                    <input type="time" class="form-control" id="saat">
                </div>
            </div>

            <div class="mt-4 d-flex gap-2">
                <button type="button" class="btn btn-primary px-4 shadow-sm" onclick="soruKaydet()">
                    <i class="bi bi-save me-2"></i>Değişiklikleri Kaydet
                </button>
                <button type="button" class="btn btn-light border px-4" onclick="formKapat()">İptal</button>
            </div>
        </div>

        <div class="card shadow-sm border-0">
            <div class="card-body p-4">
                <div class="d-flex justify-content-between align-items-center mb-4">
                    <div>
                        <h4 class="fw-bold mb-1">Soru Bankası</h4>
                        <p class="text-muted small mb-0">Karbon ayak izi hesaplamasında kullanılan aktif sorular.</p>
                    </div>
                    <button type="button" class="btn btn-success px-3 shadow-sm" onclick="yeniSoruAc()">
                        <i class="bi bi-plus-lg me-2"></i>Yeni Soru Ekle
                    </button>
                </div>

                <div id="soruListesi">
                    </div>
            </div>
        </div>
    </div>

    <script>
        let sorular = [
            { metin: "Günlük kaç saat dışarıda vakit geçiriyorsunuz?", secenekler: [{metin:"1 saatten az", puan:1},{metin:"1-3 saat", puan:2},{metin:"3 saatten fazla", puan:3}], sira: 1, bas: "2025-01-01", bit: "2025-12-31", saat: "08:00" },
            { metin: "Toplu taşıma kullanıyor musunuz?", secenekler: [{metin:"Evet", puan:3},{metin:"Hayır", puan:0}], sira: 2, bas: "2025-01-01", bit: "2025-12-31", saat: "09:00" }
        ];

        function listele() {
            const el = document.getElementById('soruListesi');
            if (!sorular.length) { 
                el.innerHTML = `
                <div class="text-center py-5 border rounded-3 bg-light">
                    <i class="bi bi-journal-x fs-1 text-muted"></i>
                    <p class="text-muted mt-2">Henüz tanımlanmış bir soru bulunmuyor.</p>
                </div>`; 
                return; 
            }
            
            el.innerHTML = sorular.map((s, i) => `
                <div class="soru-item shadow-sm">
                    <div class="d-flex justify-content-between align-items-start">
                        <div class="flex-grow-1">
                            <div class="soru-text"><span class="badge bg-secondary me-2">${s.sira}</span> ${s.metin}</div>
                            <div class="secenekler d-flex flex-wrap gap-2">
                                ${s.secenekler.map(o => `<span class="badge bg-light text-dark border"><i class="bi bi-dot"></i> ${o.metin} (${o.puan} Puan)</span>`).join('')}
                            </div>
                            <div class="text-muted small mt-2">
                                <i class="bi bi-calendar-event me-1"></i> ${s.bas} / ${s.bit} <i class="bi bi-clock ms-2 me-1"></i> ${s.saat}
                            </div>
                        </div>
                        <div class="ms-3 d-flex gap-2">
                            <button type="button" class="btn btn-sm btn-outline-primary" onclick="duzenle(${i})"><i class="bi bi-pencil-square"></i></button>
                            <button type="button" class="btn btn-sm btn-outline-danger" onclick="sil(${i})"><i class="bi bi-trash"></i></button>
                        </div>
                    </div>
                </div>`).join('');
        }

        function yeniSoruAc() {
            document.getElementById('formBaslik').textContent = 'Yeni Soru Tanımla';
            document.getElementById('duzenleIndex').value = '';
            document.getElementById('soruMetin').value = '';
            document.getElementById('siraNo').value = sorular.length + 1;
            document.getElementById('seceneklerListesi').innerHTML = '';
            secenekEkle(); // Başlangıçta bir tane boş seçenek gelsin
            document.getElementById('soruForm').classList.add('active');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        function duzenle(i) {
            const s = sorular[i];
            document.getElementById('formBaslik').textContent = 'Soruyu Güncelle';
            document.getElementById('soruMetin').value = s.metin;
            document.getElementById('siraNo').value = s.sira;
            document.getElementById('basTarih').value = s.bas;
            document.getElementById('bitTarih').value = s.bit;
            document.getElementById('saat').value = s.saat;
            document.getElementById('duzenleIndex').value = i;
            document.getElementById('seceneklerListesi').innerHTML = '';
            s.secenekler.forEach(o => secenekEkle(o.metin, o.puan));
            document.getElementById('soruForm').classList.add('active');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        function secenekEkle(metin = '', puan = 0) {
            const row = `
                <div class="secenek-row">
                    <input class="form-control form-control-sm" placeholder="Seçenek metni..." value="${metin}">
                    <input type="number" class="form-control form-control-sm" style="width:100px" placeholder="Puan" value="${puan}">
                    <button type="button" class="btn btn-outline-danger btn-sm border-0" onclick="this.parentElement.remove()">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>`;
            document.getElementById('seceneklerListesi').insertAdjacentHTML('beforeend', row);
        }

        function soruKaydet() {
            const metin = document.getElementById('soruMetin').value.trim();
            if (!metin) { alert('Lütfen soru metnini giriniz.'); return; }

            const rows = document.getElementById('seceneklerListesi').querySelectorAll('.secenek-row');
            const secenekler = [...rows].map(r => ({
                metin: r.querySelectorAll('input')[0].value,
                puan: parseInt(r.querySelectorAll('input')[1].value) || 0
            }));

            const yeniSoru = {
                metin,
                secenekler,
                sira: parseInt(document.getElementById('siraNo').value) || 0,
                bas: document.getElementById('basTarih').value,
                bit: document.getElementById('bitTarih').value,
                saat: document.getElementById('saat').value
            };

            const idx = document.getElementById('duzenleIndex').value;
            if (idx !== '') sorular[idx] = yeniSoru;
            else sorular.push(yeniSoru);

            formKapat();
            listele();
        }

        function sil(i) {
            if(confirm('Bu soruyu silmek istediğinize emin misiniz?')) {
                sorular.splice(i, 1);
                listele();
            }
        }

        function formKapat() {
            document.getElementById('soruForm').classList.remove('active');
        }

        // Sayfa yüklendiğinde listeyi çalıştır
        document.addEventListener('DOMContentLoaded', listele);
    </script>
</asp:Content>