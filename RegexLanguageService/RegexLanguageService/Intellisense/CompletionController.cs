using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio;
using System.Windows;
using System.Runtime.InteropServices;
using RegexLanguageService;

namespace RegexLanguageService.Intellisense
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(RegexStrings.RegexContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class VsTextViewCreationListener : IVsTextViewCreationListener
    {
        #region Fields

        [Import]
        private IVsEditorAdaptersFactoryService adaptersFactory;

        [Import]
        private ICompletionBroker completionBroker;

        #endregion //Fields

        #region Methods

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var view = this.adaptersFactory.GetWpfTextView(textViewAdapter);
            Debug.Assert(view != null);

            var filter = new CommandFilter(view, completionBroker);
            var next = null as IOleCommandTarget;
            textViewAdapter.AddCommandFilter(filter, out next);
            filter.Next = next;
        }

        #endregion //Methods
    }

    internal sealed class CommandFilter : IOleCommandTarget
    {
        #region Fields

        ICompletionSession currentSession;

        #endregion //Fields

        #region Properties

        public IWpfTextView TextView { get; private set; }
        public ICompletionBroker Broker { get; private set; }
        public IOleCommandTarget Next { get; set; }

        #endregion //Properties

        #region Constructors

        public CommandFilter(IWpfTextView textView, ICompletionBroker broker)
        {
            this.TextView = textView;
            this.Broker = broker;
        }

        #endregion //Constructors

        #region Methods

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            bool handled = false;
            int hresult = VSConstants.S_OK;

            // 1. Pre-process
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)nCmdID)
                {
                    case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
                    case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                        handled = StartSession();
                        break;
                    case VSConstants.VSStd2KCmdID.RETURN:
                        handled = Complete(false);
                        break;
                    case VSConstants.VSStd2KCmdID.TAB:
                        handled = Complete(true);
                        break;
                    case VSConstants.VSStd2KCmdID.CANCEL:
                        handled = Cancel();
                        break;
                }
            }

            if (!handled)
                hresult = Next.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            if (ErrorHandler.Succeeded(hresult))
            {
                if (pguidCmdGroup == VSConstants.VSStd2K)
                {
                    switch ((VSConstants.VSStd2KCmdID)nCmdID)
                    {
                        case VSConstants.VSStd2KCmdID.TYPECHAR:
                            char ch = GetTypeChar(pvaIn);
                            if (ch == ' ')
                                StartSession();
                            else if (currentSession != null)
                                Filter();
                            break;
                        case VSConstants.VSStd2KCmdID.BACKSPACE:
                            Filter();
                            break;
                    }
                }
            }

            return hresult;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID)prgCmds[0].cmdID)
                {
                    case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
                    case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
                        return VSConstants.S_OK;
                }
            }
            return Next.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        #endregion //Methods

        #region Utilities

        /// <summary>
        /// Narrow down the list of options as the user types input
        /// </summary>
        private void Filter()
        {
            if (this.currentSession == null)
                return;

            this.currentSession.SelectedCompletionSet.SelectBestMatch();
            this.currentSession.SelectedCompletionSet.Recalculate();
        }

        /// <summary>
        /// Cancel the auto-complete session, and leave the text unmodified
        /// </summary>
        private bool Cancel()
        {
            if (this.currentSession == null)
                return false;

            this.currentSession.Dismiss();
            return true;
        }

        /// <summary>
        /// Auto-complete text using the specified token
        /// </summary>
        private bool Complete(bool force)
        {
            if (this.currentSession == null)
                return false;

            if (!this.currentSession.SelectedCompletionSet.SelectionStatus.IsSelected && !force)
            {
                this.currentSession.Dismiss();
                return false;
            }
            else
            {
                this.currentSession.Commit();
                return true;
            }
        }

        /// <summary>
        /// Display list of potential tokens
        /// </summary>
        private bool StartSession()
        {
            if (this.currentSession != null)
                return false;

            var caret = TextView.Caret.Position.BufferPosition;
            var snapshot = caret.Snapshot;

            if (!Broker.IsCompletionActive(TextView))
            {
                this.currentSession = Broker.CreateCompletionSession(TextView, snapshot.CreateTrackingPoint(caret, PointTrackingMode.Positive), true);
            }
            else
            {
                this.currentSession = Broker.GetSessions(TextView)[0];
            }
            this.currentSession.Dismissed += (sender, args) => currentSession = null;

            if (!this.currentSession.IsStarted)
                this.currentSession.Start();

            return true;
        }

        private char GetTypeChar(IntPtr pvaIn)
        {
            return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
        }

        #endregion //Utilities
    }
}