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
    public int plotLastIndex;//�ı�����ָ��
    public int plotStartIndex;//�ı���ʼָ�룬����ʼָ����ڽ���ָ��ʱ�����ݳ�,ͬʱҲ�ǵ�ǰ�ı���ָ��

    //����ı�ˢ��ʱ�Ѿ���������Э�̣��򲻿����µ�Э�̣�������յ�ǰ�ı���������������
    public string showingText;//�Ѿ���ʾ���ı�
    public string toShowText;//��Ҫ��ʾ���ı�
    public bool textShowing;//����Э���Ƿ��ڿ�����
    public int textIndex;//�ı�ָ��
    public string charaName;
    public string charaImagePath;

    public Coroutine textShowCoro;
    public int lastShowType;
    public bool isKeepingShowText;//Ϊtrueʱָ���ı����Ž������˳��Ի����棬ֻ��ͨ���ⲿ�˳�


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
    //��ʼ�ݳ��ĺ���
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
        //ͨ�ô���
        if (plotData[plotStartIndex].method != "")
        {
            StartCoroutine(plotData[plotStartIndex].method);
        }
        lastShowType = plotData[plotStartIndex].showType;
        plotStartIndex++;
    }
    #endregion

    //ʵ���ݳ��ĺ���
    #region
    //��start����ʼ�ݳ��ı�����last-1�������ݳ�
    public void introductionSceneShow()//���ܳ����ݳ�
    {
        startTextShowIEnumerator(plotData[plotStartIndex].poltText, introductionText);
    }
    public void textSceneShow()//�Ի������ݳ�
    {
        //Debug.Log(plotData[plotStartIndex].poltText);
        startTextShowIEnumerator(plotData[plotStartIndex].poltText, textContent);
    }

    //�����ı����Ŷ���
    public void startTextShowIEnumerator(string text, TextMeshProUGUI showTextBox)
    {
        if (textShowing)//����Ѿ��ڲ����У�ˢ�½�Ҫ��ʾ���ı�������Ѿ���ʾ���ı����±�ָ��0
        {
            StopCoroutine(textShowCoro);
            toShowText = text;
            showingText = "";
            textIndex = 0;
            textShowCoro=StartCoroutine(textShow(showTextBox));
        }
        else//������ڲ����У�����Э��
        {
            toShowText = text;
            showingText = "";
            textIndex = 0;
            textShowCoro=StartCoroutine(textShow(showTextBox));
        }
    }
    IEnumerator textShow(TextMeshProUGUI showTextBox)//����ı�ˢ��ʱ�Ѿ���������Э�̣�����ͣЭ�̣�����ı����ٿ���
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

    //�籾����ʱʹ�õĺ���(����)

    //�籾������ͳ�ʼ��
    #region
    public class PlotData
    {
        public string poltText;//��������
        public int showType;//��ʾ���ͣ�0Ϊ���֣�1Ϊ����
        public string method="";//���ѡ���Ƿ�ִ�к���
        public string charaName = "";//�Ի�������
        public string ImageName = "none";//ͼƬ��
    }
    void dataAwke()
    {
        plotData.Add(new PlotData { poltText = "�ڱ���ʵ���У��㽫����һ��������ҽ���Ĵ��β����˵�ҽѧ����", showType = 1 });//0
        plotData.Add(new PlotData { poltText = "���Ǵ�һλ���˿��е�֪������һ����ׯ���������ߣ�Ϊ����ֹ���ߵĴ�����������������ǰ���Ǹ����ӡ�", showType = 1 });//1
        plotData.Add(new PlotData { poltText = "������·�ϣ������������վ���Ϯ��......", showType = 1 });//2
        plotData.Add(new PlotData { poltText = "�����ˣ���������ȥ����������û�л�·���������Ƿֿ��ܣ��ڴ����Ǳ߼��ϡ�", showType = 0,charaName="��ҽ��",ImageName="oldDoctor" });//3
        plotData.Add(new PlotData { poltText = "����ʦ��..........", showType = 0, charaName = "��" });//4
        plotData.Add(new PlotData { poltText = "����Ҫ�Ƕ��������ˣ���û��ȥ��������ֹ���ߴ����ˡ���ϻ��ˣ��Ͻ��ܡ�", showType = 0, charaName = "��ҽ��",ImageName = "oldDoctor" });//5
        plotData.Add(new PlotData { poltText = "��������ʾ����סA�����ƣ���סD�����ƣ�����ո������Ծ��ע���ܺ󷽵��ӵ���������µ�ը�����ڵ��水סS�����Զ��£�����ӵ���ը���Ĺ����������ׯ������ʱ���Ϯ������", showType = 0, charaName = "������ʾ" });//6
        plotData.Add(new PlotData { poltText = "��������������ˣ������", showType = 0, charaName = "�վ�" });//7
        plotData.Add(new PlotData { poltText = "���վ���׷���£����Ǳ������ׯ��ȥ���ڼ����ܽ���ׯʱ���Աߵ��������Ȼ�����˼���ǹ�죬�����ӵ��������зɳ������������վ���ȥ��", showType = 0, charaName = "" });//8
        plotData.Add(new PlotData { poltText = "С�ֵܣ�������", showType = 0, charaName = "���",ImageName="army" });//9
        plotData.Add(new PlotData { poltText = "��ѭ�����������ķ�����������֣�һ�Ӻ��������ǹ��׼�ղŵ��վ��������ǵĻ����£��㰲ȫ�ؽ��˴�ׯ��", showType = 0, charaName = "" ,method="startSceneEnd"});//10
        //���򳡾���ʾ
        #region
        plotData.Add(new PlotData { poltText = "��������������ˣ������", showType = 0, charaName = "" });//11
        #endregion

        plotData.Add(new PlotData { poltText = "��ѡ����ʵĹ�����ҽ��һλ�Ȳ����˵Ĵ��񣬵�����߿��Բ鿴���飬����������˳��ѡ��", showType = 0, charaName = "",method="medicalStart" });//12
        plotData.Add(new PlotData { poltText = "�빺��׼�������ʵĹ�����ҽ��һλ�Ȳ����˵Ĵ���", showType = 0, charaName = "", method = "" });//13
        plotData.Add(new PlotData { poltText = "��Ը������й��������Ϊһ����ɫҽ����Ϊսʤ���������������Լ��Ĺ�����", showType = 0, charaName = "���", method = "", ImageName = "army" });//14
        plotData.Add(new PlotData { poltText = "��������Ͻǵ���ά�������з����������ش�����", showType = 0, charaName = "ҽ������", method = "" });//15��K1
        plotData.Add(new PlotData { poltText = "�������������˾�����Ҫ���Ρ�", showType = 0, charaName = "ҽ������", method = "" });//16��Q2
        plotData.Add(new PlotData { poltText = "��ʾѡ����ȷҩ���豸��", showType = 0, charaName = "ҽ������", method = "" });//17,Q3
        plotData.Add(new PlotData { poltText = "���Ѿ��ɹ����ʵ�顣", showType = 1, charaName = "", method = "" });//18
        plotData.Add(new PlotData { poltText = "�ش���󣬿۳�10��", showType = 0, charaName = "ҽ������", method = "" });//19
        plotData.Add(new PlotData { poltText = "����׼�������ٿ�ʼ���Ρ�", showType = 0, charaName = "ҽ������", method = "" });//20

        plotData.Add(new PlotData { poltText = "��Ը�����ｨ����ҽԺ�Ĺ�������", showType = 0, charaName = "ҽ������", method = "" });//21��Q4
        plotData.Add(new PlotData { poltText = "�ܾ���Ϊ��ҽ���㣬��һ���������߳������ӵ������⵽���ձ������ߵ�Ϯ��������ɥ����", showType = 1, charaName = "", method = "reStart" });//22
        plotData.Add(new PlotData { poltText = "�㲻����Ϯ����ɥ�����������¿�ʼʵ�顣", showType = 1, charaName = "", method = "reStart" });//23
        plotData.Add(new PlotData { poltText = "�ش���ȷ������ʮ�֡�", showType = 0, charaName = "ҽ������", method = "" });//24
        plotData.Add(new PlotData { poltText = "���������굱ǰ�������ݣ����ύ�ɼ���", showType = 0, charaName = "ҽ������", method = "" });//25
    }
    #endregion

    public IEnumerator startSceneEnd()
    {
        sceneSystem.Instance.eventSystem.SetActive(false);//���õ��
        while (textShowing) //�Ķ���������ִ������ĺ���
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
        playerControl.Instance.gradeBar.text = "��ǰ�ɼ���70";
        sceneSystem.Instance.player.transform.position = new Vector3(-17.2f, -2.67f, 0);
        plotControl.Instance.isKeepingShowText = false;
        plotControl.Instance.continueShow();
        UIcontrol.Instance.answerInterface.SetActive(false);
        playerControl.Instance.isDead = false;
        playerControl.Instance.canMove = true;

        yield break;
    }
}
