namespace HCGStudio.DongBot.Core.Service
{
    public interface ICustomServiceProvider
    {
        void UseCustomBuilder(ContainerBuilder builder);
    }
}