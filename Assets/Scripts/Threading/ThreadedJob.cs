public class ThreadedJob
{
    private bool m_IsDone = false;
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

    public virtual void StartThread()
    {
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }

    public virtual void AbortThread()
    {
        m_Thread.Abort();
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnThreadFinished() { }

    public virtual bool UpdateThread()
    {
        if (m_Thread == null)
            return false;

        if (IsDone)
        {
            OnThreadFinished();
            m_Thread = null;
            return true;
        }
        return false;
    }
    private void Run()
    {
        ThreadFunction();
        IsDone = true;
    }
}