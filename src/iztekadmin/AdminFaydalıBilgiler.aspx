<%@ Page Title="Faydalı Bilgiler - Admin" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="AdminFaydalıBilgiler.aspx.cs" Inherits="iztekadmin.AdminFaydalıBilgiler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .content-wrapper {
            padding: 30px;
            background: #f8fafc;
            min-height: 100vh;
        }

        .bilgi-item {
            background: white;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            padding: 14px 16px;
            margin-bottom: 10px;
            cursor: pointer;
            transition: all 0.2s ease;
        }

        .bilgi-item:hover {
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
            border-color: #38bdf8;
        }

        .form-panel {
            background: white;
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            padding: 24px;
            margin-bottom: 20px;
            display: none;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
        }

        .form-panel.active {
            display: block;
        }

        .card-custom {
            background: white;
            border-radius: 12px;
            border: 1px solid #e2e8f0;
            padding: 24px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-wrapper">
        
        <div class="form-panel" id="formPanel">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="fw-bold mb-0" id="formBaslik">Yeni Bilgi Ekle</h5>
                <button type="button" class="btn-close" onclick="formKapat()"></button>
            </div>
            
            <input type="hidden" id="duzenleIndex">
            
            <div class="mb-3">
                <label class="form-label fw-semibold">Bilgi Başlığı</label>
                <input type="text" class="form-control" id="baslik" placeholder="Örn: Geri Dönüşümün Önemi">
            </div>

            <div class="mb-3">
                <label class="form-label fw-semibold">Bilgi Detayı</label>
                <textarea class="form-control" id="detay" rows="4" placeholder="Detaylı açıklama giriniz..."></textarea>
            </div>

            <div class="form-check mb-4">
                <input class="form-check-input" type="checkbox" id="aktif" checked>
                <label class="form-check-label fw-semibold" for="aktif">Bilgi yayında mı? (Aktif)</label>
            </div>

            <div class="d-flex gap-2">
                <button type="button" class="btn btn-success px-4" onclick="kaydet()">
                    <i class="bi bi-check-lg"></i> Kaydet
                </button>
                <button type="button" class="btn btn-outline-secondary" onclick="formKapat()">İptal</button>
            </div>
        </div>

        <div class="card-custom shadow-sm" id="listePaneli">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h4 class="fw-bold mb-1">Faydalı Bilgiler</h4>
                    <p class="text-muted mb-0">Uygulama içinde görünecek bilgilendirici içerikleri yönetin.</p>
                </div>
                <button type="button" class="btn btn-primary d-flex align-items-center gap-2" onclick="yeniAc()">
                    <i class="bi bi-plus-lg"></i> Yeni Bilgi Ekle
                </button>
            </div>
            <div id="bilgiListesi"></div>
        </div>

        <div class="card-custom shadow-sm" id="detayPaneli" style="display:none">
            <button type="button" class="btn btn-sm btn-outline-secondary mb-3" onclick="detayKapat()">
                <i class="bi bi-arrow-left"></i> Listeye Dön
            </button>
            
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                    <h3 id="detayBaslik" class="fw-bold mb-2"></h3>
                    <span id="detayDurum" class="badge"></span>
                </div>
                <button type="button" class="btn btn-primary btn-sm" onclick="duzenleDetay()">
                    <i class="bi bi-pencil"></i> Düzenle
                </button>
            </div>
            <hr>
            <p id="detayIcerik" class="text-secondary fs-5" style="white-space:pre-wrap; line-height: 1.6;"></p>
        </div>

    </div>

    <script>
        let bilgiler = [
            { baslik: "Karbon Ayak İzi Nedir?", detay: "Karbon ayak izi, bir kişinin faaliyetleri sonucunda atmosfere salınan toplam sera gazı miktarını ifade eder.", aktif: true },
            { baslik: "Ağaç Dikmenin Faydaları", detay: "Ağaçlar CO2 emerek oksijen üretir. Her yıl bir ağaç ortalama 22 kg CO2 emer.", aktif: true },
            { baslik: "Toplu Taşıma Kullanımı", detay: "Toplu taşıma kullanmak kişi başı karbon salınımını %70 oranında azaltabilir.", aktif: false }
        ];

        let aktifDetayIndex = null;

        function listele() {
            const container = document.getElementById('bilgiListesi');
            if (bilgiler.length === 0) {
                container.innerHTML = '<div class="text-center py-5 text-muted"><i class="bi bi-info-circle fs-1 d-block mb-2"></i>Henüz hiç bilgi eklenmemiş.</div>';
                return;
            }

            container.innerHTML = bilgiler.map((b, i) => `
                <div class="bilgi-item d-flex justify-content-between align-items-center" onclick="detayAc(${i})">
                    <div class="pe-3">
                        <div class="fw-bold text-dark mb-1">${b.baslik}</div>
                        <small class="text-muted d-block">${b.detay.substring(0, 80)}...</small>
                    </div>
                    <div class="d-flex align-items-center gap-2" onclick="event.stopPropagation()">
                        <span class="badge ${b.aktif ? 'bg-success' : 'bg-secondary'}">${b.aktif ? 'Aktif' : 'Pasif'}</span>
                        <button type="button" class="btn btn-sm btn-light border" onclick="duzenle(${i})"><i class="bi bi-pencil"></i></button>
                        <button type="button" class="btn btn-sm btn-light border text-danger" onclick="sil(${i})"><i class="bi bi-trash"></i></button>
                    </div>
                </div>`).join('');
        }

        function detayAc(i) {
            aktifDetayIndex = i;
            const b = bilgiler[i];
            document.getElementById('detayBaslik').textContent = b.baslik;
            document.getElementById('detayIcerik').textContent = b.detay;
            const durum = document.getElementById('detayDurum');
            durum.textContent = b.aktif ? 'Yayında / Aktif' : 'Arşiv / Pasif';
            durum.className = 'badge ' + (b.aktif ? 'bg-success' : 'bg-secondary');
            
            document.getElementById('listePaneli').style.display = 'none';
            document.getElementById('detayPaneli').style.display = 'block';
            document.getElementById('formPanel').classList.remove('active');
        }

        function detayKapat() {
            document.getElementById('detayPaneli').style.display = 'none';
            document.getElementById('listePaneli').style.display = 'block';
        }

        function duzenleDetay() {
            duzenle(aktifDetayIndex);
            detayKapat();
        }

        function yeniAc() {
            document.getElementById('formBaslik').textContent = 'Yeni Bilgi Ekle';
            document.getElementById('baslik').value = '';
            document.getElementById('detay').value = '';
            document.getElementById('aktif').checked = true;
            document.getElementById('duzenleIndex').value = '';
            document.getElementById('formPanel').classList.add('active');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        function duzenle(i) {
            const b = bilgiler[i];
            document.getElementById('formBaslik').textContent = 'Bilgiyi Güncelle';
            document.getElementById('baslik').value = b.baslik;
            document.getElementById('detay').value = b.detay;
            document.getElementById('aktif').checked = b.aktif;
            document.getElementById('duzenleIndex').value = i;
            document.getElementById('formPanel').classList.add('active');
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }

        function kaydet() {
            const baslik = document.getElementById('baslik').value.trim();
            const detay = document.getElementById('detay').value.trim();
            if (!baslik) { alert('Lütfen bir başlık giriniz.'); return; }

            const veri = {
                baslik: baslik,
                detay: detay,
                aktif: document.getElementById('aktif').checked
            };

            const index = document.getElementById('duzenleIndex').value;
            if (index !== '') {
                bilgiler[index] = veri;
            } else {
                bilgiler.push(veri);
            }

            formKapat();
            listele();
        }

        function sil(i) {
            if(confirm('Bu bilgiyi silmek istediğinizden emin misiniz?')) {
                bilgiler.splice(i, 1);
                listele();
            }
        }

        function formKapat() {
            document.getElementById('formPanel').classList.remove('active');
        }

        // Başlangıçta listele
        window.addEventListener('load', listele);
    </script>
</asp:Content>