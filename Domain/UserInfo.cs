namespace Domain
{
    public class UserInfo
    {
        public UserInfo()
        {
        }

        public UserInfo(string userId, string role, string method, string status)
        {
            UserId = userId;
            Role = role;
            Method = method;
            Status = status;
        }

        public string ID { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }

        public override string? ToString()
        {
            return $"{ID} - {UserId} - {Role} - {Method} - {Status}";
        }   
    }
}
