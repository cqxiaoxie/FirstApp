using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Host.Models
{
    public class CustomerSort
    {
        public List<SortClass> CheckSort(List<SortClass> Items, string sort_id, string pre_id)
        {
            if (Items == null || Items.Count == 0)
            {
                return null;
            }
            var the_item = Items.FirstOrDefault(p => p.ID == sort_id);
            if (the_item == null) { return null; }
            var remove_index = Items.FindIndex(p => p == the_item);
            Items.Remove(the_item);
            var insert_index = Items.FindIndex(p => p.ID == pre_id);
            Items.Insert(insert_index + 1, the_item);
            Items.ForEach(item => {
                item.Sort = Items.FindIndex(p => p == item) + 1;
            });
            return Items.OrderBy(p => p.Sort).ToList();
        }
    }

    public class SortClass
    {
        public string ID { get; set; }

        public int Sort { get; set; }
    }
}
