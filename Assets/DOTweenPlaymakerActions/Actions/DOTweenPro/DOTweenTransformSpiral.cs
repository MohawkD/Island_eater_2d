using UnityEngine;
using HutongGames.PlayMaker;
using DG.Tweening;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween Pro")]
    [Tooltip("Tweens a Transform's localPosition in a spiral shape.")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenTransformSpiral : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Transform))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.FsmFloat)]
        [Tooltip("The duration of the tween")]
        public FsmFloat duration;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If isSpeedBased is TRUE sets the tween as speed based (the duration will represent the number of units/degrees the tween moves x second). NOTE: if you want your speed to be constant, also set the ease to Ease.Linear.")]
        public FsmBool setSpeedBased;

        [UIHint(UIHint.FsmFloat)]
        [Tooltip("Set a delayed startup for the tween")]
        public FsmFloat startDelay;

        [UIHint(UIHint.FsmVector3)]
        [Tooltip("The axis around which the spiral will rotate.")]
        public FsmVector3 axis;

        [Tooltip("The type of spiral movement.")]
        public SpiralMode spiralMode;

        [UIHint(UIHint.FsmFloat)]
        [Tooltip("Speed of the rotations.")]
        public FsmFloat speed;

        [UIHint(UIHint.FsmFloat)]
        [Tooltip("Frequency of the rotation. Lower values lead to wider spirals.")]
        public FsmFloat frequency;

        [UIHint(UIHint.FsmFloat)]
        [Tooltip("Indicates how much the tween should move along the spiral's axis.")]
        public FsmFloat depth;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE the tween will smoothly snap all values to integers.")]
        public FsmBool snapping;

        [ActionSection("Events")]
        [UIHint(UIHint.FsmEvent)]
        public FsmEvent startEvent;
        [UIHint(UIHint.FsmEvent)]
        public FsmEvent finishEvent;
        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE this action will finish immediately, if FALSE it will finish when the tween is complete.")]
        public FsmBool finishImmediately;

        [ActionSection("Tween ID")]

        [UIHint(UIHint.Description)]
        public string tweenIdDescription = "Set an ID for the tween, which can then be used as a filter with DOTween's Control Methods";

        [Tooltip("Select the source for the tween ID")]
        public DOTweenActionsEnums.TweenId tweenIdType;

        [UIHint(UIHint.FsmString)]
        [Tooltip("Use a String as the tween ID")]
        public FsmString stringAsId;

        [UIHint(UIHint.Tag)]
        [Tooltip("Use a Tag as the tween ID")]
        public FsmString tagAsId;

        [ActionSection("Ease Settings")]

        public DOTweenActionsEnums.SelectedEase selectedEase;
        [Tooltip("Sets the ease of the tween. If applied to a Sequence instead of a Tweener, the ease will be applied to the whole Sequence as if it was a single animated timeline.Sequences always have Ease.Linear by default, independently of the global default ease settings.")]
        public Ease easeType;
        public FsmAnimationCurve animationCurve;

        [ActionSection("Loop Settings")]

        [UIHint(UIHint.Description)]
        public string loopsDescriptionArea = "Setting loops to -1 will make the tween loop infinitely.";
        [UIHint(UIHint.FsmInt)]
        [Tooltip("Setting loops to -1 will make the tween loop infinitely.")]
        public FsmInt loops;

        [Tooltip("Sets the looping options (Restart, Yoyo, Incremental) for the tween. LoopType.Restart: When a loop ends it will restart from the beginning. LoopType.Yoyo: When a loop ends it will play backwards until it completes another loop, then forward again, then backwards again, and so on and on and on. LoopType.Incremental: Each time a loop ends the difference between its endValue and its startValue will be added to the endValue, thus creating tweens that increase their values with each loop cycle. Has no effect if the tween has already started.Also, infinite loops will not be applied if the tween is inside a Sequence.")]
        public LoopType loopType = LoopType.Restart;

        [ActionSection("Special Settings")]

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If autoKillOnCompletion is set to TRUE the tween will be killed as soon as it completes, otherwise it will stay in memory and you'll be able to reuse it.")]
        public FsmBool autoKillOnCompletion;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("Sets the recycling behaviour for the tween. If you don't set it then the default value (set either via DOTween.Init or DOTween.defaultRecyclable) will be used.")]
        public FsmBool recyclable;

        [Tooltip("Sets the type of update (Normal, Late or Fixed) for the tween and eventually tells it to ignore Unity's timeScale. UpdateType.Normal: Updates every frame during Update calls. UpdateType.Late: Updates every frame during LateUpdate calls. UpdateType.Fixed: Updates using FixedUpdate calls. ")]
        public UpdateType updateType = UpdateType.Normal;

        [UIHint(UIHint.FsmBool)]
        [Tooltip(" If TRUE the tween will ignore Unity's Time.timeScale. NOTE: independentUpdate works also with UpdateType.Fixed but is not recommended in that case (because at timeScale 0 FixedUpdate won't run).")]
        public FsmBool isIndependentUpdate;

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        private Tweener tweener;

        public override void Reset()
        {
            base.Reset();

            gameObject = null;

            duration = new FsmFloat { UseVariable = false };
            setSpeedBased = new FsmBool { UseVariable = false, Value = false };
            startDelay = new FsmFloat { Value = 0 };

            axis = null;
            spiralMode = SpiralMode.Expand;
            speed = new FsmFloat { UseVariable = false, Value = 1 };
            frequency = new FsmFloat { UseVariable = false, Value = 10 };
            depth = new FsmFloat { UseVariable = false, Value = 0 };
            snapping = new FsmBool { UseVariable = false, Value = false };

            startEvent = null;
            finishEvent = null;
            finishImmediately = new FsmBool { UseVariable = false, Value = false };

            stringAsId = new FsmString { UseVariable = false };
            tagAsId = new FsmString { UseVariable = false };

            selectedEase = DOTweenActionsEnums.SelectedEase.EaseType;
            easeType = Ease.Linear;

            loops = new FsmInt { Value = 0 };
            loopType = LoopType.Restart;

            autoKillOnCompletion = new FsmBool { Value = true };
            recyclable = new FsmBool { Value = false };

            updateType = UpdateType.Normal;
            isIndependentUpdate = new FsmBool { Value = false };

            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            tweener = Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<Transform>().DOSpiral(duration.Value, axis.Value, spiralMode, speed.Value, frequency.Value, depth.Value, snapping.Value);

            if (setSpeedBased.Value) tweener.SetSpeedBased();

            if (startEvent != null) tweener.OnStart(() => { Fsm.Event(startEvent); });
            if (finishEvent != null) tweener.OnComplete(() => { Fsm.Event(finishEvent); });

            switch (tweenIdType)
            {
                case DOTweenActionsEnums.TweenId.UseString: if (string.IsNullOrEmpty(stringAsId.Value) == false) tweener.SetId(stringAsId.Value); break;
                case DOTweenActionsEnums.TweenId.UseTag: if (string.IsNullOrEmpty(tagAsId.Value) == false) tweener.SetId(tagAsId.Value); break;
                case DOTweenActionsEnums.TweenId.UseGameObject: tweener.SetId(Fsm.GetOwnerDefaultTarget(gameObject)); break;
            }

            tweener.SetDelay(startDelay.Value);

            switch (selectedEase)
            {
                case DOTweenActionsEnums.SelectedEase.EaseType: tweener.SetEase(easeType); break;
                case DOTweenActionsEnums.SelectedEase.AnimationCurve: tweener.SetEase(animationCurve.curve); break;
            }

            tweener.SetLoops(loops.Value, loopType);

            tweener.SetAutoKill(autoKillOnCompletion.Value);
            tweener.SetRecyclable(recyclable.Value);
            tweener.SetUpdate(updateType, isIndependentUpdate.Value);

            // This allows Action Sequences of this action.
            if (!finishImmediately.Value) tweener.OnComplete(Finish);

            tweener.Play();

            if (debugThis.Value) Debug.Log("GameObject [" + State.Fsm.GameObjectName + "] FSM [" + State.Fsm.Name + "] State [" + State.Name + "] - DOTween Transform Move - SUCCESS!");

            if (finishImmediately.Value) Finish();
        }

    }
}