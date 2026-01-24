using Microsoft.AspNetCore.Http;

namespace daloy_api.DTOs
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = default!;
        public DateTime BirthDate { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
