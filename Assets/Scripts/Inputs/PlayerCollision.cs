using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public LayerMask layers;
    public float colRadius = 0.25f;
    public bool isGrounded, onWall, onRightWall, onLeftWall;
    public int wallSide;
    public Vector2 bottomOffset, rightOffset, leftOffset;

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, colRadius, layers);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, colRadius, layers) ||
                 Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, colRadius, layers);
        
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, colRadius, layers);
        
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, colRadius, layers);

        wallSide = onRightWall ? 1 : -1;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position  + bottomOffset, colRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, colRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, colRadius);

        
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube((Vector2) transform.position + rightOffset*1f, new Vector3(1f,1.5f));
        //Gizmos.DrawWireCube((Vector2) transform.position + leftOffset*1f, new Vector3(1f,1.5f));
    }

    public Collider2D[] GetCollidingGameObjectsOnLeft(LayerMask layersIncluded)
    {
        return Physics2D.OverlapBoxAll((Vector2) transform.position + leftOffset * 2f, new Vector2(1f, 1.0f), layersIncluded);
    }
    
    public Collider2D[] GetCollidingGameObjectsOnRight(LayerMask layersIncluded)
    {
        return Physics2D.OverlapBoxAll((Vector2) transform.position + rightOffset * 2f, new Vector2(1f, 1.0f), layersIncluded);
    }

    // public bool IsOnDynamicPlatform(out DynamicPlatform dynamicPlatform)
    // {
    //     dynamicPlatform = null;
    //     
    //     Collider2D col = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, colRadius, layers);
    //
    //     if (!col) return false;
    //
    //     return col.TryGetComponent(out dynamicPlatform);
    // }
}
