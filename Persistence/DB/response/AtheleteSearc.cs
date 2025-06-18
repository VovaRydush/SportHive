namespace DB.SportHive.Domain
{
    public class AthleteSearchResultDto
    {
        public string Login { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfilePhotoPath { get; set; }
    }
}
