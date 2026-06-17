using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public Vector2 AimDirection { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool ToolUsePressed { get; private set; }
    public bool DashPressed { get; private set; }
    public int ToolSwitchInput { get; private set; }
    public bool SeedCyclePressed { get; private set; }

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        AimDirection = ReadAimDirection();
        AttackPressed = Input.GetMouseButtonDown(0);
        ToolUsePressed = Input.GetMouseButtonDown(1);
        DashPressed = Input.GetKeyDown(KeyCode.Space);
        ToolSwitchInput = ReadToolSwitch();
        SeedCyclePressed = Input.GetKeyDown(KeyCode.Q);
    }

    private Vector2 ReadAimDirection()
    {
        Vector3 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        return ((Vector2)(worldPos - transform.position)).normalized;
    }

    private int ReadToolSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return 3;
        return 0;
    }
}
