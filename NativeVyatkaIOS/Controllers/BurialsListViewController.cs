using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace NativeVyatkaIOS.Controllers
{
    public partial class BurialsListViewController : UITableViewController
    {
        public BurialsListViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            NavigationItem.LeftBarButtonItem = EditButtonItem;
            var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewItem);
            addButton.AccessibilityLabel = "addButton";
            NavigationItem.RightBarButtonItem = addButton;
            TableView.Source = dataSource = new DataSource(this);
        }

        void AddNewItem(object sender, EventArgs args)
        {
            dataSource.Objects.Insert(0, DateTime.Now);
            using (var indexPath = NSIndexPath.FromRowSection(0, 0))
                TableView.InsertRows(new[] { indexPath }, UITableViewRowAnimation.Automatic);
        }
        
        DataSource dataSource;
        class DataSource : UITableViewSource
        {
            static readonly NSString CellIdentifier = new NSString("Cell");
            readonly List<object> objects = new List<object>()
            {
                "qwe",
                "asd",
                "zxc",
                "wer"
            };
            readonly BurialsListViewController controller;

            public DataSource(BurialsListViewController controller)
            {
                this.controller = controller;
            }

            public IList<object> Objects
            {
                get { return objects; }
            }

            // Customize the number of sections in the table view.
            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return objects.Count;
            }

            // Customize the appearance of table view cells.
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(CellIdentifier) ?? new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
                cell.TextLabel.Text = objects[indexPath.Row].ToString();
                return cell;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                // Return false if you do not want the specified item to be editable.
                return true;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                if (editingStyle == UITableViewCellEditingStyle.Delete)
                {
                    // Delete the row from the data source.
                    objects.RemoveAt(indexPath.Row);
                    controller.TableView.DeleteRows(new[] { indexPath }, UITableViewRowAnimation.Fade);
                }
                else if (editingStyle == UITableViewCellEditingStyle.Insert)
                {
                    // Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view.
                }
            }
        }
    }
}