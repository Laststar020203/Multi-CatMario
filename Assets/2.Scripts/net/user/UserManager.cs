using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Web;

public class UserManager : MonoBehaviour, IPacketDataReceiver, IPacketDataSender
{

    //처음에 자신의 유저 정볼르 저장해놈

    private UserManager instance;
    private List<User> users;

    private User me;

    public User Me { get { return me; } }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
        DontDestroyOnLoad(instance);

        users = new List<User>();

    }



    public bool CheckResponsible(byte type)
    {
        return false;
    }

    public string DataPasing(object o)
    {
        if(o is List<User>)
        {
            List<User> users = (List<User>) o;
            StringBuilder sb = new StringBuilder();
            

            

        }

        return null;
    }

    private bool isCollectUser(User user)
    {
        foreach(User u in users)
        {
            if (u.ID == user.ID)
                return false;
        }
        return true;
    }

    public void Receive(Packet packet)
    {
        
    }

    public string CurrentSituationData()
    {
        return DataPasing(users);
    }
}

public class User
{
    private byte userID;
    private string userName;
    private byte userCharacterCode;

    public byte ID { get { return userID; } }
    public string Name { get { return userName; } }
    public byte CharacteID { get { return userCharacterCode; } }

    public User(byte userID, string userName, byte userCharacterCode)
    {
        this.userID = userID;
        this.userName = userName;
        this.userCharacterCode = userCharacterCode;
    }


}
