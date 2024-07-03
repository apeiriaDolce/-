using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;



public class playerControl : MonoBehaviour
{
    public static playerControl Instance;

    public Rigidbody2D playerBody;
    public CapsuleCollider2D playerBox;
    public BoxCollider2D playerFeet;
    public bool canMove;
    public Animator ani;
    public float walkSpeed;
    public static GameObject player;
    public BoxCollider2D groundBox;
    public int jumpStength;
    public bool runing;

    public int health;//生命值，和成绩无关，只用于跑酷场景
    public int grade;//成绩

    public List<Tool> tools = new List<Tool>();//玩家采购的道具
    public static List<Tool> gameTools = new List<Tool>();//游戏总道具

    public static TextMeshProUGUI taskTitle;//当前任务名
    public TextMeshProUGUI healthBar;//生命值栏
    public TextMeshProUGUI gradeBar;//成绩栏
    public TextMeshProUGUI nameTitle;//身份栏

    public bool isDead;//防止死亡函数过多的执行

    [DllImport("__Internal")]
    public static extern void UploadScore(int score, int learnStatus);

    [DllImport("__Internal")]
    public static extern void endStart();

    public void Awake()
    {
        Instance = this;
        playerBody = GetComponent<Rigidbody2D>();
        playerBox = GetComponent<CapsuleCollider2D>();
        playerFeet = GetComponent<BoxCollider2D>();
        ani = GetComponent<Animator>();
        player = this.gameObject;
        groundBox = GameObject.FindGameObjectWithTag("Ground").GetComponent<BoxCollider2D>();

        taskTitle = GameObject.Find("Main Camera/stateBar/taskTitle").GetComponent<TextMeshProUGUI>();
        healthBar= GameObject.Find("Main Camera/stateBar/healthBar").GetComponent<TextMeshProUGUI>();
        gradeBar= GameObject.Find("Main Camera/stateBar/gradeBar").GetComponent<TextMeshProUGUI>();
        nameTitle = GameObject.Find("Main Camera/stateBar/nameTitle").GetComponent<TextMeshProUGUI>();
    }
    // Start is called before the first frame update
    void Start()
    {
        walkSpeed = 4.0f;
        jumpStength = 10;
        health = 10;
        grade = 70;
        takeDamega(0);
        gradeBar.text = "当前成绩：70";
        ToolAwake();

        DontDestroyOnLoad(player);
    }

    // Update is called once per frame
    void Update()
    {
        Flip();
        //transform.localRotation = Quaternion.Euler(0, transform.localRotation.y * 180, 0);
        if ((Input.GetButton("Horizontal") && canMove) || runing)
        {
            Move();
            ani.SetBool("isWalk", true);
        }
        else
        {
            ani.SetBool("isWalk", false);
        }
        if (Input.GetButton("GetDown") && playerFeet.IsTouching(groundBox))
        {
            canMove = false;
            playerBox.enabled = false;
            playerBody.velocity=new Vector2 (0, 0);
            ani.SetBool("isGetDown", true);
        }
        if (Input.GetButtonUp("GetDown"))
        {
            canMove = true;
            playerBox.enabled = true;
            ani.SetBool("isGetDown", false);
        }
        if (Input.GetButtonDown("Jump") && playerFeet.IsTouching(groundBox) && canMove)
        {
            Jump();
        }
    }
    public bool AddTool(string name2, int num)
    {
        if (tools.Count <= 10)
        {
            Tool tool = new Tool { name = name2, knowledgeIndex = num };
            tools.Add(tool);
            return true;
        }
        else
        {
            return false;
        }
    }
    //根据玩家速度值对玩家动画进行左右翻转
    void Flip()
    {
        bool PlayerSpeed = Mathf.Abs(playerBody.velocity.x) > Mathf.Epsilon;
        if (PlayerSpeed)
        {
            if (playerBody.velocity.x > 0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (playerBody.velocity.x < -0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
    void Move()
    {
        float moveH = Input.GetAxis("Horizontal");
        playerBody.velocity = new Vector2(walkSpeed * moveH, playerBody.velocity.y);
    }
    void Jump()
    {
        playerBody.velocity = new Vector2(playerBody.velocity.x, jumpStength);
        ani.SetBool("isJump", true);
    }
    public class Tool
    {
        public string name;//用于加载图片和文字显示
        public int knowledgeIndex;//用于加载知识窗口的下标，当该工具被拖拽入医疗界面时，将该数据读入医疗界面进度
        public int effectIndex;//效果指针
        public bool isHaving;//用户是否拥有这个工具
        public int toolNum;//用户拥有的数量
    }
    public void ToolAwake()
    {
        gameTools.Add(new Tool { name = "绷带", knowledgeIndex = 2, effectIndex = 0, isHaving = false, toolNum = 0 });
        gameTools.Add(new Tool { name = "药水", knowledgeIndex = 2, effectIndex = 1, isHaving = false, toolNum = 0 });
        gameTools.Add(new Tool { name = "剪刀", knowledgeIndex = 2, effectIndex = 2, isHaving = false, toolNum = -1 });
    }
    public void takeDamega(int num)
    {
        if (health > 0)
        {
            health -= num;
        }
        healthBar.text = "当前生命值：" + health;
        if(health <= 0)
        {
            if (isDead == false)
            {
                canMove = false;
                isDead = true;
                plotControl.Instance.isKeepingShowText = true;
                plotControl.Instance.startShow(23, 23);
            }
        }
    }
    public void reduceGrade(int num)
    {
        grade -= num;
        if (grade <= 0) grade = 0;
        gradeBar.text = "当前成绩："+grade.ToString();
        Coroutine reduceGradeAniCoro = StartCoroutine(reduceGradeAni());
        if (grade <= 70) nameTitle.text = "医学生";
        else if (grade <= 90) nameTitle.text = "名医";
        else if (grade <= 100) nameTitle.text = "红医";
    }
    IEnumerator reduceGradeAni()//扣除成绩时的红色闪烁特效
    {
        bool isRed = false;
        for(int i = 0; i < 6; i++)
        {
            if (isRed)
            {
                gradeBar.color= Color.black;
                isRed = false;
            }
            else
            {
                gradeBar.color=Color.red;
                isRed = true;
            }
            yield return new WaitForSeconds(0.3f);
        }
        yield break;
    }
    public void endGame()
    {
        plotControl.Instance.startShow(18, 18);
        UploadScore(grade, 3);
        endStart();
    }
}
