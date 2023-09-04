using UnityEngine;

public class ResourcesData : MonoBehaviour
{
    public static ResourcesData instance = null;

    private ItemData _HPportion;
    private ItemData _MPportion;
    private ItemData _MidleAromor;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        _HPportion = Resources.Load<ItemData>("Item_Portion_HP");
        _MPportion = Resources.Load<ItemData>("Item_Portion_MP");
        _MidleAromor = Resources.Load<ItemData>("Item_Armor_Middle");
    }

    public ItemData GetHPportion() => _HPportion;
    public ItemData GetMPportion() => _MPportion;
    public ItemData GetMidleAromor() => _MidleAromor;
}