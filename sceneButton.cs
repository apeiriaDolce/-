using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class sceneButton : MonoBehaviour
{
    public int start;
    public int last;
    public int index=-1;//index为-1则为答题选项，从0开始的正数为购买选项
    public string charaName="none";//只有名字无法实现立绘切换表情，但现在这个项目不用考虑这个
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
                plotControl.Instance.plotData[11].poltText = "请购买以上三种道具";
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
            plotControl.Instance.plotData[11].poltText = playerControl.gameTools[index].name + "购买成功";
            plotControl.Instance.plotData[11].charaName = "";
            plotControl.Instance.startShow(11, 11);
        }
        else
        {
            if (playerControl.Instance.tools[index].toolNum == -1)
            {
                plotControl.Instance.plotData[11].poltText = playerControl.gameTools[index].name+"已经购买了";
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
                        plotControl.Instance.plotData[11].poltText = playerControl.Instance.tools[i].name + "数量加一，当前数量为" + playerControl.Instance.tools[i].toolNum;
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
