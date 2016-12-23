using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory {

    #region member variables

    private List<KeyValuePair<string, byte>> m_contents;

    #endregion

    public Inventory()
    {
        m_contents = new List<KeyValuePair<string, byte>>();
	}
	
    public bool Contains(string itemName)
    {
        foreach (KeyValuePair<string, byte> item in m_contents)
        {
            if (item.Key == itemName)
                return true;
        }
        Debug.Log("item not in inventory :(");
        return false;
    }

    public void AddItem(string itemName, byte amount)
    {
        foreach (KeyValuePair<string, byte> item in m_contents)
        {
            if (item.Key == itemName)
            {
                m_contents.Remove(item);
                m_contents.Add(new KeyValuePair<string, byte>(item.Key, amount));
                return;
            }
        }
        m_contents.Add(new KeyValuePair<string, byte>(itemName, amount));
    }

    public void RemoveItem(string itemName, byte amount)
    {
        foreach (KeyValuePair<string, byte> item in m_contents)
        {
            if (item.Key == itemName)
            {
                if (item.Value - amount > 0)
                {
                    m_contents.Remove(item);
                    m_contents.Add(new KeyValuePair<string, byte>(item.Key, (byte)(item.Value - amount)));
                    return;
                }
                else
                    m_contents.Remove(item);
                return;
            }
        }
        Debug.Log("[while removing item] could not find the requested item :(");
    }

    public byte ItemAmount(string itemName)
    {
        foreach (KeyValuePair<string, byte> item in m_contents)
        {
            if (item.Key == itemName)
            {
                return item.Value;
            }
        }
        Debug.Log("[while finding amount] could not find the requested item :(");
        return 0;
    }
}
