﻿using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace ReportUnitTest
{
    [TestFixture]
    public class JUnitTests
    {
        public static string ExecutableDir;
        public static string ResourcesDir;
        [OneTimeSetUp]
        public static void Setup()
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyDir == null)
            {
                throw new Exception("Failed to get assembly path");
            }

            TestContext.Out.WriteLine("Assembly: " + assemblyDir);

            ResourcesDir = Path.Combine(assemblyDir, "..", "..", "Resources");
            if (!Directory.Exists(ResourcesDir))
            {
                throw new Exception("Can't find Resources folder");
            }
            TestContext.Out.WriteLine("Resources: " + ResourcesDir);

            ExecutableDir = Path.Combine(assemblyDir, "..", "..", "..", "ReportUnit", "bin");
            if (!Directory.Exists(ExecutableDir))
            {
                throw new Exception("Can't find ReportUnit folder");
            }
            TestContext.Out.WriteLine("Executable: " + ExecutableDir);
        }

        [Test]
        public void Test()
        {
            var filename = Path.Combine(ExecutableDir, "ReportUnit.exe");
            var proc = System.Diagnostics.Process.Start(filename, "test_junit_01.xml");
            if (proc == null)
            {
                throw new Exception("Failed to start");
            }
            if (!proc.WaitForExit(5000))
            {
                throw new Exception("Timeout");
            }

            if (proc.ExitCode != 0)
            {
                throw new Exception("Exit code " + proc.ExitCode);
            }
        }
    }
}
