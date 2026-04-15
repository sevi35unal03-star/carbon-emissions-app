<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Giris.aspx.cs" Inherits="iztekadmin.Giris" %>

<!DOCTYPE html>
<html lang="tr">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Admin Giriş - Karbon Ayak İzi</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        body {
            background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0;
        }
        .login-card {
            border-radius: 16px;
            border: none;
            width: 100%;
            max-width: 400px;
        }
        .btn-primary {
            background-color: #38bdf8;
            border: none;
            padding: 12px;
            font-weight: 600;
        }
        .btn-primary:hover {
            background-color: #0ea5e9;
        }
        .form-control {
            padding: 12px;
            background-color: #f8fafc;
        }
        .input-group-text {
            background-color: #f8fafc;
            color: #64748b;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="card login-card shadow-lg p-4 m-3">
            <div class="text-center mb-4">
                <div class="mb-3">
                    <i class="bi bi-shield-lock text-primary" style="font-size: 3rem;"></i>
                </div>
                <h4 class="fw-bold text-dark">Admin Paneli</h4>
                <p class="text-muted small">Lütfen yönetici bilgilerinizi giriniz</p>
            </div>

            <div class="mb-3">
                <label class="form-label small fw-bold text-secondary text-uppercase">E-posta</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="bi bi-envelope"></i></span>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="admin@iztek.com" TextMode="Email"></asp:TextBox>
                </div>
            </div>

            <div class="mb-4">
                <label class="form-label small fw-bold text-secondary text-uppercase">Şifre</label>
                <div class="input-group">
                    <span class="input-group-text"><i class="bi bi-lock"></i></span>
                    <asp:TextBox ID="txtSifre" runat="server" CssClass="form-control" placeholder="••••••" TextMode="Password"></asp:TextBox>
                </div>
            </div>

            <div class="d-flex justify-content-between mb-4">
                <div class="form-check">
                    <input type="checkbox" class="form-check-input" id="rememberMe">
                    <label class="form-check-label small" for="rememberMe">Beni hatırla</label>
                </div>
                <a href="#" class="small text-decoration-none">Şifremi unuttum</a>
            </div>

            <asp:Button ID="btnGiris" runat="server" Text="Sisteme Giriş Yap" CssClass="btn btn-primary w-100 shadow-sm" />

            <div class="text-center mt-4">
                <p class="text-muted mb-0" style="font-size: 11px;">© 2026 IzTek Carbon Tracking System</p>
                <p class="text-muted" style="font-size: 11px;">Tüm Hakları Saklıdır.</p>
            </div>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.6/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>