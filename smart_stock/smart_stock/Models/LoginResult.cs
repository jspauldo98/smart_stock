using System.Text.Json.Serialization;

namespace smart_stock.Models
{
    public class LoginResult
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("userId")]
        public int? UserId { get; set; }
    }
}