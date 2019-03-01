using System;
using System.Collections;
using System.Collections.Generic;

namespace CollisionFloatTestNewMono.Engine
{
    public struct FixedArray2<T> : IEnumerable<T>
    {
        public T Value0, Value1;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.Value0;
                    case 1:
                        return this.Value1;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.Value0 = value;
                        break;
                    case 1:
                        this.Value1 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int IndexOf(T value)
        {
            for (int i = 0; i < 2; ++i)
                if (this[i].Equals(value))
                    return i;
            return -1;
        }

        public void Clear()
        {
            this.Value0 = this.Value1 = default(T);
        }

        private IEnumerable<T> Enumerate()
        {
            for (int i = 0; i < 2; ++i)
                yield return this[i];
        }
    }
}