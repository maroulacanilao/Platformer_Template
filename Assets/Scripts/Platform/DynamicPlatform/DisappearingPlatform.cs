using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platform.DynamicPlatform;

public class DisappearingPlatform : DynamicPlatform
{
    [SerializeField] private Collision_Handler.CollisionHandler _collisionHandler;
    [SerializeField] private float colDuration = 0.5f;
    [SerializeField] private float cooldownDuration = 2;
    [SerializeField] private GameObject platform;
    private bool isOnPlatform = false;
    private float timer;

    protected override void OnInitialized()
    {
        _collisionHandler.OnStartCollision += col =>
        {
            if(!col.CompareTag("Player")) return;

            isOnPlatform = true;
        };
        
        _collisionHandler.OnEndCollision += col =>
        {
            if(!col.CompareTag("Player")) return;

            isOnPlatform = false;
            timer = 0;
        };
    }
    

    private void FixedUpdate()
    {
        if (isOnPlatform && platform.activeSelf)
        {
            timer += Time.deltaTime;

            if (timer >= colDuration)
            {
                platform.SetActive(false);
                timer = 0;
                StartCoroutine(ReenablePlatform());
            }
        }
    }


    IEnumerator ReenablePlatform()
    {
        yield return new WaitForSeconds(cooldownDuration);
        platform.SetActive(true);
    }
    
    
    
}
