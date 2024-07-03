using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class sceneButton : MonoBehaviour
{
    public int start;
    public int last;
    public int index=-1;//indexΪ-1��Ϊ����ѡ���0��ʼ������Ϊ����ѡ��
    public string charaName="none";//ֻ�������޷�ʵ�������л����飬�����������Ŀ���ÿ������
    public void openKnowledgeWindow()
    {
        UIcontrol.Instance.openKnowledgeWindow(index);
    }
    public void hideKnowledgeWindow()
    {
        UIcontrol.Instance.closeKnowledgeWindow();
    }
    public void startShow()
    {
        plotControl.Instance.startShow(start, last);
    }
    static public void continueShow()
    {
        plotControl.Instance.continueShow();
    }
    public void selectOption()
    {
        if (index == -2)
        {
            UIcontrol.Instance.isRight(start);
        }
        else if (index == -1)
        {
            if (playerControl.Instance.tools.Count >= 3)
            {
                UIcontrol.Instance.isRight(start);
            }
            else
            {
                plotControl.Instance.plotData[11].poltText = "�빺���������ֵ���";
                plotControl.Instance.plotData[11].charaName = "";
                plotControl.Instance.startShow(11, 11);
            }
        }
        else
        {
            buyTool();
        }
    }
    public void buyTool()
    {
        if (playerControl.gameTools[index].isHaving == false)
        {
            playerControl.Instance.tools.Add(playerControl.gameTools[index]);
            playerControl.gameTools[index].isHaving = true;
            plotControl.Instance.plotData[11].poltText = playerControl.gameTools[index].name + "����ɹ�";
            plotControl.Instance.plotData[11].charaName = "";
            plotControl.Instance.startShow(11, 11);
        }
        else
        {
            if (playerControl.Instance.tools[index].toolNum == -1)
            {
                plotControl.Instance.plotData[11].poltText = playerControl.gameTools[index].name+"�Ѿ�������";
                plotControl.Instance.plotData[11].charaName = "";
                plotControl.Instance.startShow(11, 11);
            }
            else
            {
                for(int i=0;i<playerControl.Instance.tools.Count;i++)
                {
                    if (playerControl.Instance.tools[i] == playerControl.gameTools[index])
                    {
                        playerControl.Instance.tools[i].toolNum++;
                        plotControl.Instance.plotData[11].poltText = playerControl.Instance.tools[i].name + "������һ����ǰ����Ϊ" + playerControl.Instance.tools[i].toolNum;
                        plotControl.Instance.plotData[11].charaName = "";
                        plotControl.Instance.startShow(11, 11);
                    }
                }
            }
        }
    }
    public void submit()
    {
        playerControl.Instance.endGame();

    }
}
