using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Windows_Processes_UnitTest_Explore101
{
    [TestClass]
    public class Windows_Processes_UnitTest
    {
     
        [TestMethod]
        public void Test_Enumerating_Running_Processes()
        {
            string localMachineName = Environment.MachineName;

            var runningProcess = Process.GetProcesses(localMachineName);

            foreach (var process in runningProcess)
            {
                string processName = process.ProcessName;
                string processId = process.Id.ToString();

                Console.WriteLine($"Process Id: {processId} Process Name: {processName}");

                Assert.IsNotNull(processName);
            }
        }

        [TestMethod]
        public void Test_Check_And_Inspect_A_Specific_Process()
        { 
            string localMachineName = Environment.MachineName;

            var process = Process.GetProcesses(localMachineName)
                                 .FirstOrDefault(p => p.ProcessName.Contains("chrome"));

            if(process != null)
            {
                Assert.IsNotNull(process);
            }
        }

        [TestMethod]
        public void Test_Process_Thread_Set()
        {
            string localMachineName = Environment.MachineName;

            var process = Process.GetProcesses(localMachineName)
                                 .FirstOrDefault(p => p.ProcessName.Contains("chrome"));

            if (process != null)
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    string outputString = string.Empty;

                    outputString = $"Thread ID: {thread.Id} {Environment.NewLine}";
                    outputString += $"Thread ID: {thread.StartTime.ToShortDateString()}";
                    outputString += $"Priority: {thread.PriorityLevel}";

                    Console.WriteLine(outputString);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Win32Exception))]
        public void Test_Process_Module_Set_Of_Details_1_Using_ProcessObject()
        {
            string localMachineName = Environment.MachineName;

            var process = Process.GetProcesses(localMachineName)
                                 .FirstOrDefault(p => p.ProcessName.Contains("chrome"));

            if (process != null)
            {
                foreach (ProcessModule module in process.Modules)
                {
                    string outputString = string.Empty;

                    outputString = $"Module Name: {module.ModuleName} {Environment.NewLine}";
                    outputString += $"Module Filename: {module.FileName}";

                    Console.WriteLine(outputString);
                }
            }
        }

        [TestMethod]
        public void Test_Process_Module_Set_Of_Details_2_Using_ManagementObject()
        {
            string localMachineName = Environment.MachineName;

            var process = Process.GetProcesses(localMachineName)
                                 .FirstOrDefault(p => p.ProcessName.Contains("chrome"));

            if (process != null)
            {
                string query = $"SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process WHERE ProcessId={process.Id}" ;

                using(var search = new ManagementObjectSearcher(query))
                {
                    using(var results = search.Get())
                    {

                        var processModuleDetails = results.Cast<ManagementObject>().FirstOrDefault();

                        if (processModuleDetails != null)
                        {
                            var stringResult = $"Module Name: {processModuleDetails.GetPropertyValue("CommandLine")}{Environment.NewLine}";
                                stringResult += $"Module Filename:{processModuleDetails.GetPropertyValue("ExecutablePath")}{Environment.NewLine}";
                                stringResult += $"Process Id: {processModuleDetails.GetPropertyValue("ProcessId")}";


                            Console.WriteLine(stringResult);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void Test_Process_Start_Process()
        {
            string acrobatReaderName = "AcroRd32";

            try
            {
                /*
                 * For the complete argument list see the link below.
                 * https://www.adobe.com/content/dam/acom/en/devnet/acrobat/pdfs/pdf_open_parameters.pdf
                 */

                var acrobatReaderProcess = Process.Start(acrobatReaderName, "/n");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Test_Process_Stop_Process()
        {
            string acrobatReaderName = "AcroRd32";

            try
            {
                var acrobatReaderProcess = Process.GetProcessesByName(acrobatReaderName);

                if (acrobatReaderProcess != null)
                {
                    foreach (var item in acrobatReaderProcess)
                    {
                        item.Kill();

                        Console.WriteLine($"{item.ProcessName} has been killed!");

                    }
                }
            }
            catch (Win32Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
