namespace Darc.Tests
{
    using Castle.MicroKernel.Registration;
    using Core;
    using Core.Castle;
    using Dapper;

    public class ServiceLocatorInitializer
    {
        public static void Init()
        {
            CastleRegistrer.Initialize().AddComponentsTo(p =>
            {
                p.Register(AllTypes.FromAssemblyNamed("Darc.Commands")
                    .BasedOn(typeof (CommandBase))
                    .WithService.FirstInterface());

                p.Register(AllTypes.FromAssemblyNamed("Darc.Queries")
                    .BasedOn(typeof (DapperQuery))
                    .WithService.DefaultInterfaces());
            });
        }
    }
}