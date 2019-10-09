using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRequester
{
    bool IsRequesting { get; }
    byte RequestNumber { get; }
}
