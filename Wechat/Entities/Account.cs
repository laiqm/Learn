namespace Wechat.Entities
{
    public class Account
    {
        public int Id { get; set; }

        public string OpenId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Display { get; set; }
    }
}