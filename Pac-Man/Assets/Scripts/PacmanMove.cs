using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanMove : MonoBehaviour
{
    private static PacmanMove pacmanMove;
    public static PacmanMove InstancePac
    {
        get
        {
            return InstancePac;
        }
    }

    //吃豆人的移动速度
    public float speed = 0.35f;
    private Vector2 destination = Vector2.zero;   //下一次移动的目的地
    private int DirXID = Animator.StringToHash("DirX");
    private int DirYID = Animator.StringToHash("DirY");
    private void Start()
    {
        destination = transform.position;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isSuperPacman)
        {
            speed = 0.5f;
        }
        else
            speed = 0.35f;
        //插值得到移动到dest位置的下一次移动坐标
        Vector2 temp= Vector2.MoveTowards(transform.position, destination, speed);
        //通过刚体来设置物体的位置
        GetComponent<Rigidbody2D>().MovePosition(temp);
        //必须达到下一次的目的地才能发出新的指示
        if((Vector2)transform.position==destination)
        {
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))&&Valid(Vector2.up))
            {
                destination = (Vector2)transform.position + Vector2.up;
            }
            if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))&& Valid(Vector2.down))
            {
                destination = (Vector2)transform.position + Vector2.down;
            }
            if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))&& Valid(Vector2.left))
            {
                destination = (Vector2)transform.position + Vector2.left;
            }
            if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))&& Valid(Vector2.right))
            {
                destination = (Vector2)transform.position + Vector2.right;
            }
            Vector2 dir = destination - (Vector2)transform.position;//确定 下一次方向
              GetComponent<Animator>().SetFloat(DirXID, dir.x);
              GetComponent<Animator>().SetFloat(DirYID, dir.y);
        }  
      
    }
  
    /// <summary>
    /// 检查将要到的位置是否可以到达
    private bool Valid(Vector2 dir)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);//  从 pos+dir位置向当前所在位置pos发射射线
        return (hit.collider == GetComponent<Collider2D>());// 如果射线撞到墙体说明此处无法再往外穿墙
    }
}
