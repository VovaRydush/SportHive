using DB.SportHive.Domain;
using DB.SportHive.Persistence;
using Microsoft.EntityFrameworkCore;
using SportHive.Services.Interfaces;

public class AthleteService : IAthleteService
{
    private readonly AppDbContext _context;
    private readonly IPhotoProcessing _photoProcessing;
    public AthleteService(AppDbContext context, IPhotoProcessing photoProcessing)
    {
        _photoProcessing = photoProcessing;
        _context = context;
    }

    public async Task<List<AthleteSearchResultDto>> SearchAthletesAsync(string searchTerm)
{
    if (string.IsNullOrWhiteSpace(searchTerm))
        return [];

    searchTerm = searchTerm.ToLower();

    var rawAthletes = await _context.Athletes
        .Where(a =>
            a.FirsName.ToLower().Contains(searchTerm) ||
            a.LastName.ToLower().Contains(searchTerm) ||
            a.login.ToLower().Contains(searchTerm))
        .Select(a => new
        {
            a.login,
            FullName = $"{a.FirsName} {a.LastName}",
            PhotoPath = _context.UserPhotos
                .Where(up => up.login == a.login)
                .Select(up => up.ProfilePhoto)
                .FirstOrDefault()
        })
        .ToListAsync();

    var result = new List<AthleteSearchResultDto>();

    foreach (var a in rawAthletes)
    {
        var photoBase64 = await _photoProcessing.GetPhotoBase64Async(a.PhotoPath ?? "");

        result.Add(new AthleteSearchResultDto
        {
            Login = a.login,
            FullName = a.FullName,
            ProfilePhotoPath = photoBase64
        });
    }

    return result;
}

}
