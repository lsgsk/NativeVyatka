using NativeVyatkaCore;
using NativeVyatkaCore.Database;
using Microsoft.Practices.Unity;

namespace UnitTestProject
{
    public static class TestInitialization
    {
        static TestInitialization()
        {
            RegisterTypesIntoDI.InitContainer(container);
            SQLitePCL.Batteries.Init();
            BurialDatabase.InitILobbyPhoneDatabase("..\\..\\Temp");
        }

        private readonly static IUnityContainer container = new UnityContainer();

        public static IUnityContainer Container
        {
            get
            {
                return container;
            }
        }

        public static IUnityContainer CreateChildContainer()
        {
            return container.CreateChildContainer();
        }
    }
}
