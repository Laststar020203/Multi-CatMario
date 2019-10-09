using System.Text;
using System;
using UnityEngine;
public class User : PacketData
{

    //최대 글자수 제안 15글자
    private byte userId;
    private string userName;
    private byte userCharacterCode;

    public const byte NON_USER = 0;

    public byte ID { get { return userId; } set { userId = value; } }
    public string Name { get { return userName; } }
    public byte CharacteID { get { return userCharacterCode; } }

    public User(byte userId, string userName, byte userCharacterCode)
    {
        this.userId = userId;
        this.userName = userName;
        this.userCharacterCode = userCharacterCode;
    }

    public User(string userName, byte userCharacterCode) : this(Packet.Target.ACCESS_REQUESTER, userName, userCharacterCode)
    {

    }

    public User(byte[] data) : base(data)
    {

    }


    public User(byte[] data, byte newUserCode) : base(data)
    {
        this.userId = newUserCode;
    }


    protected override byte[] Serialize()
    {

        this.pData = new byte[17];

        this.pData[0] = userId;

        byte[] username = Encoding.UTF8.GetBytes(userName);

        if (username.Length > 15)
            username = Encoding.UTF8.GetBytes("IamSucker");

        Buffer.BlockCopy(username, 0, this.pData, 1, username.Length);
        pData[16] = userCharacterCode;

        return pData;
    }

    protected override void Deserialize()
    {

        this.userId = this.pData[0];
        byte[] userName = new byte[15];
        Buffer.BlockCopy(this.pData, 1, userName, 0, 15);
        this.userName = Encoding.UTF8.GetString(userName);
        this.userCharacterCode = this.pData[16];


    }
}
