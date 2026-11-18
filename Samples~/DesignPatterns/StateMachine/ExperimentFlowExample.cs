using UnityEngine;

namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Demonstrates experiment flow using the State Machine pattern.
    ///     The experiment transitions through IRB-standard phases: consent → instructions → practice → main trials → debrief.
    ///     Each phase is a self-contained state — no spaghetti if-else chains.
    ///     To extend: add states like CalibrationState (eye tracking setup), BreakState (rest periods), or QuestionnaireState.
    /// </summary>
    [RequireComponent(typeof(StateMachine))]
    [AddComponentMenu("SOSXR/Design Patterns/Experiment Flow (State Machine Example)")]
    public class ExperimentFlowExample : MonoBehaviour
    {
        [SerializeField] private GameObject m_consentUI;
        [SerializeField] private GameObject m_instructionUI;
        [SerializeField] private GameObject m_trialUI;
        [SerializeField] private GameObject m_debriefUI;
        [SerializeField] private int m_practiceTrialCount = 5;
        [SerializeField] private int m_mainTrialCount = 50;

        private StateMachine _stateMachine;

        private ConsentState _consentState;
        private InstructionState _instructionState;
        private PracticeState _practiceState;
        private MainTrialState _mainTrialState;
        private DebriefState _debriefState;


        private void Awake()
        {
            _stateMachine = GetComponent<StateMachine>();

            _consentState = new ConsentState(this);
            _instructionState = new InstructionState(this);
            _practiceState = new PracticeState(this);
            _mainTrialState = new MainTrialState(this);
            _debriefState = new DebriefState(this);

            _stateMachine.ChangeState(_consentState);
        }


        private void SetUIActive(GameObject uiObject, bool isActive)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(isActive);
            }
        }


        private sealed class ConsentState : IState
        {
            private readonly ExperimentFlowExample _context;


            public ConsentState(ExperimentFlowExample context)
            {
                _context = context;
            }


            public void Enter()
            {
                _context.SetUIActive(_context.m_consentUI, true);
                Debug.Log("Entered Consent phase. Press C to accept consent.", _context);
            }


            public void Update()
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    _context._stateMachine.ChangeState(_context._instructionState);
                }
            }


            public void FixedUpdate()
            {
            }


            public void Exit()
            {
                _context.SetUIActive(_context.m_consentUI, false);
                Debug.Log("Exited Consent phase.", _context);
            }
        }


        private sealed class InstructionState : IState
        {
            private readonly ExperimentFlowExample _context;


            public InstructionState(ExperimentFlowExample context)
            {
                _context = context;
            }


            public void Enter()
            {
                _context.SetUIActive(_context.m_instructionUI, true);
                Debug.Log("Entered Instruction phase. Press Space to continue.", _context);
            }


            public void Update()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _context._stateMachine.ChangeState(_context._practiceState);
                }
            }


            public void FixedUpdate()
            {
            }


            public void Exit()
            {
                _context.SetUIActive(_context.m_instructionUI, false);
                Debug.Log("Exited Instruction phase.", _context);
            }
        }


        private sealed class PracticeState : IState
        {
            private readonly ExperimentFlowExample _context;
            private int _completedPracticeTrials;


            public PracticeState(ExperimentFlowExample context)
            {
                _context = context;
            }


            public void Enter()
            {
                _completedPracticeTrials = 0;
                _context.SetUIActive(_context.m_trialUI, true);
                Debug.Log($"Entered Practice phase. Complete {_context.m_practiceTrialCount} practice trials with Space.", _context);
            }


            public void Update()
            {
                if (!Input.GetKeyDown(KeyCode.Space))
                {
                    return;
                }

                _completedPracticeTrials++;
                Debug.Log($"Practice trial {_completedPracticeTrials}/{_context.m_practiceTrialCount} complete.", _context);

                if (_completedPracticeTrials >= _context.m_practiceTrialCount)
                {
                    _context._stateMachine.ChangeState(_context._mainTrialState);
                }
            }


            public void FixedUpdate()
            {
            }


            public void Exit()
            {
                _context.SetUIActive(_context.m_trialUI, false);
                Debug.Log("Exited Practice phase.", _context);
            }
        }


        private sealed class MainTrialState : IState
        {
            private readonly ExperimentFlowExample _context;
            private int _completedMainTrials;


            public MainTrialState(ExperimentFlowExample context)
            {
                _context = context;
            }


            public void Enter()
            {
                _completedMainTrials = 0;
                _context.SetUIActive(_context.m_trialUI, true);
                Debug.Log($"Entered Main Trial phase. Complete {_context.m_mainTrialCount} trials with Space.", _context);
            }


            public void Update()
            {
                if (!Input.GetKeyDown(KeyCode.Space))
                {
                    return;
                }

                _completedMainTrials++;
                Debug.Log($"Main trial {_completedMainTrials}/{_context.m_mainTrialCount} complete.", _context);

                if (_completedMainTrials >= _context.m_mainTrialCount)
                {
                    _context._stateMachine.ChangeState(_context._debriefState);
                }
            }


            public void FixedUpdate()
            {
            }


            public void Exit()
            {
                _context.SetUIActive(_context.m_trialUI, false);
                Debug.Log("Exited Main Trial phase.", _context);
            }
        }


        private sealed class DebriefState : IState
        {
            private readonly ExperimentFlowExample _context;
            private bool _hasLoggedCompletion;


            public DebriefState(ExperimentFlowExample context)
            {
                _context = context;
            }


            public void Enter()
            {
                _hasLoggedCompletion = false;
                _context.SetUIActive(_context.m_debriefUI, true);
                Debug.Log("Entered Debrief phase.", _context);
            }


            public void Update()
            {
                if (_hasLoggedCompletion)
                {
                    return;
                }

                _hasLoggedCompletion = true;
                Debug.Log("Experiment flow complete. Thank you for participating.", _context);
            }


            public void FixedUpdate()
            {
            }


            public void Exit()
            {
                _context.SetUIActive(_context.m_debriefUI, false);
                Debug.Log("Exited Debrief phase.", _context);
            }
        }
    }
}
