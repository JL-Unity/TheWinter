using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
        return _instance;
        }
    }
    #region 设定初始化
    public GameObject pacman;
    public GameObject blinky;
    public GameObject clyde;
    public GameObject inky;
    public GameObject pinky;
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject startCountDownPrefab;
    public GameObject gameOverPrefab;
    public GameObject winPrefab;
    public GameObject setPanel;
    public AudioClip startClip;

    public Text remainText;
    public Text nowText;
    public Text scoreText;

    private int pacdotNum = 0;
    private int nowEat = 0;
    public int Score = 0;

    public bool isSuperPacman = false;
    private SpriteRenderer SpriteRenderer;
    private int FreezeID = Animator.StringToHash("Freeze");
    private int DirXID = Animator.StringToHash("DirX");
    private int DirYID = Animator.StringToHash("DirY"); 

    public List<int> use = new List<int>();
    public List<int> wait = new List<int> { 0, 1, 2, 3 };
    private List<GameObject>  pacdotGos = new List<GameObject>();
    #endregion 

    private void Awake()
    {
        _instance = this;
        Screen.SetResolution(1024, 768, false);
        int tempCount = wait.Count;
        for(int i =0;i<tempCount;i++)//用来使每个ghost的第一段路都不一样
        {
            int temp = Random.Range(0, wait.Count);
            use.Add(wait[temp]);
            wait.Remove(wait[temp]);
        }
        foreach (Transform dots in GameObject.Find("Maze").transform)//遍历Maze中的所有豆子 存储其位置信息
        {
            pacdotGos.Add(dots.gameObject);
        }
        pacdotNum = GameObject.Find("Maze").transform.childCount;
    }

    private void Update()
    {
        //游戏胜利 后面的条件是为了防止一直重复调用
        if(nowEat==pacdotNum&&pacman.GetComponent<PacmanMove>().enabled!=false)
        {
            gamePanel.SetActive(false);
            Instantiate(winPrefab);
            StopAllCoroutines();
            SetGameState(false);
        }
        if(nowEat==pacdotNum)
        {
            if(Input.anyKeyDown)
            {
                SceneManager.LoadScene(0);
            }
        }
        
        if(gamePanel.activeInHierarchy)//游戏已经开始
        {
            remainText.text="Remain:\n\n "+ (pacdotNum - nowEat);
            nowText.text = "Eaten:\n\n" + (nowEat);
            scoreText.text = "Score:\n\n" + Score;
        }
    }

    public void OnEatPacdot(GameObject go) //吃下豆子后在list中移除豆子
    {
        pacdotGos.Remove(go);
        nowEat++;
        Score += 100;
    }
   private void Start()//游戏还没开始前设置默认为false
    {
        SetGameState(false);
    }
    #region 开始游戏界面
    public void OnStartButtom()
    {
        StartCoroutine(PlayStartCountDown());//开始游戏的准备
        AudioSource.PlayClipAtPoint(startClip, new Vector3(23, 16,-13));//在该处播放开始音乐
        startPanel.SetActive(false);
    }

    public void OnSetButtom()
    {
        setPanel.SetActive(true);
    }
    public void OnExitButtom()
    {
        Application.Quit();
    }

    #endregion

   

   
    IEnumerator PlayStartCountDown()
    {
        GameObject go = Instantiate(startCountDownPrefab);   //加载倒计时动画
        yield return new WaitForSeconds(4f);                             //等待四秒
        Destroy(go);                                                                      //销毁go物体
        SetGameState(true);                                                        //设置游戏开始状态
        Invoke("CreatSuperDot", 10f);                                          //开始生成超级豆
        gamePanel.SetActive(true);                                              //激活游戏分数面板
        GetComponent<AudioSource>().Play();                         //播放音乐 音乐组件在物体上
    }

    public void OnEatSuperPacdot() //吃下超级豆以后
    {
        Score += 200;
        isSuperPacman = true;//标识为超级吃豆人
        FreezeEnemy();//冻住敌人
        StartCoroutine(RecoveryEnemy());//携程开始
        Invoke("CreatSuperDot", 10f);//10s以后再生成超级豆
    }
    IEnumerator RecoveryEnemy()
    {
        yield return new WaitForSeconds(3f);//等待三秒
        UnFreezeEnemy();                               //敌人解冻
        isSuperPacman = false;                         //重新变回普通吃豆人
    } 
     private void CreatSuperDot()
    {
        //豆子数量小于5时不再生成超级豆
        if(pacdotGos.Count<5)
        {
            return;
        }

        int tempIndex = Random.Range(0, pacdotGos.Count);                    //随机选择一个豆子
        pacdotGos[tempIndex].transform.localScale = new Vector3(4, 4, 4);//使其变大三倍
        pacdotGos[tempIndex].GetComponent<Animator>().enabled = true;//加载超级豆的渐变动画
        pacdotGos[tempIndex].GetComponent<PacDot>().isSuperPacdot = true;

        
    }

# region 冰冻以及解冻敌人
private void FreezeEnemy()
    {
        //通过禁用敌人的移动脚本来冻结敌人
        blinky.GetComponent<GhostMove>().enabled = false;
        pinky.GetComponent<GhostMove>().enabled = false;
        inky.GetComponent<GhostMove>().enabled = false;
        clyde.GetComponent<GhostMove>().enabled = false;
        //通过虚化颜色来表示敌人被冻结
        blinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        pinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        inky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        clyde.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
        //加载虚化动画
        blinky.GetComponent<Animator>().SetBool(FreezeID, true);
        inky.GetComponent<Animator>().SetBool(FreezeID, true);
        pinky.GetComponent<Animator>().SetBool(FreezeID, true);
        clyde.GetComponent<Animator>().SetBool(FreezeID, true);
        //固定角色
       blinky.GetComponent<Animator>().SetFloat(DirXID, 0);
        inky.GetComponent<Animator>().SetFloat(DirXID, 0);
        pinky.GetComponent<Animator>().SetFloat(DirXID, 0);
        clyde.GetComponent<Animator>().SetFloat(DirXID, 0);
        blinky.GetComponent<Animator>().SetFloat(DirYID, 0);
        inky.GetComponent<Animator>().SetFloat(DirYID, 0);
        pinky.GetComponent<Animator>().SetFloat(DirYID, 0);
        clyde.GetComponent<Animator>().SetFloat(DirYID, 0);


    }
   
    private void UnFreezeEnemy()
    {
        blinky.GetComponent<GhostMove>().enabled = true;
        pinky.GetComponent<GhostMove>().enabled = true;
        inky.GetComponent<GhostMove>().enabled = true;
        clyde.GetComponent<GhostMove>().enabled = true;
        blinky.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        pinky.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        inky.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        clyde.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        blinky.GetComponent<Animator>().SetBool(FreezeID, false);
        inky.GetComponent<Animator>().SetBool(FreezeID, false);
        pinky.GetComponent<Animator>().SetBool(FreezeID, false);
        clyde.GetComponent<Animator>().SetBool(FreezeID, false);
    }
#endregion

    private void SetGameState(bool state)//游戏状态
    {
        
        blinky.GetComponent<GhostMove>().enabled = state;
        pinky.GetComponent<GhostMove>().enabled = state;
        inky.GetComponent<GhostMove>().enabled = state;
        clyde.GetComponent<GhostMove>().enabled = state;
        pacman.GetComponent<PacmanMove>().enabled = state;
    }
    
}
