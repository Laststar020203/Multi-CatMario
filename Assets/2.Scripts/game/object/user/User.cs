using System.Text;
using System;

public class User : PacketData
{

    //최대 글자수 제안 15글자
    private string userName;
    private byte userCharacterCode;



    public string Name { get { return userName; } }
    public byte CharacteID { get { return userCharacterCode; } }

    public User(string userName, byte userCharacterCode)
    {
        this.userName = userName;
        this.userCharacterCode = userCharacterCode;
    }

    protected override byte[] Serialize()
    {
  
        this.pData = new byte[16];

        byte[] username = Encoding.UTF8.GetBytes(userName);

        if (username.Length > 15)
            username = Encoding.UTF8.GetBytes("IamSucker");

        Buffer.BlockCopy(username, 0, this.pData, 0, username.Length);
        pData[16] = userCharacterCode;

        return pData;
    }

    protected override object Deserialize()
    {
        return null;
    }
}
