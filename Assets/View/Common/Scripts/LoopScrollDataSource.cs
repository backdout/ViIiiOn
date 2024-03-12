using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UnityEngine.UI
{
    public abstract class LoopScrollDataSource
    {
        public abstract void ProvideData(Transform transform, int idx);
        public abstract void UpdateData(Transform transform);
        public abstract void AddData(object obj);
        public abstract void UpdateData(Transform transform, object value);
    }

    public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
        public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

        LoopScrollSendIndexSource() { }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("SetData", null);
        }

        public override void UpdateData(Transform transform)
        {
            transform.SendMessage("UpdateData");
        }
        public override void UpdateData(Transform transform, object value)
        {
            transform.SendMessage("UpdateData", value);
        }
        public override void AddData(object obj)
        {
        }
    }

    public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        List<T> objectsToFill;

        public LoopScrollArraySource(List<T> objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("SetData", objectsToFill[idx]);
        }

        public override void UpdateData(Transform transform)
        {
            transform.SendMessage("UpdateData");
        }
        public override void UpdateData(Transform transform, object value)
        {
            transform.SendMessage("UpdateData", value);
        }
        public override void AddData(object obj)
        {
            if (obj != null)
            {
                T[] list = obj as T[];

                objectsToFill.AddRange(list);
            }
        }
    }
}