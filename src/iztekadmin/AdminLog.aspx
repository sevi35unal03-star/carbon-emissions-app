<%@ Page Title="Sistem Log Kayıtları" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="AdminLog.aspx.cs" Inherits="iztekadmin.AdminLog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .content-wrapper { padding: 30px; background: #f8fafc; min-height: 100vh; }
        .card { border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 2px 4px rgba(0,0,0,0.02); }
        .table th { font-size: 11px; text-transform: uppercase; letter-spacing: .05em; color: #64748b; font-weight: 700; background-color: #f8fafc; }
        .table td { font-size: 13px; vertical-align: middle; font-family: 'JetBrains Mono', 'Courier New', monospace; }
        .table td.normal { font-family: system-ui, -apple-system, sans-serif; }
        
        .filter-section { background: #fff; padding: 15px; border-radius: 10px; border: 1px solid #e2e8f0; margin-bottom: 20px; }
        .log-badge { font-family: system-ui, sans-serif; font-weight: 500; font-size: 11px; }
        
        /* IP adresi ve Tarih gibi veriler için özel vurgu */
        .text-mono { font-family: monospace; color: #475569; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-wrapper">
        <div class="card p-4">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h4 class="fw-bold mb-1">Sistem Log Kayıtları</h4>
                    <p class="text-muted small mb-0">Sistem üzerinde gerçekleştirilen tüm işlemlerin tarihçesi.</p>
                </div>
                <button type="button" class="btn btn-sm btn-outline-danger d-flex align-items-center gap-2" onclick="temizle()">
                    <i class="bi bi-trash"></i> Tümünü Temizle
                </button>
            </div>

            <div class="filter-section shadow-sm">
                <div class="row g-2">
                    <div class="col-md-3">
                        <label class="small fw-bold text-muted mb-1">Kullanıcı</label>
                        <div class="input-group input-group-sm">
                            <span class="input-group-text bg-light border-end-0"><i class="bi bi-search"></i></span>
                            <input type="text" class="form-control border-start-0" id="userSearch" placeholder="Kullanıcı adı..." oninput="filtrele()">
                        </div>
                    </div>
                    <div class="col-md-2">
                        <label class="small fw-bold text-muted mb-1">İşlem Tipi</label>
                        <select class="form-select form-select-sm" id="islemFiltre" onchange="filtrele()">
                            <option value="">Tüm İşlemler</option>
                            <option>EKLEME</option>
                            <option>GÜNCELLEME</option>
                            <option>SİLME</option>
                            <option>GİRİŞ</option>
                            <option>ÇIKIŞ</option>
                        </select>
                    </div>
                    <div class="col-md-2">
                        <label class="small fw-bold text-muted mb-1">Modül</label>
                        <select class="form-select form-select-sm" id="modulFiltre" onchange="filtrele()">
                            <option value="">Tüm Modüller</option>
                            <option>Sorular</option>
                            <option>Anketler</option>
                            <option>Faydalı Bilgiler</option>
                            <option>Kullanıcılar</option>
                            <option>Sistem</option>
                        </select>
                    </div>
                    <div class="col-md-2">
                        <label class="small fw-bold text-muted mb-1">Tarih</label>
                        <input type="date" class="form-control form-control-sm" id="tarihFiltre" onchange="filtrele()">
                    </div>
                    <div class="col-md-3">
                        <label class="small fw-bold text-muted mb-1">Durum</label>
                        <select class="form-select form-select-sm" id="durumFiltre" onchange="filtrele()">
                            <option value="">Tüm Durumlar</option>
                            <option>BAŞARILI</option>
                            <option>HATA</option>
                        </select>
                    </div>
                </div>
            </div>

            <div class="table-responsive">
                <table class="table table-hover align-middle">
                    <thead>
                        <tr>
                            <th style="width: 50px;">#ID</th>
                            <th>Zaman Damgası</th>
                            <th>Kullanıcı</th>
                            <th>Modül</th>
                            <th>İşlem</th>
                            <th>İşlem Detayı</th>
                            <th>IP Adresi</th>
                            <th>Durum</th>
                        </tr>
                    </thead>
                    <tbody id="logBody">
                        </tbody>
                </table>
            </div>
            
            <div class="d-flex justify-content-between align-items-center mt-3 pt-3 border-top">
                <div class="text-muted small" id="logSayac"></div>
                <div class="small text-muted">
                    <i class="bi bi-info-circle me-1"></i> Loglar son 30 günü kapsamaktadır.
                </div>
            </div>
        </div>
    </div>

    <script>
        const islemRenk = { 
            EKLEME: 'bg-success', 
            GÜNCELLEME: 'bg-primary', 
            SİLME: 'bg-danger', 
            GİRİŞ: 'bg-info text-dark', 
            ÇIKIŞ: 'bg-secondary' 
        };

        let loglar = [
            { id: 1042, tarih: "2025-04-13 09:02:11", kullanici: "admin", modul: "Sistem", islem: "GİRİŞ", aciklama: "Yönetim paneline güvenli giriş yapıldı.", ip: "192.168.1.10", durum: "BAŞARILI" },
            { id: 1043, tarih: "2025-04-13 09:05:34", kullanici: "admin", modul: "Sorular", islem: "EKLEME", aciklama: "Yeni soru veritabanına eklendi.", ip: "192.168.1.10", durum: "BAŞARILI" },
            { id: 1044, tarih: "2025-04-13 09:12:45", kullanici: "admin", modul: "Sorular", islem: "GÜNCELLEME", aciklama: "Soru metni revize edildi (ID: 3)", ip: "192.168.1.10", durum: "BAŞARILI" },
            { id: 1045, tarih: "2025-04-13 09:20:01", kullanici: "editor_sevgi", modul: "Faydalı Bilgiler", islem: "EKLEME", aciklama: "Karbon ayak izi makalesi eklendi.", ip: "192.168.1.22", durum: "BAŞARILI" }
        ];

        function render(liste) {
            const container = document.getElementById('logBody');
            if (liste.length === 0) {
                container.innerHTML = '<tr><td colspan="8" class="text-center py-4 text-muted">Filtrelere uygun kayıt bulunamadı.</td></tr>';
                return;
            }

            container.innerHTML = liste.map(l => `
                <tr>
                    <td class="text-muted">#${l.id}</td>
                    <td class="text-mono">${l.tarih}</td>
                    <td class="normal fw-bold"><i class="bi bi-person-circle me-2 text-secondary"></i>${l.kullanici}</td>
                    <td class="normal text-muted">${l.modul}</td>
                    <td class="normal"><span class="badge log-badge ${islemRenk[l.islem] || 'bg-secondary'}">${l.islem}</span></td>
                    <td class="normal text-wrap" style="max-width:300px; font-size:12px;">${l.aciklama}</td>
                    <td class="text-mono small">${l.ip}</td>
                    <td class="normal">
                        <span class="badge log-badge ${l.durum === 'BAŞARILI' ? 'bg-success-subtle text-success border border-success-subtle' : 'bg-danger-subtle text-danger border border-danger-subtle'}">
                            ${l.durum}
                        </span>
                    </td>
                </tr>`).join('');
            
            document.getElementById('logSayac').innerHTML = `<b>${liste.length}</b> kayıt listeleniyor.`;
        }

        function filtrele() {
            const ara = document.getElementById('userSearch').value.toLowerCase();
            const islem = document.getElementById('islemFiltre').value;
            const modul = document.getElementById('modulFiltre').value;
            const tarih = document.getElementById('tarihFiltre').value;
            const durum = document.getElementById('durumFiltre').value;

            const sonuc = loglar.filter(l =>
                (!ara || l.kullanici.toLowerCase().includes(ara)) &&
                (!islem || l.islem === islem) &&
                (!modul || l.modul === modul) &&
                (!tarih || l.tarih.startsWith(tarih)) &&
                (!durum || l.durum === durum)
            );
            render(sonuc);
        }

        function temizle() {
            if (confirm('Tüm sistem logları kalıcı olarak silinecektir. Emin misiniz?')) {
                loglar = [];
                render([]);
            }
        }

        // İlk yükleme
        window.addEventListener('load', () => render(loglar));
    </script>
</asp:Content>