using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapReposition : MonoBehaviour
{

    private Player player;

    private void Awake()
    {
        
    }

    public void Init()
    {
        player = BattleSceneManager.Instance.battleManager.PlayerBody.GetComponent<Player>();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("MapArea"))
            return;

        Vector2 playerPos = BattleSceneManager.Instance.battleManager.Player.transform.position;
        Vector2 myPos = transform.position; ;
        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);

        Vector3 playerDir = BattleSceneManager.Instance.battleManager.Player.inputVec;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;


        switch (transform.tag)
        {
            case "MapGround":
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 44);
                   // Debug.Log(name.ToString() + " X " + Vector3.right * dirX * 44);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 44);
                   // Debug.Log(name.ToString() + " Y " + Vector3.up * dirY * 44);
                }
                break;
            case "MapArea":
                break;

        }





    }






}
