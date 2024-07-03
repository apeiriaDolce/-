using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIcontrol : MonoBehaviour
{
    public static UIcontrol Instance;
    //֪ʶ��������
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

    //ѡ���������
    #region
    public GameObject answerInterface;//ѡ��������
    public GameObject[] optionObject = new GameObject[4];//��ʾ��ѡ�����
    public TextMeshProUGUI[] optionText = new TextMeshProUGUI[4];//��ʾ��ѡ������
    public List<string> questionData = new List<string>();//������ı�����
    public List<OptionData> optionData = new List<OptionData>();//ѡ����ı�����
    public int questionIndex;//��ǰ�����ѡ�����ݵ�ָ��
    public int optionNum;//��һ��ѡ�����ж�ȡ����ѡ������
    public OptionTypeSetting[] optionTypeSettings = new OptionTypeSetting[4];//�����Ű�����

    public bool right;//�û���ǰ״̬�Ƿ���
    #endregion

    //ҽ�ƽ�������
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
        //ѡ���Ű��ʼ��
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

    //֪ʶ������غ���
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
    IEnumerator openKnowledgeWindowAni()//��Э��û�п�������,ע�ⲻҪ��������
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

    //ѡ�������غ���
    #region
    //ͨ���ϴ�������±��ȡ�����Ϣ
    public void loadOptionInterface(int index, int[] type)
    {
        plotControl.Instance.isKeepingShowText = true;
        answerInterface.SetActive(true);
        questionIndex = index;
        int optionNum = optionData[index].data.Count;
        for (int i = 0; i < optionNum; i++)//ѡ�����Ŀ������Ű�
        {
            optionObject[i].SetActive(true);
            optionObject[i].GetComponent<RectTransform>().localPosition = optionTypeSettings[optionNum - 1].typeSetting[i];
            optionObject[i].GetComponent<sceneButton>().index = type[i];
            optionText[i].text = optionData[index].data[i];
        }
        for (int i = optionNum; i < 4; i++)//ѡ�����Ľ���
        {
            optionObject[i].SetActive(false);
        }
    }
    //�жϻش��Ƿ���ȷ
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
    public void selectRight0()//�ӹ��򳡾�����ҽ�Ƴ���
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

    //ҽ�ƽ�������
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

    //�ı�����
    #region
    public void DataAwake()
    {
        knowledgeTitleData.Add("������ʾ");//0
        knowledgeData.Add("������ʾ����סA�����ƣ���סD�����ƣ�����ո������Ծ��ע���ܺ󷽵��ӵ���������µ�ը�����ڵ��水סS�����Զ��£�����ӵ���ը���Ĺ����������ׯ������ʱ���Ϯ����");
        knowledgeTitleData.Add("���߹�������");//1
        knowledgeData.Add("������........");
        knowledgeTitleData.Add("ҽ�Ƴ�����������");//2
        knowledgeData.Add("��˳�򽫱�����ҩˮ�������ϵ���Ӧλ�ü�����������϶�������۳���������ʾ�û�Ӧ������ʲô��");

        optionAwake();
    }
    public void optionAwake()
    {
        optionData.Add(new OptionData());//0
        optionData[0].data.Add("�������");
        optionData[0].data.Add("����ҩˮ");
        optionData[0].data.Add("�������");
        optionData[0].data.Add("��������");
        optionData[0].answerIndex = 3;

        optionData.Add(new OptionData());//1
        optionData[1].data.Add("��Ը�⣡");
        optionData[1].answerIndex = 0;

        optionData.Add(new OptionData());//2
        optionData[2].data.Add("��������豸");
        optionData[2].data.Add("ɴ����������������");
        optionData[2].data.Add("�ڿ�������豸���������ƻ�");
        optionData[2].answerIndex = 1;

        optionData.Add(new OptionData());//3
        optionData[3].data.Add("���䶡�����");
        optionData[3].data.Add("����Ϣʹ����̫��");
        optionData[3].data.Add("�������֣���˾ƥ�֡���ù��");
        optionData[3].answerIndex = 2;

        optionData.Add(new OptionData());//4
        optionData[4].data.Add("Ը��");
        optionData[4].data.Add("��Ը��");
        optionData[4].answerIndex = 0;

        optionData.Add(new OptionData());//5
        optionData[5].data.Add("�������������Ⲣ����һ�����ص����⣬���Զ��ڽ����");
        optionData[5].data.Add("��ά����Ȩ�Գ��׵ĸ��ƹ��˽׼�������״��ΪĿ�ġ�");
        optionData[5].data.Add("Ϊ����Ⱥ�ڵĽ���������������񲿾���С��ģ�ķ����˶���");
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

    //ҽ�Ƴ���Э��
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

        Vector2 uisize = medicalInterface.GetComponent<RectTransform>().sizeDelta;//�õ������ĳߴ�
        Vector2 screenpos = Input.mousePosition;
        Vector2 screenpos2;

        int effectIndex = 0 ;
        int medicalProcess = 0;

        GameObject bePointerTool = null;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))//����갴��ʱ����ȡ֪ʶ���ݣ������ͼƬ��Χ�ڣ����ȡ�ð�ť��knowledgeIndex��
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
                screenpos2.x = screenpos.x - (Screen.width / 2);//ת��Ϊ����Ļ����Ϊԭ�����Ļ����
                screenpos2.y = screenpos.y - (Screen.height / 2);
                pointerPos.x = screenpos2.x * (uisize.x / Screen.width);//ת�������Ļ����*��������Ļ��߱�
                pointerPos.y = screenpos2.y * (uisize.y / Screen.height);


                bePointerTool.GetComponent<RectTransform>().localPosition = pointerPos;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))//�����̧��ʱ��ʶ��ǰ��Χ�������ҽ��ʶ��Χ�ڣ���knowledgeIndex���볡������ť�ƶ��᷵��ԭ����
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
                                rightToolName += strings[i] + "����";
                            }
                            rightToolName += strings[strings.Count-1];
                            
                            medicalProcess++;
                            plotControl.Instance.plotData[11].charaName = "ҽ������";
                            plotControl.Instance.plotData[11].poltText = "ҽ���������ѣ�����Ӧ��ʹ��" + rightToolName+"���Ѿ�Ϊ���Զ�ʹ�ã������ʵ�顣";
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
    public bool isPointerEnterSubmitButton()//�ж�����Ƿ����ύ������
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
