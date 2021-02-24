﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DaScript
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
        T GetValue<T>();
    }
}