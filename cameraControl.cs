using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class cameraControl : MonoBehaviour
{
    public static cameraControl Instance;
    public float width;
    public float height;
    public float left;
    public float top;
    public Vector3 playerPosX;
    public Vector3 playerPosY;
    public float moveSpeedX;
    public float moveSpeedY;
    public Camera thisCamera;

    //����һ���µĳ���ʱ��ȡ�����Ŀ�͸�
    public void Awake()
    {
        Instance = this;
        thisCamera = GetComponent<Camera>();
        //��ȡ�����Ŀ�͸�
       
        DontDestroyOnLoad(this.gameObject);
    }
    // ��Awake֮����ã������屻����֮���ٴμ���ʱҲ������ִ�У�����ɶʱ��ִ����Ҳ��֪����
    public void Start()
    {   
        width = sceneSystem.Instance.backGround.GetComponent<SpriteRenderer>().bounds.size.x;
        height = sceneSystem.Instance.backGround.GetComponent<SpriteRenderer>().bounds.size.y;
        playerPosX = playerControl.player.gameObject.transform.position;
        playerPosY = playerControl.player.gameObject.transform.position;
        moveSpeedX = 1f;
        moveSpeedY = 0.01f;
        left = width / 2 - thisCamera.orthographicSize * 2.1f;
        top = Instance.height / 2 - thisCamera.orthographicSize;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        followPlayer();
    }
    //Ĭ�����б������������궼�ǣ�0,0��
    void limit()
    {
        if(transform.position.x > left) { 
            transform.position=new Vector3(left,transform.position.y,transform.position.z);
        }
        else if(transform.position.x < -left)
        {
            transform.position = new Vector3(-left, transform.position.y, transform.position.z);
        }
        if (transform.position.y> top)
        {
            transform.position = new Vector3(transform.position.x, top, transform.position.z);
        }
        else if(transform.position.y < -top)
        {
            transform.position = new Vector3(transform.position.x, -top, transform.position.z);
        }
    }
    void followPlayer()
    {
        playerPosX = new Vector3(playerControl.player.transform.position.x, this.transform.position.y, -10);
        playerPosY = new Vector3(this.transform.position.x,playerControl.player.transform.position.y, -10);
        if (playerPosX != this.transform.position)
        {
            this.transform.position = Vector3.Lerp(this.transform.position,playerPosX, moveSpeedX);
        }
        if(playerPosY != this.transform.position)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, playerPosY, moveSpeedY);
        }
        limit();
    }
}
