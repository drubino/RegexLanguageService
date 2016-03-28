using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RegexLanguageService
{
    [ProvideLanguageService(typeof(RegexLanguageService),
                             "Regex Language",
                             106,             // resource ID of localized language name
                             CodeSense = true,             // Supports IntelliSense
                             RequestStockColors = false,   // Supplies custom colors
                             EnableCommenting = true,      // Supports commenting out code
                             EnableAsyncCompletion = true  // Supports background parsing
                             )]
    [ProvideLanguageExtension(typeof(RegexLanguageService), ".rx")]
    [Guid("56b26fab-e2fe-49a7-95b0-c79ee10edef9")]
    public class RegexLanguagePackage : Package, IOleComponent
    {
        private uint componentId;

        protected override void Initialize()
        {
            base.Initialize();

            //Proffer the service.
            var serviceContainer = this as IServiceContainer;
            var languageService = new RegexLanguageService();
            languageService.SetSite(this);
            serviceContainer.AddService(typeof(RegexLanguageService), languageService, promote: true);

            // Register a timer to call our language service during idle periods.
            var oleComponentManager = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
            if (this.componentId == 0 && oleComponentManager != null)
            {
                var oldCrInfo = new OLECRINFO[1];
                oldCrInfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                oldCrInfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
                                        (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                oldCrInfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
                                        (uint)_OLECADVF.olecadvfRedrawOff |
                                        (uint)_OLECADVF.olecadvfWarningsOff;
                oldCrInfo[0].uIdleTimeInterval = 1000;
                oleComponentManager.FRegisterComponent(this, oldCrInfo, out this.componentId);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.componentId != 0)
            {
                var oleComponentManager = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
                if(oleComponentManager != null)
                    oleComponentManager.FRevokeComponent(this.componentId);
                this.componentId = 0;
            }

            base.Dispose(disposing);
        }

        #region IOleComponent Members

        public int FDoIdle(uint grfidlef)
        {
            var periodic = (grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0;
            var service = GetService(typeof(RegexLanguageService)) as LanguageService;
            if(service != null)
                service.OnIdle(periodic);

            return 0;
        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void Terminate()
        {
        }

        #endregion //IOleComponent Members
    }
}
