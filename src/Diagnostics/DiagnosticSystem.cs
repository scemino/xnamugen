using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace xnaMugen.Diagnostics
{
    internal class DiagnosticSystem : SubSystem
    {
        public DiagnosticSystem(SubSystems subsystems)
            : base(subsystems)
        {
            m_form = new DiagnosticForm();
            m_formthread = new Thread(StartFormThread);
            m_lock = new object();
        }

        public override void Initialize()
        {
            var settings = GetSubSystem<InitializationSettings>();

            if (settings.ShowDiagnosticWindow || Debugger.IsAttached)
            {
                Start();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
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
                if (m_formthread.IsAlive)
                {
                    ThreadStart func = StopFormThread;
                    m_form.Invoke(func);

                    m_formthread.Join();
                }
            }
        }

        public void Update(Combat.FightEngine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            lock (m_lock)
            {
                if (m_formthread.IsAlive)
                {
                    Action<Combat.FightEngine> func = UpdateForm;
                    m_form.Invoke(func, engine);
                }
            }
        }

        private void StartFormThread()
        {
            Application.Run(m_form);
        }

        private void StopFormThread()
        {
            Application.ExitThread();
        }

        private void UpdateForm(Combat.FightEngine engine)
        {
            if (engine == null) throw new ArgumentNullException(nameof(engine));

            m_form.Set(engine);
        }

        #region Fields

        private readonly DiagnosticForm m_form;

        private readonly Thread m_formthread;

        private readonly object m_lock;

        #endregion
    }
}