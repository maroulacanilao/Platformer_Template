using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platform.DynamicPlatform;

public class MovingVerticalPlatform : DynamicPlatform
{
    protected override void OnInitialized()
    {
        tickValue = 0;
    }
}
