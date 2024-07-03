using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIcontrol : MonoBehaviour
{
    public static UIcontrol Instance;
    //知识窗口数据
    #region
    public GameObject knowledgeWindow;

    public TextMeshProUGUI knowledgeWindowTitleText;
    public TextMeshProUGUI knowledgeWindowContextText;
    public Scrollbar knowledageBarV;

    public List<string> knowledgeData = new List<string>();
    public List<string> knowledgeTitleData = new List<string>();
    public bool knowledgeWindowVisible;
    public int knowledgeDataIndex;
    #endregion

    //选项界面数据
    #region
    public GameObject answerInterface;//选项界面对象
    public GameObject[] optionObject = new GameObject[4];//显示的选项对象
    public TextMeshProUGUI[] optionText = new TextMeshProUGUI[4];//显示的选项内容
    public List<string> questionData = new List<string>();//问题的文本内容
    public List<OptionData> optionData = new List<OptionData>();//选项的文本内容
    public int questionIndex;//当前问题和选项内容的指针
    public int optionNum;//从一个选项类中读取到的选项数量
    public OptionTypeSetting[] optionTypeSettings = new OptionTypeSetting[4];//四种排版数据

    public bool right;//用户当前状态是否答对
    #endregion

    //医疗界面数据
    #region
    public GameObject medicalInterface;
    public GameObject peopleImage;
    public GameObject[] toolGameObjects = new GameObject[16];
    public Coroutine medicalCoro;
    public Image processImage;
    public TextMeshProUGUI processText;
    public GameObject submitButton;
    public List<string> answerToolName= new List<string>();
    public List<int> answerToolIndex=new List<int>();
    #endregion

    public EventSystem eventSystem;


    private void Awake()
    {
        Instance = this;

        knowledgeWindow = GameObject.Find("Main Camera/knowledgeWindow");
        knowledgeWindowTitleText = GameObject.Find("Main Camera/knowledgeWindow/knowledgeTitle").GetComponent<TextMeshProUGUI>();
        knowledgeWindowContextText = GameObject.Find("Main Camera/knowledgeWindow/scrollView/viewPort/context").GetComponent<TextMeshProUGUI>();
        knowledageBarV = GameObject.Find("Main Camera/knowledgeWindow/scrollView/scrollBarHorizontal").GetComponent<Scrollbar>();

        answerInterface = GameObject.Find("Main Camera/answerInterface");
        for (int i = 0; i < 4; i++)
        {
            optionObject[i] = GameObject.Find("Main Camera/answerInterface/option" + i);
            optionText[i] = GameObject.Find("Main Camera/answerInterface/option" + i + "/optionText" + i).GetComponent<TextMeshProUGUI>();
        }

        medicalInterface = GameObject.Find("Main Camera/medicalInterface");
        peopleImage = GameObject.Find("Main Camera/medicalInterface/peopleInterface");
        for (int i = 0; i < 16; i++)
        {
            toolGameObjects[i] = GameObject.Find("Main Camera/medicalInterface/toolInterface/tool" + i);
        }
        processImage = GameObject.Find("Main Camera/medicalInterface/processBar/processImage").GetComponent<Image>();
        processText = GameObject.Find("Main Camera/medicalInterface/processBar/processText").GetComponent<TextMeshProUGUI>();
        submitButton = GameObject.Find("Main Camera/medicalInterface/peopleInterface/submitButton");

        eventSystem = GameObject.Find("Main Camera/EventSystem").GetComponent<EventSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //选项排版初始化
        #region
        optionTypeSettings[0] = new OptionTypeSetting();
        optionTypeSettings[0].typeSetting.Add(new Vector3(0, 0.5f, 0));

        optionTypeSettings[1] = new OptionTypeSetting();
        optionTypeSettings[1].typeSetting.Add(new Vector3(0, 0.9f, 0));
        optionTypeSettings[1].typeSetting.Add(new Vector3(0, 0.4f, 0));

        optionTypeSettings[2] = new OptionTypeSetting();
        optionTypeSettings[2].typeSetting.Add(new Vector3(0, 1.2f, 0));
        optionTypeSettings[2].typeSetting.Add(new Vector3(0, 0.7f, 0));
        optionTypeSettings[2].typeSetting.Add(new Vector3(0, 0.2f, 0));

        optionTypeSettings[3] = new OptionTypeSetting();
        optionTypeSettings[3].typeSetting.Add(new Vector3(0, 1.4f, 0));
        optionTypeSettings[3].typeSetting.Add(new Vector3(0, 0.9f, 0));
        optionTypeSettings[3].typeSetting.Add(new Vector3(0, 0.4f, 0));
        optionTypeSettings[3].typeSetting.Add(new Vector3(0, -0.1f, 0));
        #endregion

        answerInterface.SetActive(false);
        knowledgeWindow.SetActive(false);
        medicalInterface.SetActive(false);
        knowledgeWindowVisible = true;
        DataAwake();
    }

    //知识窗口相关函数
    #region
    public void openKnowledgeWindow(int index)
    {
        if (knowledgeWindowVisible)
        {
            StartCoroutine(openKnowledgeWindowAni());
            knowledgeWindow.SetActive(true);
            refreshKnowledgeWindow(index);
        }
    }
    IEnumerator openKnowledgeWindowAni()//该协程没有开启限制,注意不要开启过多
    {
        float i = 0;
        while (i <= 1)
        {
            i += 0.1f;
            knowledgeWindow.GetComponent<CanvasGroup>().alpha = i;
            if (i == 1)
            {
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    void refreshKnowledgeWindow(int index)
    {
        knowledageBarV.value = 0;
        knowledgeWindowTitleText.text = knowledgeTitleData[index];
        knowledgeWindowContextText.text = knowledgeData[index];
    }
    public void closeKnowledgeWindow()
    {
        knowledgeWindow.GetComponent<CanvasGroup>().alpha = 0;
        knowledgeWindow.SetActive(false);
    }
    #endregion

    //选项界面相关函数
    #region
    //通过上传问题的下标读取相关信息
    public void loadOptionInterface(int index, int[] type)
    {
        plotControl.Instance.isKeepingShowText = true;
        answerInterface.SetActive(true);
        questionIndex = index;
        int optionNum = optionData[index].data.Count;
        for (int i = 0; i < optionNum; i++)//选项对象的开启和排版
        {
            optionObject[i].SetActive(true);
            optionObject[i].GetComponent<RectTransform>().localPosition = optionTypeSettings[optionNum - 1].typeSetting[i];
            optionObject[i].GetComponent<sceneButton>().index = type[i];
            optionText[i].text = optionData[index].data[i];
        }
        for (int i = optionNum; i < 4; i++)//选项对象的禁用
        {
            optionObject[i].SetActive(false);
        }
    }
    //判断回答是否正确
    public void isRight(int index)
    {
        if (index == optionData[questionIndex].answerIndex)
        {
            right = true;
            Invoke("selectRight" + questionIndex, 0.1f);
        }
        else
        {
            right = false;
            Invoke("selectRight" + questionIndex, 0.1f);
        }
    }
    public void selectRight0()//从购买场景进入医疗场景
    {
        if(!right) { return; }
        answerInterface.SetActive(false);
        plotControl.Instance.isKeepingShowText = false;
        plotControl.Instance.continueShow();

    }
    public void selectRight1()
    {
        answerInterface.SetActive(false);
        plotControl.Instance.isKeepingShowText = false;
        plotControl.Instance.continueShow();
        GameObject doctor = GameObject.Find("SystemObject/redDoctor");
        doctor.SetActive(false);
    }
    public void selectRight2()
    {
        if(right)
        {
            plotControl.Instance.isKeepingShowText = false;
            plotControl.Instance.continueShow();
            plotControl.Instance.isKeepingShowText = true;
            plotControl.Instance.startShow(17, 17);
            int[] type = new int[3];
            type[0] = -2;
            type[1] = -2;
            type[2] = -2;
            loadOptionInterface(3, type);
            playerControl.Instance.reduceGrade(-10);
        }
        else
        {
            playerControl.Instance.reduceGrade(10);
            plotControl.Instance.startShow(19, 19);
        }
    }
    public void selectRight3()
    {
        if (right)
        {
            playerControl.Instance.reduceGrade(-10);
            closeKnowledgeWindow();
            plotControl.Instance.isKeepingShowText = false;
            plotControl.Instance.continueShow();
            plotControl.Instance.startShow(24, 24);
            answerInterface.SetActive(false);
        }
        else
        {
            playerControl.Instance.reduceGrade(10);
            plotControl.Instance.startShow(19, 19);
        }
    }
    public void selectRight4()
    {
        if (right)
        {
            plotControl.Instance.isKeepingShowText= false;
            plotControl.Instance.continueShow();
            answerInterface.SetActive(false);
        }
        else
        {
            plotControl.Instance.isKeepingShowText = true;
            plotControl.Instance.startShow(22, 22);
        }
    }
    public void selectRight5()
    {
        if (right)
        {
            playerControl.Instance.reduceGrade(-10);
            closeKnowledgeWindow();
            plotControl.Instance.isKeepingShowText = false;
            plotControl.Instance.continueShow();
            plotControl.Instance.startShow(24, 24);
            answerInterface.SetActive(false);
        }
        else
        {
            playerControl.Instance.reduceGrade(10);
            plotControl.Instance.startShow(19, 19);
        }
    }
    #endregion

    //医疗界面数据
    public void loadTool()
    {
        for (int i = 0; i < playerControl.Instance.tools.Count; i++)
        {
            toolGameObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = playerControl.Instance.tools[i].name;
        }
        for (int i = playerControl.Instance.tools.Count; i < 16; i++)
        {
            toolGameObjects[i].SetActive(false);
        }
    }

    //文本数据
    #region
    public void DataAwake()
    {
        knowledgeTitleData.Add("操作提示");//0
        knowledgeData.Add("操作提示：按住A键左移，按住D键右移，点击空格可以跳跃，注意躲避后方的子弹和随机落下的炸弹，在地面按住S键可以蹲下，躲避子弹和炸弹的攻击，进入村庄可以暂时躲避袭击。");
        knowledgeTitleData.Add("防疫管理条例");//1
        knowledgeData.Add("待补充........");
        knowledgeTitleData.Add("医疗场景测试用例");//2
        knowledgeData.Add("按顺序将绷带，药水，剪刀拖到相应位置即可完成任务，拖动错误则扣除分数并提示用户应该拖入什么。");

        optionAwake();
    }
    public void optionAwake()
    {
        optionData.Add(new OptionData());//0
        optionData[0].data.Add("购买绷带");
        optionData[0].data.Add("购买药水");
        optionData[0].data.Add("购买剪刀");
        optionData[0].data.Add("结束购买");
        optionData[0].answerIndex = 3;

        optionData.Add(new OptionData());//1
        optionData[1].data.Add("我愿意！");
        optionData[1].answerIndex = 0;

        optionData.Add(new OptionData());//2
        optionData[2].data.Add("超声诊断设备");
        optionData[2].data.Add("纱布，绷带，手术床");
        optionData[2].data.Add("内窥镜检查设备，激光治疗机");
        optionData[2].answerIndex = 1;

        optionData.Add(new OptionData());//3
        optionData[3].data.Add("杜冷丁，碘酒");
        optionData[3].data.Add("扑热息痛，保太松");
        optionData[3].data.Add("盘尼西林，阿司匹林、青霉素");
        optionData[3].answerIndex = 2;

        optionData.Add(new OptionData());//4
        optionData[4].data.Add("愿意");
        optionData[4].data.Add("不愿意");
        optionData[4].answerIndex = 0;

        optionData.Add(new OptionData());//5
        optionData[5].data.Add("苏区的瘟疫问题并非是一个严重的问题，可以短期解决。");
        optionData[5].data.Add("苏维埃政权以彻底的改善工人阶级的生活状况为目的。");
        optionData[5].data.Add("为保障群众的健康，决议责成内务部举行小规模的防疫运动。");
        optionData[5].answerIndex = 1;
    }
    public class OptionData
    {
        public List<string> data = new List<string>();
        public int answerIndex;
    }
    public class OptionTypeSetting
    {
        public List<Vector3> typeSetting = new List<Vector3>();
    }
    #endregion

    //医疗场景协程
    #region
    public void openMedicalInterface(string[] answerToolName,int[] answerToolIndex)
    {
       this.answerToolIndex.Clear();
       this.answerToolName.Clear();
       for(int i = 0; i < answerToolIndex.Length; i++)
        {
            this.answerToolName.Add(answerToolName[i]);
            this.answerToolIndex.Add(answerToolIndex[i]);
        }
        loadTool();
        medicalInterface.SetActive(true);
        //medicalCoro = StartCoroutine(medeicalInterfaceMethod());
    }
    public void closeMedicalInterface()
    {
        StopCoroutine(medicalCoro);
        medicalInterface.SetActive(false);
        plotControl.Instance.startShow(25, 25);
    }
    public IEnumerator medeicalInterfaceMethod()
    {
        Vector3 startPos = new Vector3(0, 0, 0);

        Vector3 pointerPos=new Vector3(0,0,0);

        Vector2 uisize = medicalInterface.GetComponent<RectTransform>().sizeDelta;//得到画布的尺寸
        Vector2 screenpos = Input.mousePosition;
        Vector2 screenpos2;

        int effectIndex = 0 ;
        int medicalProcess = 0;

        GameObject bePointerTool = null;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))//当鼠标按下时，读取知识数据，如果在图片范围内，则获取该按钮和knowledgeIndex。
            {
                if (EventSystem.current.currentSelectedGameObject != null&&EventSystem.current.currentSelectedGameObject.name!="textBoxImage")
                {
                    bePointerTool = EventSystem.current.currentSelectedGameObject;
                    startPos = bePointerTool.GetComponent<RectTransform>().position;
                    effectIndex = playerControl.Instance.tools[bePointerTool.GetComponent<sceneButton>().index].effectIndex;
                }
            }
            if (Input.GetKey(KeyCode.Mouse0) && bePointerTool != null)
            {
                screenpos = Input.mousePosition;
                screenpos2.x = screenpos.x - (Screen.width / 2);//转换为以屏幕中心为原点的屏幕坐标
                screenpos2.y = screenpos.y - (Screen.height / 2);
                pointerPos.x = screenpos2.x * (uisize.x / Screen.width);//转换后的屏幕坐标*画布与屏幕宽高比
                pointerPos.y = screenpos2.y * (uisize.y / Screen.height);


                bePointerTool.GetComponent<RectTransform>().localPosition = pointerPos;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))//当鼠标抬起时，识别当前范围，如果在医疗识别范围内，则将knowledgeIndex输入场景，按钮移动会返回原坐标
            {
                if(bePointerTool != null)
                {
                    bePointerTool.GetComponent<RectTransform>().position = startPos;
                    bePointerTool = null;
                    if (isPointerEnterSubmitButton())
                    {
                        if (effectIndex == answerToolIndex[medicalProcess])
                        {
                            medicalProcess++;
                        }
                        else
                        {
                            string rightToolName = "";
                            List<string> strings = new List<string>();
                            Debug.Log(1);
                            for(int i=0;i<playerControl.gameTools.Count;i++)
                            {
                                if (playerControl.gameTools[i].effectIndex == answerToolIndex[medicalProcess])
                                {
                                    strings.Add(playerControl.gameTools[i].name);
                                }
                            }
                            Debug.Log(strings.Count);
                            for(int i=0;i<strings.Count-1;i++)
                            {
                                rightToolName += strings[i] + "或者";
                            }
                            rightToolName += strings[strings.Count-1];
                            
                            medicalProcess++;
                            plotControl.Instance.plotData[11].charaName = "医疗助手";
                            plotControl.Instance.plotData[11].poltText = "医疗助手提醒：这里应该使用" + rightToolName+"，已经为您自动使用，请继续实验。";
                            plotControl.Instance.startShow(11, 11);
                            playerControl.Instance.reduceGrade(10);
                        }
                        if (medicalProcess == answerToolIndex.Count)
                        {
                            closeMedicalInterface();
                            break;
                        }
                    }
                }
            }
            yield return 0;
        }
        yield break;
    }
    public bool isPointerEnterSubmitButton()//判断鼠标是否在提交区域内
    {
        PointerEventData eventData=new PointerEventData(EventSystem.current);
        eventData.position=Input.mousePosition;
        GraphicRaycaster gr=submitButton.GetComponent<GraphicRaycaster>();
        List<RaycastResult> grapHic=new List<RaycastResult>();
        gr.Raycast(eventData, grapHic);
        if (grapHic.Count > 0 && grapHic[0].gameObject==submitButton)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
