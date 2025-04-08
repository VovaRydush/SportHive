namespace SportHive.Exceptions{
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class CustomAppException : Exception
{
    public int StatusCode { get; }
    public string? Title { get; }

    public CustomAppException(string message, int statusCode = 400, string? title = null) : base(message)
    {
        StatusCode = statusCode;
        Title = title;
    }
}
}