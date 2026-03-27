using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace IzTek.Carbon.Footprint.Persistence;

public static class ServiceCollectionExtensions  // ← class eklendi
{
    public static IHostApplicationBuilder ConfigurePersistence(this IHostApplicationBuilder builder)
    {
        // Interceptor'ları DI'a kaydet
        builder.Services.AddScoped<DispatchDomainEventsInterceptor>();
        builder.Services.AddScoped<AuditableEntityInterceptor>();
        builder.Services.AddScoped<AuditInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(
                serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>(),
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
                serviceProvider.GetRequiredService<AuditInterceptor>()
            );
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
            options.InstanceName = "CarbonFootprint:";
        });

        builder.Services.ConfigureServices();
        return builder;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());
        return services;
    }

    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();
        var appDbContext = serviceScope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        await appDbContext.Database.MigrateAsync();
    }

    public static async Task InitializeAssetsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var fileStorage = scope.ServiceProvider
            .GetRequiredService<IFileStorageService>();

        // Assets bucket'ını public olarak oluştur
        //await fileStorage.EnsureAssetsBucketAsync();
    }

    public static async Task SeedRolesAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<Role>>();

        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Role { Name = role });
        }
    }

    public static async Task SeedAdminUserAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        const string adminEmail = "admin@iztek.com";
        const string adminPassword = "Sifre123!";

        var existingUser = await userManager.FindByEmailAsync(adminEmail);

        if (existingUser is not null)
        {
            // Kullanıcı var ama rolü yoksa ekle
            var existingRoles = await userManager.GetRolesAsync(existingUser);
            if (!existingRoles.Contains("Admin"))
                await userManager.AddToRoleAsync(existingUser, "Admin");
            return;
        }

        var adminUser = new User(
            email: adminEmail,
            name: "Admin",
            surname: "User",
            birthDate: DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc),
            identityNumber: "67890123452",
            phoneNumber: "+905001234567",
            isKvkkApproved: true
        );

        adminUser.ClearDomainEvents();

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin"); // ← Bu satır eksikti
    }

    public static async Task SeedScoringSettingsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        if (await context.ScoringSettings.AnyAsync())
            return;

        var settings = new List<ScoringSetting>
    {
        // General
        new("BaseScore", 1.0, ScoringCategory.General),

        // Transport
        new("Car", 2.5, ScoringCategory.Transport),
        new("PublicTransport", 0.5, ScoringCategory.Transport),
        new("Bicycle", 0.0, ScoringCategory.Transport),
        new("Walking", 0.0, ScoringCategory.Transport),

        // Energy
        new("Electricity", 1.5, ScoringCategory.Energy),
        new("NaturalGas", 2.0, ScoringCategory.Energy),
        new("Renewable", 0.2, ScoringCategory.Energy),

        // Nutrition
        new("Meat", 3.0, ScoringCategory.Nutrition),
        new("Vegetarian", 0.5, ScoringCategory.Nutrition),
        new("Vegan", 0.2, ScoringCategory.Nutrition),

        // Waste
        new("Recycling", 0.1, ScoringCategory.Waste),
        new("NoRecycling", 1.5, ScoringCategory.Waste),
    };

        await context.ScoringSettings.AddRangeAsync(settings);
        await context.SaveChangesAsync();
    }

    public static async Task SeedActivityQuestionsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        if (await context.ActivityQuestions.AnyAsync())
            return;

        var today = DateTime.UtcNow.Date;
        var endOfDay = today.AddDays(1).AddSeconds(-1);

        // Soru 3 — Sefer Sayısı (önce oluştur çünkü Soru 2 buna referans verecek)
        var soru3 = new ActivityQuestion(
            "Sefer Sayısı",
            TimeSpan.FromHours(8),
            3,
            today,
            endOfDay);

        soru3.AddOption("1 Sefer", 5.0);
        soru3.AddOption("2 Sefer", 10.0);
        soru3.AddOption("3 Sefer", 15.0);
        soru3.AddOption("4+ Sefer", 20.0);

        await context.ActivityQuestions.AddAsync(soru3);
        await context.SaveChangesAsync();

        // Soru 2 — Ulaşım aracı (Soru 3'e yönlendirir)
        var soru2 = new ActivityQuestion(
            "Kullandığınız ulaşım aracını seçiniz.",
            TimeSpan.FromHours(8),
            2,
            today,
            endOfDay);

        soru2.AddOption("Otobüs", 8.0, soru3.Id);
        soru2.AddOption("Metro", 6.0, soru3.Id);
        soru2.AddOption("Minibüs", 9.0, soru3.Id);

        await context.ActivityQuestions.AddAsync(soru2);
        await context.SaveChangesAsync();

        // Soru 1 — Ana soru (Soru 2 veya flow biter)
        var soru1 = new ActivityQuestion(
            "Bu sabah işe hangi ulaşım aracıyla gideceksiniz?",
            TimeSpan.FromHours(8),
            1,
            today,
            endOfDay);

        soru1.AddOption("Toplu Ulaşım", 5.0, soru2.Id);
        soru1.AddOption("Araba", 25.0);
        soru1.AddOption("Yürüyüş / Bisiklet", 0.0);

        await context.ActivityQuestions.AddAsync(soru1);
        await context.SaveChangesAsync();
    }

    public static async Task SeedMonthlyPollAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        if (await context.PollSets.AnyAsync())
            return;

        var pollSet = new PollSet(
            name: "Mart 2026 Karbon Ayak İzi Anketi",
            description: "Aylık karbon ayak izi hesaplama anketi",
            displayOrder: 1,
            month: 3,
            year: 2026);

        context.PollSets.Add(pollSet);
        await context.SaveChangesAsync();

        // Soru 1 — Evinizi kaç kişi ile paylaşıyorsunuz?
        var soru1 = new PollQuestion(pollSet.Id, "Evinizi kaç kişi ile paylaşıyorsunuz?", 1);
        context.PollQuestions.Add(soru1);
        await context.SaveChangesAsync();

        soru1.AddOption("Yalnız yaşıyorum.", 10.0, "Araştırmalar, yalnız yaşayan insanların daha fazla kaynak tükettiğini gösteriyor...", null, 1);
        soru1.AddOption("Evimi 1 kişi ile paylaşıyorum.", 8.0, null, null, 2);
        soru1.AddOption("Evimi 2 kişi ile paylaşıyorum.", 6.0, null, null, 3);
        soru1.AddOption("Evimi 3 kişi ile paylaşıyorum.", 4.0, null, null, 4);
        soru1.AddOption("Evimi 4 kişi ile paylaşıyorum.", 3.0, null, null, 5);
        soru1.AddOption("Evimi 5 kişi ile paylaşıyorum.", 2.0, null, null, 6);
        soru1.AddOption("Evimi 6 kişi ile paylaşıyorum.", 1.0, null, null, 7);

        // Soru 2 — Evinizin türü nedir?
        var soru2 = new PollQuestion(pollSet.Id, "Evinizin türü nedir?", 2);
        context.PollQuestions.Add(soru2);
        await context.SaveChangesAsync();

        soru2.AddOption("Villada yaşıyorum.", 10.0, "Müstakil bir evde veya villada yaşıyorsanız...", null, 1);
        soru2.AddOption("Müstakil evde yaşıyorum.", 8.0, null, null, 2);
        soru2.AddOption("Apartmanda yaşıyorum.", 4.0, null, null, 3);

        // Soru 3 — Beslenme et tüketimi
        var soru3 = new PollQuestion(pollSet.Id, "Beslenme tercihlerinizi değerlendirin.", 3);
        context.PollQuestions.Add(soru3);
        await context.SaveChangesAsync();

        soru3.AddOption("Sık sık et tüketirim.", 15.0, "Araştırmalar, et ve süt ürünleri tüketiminin karbon ayak izine negatif etkisi olduğunu gösteriyor...", null, 1);
        soru3.AddOption("Nadiren et tüketirim.", 8.0, null, null, 2);
        soru3.AddOption("Vejetaryen besleniyorum.", 4.0, null, null, 3);
        soru3.AddOption("Vegan besleniyorum.", 2.0, null, null, 4);

        // Soru 4 — Beslenme ambalaj
        var soru4 = new PollQuestion(pollSet.Id, "Beslenme tercihlerinizi değerlendirin.", 4);
        context.PollQuestions.Add(soru4);
        await context.SaveChangesAsync();

        soru4.AddOption("Sadece ambalajlı ürünler tüketirim.", 12.0, "Ambalajlı ürün kullanımını azaltıp yerel ürünleri...", null, 1);
        soru4.AddOption("Çoğunlukla taze ürünleri tercih ederim.", 6.0, null, null, 2);
        soru4.AddOption("Sadece taze ürünleri tercih ederim. Ambalajlı ürün satın almam.", 2.0, null, null, 3);

        // Soru 5 — Su tüketimi çamaşır
        var soru5 = new PollQuestion(pollSet.Id, "Su tüketiminizi tanımlayın.", 5);
        context.PollQuestions.Add(soru5);
        await context.SaveChangesAsync();

        soru5.AddOption("Çamaşır makinesini haftada ortalama 9 kez çalıştırırım.", 12.0, "Çamaşır makinesinin kullanımını azaltarak hem enerji verimliliğine hem de su kaynaklarının sürdürülebilirliğine katkıda bulunabilirsin.", null, 1);
        soru5.AddOption("Çamaşır makinesini haftada ortalama 6 kez çalıştırırım.", 8.0, null, null, 2);
        soru5.AddOption("Çamaşır makinesini haftada ortalama 3 kez çalıştırırım.", 4.0, null, null, 3);

        // Soru 6 — Su tüketimi bulaşık
        var soru6 = new PollQuestion(pollSet.Id, "Su tüketiminizi tanımlayın.", 6);
        context.PollQuestions.Add(soru6);
        await context.SaveChangesAsync();

        soru6.AddOption("Bulaşık makinesi yok.", 10.0, "Bulaşık makinesi kullanarak hem su tüketimini azaltabilir hem de su kaynaklarının sürdürülebilirliğine katkıda bulunabilirsin.", null, 1);
        soru6.AddOption("Bulaşık makinesini haftada ortalama 9 kez çalıştırırım.", 8.0, null, null, 2);
        soru6.AddOption("Bulaşık makinesini haftada ortalama 6 kez çalıştırırım.", 5.0, null, null, 3);
        soru6.AddOption("Bulaşık makinesini haftada ortalama 3 kez çalıştırırım.", 3.0, null, null, 4);

        // Soru 7 — Giyim alışverişi
        var soru7 = new PollQuestion(pollSet.Id, "Giyim alışverişlerinizi tanımlayın.", 7);
        context.PollQuestions.Add(soru7);
        await context.SaveChangesAsync();

        soru7.AddOption("Sık sık giyim alışverişi yaparım.", 12.0, "Tüketim alışkanlıklarını gözden geçirip, ihtiyaç halinde veya ikinci el ürünleri satın almayı tercih edebilirsin.", null, 1);
        soru7.AddOption("İhtiyacım olduğunda satın alırım.", 6.0, null, null, 2);
        soru7.AddOption("Sadece 2. el ürünleri tercih ederim.", 2.0, null, null, 3);

        // Soru 8 — Yıllık eşya alımı
        var soru8 = new PollQuestion(pollSet.Id, "Yıllık ortalama eşya satın alımlarınızı değerlendirin.", 8);
        context.PollQuestions.Add(soru8);
        await context.SaveChangesAsync();

        soru8.AddOption("Yılda 7 adetten fazla elektronik eşya, beyaz eşya veya mobilya alıyorum.", 15.0, null, null, 1);
        soru8.AddOption("Yılda ortalama 5 ila 7 adet arasında elektronik eşya, beyaz eşya veya mobilya alıyorum.", 10.0, null, null, 2);
        soru8.AddOption("Yılda ortalama 3 ila 5 adet arasında elektronik eşya, beyaz eşya veya mobilya alıyorum.", 7.0, null, null, 3);
        soru8.AddOption("Yılda ortalama 3 adetten az elektronik eşya, beyaz eşya veya mobilya alıyorum.", 4.0, null, null, 4);
        soru8.AddOption("Neredeyse hiç almıyorum. İhtiyacım olursa da ikinci el eşyaları tercih ediyorum.", 1.0, null, null, 5);

        // Soru 9 — Poşet çöp
        var soru9 = new PollQuestion(pollSet.Id, "Bireysel olarak haftada kaç poşet çöp üretiyorsunuz?", 9);
        context.PollQuestions.Add(soru9);
        await context.SaveChangesAsync();

        soru9.AddOption("Ortalama 12 çöp poşeti.", 12.0, null, null, 1);
        soru9.AddOption("Ortalama 9 çöp poşeti.", 9.0, null, null, 2);
        soru9.AddOption("Ortalama 6 çöp poşeti.", 6.0, null, null, 3);
        soru9.AddOption("Ortalama 3 çöp poşeti.", 3.0, null, null, 4);
        soru9.AddOption("3 çöp poşetinden daha az.", 1.0, null, null, 5);

        // Soru 10 — Geri dönüşüm
        var soru10 = new PollQuestion(pollSet.Id, "Hangi atıkları geri dönüştürüyorsunuz?", 10);
        context.PollQuestions.Add(soru10);
        await context.SaveChangesAsync();

        soru10.AddOption("Cam", -2.0, null, null, 1);
        soru10.AddOption("Plastik", -2.0, null, null, 2);
        soru10.AddOption("Kağıt", -2.0, null, null, 3);
        soru10.AddOption("Alüminyum", -2.0, null, null, 4);
        soru10.AddOption("Çelik", -2.0, null, null, 5);
        soru10.AddOption("Yemek Atığı", -2.0, null, null, 6);

        await context.SaveChangesAsync();
    }

    public static async Task SeedUsefulInformationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        if (await context.UsefulInformations.AnyAsync())
            return;

        var informations = new List<UsefulInformation>
    {
        new("Karbon Ayak İzi Nedir?",
            "Karbon ayak izi, bir kişinin veya bir organizasyonun ürettiği veya tüketim süreçlerinde serbest bıraktığı toplam karbon dioksit ve diğer sera gazlarının miktarını ifade eder. Bu terim, genellikle bir kişinin, bir şirketin, bir ülkenin veya belirli bir ürünün veya hizmetin çevreye olan etkisini ölçmek için kullanılır.\n\nKarbon ayak izi, sera gazı emisyonlarından kaynaklanan etkileri değerlendirmek ve azaltmak için önemli bir metriктir.",
            1),

        new("Karbon Ayak İzi Ne Anlama Geliyor?",
            "\"Karbon ayak izi\", bir kişinin, bir organizasyonun, bir ürünün veya bir hizmetin üretiminden veya tüketiminden kaynaklanan karbon dioksit ve diğer sera gazları emisyonlarının miktarını ifade eder. Bu terim, çevresel etkileri ölçer ve sera gazı emisyonlarını azaltmak için kullanılır.\n\nKarbon ayak izi, genellikle ton cinsinden ölçülür ve sera gazlarının (karbon dioksit, metan, nitrojen oksit gibi) atmosfere salınmasından kaynaklanan etkileri hesaplar.",
            2),

        new("Karbon Ayak İzi Neden Önemlidir?",
            "Karbon ayak izi, çevresel sürdürülebilirlik ve küresel iklim değişikliği açısından önemlidir:\n\n1. İklim Değişikliği Etkisi: Karbon ayak izi, sera gazlarının atmosfere salınmasının bir ölçüsüdür. Bu gazlar, sera etkisi yaratarak dünya atmosferindeki sıcaklığı artırır ve iklim değişikliğine neden olur.\n\n2. Çevresel Etki: Karbon ayak izi, doğal kaynakların tüketimine, doğal habitatların bozulmasına ve biyoçeşitliliğin azalmasına yol açabilecek faaliyetlerin bir göstergesidir.\n\n3. Toplumsal ve Ekonomik Etki: Fosil yakıtların aşırı kullanımı, hava kirliliği, sağlık sorunları ve enerji kaynaklarının azalmasına yol açabilir.",
            3),

        new("Karbon Ayak İzi Nasıl Hesaplanır?",
            "Karbon ayak izi hesaplaması, çeşitli faktörleri içerir:\n\n1. Enerji Tüketimi: Evde ve işyerinde kullanılan elektrik, doğalgaz ve diğer enerji kaynaklarının tüketimi hesaplanır.\n\n2. Ulaşım: Araba, uçak, toplu taşıma gibi ulaşım araçlarının kullanımından kaynaklanan emisyonlar değerlendirilir.\n\n3. Beslenme Alışkanlıkları: Et tüketimi, gıda israfı ve yerel/ithal ürün tercihleri hesaba katılır.\n\n4. Tüketim Alışkanlıkları: Satın alınan ürünlerin üretim süreçlerindeki emisyonlar değerlendirilir.",
            4),

        new("Karbon Ayak İzimizi Nasıl Azaltırız?",
            "Karbon ayak izimizi azaltmak için alabileceğimiz önlemler:\n\n1. Yenilenebilir Enerji Kullanımı: Güneş, rüzgar gibi yenilenebilir enerji kaynaklarına geçiş yapılabilir.\n\n2. Toplu Taşıma ve Bisiklet: Özel araç kullanımını azaltarak toplu taşıma, bisiklet veya yürüyüş tercih edilebilir.\n\n3. Bitkisel Beslenme: Et tüketimini azaltmak, karbon ayak izini önemli ölçüde düşürür.\n\n4. Enerji Verimliliği: Enerji tasarruflu cihazlar kullanmak ve gereksiz enerji tüketimini önlemek önemlidir.\n\n5. Geri Dönüşüm: Atıkları geri dönüştürmek ve israfı azaltmak çevreye katkı sağlar.",
            5),
    };

        await context.UsefulInformations.AddRangeAsync(informations);
        await context.SaveChangesAsync();
    }
}