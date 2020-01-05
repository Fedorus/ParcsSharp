using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parcs
{
    public class DataObjectsContainer<T>
    {
        public List<T> _items { get; set; } = new List<T>();
        public event EventHandler<DataReceivedEventArgs<T>> OnAdd;
        public void Add(T item)
        {
            var evArg = new DataReceivedEventArgs<T>(item); 
            OnAdd?.Invoke(this, evArg); 
            
            if (evArg.ReceivedItem != null)
            {
                _items.Add(item);
            }
        }
    }
    public class DataReceivedEventArgs<T> : EventArgs
    {
        public DataReceivedEventArgs(T receivedObject)
        {
            ReceivedItem = receivedObject;
        }
        public T ReceivedItem { get; set; }
    }
}
