﻿using System.Collections.Generic;
using System.Linq;

namespace CLI.Commands
{
    public abstract class CommandBase
    {
        protected ContextStack stack;
        public abstract string Name { get; }
        public abstract string[] Aliases { get; }

        public virtual bool Match(string cmd)
        {
            return cmd.StartsWith(this.Name) || this.Aliases.Any(c => cmd.StartsWith(c));
        }
    }
}
