using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib
{
    public interface IJobQueue
    {
        void Push(Action job);
    }
    // JobQueue에 밀어넣는 쓰레드는 여럿이지만
    // Flush를 실행시키는 놈은 하나다.
    public class JobQueue : IJobQueue
    {
        // JobQueue는 할 행동을 모아놓은 자료구조
        Queue<Action> _jobQueue = new();
        object _lock = new();
        bool _flush = false;

        public void Push(Action job)
        {
            bool flush = false;
            lock (_lock)
            {
                _jobQueue.Enqueue(job);
                if (_flush == false)
                {
                    flush = _flush = true;
                }
            }
            if (flush)
            {
                Flush();
            }
        }
        void Flush()
        {
            while (true)
            {
                Action action = Pop();
                if (action == null)
                    return;
                action.Invoke();
            }
        }
        Action Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count == 0 )
                {
                    _flush = false;
                    return null;
                }
                    
                return _jobQueue.Dequeue();
            }
        }
    }
}
