
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D WeaponCollider;
    UnitHandler Unit;


    public void Init(UnitHandler unit)
    {      
        Unit = unit;
        WeaponCollider.size = Unit.GetCharAni()._spriteOBj._weaponList[0].size;
    }



    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.activeSelf)
            return;
        UnitHandler target = collision.GetComponent<UnitHandler>();
        if (target != null)
            target.DoDamageEvent(Unit);
       
    }


}
