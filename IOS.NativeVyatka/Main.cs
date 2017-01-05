using Microsoft.Practices.Unity;
using NativeVyatkaCore;
using NativeVyatkaCore.Database;
using NativeVyatkaCore.Utilities;
using System;
using UIKit;
using NativeVyatkaIOS.Utilities;
using Plugin.Media;

namespace NativeVyatkaIOS
{
    public class App
    {
        static void Main(string[] args)
        {           
            BurialDatabase.InitILobbyPhoneDatabase(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            iConsole.Init(new ConsoleRealization());
            CrossMedia.Current.Initialize();
            RegisterTypesIntoDI.InitContainer(Container);
            UIApplication.Main(args, null, "AppDelegate");
        }
        public static UnityContainer Container { get; } = new UnityContainer();
    }
}