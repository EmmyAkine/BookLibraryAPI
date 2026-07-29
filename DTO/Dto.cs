namespace BookLibraryAPI.DTO {
    public class RegisterDto {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginDto {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class UploadBookDto {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string ISBN { get; set; } = "";
        public int CopiesAvailable { get; set; }
    }
}
