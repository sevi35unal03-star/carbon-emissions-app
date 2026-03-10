namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;

public record DonateTreesResponse(
    int DonatedTreeCount,      // Bu bağışta verilen ağaç sayısı
    int TotalDonatedTreeCount); // Toplam bağışlanan ağaç sayısı