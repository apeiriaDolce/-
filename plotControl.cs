using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class plotControl : MonoBehaviour
{
    public static plotControl Instance;

    public GameObject plotScene;
    public GameObject introductionScene;
    public GameObject talkScene;
    public Image charaImage;
    public TextMeshProUGUI charaNameContent;
    public TextMeshProUGUI introductionText;
    public TextMeshProUGUI textContent;

    public List<PlotData> plotData = new List<PlotData>();
    public int plotLastIndex;//文本结束指针
    public int plotStartIndex;//文本开始指针，当开始指针等于结束指针时结束演出,同时也是当前文本的指针

    //如果文本刷新时已经开启播放协程，则不开启新的协程，而是清空当前文本框，重新输入内容
    public string showingText;//已经显示的文本
    public string toShowText;//将要显示的文本
    public bool textShowing;//播放协程是否在开启中
    public int textIndex;//文本指针
    public string charaName;
    public string charaImagePath;

    public Coroutine textShowCoro;
    public int lastShowType;
    public bool isKeepingShowText;//为true时指定文本播放结束后不退出对话界面，只能通过外部退出


    private void Awake()
    {
        Instance = this;
        plotScene = GameObject.Find("Main Camera/plotScene");
        introductionScene = GameObject.Find("Main Camera/plotScene/introductionScene");
        talkScene = GameObject.Find("Main Camera/plotScene/talkScene");
        charaImage = GameObject.Find("Main Camera/plotScene/talkScene/charaImage").GetComponent<Image>();
        charaNameContent = GameObject.Find("Main Camera/plotScene/talkScene/textBoxImage/charaNameImage/nameContent").GetComponent<TextMeshProUGUI>();
        introductionText = GameObject.Find("Main Camera/plotScene/introductionScene/introductionText").GetComponent<TextMeshProUGUI>();
        textContent = GameObject.Find("Main Camera/plotScene/talkScene/textBoxImage/textContent").GetComponent<TextMeshProUGUI>();

    }
    // Start is called before the first frame update
    void Start()
    {
        toShowText = "";
        showingText = "";

        dataAwke();
        
        charaImagePath = "charaData/";

        startShow(0, 6);
    }
    //开始演出的函数
    #region
    public void startShow(int start,int last)
    {
        plotStartIndex = start;
        plotLastIndex = last;
        playerControl.Instance.canMove = false;
        lastShowType = -1;
        continueShow();
    }
    public void continueShow()
    {
        if (plotStartIndex == plotLastIndex+1)
        {
            if(isKeepingShowText == false)
            {
                talkScene.SetActive(false);
                introductionScene.SetActive(false);
                playerControl.Instance.canMove = true;
                textShowing = false;
                return;
            }
            else
            {
                //isKeepingShowText = false;
                return;
            }
        }
        if (plotData[plotStartIndex].showType==0)
        {
            if(lastShowType!= plotData[plotStartIndex].showType)
            {
                talkScene.SetActive(true);
                introductionScene.SetActive(false);
            }
            charaImage.sprite = Resources.Load<Sprite>(charaImagePath + plotData[plotStartIndex].ImageName);
            if(charaImage == null) 
            {
                Resources.Load<Image>(charaImagePath + "none");
            }
            charaNameContent.text = plotData[plotStartIndex].charaName;
            textSceneShow();
        }
        else
        {
            if (lastShowType != plotData[plotStartIndex].showType)
            {
                talkScene.SetActive(false);
                introductionScene.SetActive(true);
            }
            introductionSceneShow();
        }
        //通用处理
        if (plotData[plotStartIndex].method != "")
        {
            StartCoroutine(plotData[plotStartIndex].method);
        }
        lastShowType = plotData[plotStartIndex].showType;
        plotStartIndex++;
    }
    #endregion

    //实现演出的函数
    #region
    //从start处开始演出文本，到last-1处结束演出
    public void introductionSceneShow()//介绍场景演出
    {
        startTextShowIEnumerator(plotData[plotStartIndex].poltText, introductionText);
    }
    public void textSceneShow()//对话场景演出
    {
        //Debug.Log(plotData[plotStartIndex].poltText);
        startTextShowIEnumerator(plotData[plotStartIndex].poltText, textContent);
    }

    //开启文本播放动画
    public void startTextShowIEnumerator(string text, TextMeshProUGUI showTextBox)
    {
        if (textShowing)//如果已经在播放中，刷新将要显示的文本，清空已经显示的文本，下标指向0
        {
            StopCoroutine(textShowCoro);
            toShowText = text;
            showingText = "";
            textIndex = 0;
            textShowCoro=StartCoroutine(textShow(showTextBox));
        }
        else//如果不在播放中，则开启协程
        {
            toShowText = text;
            showingText = "";
            textIndex = 0;
            textShowCoro=StartCoroutine(textShow(showTextBox));
        }
    }
    IEnumerator textShow(TextMeshProUGUI showTextBox)//如果文本刷新时已经开启播放协程，则暂停协程，清空文本后再开启
    {
        textShowing = true;
        for (textIndex = 0; textIndex < toShowText.Length; textIndex++)
        {
            showingText += toShowText[textIndex];
            showTextBox.text = showingText;
            yield return new WaitForSeconds(0.07f);
        }
        textShowing = false;
        yield break;
    }
    #endregion

    //剧本播放时使用的函数(暂无)

    //剧本数据类和初始化
    #region
    public class PlotData
    {
        public string poltText;//文字内容
        public int showType;//显示类型，0为文字，1为介绍
        public string method="";//这个选项是否执行函数
        public string charaName = "";//对话人物名
        public string ImageName = "none";//图片名
    }
    void dataAwke()
    {
        plotData.Add(new PlotData { poltText = "在本次实验中，你将扮演一个跟随老医生四处治病救人的医学生。", showType = 1 });//0
        plotData.Add(new PlotData { poltText = "你们从一位病人口中得知附近有一个村庄发生了鼠疫，为了阻止鼠疫的传播，你们立即动身前往那个村子。", showType = 1 });//1
        plotData.Add(new PlotData { poltText = "不料在路上，你们遭遇了日军的袭击......", showType = 1 });//2
        plotData.Add(new PlotData { poltText = "不行了，这样跑下去咱俩两个都没有活路，等下咱们分开跑，在村子那边集合。", showType = 0,charaName="老医生",ImageName="oldDoctor" });//3
        plotData.Add(new PlotData { poltText = "可是师傅..........", showType = 0, charaName = "你" });//4
        plotData.Add(new PlotData { poltText = "我们要是都死在这了，就没人去村子里阻止鼠疫传播了。别废话了，赶紧跑。", showType = 0, charaName = "老医生",ImageName = "oldDoctor" });//5
        plotData.Add(new PlotData { poltText = "（操作提示：按住A键左移，按住D键右移，点击空格可以跳跃，注意躲避后方的子弹和随机落下的炸弹，在地面按住S键可以蹲下，躲避子弹和炸弹的攻击，进入村庄可以暂时躲避袭击。）", showType = 0, charaName = "操作提示" });//6
        plotData.Add(new PlotData { poltText = "（日语）发现他们了，射击！", showType = 0, charaName = "日军" });//7
        plotData.Add(new PlotData { poltText = "在日军的追击下，你狼狈地向村庄跑去。在即将跑进村庄时，旁边的树林里忽然响起了几声枪响，几颗子弹从树林中飞出，朝着身后的日军打去。", showType = 0, charaName = "" });//8
        plotData.Add(new PlotData { poltText = "小兄弟，这里，这里。", showType = 0, charaName = "红军",ImageName="army" });//9
        plotData.Add(new PlotData { poltText = "你循着声音传来的方向钻进了树林，一队红军正举着枪瞄准刚才的日军，在他们的护送下，你安全地进了村庄。", showType = 0, charaName = "" ,method="startSceneEnd"});//10
        //购买场景提示
        #region
        plotData.Add(new PlotData { poltText = "（日语）发现他们了，射击！", showType = 0, charaName = "" });//11
        #endregion

        plotData.Add(new PlotData { poltText = "请选择合适的工具来医治一位腿部受伤的村民，点击工具可以查看详情，请根据情况按顺序选择。", showType = 0, charaName = "",method="medicalStart" });//12
        plotData.Add(new PlotData { poltText = "请购买（准备）合适的工具来医治一位腿部受伤的村民。", showType = 0, charaName = "", method = "" });//13
        plotData.Add(new PlotData { poltText = "你愿意加入中国红军，成为一名红色医生，为战胜外来侵略者作出自己的贡献吗？", showType = 0, charaName = "红军", method = "", ImageName = "army" });//14
        plotData.Add(new PlotData { poltText = "请根据左上角的苏维埃区暂行防疫条例来回答问题", showType = 0, charaName = "医疗助手", method = "" });//15，K1
        plotData.Add(new PlotData { poltText = "紧急任务！有受伤军民需要救治。", showType = 0, charaName = "医疗助手", method = "" });//16，Q2
        plotData.Add(new PlotData { poltText = "提示选择正确药物设备：", showType = 0, charaName = "医疗助手", method = "" });//17,Q3
        plotData.Add(new PlotData { poltText = "您已经成功完成实验。", showType = 1, charaName = "", method = "" });//18
        plotData.Add(new PlotData { poltText = "回答错误，扣除10分", showType = 0, charaName = "医疗助手", method = "" });//19
        plotData.Add(new PlotData { poltText = "请先准备工具再开始救治。", showType = 0, charaName = "医疗助手", method = "" });//20

        plotData.Add(new PlotData { poltText = "你愿意加入筹建福音医院的过程中吗？", showType = 0, charaName = "医疗助手", method = "" });//21，Q4
        plotData.Add(new PlotData { poltText = "拒绝成为红医的你，在一次意外中走出了闽赣地区，遭到了日本侵略者的袭击，不幸丧生。", showType = 1, charaName = "", method = "reStart" });//22
        plotData.Add(new PlotData { poltText = "你不幸在袭击中丧生，即将重新开始实验。", showType = 1, charaName = "", method = "reStart" });//23
        plotData.Add(new PlotData { poltText = "回答正确，增加十分。", showType = 0, charaName = "医疗助手", method = "" });//24
        plotData.Add(new PlotData { poltText = "您已体验完当前所有内容，请提交成绩。", showType = 0, charaName = "医疗助手", method = "" });//25
    }
    #endregion

    public IEnumerator startSceneEnd()
    {
        sceneSystem.Instance.eventSystem.SetActive(false);//禁用点击
        while (textShowing) //阅读结束后再执行下面的函数
        {
            yield return new WaitForSeconds(1);
        }
        Vector3 toPos = new Vector3(0, 0, 10);
        sceneSystem.Instance.switchInterfacePos.localPosition = new Vector3(-8.5f, 0, 10);
        while (sceneSystem.Instance.switchInterfacePos.localPosition != toPos)
        {
            sceneSystem.Instance.switchInterfacePos.localPosition = Vector3.Lerp(sceneSystem.Instance.switchInterfacePos.localPosition, toPos, 0.7f);
            yield return new WaitForSeconds(0.1f);
        }
        AsyncOperation loadSceneAsyncOperation = SceneManager.LoadSceneAsync(1);
        while (loadSceneAsyncOperation.isDone == false)
        {
            yield return 0;
        }
        sceneSystem.Instance.eventSystem.SetActive(true);
        StartCoroutine(sceneSystem.Instance.switchScene(0));
        sceneSystem.Instance.VillagerSceneSystem();
        sceneSystem.Instance.player.transform.position = new Vector3(-17.2f, -2.67f, 0);

        sceneSystem.Instance.Awake();
        cameraControl.Instance.Awake();
        cameraControl.Instance.Start();
        playerControl.Instance.Awake();

        yield break;
    }
    IEnumerator medicalStart()
    {
        while (textShowing)
        {
            yield return new WaitForSeconds(1);
        }
        isKeepingShowText = false;
        continueShow();
        UIcontrol.Instance.medicalCoro = StartCoroutine(UIcontrol.Instance.medeicalInterfaceMethod());
        yield break;
    }
    IEnumerator reStart()
    {
        while (textShowing)
        {
            yield return new WaitForSeconds(1);
        }
        sceneSystem.Instance.StopCoro();
        AsyncOperation loadSceneAsyncOperation = SceneManager.LoadSceneAsync(2);
        while (loadSceneAsyncOperation.isDone == false)
        {
            yield return 0;
        }
        playerControl.Instance.tools.Clear();

        sceneSystem.Instance.Awake();
        sceneSystem.Instance.StartSceneSystem();
        cameraControl.Instance.Awake();
        cameraControl.Instance.Start();
        playerControl.Instance.Awake();
        
        playerControl.Instance.health = 10;
        playerControl.Instance.grade = 70;
        playerControl.Instance.takeDamega(0);
        playerControl.Instance.gradeBar.text = "当前成绩：70";
        sceneSystem.Instance.player.transform.position = new Vector3(-17.2f, -2.67f, 0);
        plotControl.Instance.isKeepingShowText = false;
        plotControl.Instance.continueShow();
        UIcontrol.Instance.answerInterface.SetActive(false);
        playerControl.Instance.isDead = false;
        playerControl.Instance.canMove = true;

        yield break;
    }
}
