using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpdateableGameObject : MonoBehaviour
{
    public abstract void UpdateEveryFrame();
    public abstract void UpdateFixedFrame();

}
