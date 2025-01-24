namespace DAL;

public class DatabaseSeeder
{
    public const int Generateamount = 0;
    
    public static void SeedDatabase(MangoWalletDbContext ctx)
    {
        if (!ctx.IsDatabaseEmpty())
        {
            throw new InvalidOperationException("Can't seed non-empty database.");
        }

        //seed data here
    }

    public static string GetRandomName()
    {
        Random r = new Random();
        string[] strings = new[] { "Nick", "Tijn", "Thijs", "Jasper", "Lennon", "Dylan", "Elliot" };
        return strings[r.Next(0, strings.Length)];
    }
}