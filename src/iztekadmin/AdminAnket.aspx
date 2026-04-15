<%@ Page Title="Anket Yönetimi" Language="C#" MasterPageFile="~/master.Master" AutoEventWireup="true" CodeBehind="AdminAnket.aspx.cs" Inherits="iztekadmin.AdminAnket" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .content-wrapper {
            padding: 30px;
            background: #f8fafc;
            min-height: 100vh;
        }

        .survey-card {
            background: white;
            border-radius: 12px;
            border: 1px solid #e2e8f0;
            margin-bottom: 20px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0,0,0,0.05);
        }

        .survey-header {
            padding: 18px 22px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            cursor: pointer;
            background: white;
        }

        .survey-header:hover { background: #f8fafc; }

        .survey-left {
            display: flex;
            align-items: center;
            gap: 14px;
        }

        .survey-icon {
            width: 45px;
            height: 45px;
            background: #e0f2fe;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .survey-body {
            padding: 20px 22px;
            border-top: 1px solid #e2e8f0;
        }

        .option-row {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 12px 14px;
            background: #f8fafc;
            border-radius: 8px;
            margin-bottom: 10px;
            border: 1px solid #e2e8f0;
        }

        .option-row input {
            flex: 1;
            border: none;
            background: transparent;
            outline: none;
            font-size: 15px;
        }

        .empty-state {
            padding: 80px 20px;
            text-align: center;
            color: #94a3b8;
        }
    </style>
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content-wrapper">
        <div class="d-flex justify-content-between align-items-start mb-4">
            <div>
                <h3 class="fw-bold">Anket Yönetimi</h3>
                <p class="text-muted">Anketleri ve seçenekleri buradan yönetebilirsiniz.</p>
            </div>
            <button type="button" class="btn btn-primary d-flex align-items-center gap-2" onclick="openModal()">
                <i class="bi bi-plus-lg"></i> Yeni Anket Oluştur
            </button>
        </div>

        <div id="surveys-list"></div>
    </div>

    <div class="modal fade" id="surveyModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Yeni Anket Oluştur</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Anket Başlığı <span class="text-danger">*</span></label>
                        <input type="text" class="form-control" id="modal-title" placeholder="Örn: Çevresel Farkındalık Anketi">
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Açıklama (Opsiyonel)</label>
                        <input type="text" class="form-control" id="modal-desc" placeholder="Kısa açıklama...">
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">İptal</button>
                    <button type="button" class="btn btn-primary" onclick="createSurvey()">Oluştur</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        let surveys = [
            { id: 1, title: "Çevresel Farkındalık Anketi", desc: "Kullanıcıların çevre bilincini ölçer", options: ["Evet", "Hayır", "Kısmen"], open: true, status: "active" },
            { id: 2, title: "Karbon Ayak İzi Değerlendirmesi", desc: "Günlük yaşam alışkanlıkları", options: ["Her gün", "Haftada birkaç kez", "Nadiren"], open: true, status: "draft" }
        ];
        let nextId = 3;

        function render() {
            const container = document.getElementById('surveys-list');
            if (surveys.length === 0) {
                container.innerHTML = `<div class="empty-state"><h5>Henüz anket bulunmuyor</h5></div>`;
                return;
            }

            container.innerHTML = surveys.map(s => `
                <div class="survey-card">
                    <div class="survey-header" onclick="toggleCard(${s.id})">
                        <div class="survey-left">
                            <div class="survey-icon"><i class="bi bi-clipboard-check text-primary fs-4"></i></div>
                            <div>
                                <h5 class="mb-1">${s.title}</h5>
                                <small class="text-muted">${s.desc || ''} • ${s.options.length} seçenek</small>
                            </div>
                        </div>
                        <span class="badge ${s.status === 'active' ? 'bg-success' : 'bg-warning text-dark'}">${s.status === 'active' ? 'Aktif' : 'Taslak'}</span>
                    </div>
                    ${s.open ? `
                    <div class="survey-body">
                        ${s.options.map((opt, i) => `
                            <div class="option-row">
                                <span class="badge bg-dark text-white">${i + 1}</span>
                                <input value="${opt}" onchange="updateOption(${s.id}, ${i}, this.value)">
                                <button class="btn btn-link text-danger p-0 ms-auto" onclick="removeOption(${s.id}, ${i})">
                                    <i class="bi bi-trash3"></i>
                                </button>
                            </div>
                        `).join('')}
                        <div class="input-group mt-3">
                            <input type="text" id="new-opt-${s.id}" class="form-control" placeholder="Yeni seçenek ekle...">
                            <button class="btn btn-primary" type="button" onclick="addOption(${s.id})">Ekle</button>
                        </div>
                        <div class="mt-4 d-flex gap-2">
                            <button type="button" class="btn ${s.status === 'active' ? 'btn-warning' : 'btn-success'}" onclick="toggleStatus(${s.id})">
                                ${s.status === 'active' ? 'Pasif Yap' : 'Aktif Yap'}
                            </button>
                            <button type="button" class="btn btn-outline-danger" onclick="deleteSurvey(${s.id})">Sil</button>
                        </div>
                    </div>` : ''}
                </div>
            `).join('');
        }

        function toggleCard(id) { const s = surveys.find(x => x.id === id); if (s) s.open = !s.open; render(); }
        function addOption(id) {
            const input = document.getElementById(`new-opt-${id}`);
            if (!input.value.trim()) return;
            surveys.find(x => x.id === id).options.push(input.value.trim());
            input.value = '';
            render();
        }
        function removeOption(id, index) { if (confirm("Bu seçeneği silmek istiyor musunuz?")) { surveys.find(x => x.id === id).options.splice(index, 1); render(); } }
        function updateOption(id, index, value) { surveys.find(x => x.id === id).options[index] = value; }
        function toggleStatus(id) { const s = surveys.find(x => x.id === id); if (s) s.status = s.status === 'active' ? 'draft' : 'active'; render(); }
        function deleteSurvey(id) { if (confirm("Bu anketi tamamen silmek istediğinizden emin misiniz?")) { surveys = surveys.filter(x => x.id !== id); render(); } }
        function openModal() {
            const modalElement = document.getElementById('surveyModal');
            const modal = new bootstrap.Modal(modalElement);
            document.getElementById('modal-title').value = '';
            document.getElementById('modal-desc').value = '';
            modal.show();
        }
        function createSurvey() {
            const title = document.getElementById('modal-title').value.trim();
            if (!title) return alert("Anket başlığı girmelisiniz!");
            surveys.push({ id: nextId++, title: title, desc: document.getElementById('modal-desc').value.trim(), options: [], open: true, status: "draft" });
            const modalElement = document.getElementById('surveyModal');
            bootstrap.Modal.getInstance(modalElement).hide();
            render();
        }
        window.addEventListener('load', render);
    </script>
</asp:Content>