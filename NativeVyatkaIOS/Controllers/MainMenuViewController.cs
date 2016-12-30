using Abstractions.Interfaces.Plugins;
using System;
using Microsoft.Practices.Unity;
using UIKit;
using Abstractions.Models;
using Foundation;
using SidebarNavigation;

namespace NativeVyatkaIOS.Controllers
{
    public partial class MainMenuViewController : UIViewController
    {
        public MainMenuViewController(IntPtr handle) : base(handle)
        {
            mNavigator = App.Container.Resolve<ICrossPageNavigator>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var tableItems = new MenuItem[]
                                 {
                                     new MenuItem()
                                         {
                                             Id = NavigationMenuIds.RecordsList,
                                             Label = "Register Visitor",
                                             Image = "icon_visitor_signin"
                                         },
                                    new MenuItem()
                                        {
                                            Id = NavigationMenuIds.RecordsMap,
                                            Label = "Sign-Out Visitor",
                                            Image = "icon_signout_visitor"
                                        }                                    
                                 };

            var ds = new TableSource(tableItems);
            ds.ItemSelected += (item) =>
            {
                switch (item.Id)
                {
                    case NavigationMenuIds.RecordsList:
                    case NavigationMenuIds.RecordsMap:
                        break;
                }
                //SidebarController.CloseMenu();
            };
            //uxMenuTableView.Source = ds;
        }

        //protected SidebarController SidebarController => (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.SidebarController;
        private readonly ICrossPageNavigator mNavigator;

        protected class TableSource : UITableViewSource
        {
            private readonly MenuItem[] _tableItems;
            private const string CellIdentifier = "TableCell";

            public delegate void OnItemSelected(MenuItem obj);
            public event OnItemSelected ItemSelected;

            public TableSource(MenuItem[] items)
            {
                _tableItems = items;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return _tableItems.Length;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow(indexPath, true);
                ItemSelected(_tableItems[indexPath.Row]);
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) ?? new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
                var item = _tableItems[indexPath.Row];

                if (!string.IsNullOrEmpty(item.Image))
                {
                    cell.ImageView.Image = UIImage.FromBundle(item.Image);
                    cell.ImageView.TintColor = UIColor.Red;
                }
                cell.TintColor = UIColor.Gray;
                cell.TextLabel.Text = item.Label;
                return cell;
            }
        }

        public class MenuItem
        {
            public NavigationMenuIds Id { get; set; }
            public string Image { get; set; }
            public string Label { get; set; }           
        }
    }
}