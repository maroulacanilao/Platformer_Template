using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessEndCollision : MonoBehaviour
{
    public Action<GameObject> OnTriggerEnter;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        OnTriggerEnter?.Invoke(col.gameObject);
    }
}
