namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetDonationHistory;

public record GetDonationHistoryResponse(
    int TotalDonatedTreeCount,
    List<DonationDto> Donations);

public record DonationDto(
    int TreeCount,
    double PointsSpent,
    DateTime DonationDate);