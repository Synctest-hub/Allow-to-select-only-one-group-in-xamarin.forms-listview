using Syncfusion.DataSource;
using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ListViewSample
{
   public class Behavior :Behavior<SfListView>
    {
        SfListView listView;
        protected override void OnAttachedTo(SfListView bindable)
        {
            listView = bindable;
            listView.DataSource.SortDescriptors.Add(new SortDescriptor() { PropertyName = "ContactName", Direction = ListSortDirection.Ascending });
            listView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "ContactName",
                KeySelector = (object obj1) =>
                {
                    var item = (obj1 as Contacts);
                    return item.ContactName[0].ToString();
                },
            });
            listView.SelectionChanging += ListView_SelectionChanging;
            base.OnAttachedTo(bindable);
        }
        private void ListView_SelectionChanging(object sender, ItemSelectionChangingEventArgs e)
        {
            GroupResult actualGroup = null;
            object key = null;
            var selectedItems = listView.SelectedItems;

            //To Cancel the Deselection
            if (e.RemovedItems.Count > 0 && selectedItems.Contains(e.RemovedItems[0]))
            {
                e.Cancel = true;
                return;
            }

            //To return when SelectedItems is zero
            if (e.AddedItems.Count <= 0)
                return;

            var itemData = (e.AddedItems[0] as Contacts);

            var descriptor = listView.DataSource.GroupDescriptors[0];
            if (descriptor.KeySelector == null)
            {
                var pbCollection = new PropertyInfoCollection(itemData.GetType());
                key = pbCollection.GetValue(itemData, descriptor.PropertyName);
            }
            else
                key = descriptor.KeySelector(itemData);

            for (int i = 0; i < listView.DataSource.Groups.Count; i++)
            {
                var group = listView.DataSource.Groups[i];

                if ((group.Key != null && group.Key.Equals(key)) || group.Key == key)
                {
                    actualGroup = group;
                    break;
                }
            }

            if (selectedItems.Count > 0)
            {
                foreach (var item in actualGroup.Items)
                {
                    var groupItem = item;

                    if (selectedItems.Contains(groupItem))
                    {
                        listView.SelectedItems.Remove(groupItem);
                        break;
                    }
                }
            }
        }

    }
}
