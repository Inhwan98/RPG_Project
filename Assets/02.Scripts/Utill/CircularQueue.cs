using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InHwan.CircularQueue
{
    public class CircularQueue<T>
    {
        T[] queue;
        int front;
        int rear;
        int maxQueueSize;

        public CircularQueue(int k)
        {
            maxQueueSize = k + 1;
            queue = new T[maxQueueSize + 1];
            front = 0;
            rear = 0;
        }

        public bool EnQueue(T value)
        {
            if(IsFull()) return false;
            else
            {
                rear = ++rear % maxQueueSize;
                queue[rear] = value;
            }
            return true;
        }

        public bool DeQueue()
        {
            if(IsEmpty())
            {
                return false;
            }
            else
            {
                front = ++front % maxQueueSize;
                if (IsEmpty()) front = 0;
            }
            return true;
        }

        public T Front()
        {
            if (IsEmpty()) return default(T);
            else
            {
                return queue[(front + 1) % maxQueueSize];
            }
        }

        public T Rear()
        {
            if (IsEmpty()) return default(T);
            else
            {
                return queue[rear];
            }
        }

        public bool IsEmpty()
        {
            //Debug.Assert((front != rear), "Circular Queue is Empty !!! ");
            return (front == rear);
        }

        public bool IsFull()
        {
            return (front == (rear + 1) % maxQueueSize);
        }

    }
}
