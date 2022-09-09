using Baracuda.Monitoring;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private Transform Player;
    private Vector3 startingPos;

    [Monitor] public static int Score { get; private set; } = 0;
    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
        
        Player = FindObjectOfType<Inputs.PlayerController>().transform;
        
        startingPos = Player.position;
    }

    private void FixedUpdate()
    {
        float dist = Player.position.x - startingPos.x;
        
        if(dist <= Score) return;
        Score = (int) dist;
    }
}
