namespace InfrastructureManagmentWebFramework.Helpers
{
    public class JWT
    {
        public static string key => "bd1a1ccf8095037f361a4d351e7c0de65f0776bfc2f478ea8d312c763bb6caca";

        public static string wekey => "MTEwMzExODc5NzM6e3pdQSRUSXFbMHt2dlBlOjE2NDA=";
        public string Issuer { get; set; }

        public string Audience { get; set; }    

        public double DurationInHours {  get; set; }
    }
}
