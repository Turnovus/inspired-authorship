using System;
using Verse;
using Verse.Grammar;

namespace InspiredAuthorship.Passages
{
    public class PassageDef : Def
    {
        public float baseCommonality = 1.0f;

        public Type workerClass;

        public RulePack rules;

        private PassageWorker workerInt;

        public PassageWorker Worker
        {
            get
            {
                if (workerInt == null)
                {
                    workerInt = (PassageWorker)Activator.CreateInstance(this.workerClass);
                    workerInt.def = this;
                }

                return workerInt;
            }
        }
    }
}