namespace SetupBootstrapperUI
{
    using System.Collections.Generic;
    using WixToolset.BootstrapperApplicationApi;

    public interface IBootstrapperUiRuntime
    {
        void AttachForm(MainForm form);

        void Plan(LaunchAction action);

        void SetSelectedFeatures(IEnumerable<string> features);

        void SetVariableString(string name, string value, bool formatted = false);

        string GetVariableString(string name);

        void ClearBundleCertVariables();
    }
}
