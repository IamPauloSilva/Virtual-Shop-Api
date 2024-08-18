namespace VShop.IdentityServer.SeedDataBase
{
    public interface IDatabaseSeedInitializer
    {
        void InitializeSeedRoles();
        void InitializeSeedUsers();
    }
}
