﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaviorLibrary.Components.Composites
{
    public class Sequence : BehaviorComponent
    {

        protected BehaviorComponent[] s_Behaviors;

        private short sequence = 0;

        private short seqLength = 0;
        
        /// <summary>
        /// Performs the given behavior components sequentially
        /// Performs an AND-Like behavior and will perform each successive component
        /// -Returns Success if all behavior components return Success
        /// -Returns Running if an individual behavior component returns Success or Running
        /// -Returns Failure if a behavior components returns Failure or an error is encountered
        /// </summary>
        /// <param name="behaviors">one to many behavior components</param>
        public Sequence(params BehaviorComponent[] behaviors)
        {
            s_Behaviors = behaviors;
            seqLength = (short) s_Behaviors.Length;
        }

        /// <summary>
        /// performs the given behavior
        /// </summary>
        /// <returns>the behaviors return code</returns>
        public BehaviorReturnCode Behave()
        {
            //while you can go through them, do so
            while (sequence < seqLength)
            {
                try
                {
                    switch (s_Behaviors[sequence].Behave())
                    {
                        case BehaviorReturnCode.Failure:
                            sequence = 0;
                            this.ReturnCode = BehaviorReturnCode.Failure;
                            return BehaviorReturnCode.Failure;
                        case BehaviorReturnCode.Success:
                            sequence++;
                            this.ReturnCode = BehaviorReturnCode.Running;
                            return BehaviorReturnCode.Running;
                        case BehaviorReturnCode.Running:
                            this.ReturnCode = BehaviorReturnCode.Running;
                            return BehaviorReturnCode.Running;
                    }
                }
                catch (Exception)
                {
                    sequence = 0;
                    this.ReturnCode = BehaviorReturnCode.Failure;
                    return BehaviorReturnCode.Failure;
                }

            }

            sequence = 0;
            this.ReturnCode = BehaviorReturnCode.Success;
            return BehaviorReturnCode.Success;

        }

    }
}
