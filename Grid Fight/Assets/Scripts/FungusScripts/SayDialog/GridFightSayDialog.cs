using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;


public class GridFightSayDialog : SayDialog
{

    protected override void Awake()
    {
        closeOtherDialogs = false;
        base.Awake();
    }


}
