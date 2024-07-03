using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class sceneSystem : MonoBehaviour
{
    public static sceneSystem Instance;
    public GameObject backGround { get; private set; }
    public GameObject ground { get; private set; }
    public GameObject thisScene;
    public GameObject eventSystem;
    public Transform switchInterfacePos;
    public Transform cameraPos;

    public CapsuleCollider2D player;

    public string Name;
    public float timeScale;

    public Vector3 playerStartPosition;
    public bool bulletAttack;

    public List<BoxCollider2D> taskPoint = new List<BoxCollider2D>();//每个场景的任务检查点,每次切换场景时重新输入
    public bool isCycling;//是否处于循环中

    public List<Coroutine> taskCheck=new List<Coroutine>();

    Rigidbody2D bullet0;
    Rigidbody2D bullet1;
    Rigidbody2D bullet2;

    public List<GameObject> taskObject = new List<GameObject>();
    public void Awake()
    {
        Instance = this;
        backGround = GameObject.Find("SystemObject/backGround");
        ground = GameObject.Find("SystemObject/Ground");
        switchInterfacePos = GameObject.Find("Main Camera/switchInterface").GetComponent<Transform>();
        cameraPos = GameObject.Find("Main Camera").GetComponent<Transform>();
        eventSystem = GameObject.Find("Main Camera/EventSystem");

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartSceneSystem();
        //VillagerSceneSystem();
        playerControl.taskTitle.text = "无";

        switchScene(0);
    }

    //各场景开始时加载的函数，读取任务触发点
    #region
    public void StartSceneSystem()
    {
        bullet0 = GameObject.Find("shuyiPointer/bulletObject0").GetComponent<Rigidbody2D>();
        bullet1 = GameObject.Find("shuyiPointer/bulletObject1").GetComponent<Rigidbody2D>();
        bullet2 = GameObject.Find("shuyiPointer/bulletObject2").GetComponent<Rigidbody2D>();
        taskPoint.Clear();
        taskCheck.Clear();
        taskPoint.Add(GameObject.Find("SystemObject/hitEventPointer").GetComponent<BoxCollider2D>());
        taskPoint.Add(GameObject.Find("SystemObject/shuyiPointer").GetComponent<BoxCollider2D>());
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[0], "hitEventTask")));
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[1], "shuyiTask")));
    }
    public void VillagerSceneSystem()
    {
        //初始化
        taskObject.Clear();
        taskPoint.Clear();
        taskCheck.Clear();
        taskPoint.Add(GameObject.Find("SystemObject/medicalEventPointer").GetComponent<BoxCollider2D>());
        taskObject.Add(GameObject.Find("SystemObject/medicalEventPointer/prompt"));
        taskPoint.Add(GameObject.Find("SystemObject/takepartEventPointer").GetComponent<BoxCollider2D>());
        taskPoint.Add(GameObject.Find("SystemObject/QAEventPointer").GetComponent<BoxCollider2D>());
        taskPoint.Add(GameObject.Find("SystemObject/doctorEventPointer").GetComponent<BoxCollider2D>());
        taskPoint.Add(GameObject.Find("SystemObject/buyEventPointer").GetComponent<BoxCollider2D>());
        taskObject.Add(GameObject.Find("SystemObject/buyEventPointer/prompt"));
        taskPoint.Add(GameObject.Find("SystemObject/medicalEventPointer2").GetComponent<BoxCollider2D>());
        taskPoint.Add(GameObject.Find("SystemObject/endEventPointer").GetComponent<BoxCollider2D>());
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[0], "medicalEventTask")));
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[1], "takepartEventTask")));
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[2], "QAEventPointer")));
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[3], "doctorEventPointer")));
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[4], "buyEventPointer")));
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[5], "medicalEventTask2")));
        taskCheck.Add(StartCoroutine(touchingPoint(taskPoint[6], "endEventPointer")));

        plotControl.Instance.continueShow();
    }
    #endregion

    //玩家是否接触检查点协程,接触则通过Invoke的方式调用methodName的方式调用对应函数
    IEnumerator touchingPoint(BoxCollider2D taskPoint, string methodName)
    {
        while (true)
        {
            if (taskPoint.IsTouching(player))
            {
                taskPoint.gameObject.GetComponentInChildren<taskAttation>().gameObject.SetActive(false);
                Invoke(methodName, 0.1f);
                yield break;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    //场景特性，事件函数
    #region
    //开始场景
    #region
    public void endEventPointer()
    {
        playerControl.Instance.endGame();
    }
    public void medicalEventTask2()
    {
        int[] type = new int[3];
        type[0] = -2;
        type[1] = -2;
        type[2] = -2;
        plotControl.Instance.startShow(15, 16);
        UIcontrol.Instance.openKnowledgeWindow(1);
        plotControl.Instance.isKeepingShowText = true;
        UIcontrol.Instance.loadOptionInterface(2, type);
    }
    public void hitEventTask()
    {
        UIcontrol.Instance.knowledgeDataIndex = 0;
        UIcontrol.Instance.openKnowledgeWindow(0);
        bulletAttack = true;
        taskCheck.Add(StartCoroutine(bulletAttackEvent()));
        taskCheck.Add(StartCoroutine(bombAttackEvent()));
        plotControl.Instance.startShow(7, 7);
        playerControl.taskTitle.text = "逃离袭击";
    }
    public void shuyiTask()
    {
        playerControl.taskTitle.text = "鼠疫";
        bulletAttack = false;
        UIcontrol.Instance.closeKnowledgeWindow();

        bullet0.velocity = new Vector2(-23, 0);
        bullet1.velocity = new Vector2(-25, 0);
        bullet2.velocity = new Vector2(-27, 0);
        Invoke("destoryBullet", 3f);
        plotControl.Instance.startShow(8, 10);
    }
    void destoryBullet()
    {
        Destroy(bullet0.gameObject);
        Destroy(bullet1.gameObject);
        Destroy(bullet2.gameObject);
    }
    #endregion

    //村庄场景
    #region
    void medicalEventTask()//先执行购买，再执行医疗,可重复执行
    {
        if (playerControl.Instance.tools.Count > 0)
        {
            int[] ansArray = new int[3];
            string[] ansNameArray = new string[3];
            ansArray[0] = 0;
            ansNameArray[0] = "绷带";
            ansArray[1] = 1;
            ansNameArray[1] = "药水";
            ansArray[2] = 2;
            ansNameArray[2] = "剪刀";

            plotControl.Instance.startShow(12, 12);
            UIcontrol.Instance.openMedicalInterface(ansNameArray, ansArray);
            UIcontrol.Instance.openKnowledgeWindow(2);
        }
        else
        {
            plotControl.Instance.startShow(20, 20);
        }
        
        StartCoroutine(reStartMedical());
    }
    IEnumerator reStartMedical()
    {
        while (taskPoint[0].IsTouching(player))
        {
            yield return new WaitForSeconds(0.5f);
        }
        taskObject[0].SetActive(true);
        taskObject[0].GetComponent<taskAttation>().start();
        StartCoroutine(touchingPoint(taskPoint[0], "medicalEventTask"));
        yield break;
    }
    void takepartEventTask()
    {
        StartCoroutine(NpcMove());
    }
    IEnumerator NpcMove()
    {
        playerControl.Instance.canMove = false;
        GameObject doctor = GameObject.Find("SystemObject/redDoctor");
        Vector3 talkPos = new Vector3(-24.18f, -8.09f, 0);
        doctor.GetComponent<Animator>().SetBool("isWalk", true);
        while (doctor.GetComponent<BoxCollider2D>().IsTouching(taskPoint[1])==false)
        {
            doctor.GetComponent<Rigidbody2D>().velocity = new Vector2(-3, 0);
            yield return 0;
        }
        doctor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        doctor.GetComponent<Animator>().SetBool("isWalk", false);
        int[] type = new int[1];
        type[0] = -2;

        plotControl.Instance.startShow(14, 14);
        UIcontrol.Instance.loadOptionInterface(1, type);
        yield break;
    }
    void QAEventPointer()
    {
        int[] type = new int[3];
        type[0] = -2;
        type[1] = -2;
        type[2] = -2;
        plotControl.Instance.startShow(15, 15);
        UIcontrol.Instance.openKnowledgeWindow(1);
        plotControl.Instance.isKeepingShowText = true;
        UIcontrol.Instance.loadOptionInterface(5, type);
    }
    void buyEventPointer()
    {
        int[] type = new int[4];
        type[0] = 0;
        type[1] = 1;
        type[2] = 2;
        type[3] = -1;
        UIcontrol.Instance.loadOptionInterface(0, type);
        plotControl.Instance.startShow(13, 13);
        StartCoroutine(reStartBuy());
    }
    IEnumerator reStartBuy()
    {
        while (taskPoint[4].IsTouching(player))
        {
            yield return new WaitForSeconds(0.5f);
        }
        taskObject[1].SetActive(true);
        taskObject[1].GetComponent<taskAttation>().start();
        StartCoroutine(touchingPoint(taskPoint[4], "buyEventPointer"));
        yield break;
    }
    void doctorEventPointer()
    {
        int[] type = new int[2];
        type[0] = -2;
        type[1] = -2;
        plotControl.Instance.startShow(21, 21);
        UIcontrol.Instance.loadOptionInterface(4, type);
    }
    #endregion

    #endregion

    //开始场景结束协程
    public IEnumerator startSceneEnd()
    {
        eventSystem.SetActive(false);//禁用点击
        while(plotControl.Instance.textShowing) //阅读结束后再执行下面的函数
        {
            yield return new WaitForSeconds(1);
        }
        Vector3 toPos = new Vector3(0, 0, 10);
        switchInterfacePos.localPosition = new Vector3(-8.5f, 0, 10);
        while (switchInterfacePos.localPosition != toPos)
        {
            switchInterfacePos.localPosition = Vector3.Lerp(switchInterfacePos.localPosition, toPos, 0.7f);
            yield return new WaitForSeconds(0.1f);
        }
        AsyncOperation loadSceneAsyncOperation = SceneManager.LoadSceneAsync(1);
        while (loadSceneAsyncOperation.isDone==false)
        {
            yield return 0;
        }
        cameraControl.Instance.Start();
        eventSystem.SetActive(true);
        StartCoroutine(switchScene(0));
        VillagerSceneSystem();
        player.transform.position = new Vector3(-17.2f, -2.67f, 0);
        yield break;
    }

    //跑酷场景子弹攻击协程,接触到玩家或者下一个场景出发点时从起始位置重新开始移动
    #region
    IEnumerator bulletAttackEvent()
    {
        GameObject bullet = GameObject.Find("hitEventPointer/bulletObject");
        Rigidbody2D bulletRB = GameObject.Find("hitEventPointer/bulletObject").GetComponent<Rigidbody2D>();
        BoxCollider2D bulletBox = GameObject.Find("hitEventPointer/bulletObject").GetComponent<BoxCollider2D>();
        while (bulletAttack)
        {
            bulletRB.velocity = new Vector2(14, 0);
            bullet.transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (bulletBox.IsTouching(player))
            {
                playerControl.Instance.takeDamega(1);
                float bulletY = Random.Range(-2.29f, -0.59f);
                bullet.transform.localPosition = new Vector3(-14, bulletY, 0);
            }
            if (bulletBox.IsTouching(taskPoint[1]))
            {
                float bulletY = Random.Range(-2.29f, -0.59f);
                bullet.transform.localPosition = new Vector3(-14, bulletY, 0);
            }
            yield return 0;
        }
        Destroy(bullet);
        yield break;
    }
    #endregion

    //跑酷场景炸弹攻击协程,接触到地面时扩散到最大后从顶端随机落下
    #region
    bool isTouchingPlayer = false;
    IEnumerator bombAttackEvent()
    {
        GameObject bomb = GameObject.Find("hitEventPointer/bombObject");
        Rigidbody2D bombRB = GameObject.Find("hitEventPointer/bombObject").GetComponent<Rigidbody2D>();
        BoxCollider2D bombBox = GameObject.Find("hitEventPointer/bombObject").GetComponent<BoxCollider2D>();
        BoxCollider2D ground = GameObject.FindGameObjectWithTag("Ground").GetComponent<BoxCollider2D>();
        Animator ani = bomb.GetComponent<Animator>();
        float scale = 1;
        bombRB.gravityScale = 2.0f;
        while (bulletAttack)
        {
            bomb.transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (bombBox.IsTouching(ground))
            {
                ani.SetBool("isExpression", true);
                bombRB.gravityScale = 0.0f;
                bombRB.velocity = new Vector2(0, 0);
                if (scale < 2.125)
                {
                    scale += 0.125f;
                    bomb.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
                    bool isTouchingPlayer = false;
                    if (bombBox.IsTouching(player) && isTouchingPlayer == false)
                    {
                        isTouchingPlayer = true;
                        playerControl.Instance.takeDamega(1);
                    }
                }
                else
                {
                    ani.SetBool("isExpression", false);
                    bomb.GetComponent<RectTransform>().localScale = new Vector3(0.4f, 0.3f, 1);
                    bombRB.velocity = new Vector2(0, 0);
                    float bombX = Random.Range(3.3f, 29.2f);
                    bomb.transform.localPosition = new Vector3(bombX, 10.0f, 0);
                    scale = 1;
                    bombRB.gravityScale = 2.0f;
                    isTouchingPlayer = false;
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(bomb);
        yield break;
    }
    #endregion

    //场景切换特效协程,type=0为开始，type为1为结束,14,-14
    public IEnumerator switchScene(int type)
    {
        if (type == 0)
        {
            Vector3 toPos = new Vector3(-8.5f, 0, 10);
            switchInterfacePos.localPosition = new Vector3(0, 0, 10);
            while (switchInterfacePos.localPosition != toPos)
            {
                switchInterfacePos.localPosition = Vector3.Lerp(switchInterfacePos.localPosition, toPos, 0.7f);
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Vector3 toPos = new Vector3(0, 0, 10);
            switchInterfacePos.localPosition = new Vector3(-8.5f, 0, 10);
            while (switchInterfacePos.localPosition != toPos)
            {
                switchInterfacePos.localPosition = Vector3.Lerp(switchInterfacePos.localPosition, toPos, 0.7f);
                yield return new WaitForSeconds(0.1f);
            }
            //SceneManager.LoadScene(sceneIndex);
        }
        yield break;
    }
    public void StopCoro()
    {
        for (int i = 0; i < sceneSystem.Instance.taskCheck.Count; i++)
        {
            StopCoroutine(sceneSystem.Instance.taskCheck[i]);
        }
    }
}
