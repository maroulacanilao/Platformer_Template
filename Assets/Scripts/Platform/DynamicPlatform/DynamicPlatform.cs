using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platform.DynamicPlatform
{
    public abstract class DynamicPlatform : MonoBehaviour
    {
        protected Rigidbody2D platRb;
        protected SpriteRenderer spriteRenderer;
        protected Collider2D collider;
        protected float tickValue = 1.0f;

        private void Awake()
        {
            this.TryGetComponent<Rigidbody2D>(out platRb);
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<Collider2D>();

            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        protected IEnumerator Tick()
        {
            while (this.enabled)
            {
                OnTick();
                yield return new WaitForSeconds(tickValue);
            }
        }

        protected virtual void OnTick()
        {
        }
    }
}
