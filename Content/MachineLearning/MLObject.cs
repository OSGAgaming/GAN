
using System;

namespace ArmourGan.MachineLearning
{
    [Serializable]
    public class MLObject
    {
        public void Load()
        {
            OnLoad();
        }
        public void Update()
        {
            OnUpdate();
        }
        public virtual void OnLoad() { ; }

        public virtual void OnUpdate() { ; }
        public MLObject()
        {
            Load();
        }
    }
}