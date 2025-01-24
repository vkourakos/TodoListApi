namespace Ergasia2.Tests.Responses;
public class CustomAccessTokenResponse
{
    public string TokenType { get; set; }
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
}

