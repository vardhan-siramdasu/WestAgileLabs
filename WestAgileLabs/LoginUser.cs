namespace WestAgileLabs
{
    public class LoginUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

    }

    public static class SecondaryLoginUser
    {
        private static LoginUser loginuser = new LoginUser();
        public static LoginUser Loginuser
        {
            get { return loginuser; }
            set { loginuser = value; }
        }
    }
}
