using Microsoft.Practices.Unity;
using NativeVyatkaCore;
using NativeVyatkaCore.Database;
using NativeVyatkaCore.Utilities;
using System;
using UIKit;
using NativeVyatkaIOS.Utilities;

namespace NativeVyatkaIOS
{
    public class App
    {
        static void Main(string[] args)
        {
            RegisterTypesIntoDI.InitContainer(Container);
            BurialDatabase.InitILobbyPhoneDatabase(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            iConsole.Init(new ConsoleRealization());
            UIApplication.Main(args, null, "AppDelegate");
        }
        public static UnityContainer Container { get; } = new UnityContainer();
    }
}