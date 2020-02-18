using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GhostMove : MonoBehaviour
{
    public GameObject[ ]  wayPointsGo;
    public float speed=0.2F;
    private int index = 0;
    private Vector3 startPosition;
    private List<Vector3> wayPoints = new List<Vector3>();
    private int DirXID = Animator.StringToHash("DirX");
    private int DirYID = Animator.StringToHash("DirY");

    public Slider Speed;
    public void SetSpeed()//设置速度
    {
        speed = Speed.value;
    }

    private void Start()
    {
        //设置个敌人的开始位置
        startPosition = transform.position + new Vector3(0, 3, 0);
       //range的范围为左闭右开区间
        Loads(wayPointsGo[GameManager.Instance.use[GetComponent<SpriteRenderer>().sortingOrder - 2]]);
    }
    private void FixedUpdate()
    {
        if (transform.position != wayPoints[index])
        {
            Vector2 temp = Vector2.MoveTowards(transform.position, wayPoints[index], speed);
            GetComponent<Rigidbody2D>().MovePosition(temp);

        }
        else
        {
            index++;
            if(index>=wayPoints.Count)
            {
                index = 0;
                Loads(wayPointsGo[Random.Range(0, 3)]);
            }
        }
        Vector2 dir = wayPoints[index] - transform.position;

        //将获得的移动方向返回给状态机
        GetComponent<Animator>().SetFloat(DirXID, dir.x);
        GetComponent<Animator>().SetFloat(DirYID, dir.y);
    }

    private void Loads(GameObject load)
    {
        wayPoints.Clear();
        foreach (Transform items in load.transform)
        {
            wayPoints.Add(items.position);
        }
        //在头尾插入起始节点
        wayPoints.Insert(0, startPosition);
        wayPoints.Add(startPosition);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Pacman")
        {
            if (GameManager.Instance.isSuperPacman)
            {
                GameManager.Instance.Score += 500;
                //回家
                transform.position = startPosition - new Vector3(0, 3, 0);
                index = 0;//回到起始点
            }
            else
            {
                collision.gameObject.SetActive(false);
                GameManager.Instance.gamePanel.SetActive(false);
                Instantiate(GameManager.Instance.gameOverPrefab);
                Invoke("ReStart", 3f);
            }      
        }
    }
    private void ReStart()
    {
        SceneManager.LoadScene(0);
    }
}
