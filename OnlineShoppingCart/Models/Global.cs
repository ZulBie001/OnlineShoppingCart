namespace OnlineShoppingCart
{
    
    public class Global
    {
        public static string SessionName { get; } = "OSC-Session";
        public static string LoginCookieName { get; } = "OSC-AUTH";
        public const string AdminRole = "Admin";
        public const string ShopKeeperRole = "Shopkeeper";
    }
}
