using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{

    public List<Item> commonItemPrefabs;
    public List<Item> uncommonItemPrefabs;
    public List<Item> rareItemPrefabs;

    Dictionary<ItemRarity, List<int>> itemIndexesDictionary;


    public ItemDrawer_SO currentDrawer;

    public List<int> GenerateBag(int levelGoal, float multiplier)
    {
        // считаем, сколько итемов нам понадобится на весь уровень в целом и какого вида(берем из шкафа)
        int itemCount = Mathf.RoundToInt(levelGoal * multiplier);
        PopulateListsOfItems(currentDrawer);

        // создаем список индексов объектов в бассейне и населяем его в зависимости от редкости итема
        List<int> itemIndexes = new List<int>();
        itemIndexes.AddRange(itemIndexesDictionary[ItemRarity.rare]); //все редкие добавляются по одной штуке
        itemIndexes.AddRange(CreateSubset(itemCount / 3, itemIndexesDictionary[ItemRarity.uncommon])); // создаем набор из необычных итемов
        itemIndexes.AddRange(CreateSubset(itemCount - itemIndexes.Count, itemIndexesDictionary[ItemRarity.common])); //все остальные слоты заполняются обычными итемами

        // перемешиваем полученный список индексов и возвращаем в менеджер
        ShuffleItems(itemIndexes, 2);
        //print(string.Join(", ", itemIndexes));
        return itemIndexes;
    }

    List<int> CreateSubset(int amount, List<int> usableIndexes)
    {
        List<int> result = new List<int>(amount);
        while (amount > 0)
        {
            result.Add(usableIndexes[Random.Range(0, usableIndexes.Count)]);
            amount--;
        }
        return result;
    }

    // этот метод перемешивает индексы в списке при создании итемов на весь уровень
    void ShuffleItems(List<int> list, int iterationsAmount)
    {
        while (iterationsAmount > 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int random = Random.Range(i, list.Count);
                int aux = list[random];
                list[random] = list[i];
                list[i] = aux;
            }
            iterationsAmount--;
        }
    }

    // это метод нужен для перемешивания готовых итемов в процессе игры
    public void ShuffleItems(List<Item> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);
            Item aux = list[random];
            list[random] = list[i];
            list[i] = aux;
        }
    }

    // достаем итемы из шкафа и раскладываем по словарю
    void PopulateListsOfItems(ItemDrawer_SO currDrawer)
    {
        // заполняем словарь индексов в зависимости от редкости объекта
        itemIndexesDictionary = new Dictionary<ItemRarity, List<int>>();
        foreach (Item i in currDrawer.availableItems)
        {
            if (itemIndexesDictionary.ContainsKey(i.itemRarity))
            {
                itemIndexesDictionary[i.itemRarity].Add(i.PoolIndex);
            }
            else
            {
                itemIndexesDictionary.Add(i.itemRarity, new List<int> { i.PoolIndex });
            }
        }
    }

}
