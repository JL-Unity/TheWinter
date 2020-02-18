using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacDot : MonoBehaviour
{
    public bool isSuperPacdot = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman")
        {
            if(isSuperPacdot)
            {
                GameManager.Instance.OnEatPacdot(gameObject);//在list中移除该点
                GameManager.Instance.OnEatSuperPacdot();
                Destroy(gameObject);//销毁豆点
            }  
            else
            {
                GameManager.Instance.OnEatPacdot(gameObject);//在list中移除该点
                Destroy(gameObject);
            }
        }
      
    }

}
