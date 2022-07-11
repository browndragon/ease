using System;
namespace BDEase
{
    public static class Logging
    {
        public static Action<string> DefaultLog = SDTWriteLine;
        public static Action<Exception> DefaultException = SDTWriteLine;
        public static void SDTWriteLine(Exception e) => System.Diagnostics.Trace.TraceWarning($"{e}");
        public static void SDTWriteLine(string s) => System.Diagnostics.Trace.TraceInformation(s);
    }
}