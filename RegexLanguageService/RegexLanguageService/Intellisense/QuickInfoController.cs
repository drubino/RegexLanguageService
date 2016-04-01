using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace RegexLanguageService.Intellisense
{
    internal class QuickInfoController : IIntellisenseController
    {
        #region Fields

        private ITextView textView;
        private IList<ITextBuffer> subjectBuffers;
        private QuickInfoControllerProvider componentContext;

        private IQuickInfoSession _session;

        #endregion //Fields

        #region Constructors

        internal QuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, QuickInfoControllerProvider componentContext)
        {
            this.textView = textView;
            this.subjectBuffers = subjectBuffers;
            this.componentContext = componentContext;

            this.textView.MouseHover += OnTextViewMouseHover;
        }

        #endregion

        #region Methods

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
        {
        }

        public void Detach(ITextView textView)
        {
            if (this.textView == textView)
            {
                this.textView.MouseHover -= OnTextViewMouseHover;
                this.textView = null;
            }
        }

        #endregion //Methods

        #region Utilities

        /// <summary>
        /// Determine if the mouse is hovering over a token. If so, highlight the token and display QuickInfo
        /// </summary>
        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
        {
            SnapshotPoint? point = GetMousePosition(new SnapshotPoint(textView.TextSnapshot, e.Position));

            if (point != null)
            {
                ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position,
                    PointTrackingMode.Positive);

                // Find the broker for this buffer

                if (!componentContext.QuickInfoBroker.IsQuickInfoActive(textView))
                {
                    _session = componentContext.QuickInfoBroker.CreateQuickInfoSession(textView, triggerPoint, true);
                    _session.Start();
                }
            }
        }
        
        /// <summary>
        /// get mouse location onscreen. Used to determine what word the cursor is currently hovering over
        /// </summary>
        private SnapshotPoint? GetMousePosition(SnapshotPoint topPosition)
        {
            // Map this point down to the appropriate subject buffer.

            return textView.BufferGraph.MapDownToFirstMatch
                (
                topPosition,
                PointTrackingMode.Positive,
                snapshot => subjectBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor
                );
        }

        #endregion //Utilities
    }
}