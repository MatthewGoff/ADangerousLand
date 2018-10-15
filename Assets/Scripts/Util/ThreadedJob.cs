using System.Collections;

namespace ADL.Util
{
    public class ThreadedJob
    {
        private bool m_IsDone = false;
        private bool m_IsRunning = false;
        private object m_Handle = new object();
        private System.Threading.Thread m_Thread = null;
        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (m_Handle)
                {
                    tmp = m_IsDone;
                }
                return tmp;
            }
            set
            {
                lock (m_Handle)
                {
                    m_IsDone = value;
                }
            }
        }
        public bool IsRunning
        {
            get
            {
                bool tmp;
                lock (m_Handle)
                {
                    tmp = m_IsRunning;
                }
                return tmp;
            }
            set
            {
                lock (m_Handle)
                {
                    m_IsRunning = value;
                }
            }
        }

        public virtual void Start()
        {
            m_Thread = new System.Threading.Thread(Run);
            m_Thread.Start();
        }
        public virtual void Abort()
        {
            m_Thread.Abort();
        }

        protected virtual void ThreadFunction() { }

        protected virtual void OnFinished() { }

        public virtual void Update()
        {
            if (IsDone)
            {
                OnFinished();
            }
        }
        public IEnumerator WaitFor()
        {
            while (!IsDone)
            {
                yield return null;
            }
        }
        private void Run()
        {
            IsRunning = true;
            ThreadFunction();
            IsRunning = false;
            IsDone = true;
        }
    }
}