// Guids.cs
// MUST match guids.h
using System;

namespace None.IronRacket_ReplWindow
{
    static class GuidList
    {
        public const string guidIronRacket_ReplWindowPkgString = "5ea0739e-79d2-4528-b4e1-71568943b68b";
        public const string guidIronRacket_ReplWindowCmdSetString = "efcc08de-adf1-41f4-944a-515314eab074";
        public const string guidToolWindowPersistanceString = "82546567-9eb2-4a3f-af1a-81f160588eb9";

        public static readonly Guid guidIronRacket_ReplWindowCmdSet = new Guid(guidIronRacket_ReplWindowCmdSetString);
    };
}