using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPacketDataReceiver
{
    /// <summary>
    /// UserManager에서 packet을 Receive 인자로 지급
    /// </summary>
    /// <param name="packet"></param>
    void Receive(Packet packet);

    /// <summary>
    /// Packet의 type체크로 받을 조건이 충족한지 확인
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool CheckResponsible(byte type);

}
