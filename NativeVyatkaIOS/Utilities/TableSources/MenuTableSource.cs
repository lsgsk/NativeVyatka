using Abstractions.Models;
using FFImageLoading;
using Foundation;
using System;
using UIKit;

namespace NativeVyatkaIOS.Utilities.TableSources
{
    public class MenuItem
    {
        public NavigationMenuIds Id { get; private set; }
        public string ImagePath { get; private set; }
        public string LabelText { get; private set; }

        public MenuItem(NavigationMenuIds id, string labelText, string imagePath)
        {
            this.Id = id;           
            this.LabelText = labelText;
            this.ImagePath = imagePath;
        }
    }

    public class MenuTableSource : UITableViewSource
    {
        public MenuTableSource(MenuItem[] items)
        {
            mItems = items;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return mItems.Length;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            ItemSelected?.Invoke(this, mItems[indexPath.Row]);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier) ?? new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            cell.TextLabel.Text = mItems[indexPath.Row].LabelText;
            //cell.ImageView.Image = UIImage.FromBundle(mItems[indexPath.Row].ImagePath);
            return cell;
        }

        private readonly MenuItem[] mItems;
        private const string CellIdentifier = "TableCell";
        public event EventHandler<MenuItem> ItemSelected;
    }
}