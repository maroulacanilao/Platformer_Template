using System.Collections;
using System.Collections.Generic;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using Inputs;
using UnityEngine;

public class MonitoredProperties : MonitoredBehaviour
{
    private Rigidbody2D playerRigidbody;
    private PlayerController controller;
    protected override void Awake()
    {
        controller = FindObjectOfType<Inputs.PlayerController>();
        playerRigidbody = controller.GetComponent<Rigidbody2D>();
        MonitoringSystems.Resolve<IMonitoringManager>().RegisterTarget(this);
        this.RegisterMonitor();
        // Debug.Log();
    }

    [Monitor] private Vector2 playerVel => playerRigidbody.velocity;
    [Monitor] private Vector2 Input => new Vector2(controller.inputs.X, controller.inputs.Y);
    [Monitor] private Vector2 RawInput => new Vector2(controller.inputs.RawX, controller.inputs.RawY);
}
