using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collision_Handler
{
    public class CollisionHandler : MonoBehaviour
    {
        public Action<GameObject> OnStartCollision;
        public Action<GameObject> OnEndCollision;
        public Action<GameObject> OnWhileCollision;

        private void OnCollisionEnter2D(Collision2D col)
        {
            OnStartCollision?.Invoke(col.gameObject);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnWhileCollision?.Invoke(collision.gameObject);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            OnEndCollision?.Invoke(other.gameObject);
        }
    }
}

