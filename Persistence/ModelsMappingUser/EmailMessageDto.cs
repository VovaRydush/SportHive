namespace DB.SportHive.Domain
{
    public class EmailMessageDto
    {
        public string To { get; set; } = null!;
        public string From { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}