using System;

namespace PersonaBuildTool
{

    public abstract class PToolMode
    {
        public abstract void Execute();

        public abstract void ProcessCommandLine(Span<string> Arguments);
    }

}