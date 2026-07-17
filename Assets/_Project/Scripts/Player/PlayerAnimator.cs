using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController _normalController;
    [SerializeField] private RuntimeAnimatorController _armoredController;

    private Animator _animator;
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static bool _armored;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAppearance() => _armored = false;

    public static void ResetProgress() => _armored = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        ApplyAppearance(_armored);
    }

    public void SetMoving(bool value) => _animator.SetBool(IsMoving, value);
    public void SetAttacking(bool value) => _animator.SetBool(IsAttacking, value);
    public void SetDead() => _animator.SetBool(IsDead, true);
    public void TriggerDash() => _animator.SetTrigger("Dash");

    public void SetArmored(bool armored)
    {
        _armored = armored;
        ApplyAppearance(armored);
    }

    private void ApplyAppearance(bool armored)
    {
        if (armored && _armoredController != null)
            _animator.runtimeAnimatorController = _armoredController;
        else if (_normalController != null)
            _animator.runtimeAnimatorController = _normalController;
    }
}
