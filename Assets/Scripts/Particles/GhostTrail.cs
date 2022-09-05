using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Particles
{
    public class GhostTrail : MonoBehaviour
    { 
        public int ClonesPerSecond = 30; 
        private SpriteRenderer spriteRenderer; 
        private Rigidbody2D rb; 
        private List<SpriteRenderer> clones; 
        public Vector3 desiredScale = new Vector3(0f, 0f, 0f); 
        public Color desiredColor = new Color(0, 0, 0, 1f); 
        public float timer=0;
        
        private Vector3 ownerScale = new Vector3(); 
        private void Awake() 
        { 
            rb = GetComponentInParent<Rigidbody2D>();
            spriteRenderer = GetComponentInParent<SpriteRenderer>();
            clones = new List<SpriteRenderer>();
            ownerScale = rb.transform.localScale;
        }

        void OnEnable()
        {
            StartCoroutine(CreateTrail());
            timer = 0;
        }

        private void OnDisable()
        {
            StopCoroutine(CreateTrail());
            DestroyAllClones();
        }

        private void OnDestroy()
        {
            DestroyAllClones();
        }

        void Update()
        {
            timer += Time.deltaTime * 2;
            for (int i = 0; i < clones.Count; i++)
            {
                //clones[i].color -= colorPerSecond * Time.deltaTime;
                ////clones[i].transform.localScale -= scalePerSecond * Time.deltaTime;

                clones[i].color = Color.Lerp(clones[i].color, desiredColor, timer);
                //clones[i].transform.localScale = Vector3.Lerp(clones[i].transform.localScale, desiredScale, timer);

                if (clones[i].color.a <= 0f || clones[i].transform.localScale == Vector3.zero)
                {
                    Destroy(clones[i].gameObject);
                    clones.RemoveAt(i);
                    i--;
                } 
            }
        }

        IEnumerator CreateTrail()
        {
            while(this.enabled)
            { 
                if (rb.velocity != Vector2.zero)
                { 
                    var clone = new GameObject("trailClone"); 
                    clone.transform.position = transform.position;
                    clone.transform.localScale = ownerScale;
                    var cloneRend = clone.AddComponent<SpriteRenderer>();
                    cloneRend.sprite = spriteRenderer.sprite; 
                    cloneRend.sortingOrder = spriteRenderer.sortingOrder - 1;
                    cloneRend.flipX = spriteRenderer.flipX;
                    clones.Add(cloneRend);
                }
                yield return new WaitForSeconds(1f / ClonesPerSecond);
            }
            //DestroyAllClones();
        }

        void DestroyAllClones()
        { 
            if(clones.Count == 0 ) return;

            for (int i = clones.Count - 1; i >= 0; i--)
            {
                Destroy(clones[i].gameObject);
                clones.RemoveAt(i);
            }
        }
    }
}