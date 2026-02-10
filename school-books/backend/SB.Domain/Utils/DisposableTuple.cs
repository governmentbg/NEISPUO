namespace SB.Domain;

using System;

public class DisposableTuple<T1, T2> : IDisposable
{
    private bool disposed;

    public T1 Item1 { get; private set; }
    public T2 Item2 { get; private set; }
    public DisposableTuple(T1 item1, T2 item2)
    {
        this.Item1 = item1;
        this.Item2 = item2;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        try
        {
            if (disposing)
            {
                using (this.Item1 as IDisposable)
                using (this.Item2 as IDisposable)
                {
                }
            }
        }
        finally
        {
            this.disposed = true;
        }
    }
}

public class DisposableTuple<T1, T2, T3> : IDisposable
{
    private bool disposed;

    public T1 Item1 { get; private set; }
    public T2 Item2 { get; private set; }
    public T3 Item3 { get; private set; }
    public DisposableTuple(T1 item1, T2 item2, T3 item3)
    {
        this.Item1 = item1;
        this.Item2 = item2;
        this.Item3 = item3;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        try
        {
            if (disposing)
            {
                using (this.Item1 as IDisposable)
                using (this.Item2 as IDisposable)
                using (this.Item3 as IDisposable)
                {
                }
            }
        }
        finally
        {
            this.disposed = true;
        }
    }
}

public class DisposableTuple<T1, T2, T3, T4> : IDisposable
{
    private bool disposed;

    public T1 Item1 { get; private set; }
    public T2 Item2 { get; private set; }
    public T3 Item3 { get; private set; }
    public T4 Item4 { get; private set; }
    public DisposableTuple(T1 item1, T2 item2, T3 item3, T4 item4)
    {
        this.Item1 = item1;
        this.Item2 = item2;
        this.Item3 = item3;
        this.Item4 = item4;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
        try
        {
            if (disposing && !(this.disposed))
            {
                using (this.Item1 as IDisposable)
                using (this.Item2 as IDisposable)
                using (this.Item3 as IDisposable)
                using (this.Item4 as IDisposable)
                {
                }
            }
        }
        finally
        {
            this.disposed = true;
        }
    }
}

public static class DisposableTuple
{
    public static DisposableTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
    {
        return new DisposableTuple<T1, T2>(item1, item2);
    }

    public static DisposableTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
    {
        return new DisposableTuple<T1, T2, T3>(item1, item2, item3);
    }

    public static DisposableTuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
    {
        return new DisposableTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
    }
}
