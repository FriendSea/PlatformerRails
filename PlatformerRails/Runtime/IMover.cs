using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerRails
{
    public interface IMover
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public IRail Rail { get; }

        public event Action<IRail> RailChangeEvent;
    }
}
