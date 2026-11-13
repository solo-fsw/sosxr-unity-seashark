using SOSXR.SeaShark;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class AnimatorParameterExample : MonoBehaviour
{
    [AnimatorParameter] public string param;

    [AnimatorParameter(AnimatorParameterAttribute.ParameterType.Float)]
    public string floatParam;

    [AnimatorParameter(AnimatorParameterAttribute.ParameterType.Int)]
    public string intParam;

    [AnimatorParameter(AnimatorParameterAttribute.ParameterType.Bool)]
    public string boolParam;

    [AnimatorParameter(AnimatorParameterAttribute.ParameterType.Trigger)]
    public string triggerParam;


    private Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        var f = animator.GetFloat(floatParam);

        var i = animator.GetInteger(intParam);

        var b = animator.GetBool(boolParam);

        animator.SetTrigger(triggerParam);
    }
}