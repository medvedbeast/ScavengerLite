using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class ObservableList<T> : IEnumerable<T>
{
    public event Action Changed;

    private List<T> collection;

    public ObservableList()
    {
        collection = new List<T>();
    }

    public int Count
    {
        get { return collection.Count; }
    }

    public void Add(T item, bool silent = false)
    {
        collection.Add(item);
        if (!silent)
        {
            Changed?.Invoke();
        }
    }

    public void AddRange(IEnumerable<T> items, bool silent = false)
    {
        collection.AddRange(items);
        if (!silent)
        {
            Changed?.Invoke();
        }
    }

    public void Remove(T item, bool silent = false)
    {
        collection.Remove(item);
        if (!silent)
        {
            Changed?.Invoke();
        }
    }

    public void RemoveRange(IEnumerable<T> items, bool silent = false)
    {
        for (int i = 0; i < items.Count(); i++)
        {
            collection.Remove(items.ElementAt(i));
        }
        if (!silent)
        {
            Changed?.Invoke();
        }
    }

    public void Insert(T item, int index, bool silent = false)
    {
        collection.Insert(index, item);
        if (!silent)
        {
            Changed?.Invoke();
        }
    }

    public void RemoveAt(int index, bool silent = false)
    {
        collection.RemoveAt(index);
        if (!silent)
        {
            Changed?.Invoke();
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    public T this[int index]
    {
        get
        {
            if (index >= 0 && index < collection.Count)
            {
                return collection[index];
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
        set
        {
            if (index >= 0 && index < collection.Count)
            {
                collection[index] = value;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }
    }

}
