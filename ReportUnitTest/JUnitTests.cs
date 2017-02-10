﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
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
            var assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            TestContext.Progress.Write("AssemblyDir: " + assemblyDir);
            if (assemblyDir == null || !Directory.Exists(assemblyDir))
            {
                throw new Exception("Failed to get assembly path");
            }
            
            ResourcesDir = Path.Combine(assemblyDir, "..", "..", "Resources");
            TestContext.Progress.Write("ResourcesDir: " + ResourcesDir);
            if (!Directory.Exists(ResourcesDir))
            {
                throw new Exception("Can't find Resources folder");
            }

            ExecutableDir = Path.Combine(assemblyDir, "..", "..", "..", "ReportUnit", "bin");
            TestContext.Progress.Write("ExecutableDir: " + ExecutableDir);
            if (!Directory.Exists(ExecutableDir))
            {
                throw new Exception("Can't find ReportUnit folder");
            }

            if (!File.Exists(Path.Combine(ExecutableDir, "ReportUnit.exe")))
            {
                throw new Exception("Can't find ReportUnit.exe");
            }
        }

        [Test]
        public void Test()
        {
            var filename = Path.Combine(ExecutableDir, "ReportUnit.exe");
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = filename,
                Arguments = Path.Combine(ResourcesDir, "JUnit", "test_junit_01.xml"),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = false,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = ExecutableDir
            };

            if (IsRunningOnMono())
            {
                processInfo.FileName = "mono";
                processInfo.Arguments = filename + " " + processInfo.Arguments;
            }

            TestContext.Progress.Write("Start Process...");
            TestContext.Progress.Write("Filename: " + processInfo.FileName);
            TestContext.Progress.Write("Arguments: " + processInfo.Arguments);

            var proc = Process.Start(processInfo);
            if (proc == null)
            {
                throw new Exception("Failed to start");
            }
            if (!proc.WaitForExit(5000))
            {
                throw new Exception("Timeout");
            }

            while (!proc.StandardOutput.EndOfStream)
            {
                TestContext.Progress.Write(proc.StandardOutput.ReadLine());
            }
            if (proc.ExitCode != 0)
            {
                throw new Exception("Exit code " + proc.ExitCode);
            }

            var htmlFile = Path.Combine(ResourcesDir, "JUnit", "test_junit_01.html");
            if (!File.Exists(htmlFile))
            {
                throw new Exception("No HTML report");
            }
            //W3CValidate(htmlFile);
        }

        private bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }


        public static void W3CValidate(string htmlFile)
        {
            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create("http://validator.nu/&out=gnu");
            
            // Set the Method property of the request to POST.  
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(File.ReadAllText(htmlFile));

            // Set the ContentType property of the WebRequest.  
            request.ContentType = "text/html; charset=utf-8";

            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            // Display the content.  
            Console.WriteLine(responseFromServer);
            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();
        }

    }
}
