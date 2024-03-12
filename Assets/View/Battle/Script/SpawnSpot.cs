using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������� �� ��� ���� ����Ʈ�� �ٸ���, SpawnManager���� ���������� �޾ƿ´�
/// </summary>
public class SpawnSpot : MonoBehaviour
{
    

    public void SetData(GameObject mobPrefab)
    {
        if (mobPrefab == null)
            return;

        var mob = InstantiateMob(mobPrefab);

        mob.GetComponent<Monster>().Init();
        BattleSceneManager.Instance.battleManager.AddMonsterList(mob.GetComponent<Monster>());
        mob.SetActive(true);
        

    }


    private GameObject InstantiateMob(GameObject mob)
    {
        GameObject g = ObjectPoolManager.Instance.doInstantiate(mob);

        g.SetActive(false);
        g.transform.SetParent(this.transform, false);
        g.transform.SetPositionAndRotation(this.transform.localPosition, Quaternion.identity);
        return g;
    }


   

}
