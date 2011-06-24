using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
/// PowerShell
using System.Management.Automation;
using System.Management.Automation.Runspaces;
/// Workflow
using System.Workflow.ComponentModel;
/// FIM
using Microsoft.ResourceManagement.Workflow.Activities;

namespace FimExtensions.FimActivityLibrary
{
    public partial class PowerShellActivity : Activity
    {
        /// TraceSource for the Activity
        private static TraceSourceEx trace = TraceSourceEx.Instance;

        #region Dependency Properties
        /// <summary>
        /// Dependency Properties for this activity
        /// </summary>
        public String Script
        {
            get { return (String)GetValue(ScriptProperty); }
            set { SetValue(ScriptProperty, value); }
        }
        public String PowerShellVariables
        {
            get { return (String)GetValue(PowerShellVariablesProperty); }
            set { SetValue(PowerShellVariablesProperty, value); }
        }
        public String PowerShellModule
        {
            get { return (String)GetValue(PowerShellModuleProperty); }
            set { SetValue(PowerShellModuleProperty, value); }
        }
        public String WorkflowDataNameForOutput
        {
            get { return (String)GetValue(WorkflowDataNameForOutputProperty); }
            set { SetValue(WorkflowDataNameForOutputProperty, value); }
        }

        public static readonly DependencyProperty ScriptProperty = DependencyProperty.Register("Script", typeof(String), typeof(PowerShellActivity));
        public static readonly DependencyProperty PowerShellVariablesProperty = DependencyProperty.Register("PowerShellVariables", typeof(String), typeof(PowerShellActivity));
        public static readonly DependencyProperty PowerShellModuleProperty = DependencyProperty.Register("PowerShellModule", typeof(String), typeof(PowerShellActivity));
        public static readonly DependencyProperty WorkflowDataNameForOutputProperty = DependencyProperty.Register("WorkflowDataNameForOutput", typeof(String), typeof(PowerShellActivity));
        
        #endregion

        /// <summary>
        /// PowerShellActivity Constructor
        /// </summary>
        public PowerShellActivity()
        {
            trace.TraceStart("PowerShellActivity Constructor");            
            InitializeComponent();
            trace.TraceStop("PowerShellActivity Constructor");
        }
        
        /// <summary>
        /// Run the script 
        /// </summary>
        /// <param name="ActivityExecutionStatus"></param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            trace.TraceStart("START PowerShellActivity Execute");
            try
            {
          
                    this.RunScript();
            }
            catch (Exception ex)
            {              
                throw new InvalidOperationException(ex.Message);
            }

            trace.TraceStop("STOP PowerShellActivity Execute");
            return ActivityExecutionStatus.Closed;
        }

        /// <summary>
        /// Copy the FIM WorkflowData items to a Dictionary
        /// </summary>
        /// <param name="variablesInputString">the String containing the WorkflowData item name(s)</param>
        /// <returns>a Dictionary containing WorkflowData.Name WorkflowData.Value</returns>
        private Dictionary<String, Object> PowerShellSessionVariables(String variablesInputString)
        {
            Dictionary<String, Object> powerShellSessionVariables = new Dictionary<String, Object>();               

            // In order to read the Workflow Dictionary we need to get the containing (parent) workflow
            SequentialWorkflow containingWorkflow = null;
            if (!SequentialWorkflow.TryGetContainingWorkflow(this, out containingWorkflow))
            {
                throw new InvalidOperationException("Unable to get Containing Workflow");
            }

            String logOutput = "Containing Workflow Dictionary (WorkflowData):";
            foreach(String workflowDataName in variablesInputString.Split(new Char[] { ' ', ',', '.', ':', ';' }))
            {
                try
                {
                    String workflowDataValue = containingWorkflow.WorkflowDictionary[workflowDataName].ToString();
                    powerShellSessionVariables.Add(workflowDataName, workflowDataValue);
                    logOutput += String.Format("\n\t{0}: {1}", workflowDataName, workflowDataValue);
                }
                catch (KeyNotFoundException keyNotFoundException)
                {
                    trace.TraceFatal("Required item missing from FIM Workflow Data.\n", keyNotFoundException.StackTrace);
                    throw new KeyNotFoundException("Required item missing from FIM Workflow Data.", keyNotFoundException.InnerException);
                }

            }    
            trace.TraceVerbose(logOutput);

            return powerShellSessionVariables;
        }

        /// <summary>
        /// Runs the PowerShell Script using:
        ///  - FIM WorkflowData items as PowerShell variables
        ///  - the specified PowerShell module (if specified in the WF)
        ///  - stores the script output in a new FIM WorkflowData item (if specified in the WF)
        /// </summary>
        private void RunScript()
        {
            trace.TraceStart("START PowerShellActivity RunScript");

            ///
            /// Translate the WorkflowData items to PowerShell variables
            /// 
            InitialSessionState initialSessionState = InitialSessionState.CreateDefault();
            String logDetailForPowerShellVariables = "### PowerShell Variables from FIM WorkflowData";
            foreach (KeyValuePair<String, Object> sessionVariable in this.PowerShellSessionVariables(PowerShellVariables))
            {
                trace.TraceVerbose("Adding PowerShell Session Variable:\n{0} : {1}", sessionVariable.Key, sessionVariable.Value);
                initialSessionState.Variables.Add(new SessionStateVariableEntry(sessionVariable.Key, sessionVariable.Value, null));
                logDetailForPowerShellVariables += String.Format("\n${0} = '{1}'", sessionVariable.Key, sessionVariable.Value);
            }

            ///
            /// Load the PowerShell Module if specified by the WF
            /// 
            if (!String.IsNullOrEmpty(this.PowerShellModule))
            {
                trace.TraceInformation("Loading PowerShell Module: {0}", this.PowerShellModule);
                initialSessionState.ImportPSModule(new String[] { PowerShellModule });
            }

            // Call the RunspaceFactory.CreateRunspace(InitialSessionState) 
            // method to create the runspace where the pipeline is run.
            Runspace runspace = RunspaceFactory.CreateRunspace(initialSessionState);
            runspace.Open();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = runspace;
                powershell.AddScript(Script);
                powershell.AddParameter("Verbose");      

                // Invoke the PowerShell Pipeline synchronously
                trace.TraceInformation("### Invoking Pipeline\n{0}\n\n###PowerShell Script from WF\n{1}", logDetailForPowerShellVariables, Script);
                try
                {
                    Collection<PSObject> results = powershell.Invoke();

                    // Display the results.
                    trace.TraceVerbose("{0} results returned.", results.Count);
                    foreach (PSObject result in results)
                    {
                        String psObjectDetail = result.ToString();
                        //foreach (PSMemberInfo member in result.Members)
                        //{
                        //    psObjectDetail += String.Format("\n\t{0}:{1}", member.Name, member.Value);
                        //}
                        foreach (PSMemberInfo member in result.Properties)
                        {
                            psObjectDetail += String.Format("\n\t{0}:{1}", member.Name, member.Value);
                        }
                        trace.TraceVerbose(psObjectDetail);
                    }
                    // Throw on any non-terminating errors.
                    foreach (ErrorRecord error in powershell.Streams.Error)
                    {
                        trace.TraceWarning("PowerShell Error: {0}", error);
                        String errorText = String.Format("PowerShell activity failed with the following error: \n{0}", error.ToString());
                        throw new Microsoft.ResourceManagement.Workflow.WorkflowExtensionException("PowerShell Activity", errorText);
                    }

                    ///
                    /// Copy the PowerShell output to the FIM WorkflowData dictionary
                    /// 
                    if (results.Count == 1 & !String.IsNullOrEmpty(this.WorkflowDataNameForOutput))
                    {
                        trace.TraceVerbose("Storing the PowerShell output in FIM WorkflowData item named: {0}", this.WorkflowDataNameForOutput);
                        // In order to add our PowerShell result to the FIM Workflow Dictionary we need to get the containing (parent) workflow
                        SequentialWorkflow containingWorkflow = null;
                        if (!SequentialWorkflow.TryGetContainingWorkflow(this, out containingWorkflow))
                        {
                            trace.TraceError("Unable to get Containing Workflow.");
                            throw new InvalidOperationException("Unable to get Containing Workflow");
                        }
                        containingWorkflow.WorkflowDictionary.Add(this.WorkflowDataNameForOutput, results[0].ToString());
                    }
                }
                catch (RuntimeException ex)
                {
                    trace.TraceError("PowerShell Error: {0}", ex.Message);
                    throw;
                }
                finally
                {
                    // Throw on any non-terminating errors.
                    String verboseOutput = "PowerShell Verbose Output:\n";
                    foreach (VerboseRecord verboseRecord in powershell.Streams.Verbose)
                    {
                        verboseOutput += verboseRecord.ToString();
                    }
                    trace.TraceVerbose(verboseOutput);
                }

            }

            trace.TraceStop("STOP PowerShellActivity RunScript");
        }        
    }
}
