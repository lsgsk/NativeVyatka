using Abstractions.Models;
using Foundation;
using System;
using UIKit;

namespace NativeVyatkaIOS.Utilities.TableSources
{
    public class MenuItem
    {
        public NavigationMenuIds Id { get; private set; }
        public string Image { get; private set; }
        public string Label { get; private set; }

        public MenuItem(NavigationMenuIds id, string image, string label)
        {
            this.Id = id;
            this.Image = image;
            this.Label = label;
        }
    }

    public class TableSource : UITableViewSource
    {
        public TableSource(MenuItem[] items)
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
            ItemSelected(mItems[indexPath.Row]);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier) ?? new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            var item = mItems[indexPath.Row];

            if (!string.IsNullOrEmpty(item.Image))
            {
                cell.ImageView.Image = UIImage.FromBundle(item.Image);
                cell.ImageView.TintColor = UIColor.Red;
            }
            cell.TintColor = UIColor.Gray;
            cell.TextLabel.Text = item.Label;
            return cell;
        }

        private readonly MenuItem[] mItems;
        private const string CellIdentifier = "TableCell";
        public delegate void OnItemSelected(MenuItem obj);
        public event OnItemSelected ItemSelected;
    }
}