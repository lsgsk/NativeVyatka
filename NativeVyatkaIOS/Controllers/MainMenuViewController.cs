using UIKit;
using Abstractions.Models;
using Abstractions.Models.AppModels;
using NativeVyatkaIOS.Utilities.TableSources;
using System;
using FFImageLoading;
using FFImageLoading.Work;

namespace NativeVyatkaIOS.Controllers
{
    public partial class MainMenuViewController : UIViewController
    {
        public MainMenuViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var tableItems = new MenuItem[]
                                 {
                                    new MenuItem(NavigationMenuIds.RecordsList, "Мои записи", "menu_list.png"),
                                    new MenuItem(NavigationMenuIds.RecordsMap, "Карта захоронений","menu_globe.png")
                                 };
            var ds = new MenuTableSource(tableItems);
            ds.ItemSelected += (s, e) =>
            {
                switch (e.Id)
                {
                    case NavigationMenuIds.RecordsList:
                    case NavigationMenuIds.RecordsMap:
                        break;
                }
                CloceMenu?.Invoke(this, EventArgs.Empty);

            };
            tvMenuList.Source = ds;
        }

        public void SetProfile(ProfileModel profile)
        {
            lbProfileName.Text = profile.Name;
            lbProfileEmail.Text = profile.Email;
            imgProfilePhoto.Layer.CornerRadius = imgProfilePhoto.Frame.Width / 2;
            imgProfilePhoto.ClipsToBounds = true;
            if (string.IsNullOrEmpty(profile.PictureUrl))
            {
                ImageService.Instance.LoadFile("nophoto.png").Into(imgProfilePhoto);
            }
            else
            {
                ImageService.Instance.LoadUrl(profile.PictureUrl)
                    .LoadingPlaceholder("nophoto.png", ImageSource.ApplicationBundle)
                    .ErrorPlaceholder("nophoto.png", ImageSource.ApplicationBundle)
                    .Into(imgProfilePhoto);
            }
        }
        public event EventHandler CloceMenu;
    }
}