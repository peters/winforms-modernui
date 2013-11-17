using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace MetroFramework
{
    /// <summary>
    /// Message box overlay display properties.
    /// </summary>
    public class MetroMessageBoxProperties
    {

        /// <summary>
        /// Creates a new instance of MessageBoxOverlayProperties.
        /// </summary>
        /// <param name="owner"></param>
        public MetroMessageBoxProperties(MetroMessageBoxControl owner)
        { _owner = owner; }

        /// <summary>
        /// Gets or sets the message box buttons in the message box overlay.
        /// </summary>
        public MessageBoxButtons Buttons
        { get; set; }

        /// <summary>
        /// Gets or sets the message box default button.
        /// </summary>
        public MessageBoxDefaultButton DefaultButton
        { get; set; }

        /// <summary>
        /// Gets or sets the message box overlay icon.
        /// </summary>
        public MessageBoxIcon Icon
        { get; set;  }

        /// <summary>
        /// Gets or sets the message box overlay message contents.
        /// </summary>
        public string Message
        { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MetroMessageBoxControl _owner = null;

        /// <summary>
        /// Gets the property owner.
        /// </summary>
        public MetroMessageBoxControl Owner
        { get { return _owner; } }

        /// <summary>
        /// Gets or sets the message box overlat title.
        /// </summary>
        public string Title
        { get; set; }

    }
}
