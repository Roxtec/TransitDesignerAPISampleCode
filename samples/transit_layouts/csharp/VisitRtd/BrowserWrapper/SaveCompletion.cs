using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;

namespace BrowserWrapper
{
    /// <summary>
    /// This class is used to communicate between BrowserWrapper and Transit Designer.
    /// It registers a save completion object (and instance of this class) with the browser.
    /// When the user wants to exit, the TrySave method executes a script that saves the
    /// current transit. Once that is done, one of the OnXX methods of the registered instance of
    /// this class is called, which in turn completes the Task returned from TrySave. This
    /// allows the caller to act on the save result and close the application.
    /// </summary>
    public class SaveCompletion
    {
        private readonly ChromiumWebBrowser browser;
        private readonly TaskCompletionSource<bool> taskCompletionSource;

        public SaveCompletion(ChromiumWebBrowser browser)
        {
            this.browser = browser;
            taskCompletionSource = new TaskCompletionSource<bool>();
            browser.JavascriptObjectRepository.Register("saveCompletion", this);
        }

        public Task<bool> TrySave()
        {
            browser.ExecuteScriptAsync(@"
(async () => {
    await CefSharp.BindObjectAsync('saveCompletion');

    if (window.transitDesignerApi) {
        try {
            const didSave = await window.transitDesignerApi.saveChanges();
            await saveCompletion.onSaveSuccess(didSave);
        } catch (e) {
            await saveCompletion.onSaveFailure(e.stack);
        }
    } else {
        // Most likely the user is not on a transit page.
        await saveCompletion.onSaveSuccess(false);
    }
})();
");
            return taskCompletionSource.Task;
        }

        // This method is called from JavaScript when the save is successful (or if it is not needed).
        // ReSharper disable once UnusedMember.Global
        public void OnSaveSuccess(bool didSave)
            => taskCompletionSource.TrySetResult(didSave);

        // This method is called from JavaScript if the save fails.
        // ReSharper disable once UnusedMember.Global
        public void OnSaveFailure(string stack)
            => taskCompletionSource.TrySetException(new Exception(stack));
    }
}