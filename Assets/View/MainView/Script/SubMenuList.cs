using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using System.Reflection;
public class SubMenuList : MonoBehaviour
{
    public GameObject ManuBoard; 
    public List<CanvasGroup> MenuList;


    bool isShowMenu;
    private void Start()
    {
        ManuBoard.SetActive(false);
        isShowMenu = false; 
    }



    public void OnClickMenu()
    {
        if (isShowMenu)
        {
            Start();
            return; 
        }


        if (coShowList != null)
            StopCoroutine(coShowList);

        coShowList = StartCoroutine(CoShowList());

    }


    Coroutine coShowList; 

    IEnumerator CoShowList()
    {
        if(ManuBoard.activeSelf)
            yield break;

        ManuBoard.SetActive(true);
        foreach (CanvasGroup menu in MenuList)
        {
            menu.alpha = 0;
        }
       // yield return new WaitForEndOfFrame();

        float time = 0f;
      
        foreach (CanvasGroup menu in MenuList)
        {
            if (menu.gameObject.activeSelf == false)
                continue;

            time = 0f;
            while (time <= 0.10000f)
            {
                menu.alpha = Mathf.Lerp( 0f, 1f, time * 10f);
                time += Time.deltaTime;
                yield return null;
            }
           
        }

        isShowMenu = ManuBoard.activeSelf;
        
        yield break;
    }


    
}
