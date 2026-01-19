namespace daloy_api.DTOs
{
    public class RefreshRequest
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }

}
