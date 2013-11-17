// Guids.cs
// MUST match guids.h
using System;

namespace IronPlot.IronRacketVSPackage
{
    static class GuidList
    {
        public const string guidIronRacketVSPackagePkgString = "edee9ac0-f922-4851-a6c7-d6bafa5e4388";
        public const string guidIronRacketVSPackageCmdSetString = "5c48336f-bd54-4e9a-b1bc-1e59ae4df9b7";
        public const string guidIronRacketVSPackageEditorFactoryString = "a37cef70-1331-4274-a119-79d407ee7743";

        public static readonly Guid guidIronRacketVSPackageCmdSet = new Guid(guidIronRacketVSPackageCmdSetString);
        public static readonly Guid guidIronRacketVSPackageEditorFactory = new Guid(guidIronRacketVSPackageEditorFactoryString);
    };
}