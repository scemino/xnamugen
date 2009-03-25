using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace xnaMugen.Diagnostics
{
    class DiagnosticSystem : SubSystem
    {
        public DiagnosticSystem(SubSystems subsystems)
            : base(subsystems)
        {
            m_form = new DiagnosticForm();
            m_formthread = new Thread(StartFormThread);
            m_lock = new Object();
        }

        public override void Initialize()
        {
            InitializationSettings settings = GetSubSystem<InitializationSettings>();

            if (settings.ShowDiagnosticWindow == true || Debugger.IsAttached == true)
            {
                Start();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                Stop();
            }

            base.Dispose(disposing);
        }

        public void Start()
        {
            lock (m_lock)
            {
                if (m_formthread.IsAlive == false)
                {
                    m_formthread.Start();
                }
            }
        }

        public void Stop()
        {
            lock (m_lock)
            {
                if (m_formthread.IsAlive == true)
                {
                    ThreadStart func = StopFormThread;
                    m_form.Invoke(func);

                    m_formthread.Join();
                }
            }
        }

        public void Update(Combat.FightEngine engine)
        {
            if (engine == null) throw new ArgumentNullException("engine");

            lock (m_lock)
            {
                if (m_formthread.IsAlive == true)
                {
                    Action<Combat.FightEngine> func = UpdateForm;
                    m_form.Invoke(func, engine);
                }
            }
        }

        void StartFormThread()
        {
            Application.Run(m_form);
        }

        void StopFormThread()
        {
            Application.ExitThread();
        }

        void UpdateForm(Combat.FightEngine engine)
        {
            if (engine == null) throw new ArgumentNullException("engine");

            m_form.Set(engine);
        }

        #region Fields

        readonly DiagnosticForm m_form;

        readonly Thread m_formthread;

        readonly Object m_lock;

        #endregion
    }
}