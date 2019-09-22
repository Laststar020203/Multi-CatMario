using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketDataReceiver
{
    void Receive(Packet packet);
    bool CheckResponsible(byte type);

}
