using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCluster : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private EndlessEndCollision endCollision;

    private void Awake()
    {
        endCollision = GetComponentInChildren<EndlessEndCollision>();
    }

    private void OnEnable()
    {
        endCollision.OnTriggerEnter += OnTrigger;
    }

    private void OnDisable()
    {
        endCollision.OnTriggerEnter -= OnTrigger;
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(-speed*Time.deltaTime, 0, 0);
    }

    void OnTrigger(GameObject col)
    {
        if (!col.CompareTag("Player")) return;
        
        Debug.Log("PLAYERTRR");
        
        endCollision.OnTriggerEnter -= OnTrigger;
        Destroy(endCollision.gameObject);
    }
}
