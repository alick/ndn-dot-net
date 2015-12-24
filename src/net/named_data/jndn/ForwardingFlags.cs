// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
// 12/23/15 3:55 PM    
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2015 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A ForwardingFlags object holds the flags which specify how the forwarding
	/// daemon should forward an interest for a registered prefix.  We use a separate
	/// ForwardingFlags object to retain future compatibility if the daemon
	/// forwarding bits are changed, amended or deprecated.
	/// </summary>
	///
	public class ForwardingFlags {
		/// <summary>
		/// Create a new ForwardingFlags with "childInherit" set and all other flags
		/// cleared.
		/// </summary>
		///
		public ForwardingFlags() {
			this.childInherit_ = true;
			this.capture_ = false;
		}
	
		/// <summary>
		/// Create a new ForwardingFlags as a copy of the given value.
		/// </summary>
		///
		/// <param name="forwardingFlags">The ForwardingFlags to copy.</param>
		public ForwardingFlags(ForwardingFlags forwardingFlags) {
			this.childInherit_ = true;
			this.capture_ = false;
			childInherit_ = forwardingFlags.childInherit_;
			capture_ = forwardingFlags.capture_;
		}
	
		/// <summary>
		/// Get the value of the "childInherit" flag.
		/// </summary>
		///
		/// <returns>true if the flag is set, false if it is cleared.</returns>
		public bool getChildInherit() {
			return childInherit_;
		}
	
		/// <summary>
		/// Get the value of the "capture" flag.
		/// </summary>
		///
		/// <returns>true if the flag is set, false if it is cleared.</returns>
		public bool getCapture() {
			return capture_;
		}
	
		/// <summary>
		/// Set the value of the "childInherit" flag
		/// </summary>
		///
		/// <param name="childInherit">true to set the flag, false to clear it.</param>
		public void setChildInherit(bool childInherit) {
			childInherit_ = childInherit;
		}
	
		/// <summary>
		/// Set the value of the "capture" flag
		/// </summary>
		///
		/// <param name="capture">true to set the flag, false to clear it.</param>
		public void setCapture(bool capture) {
			capture_ = capture;
		}
	
		/// <summary>
		/// Get an integer with the bits set according to the NFD forwarding flags as
		/// used in the ControlParameters of the command interest.
		/// </summary>
		///
		/// <returns>An integer with the bits set.</returns>
		public int getNfdForwardingFlags() {
			int result = 0;
	
			if (childInherit_)
				result |= NfdForwardingFlags_CHILD_INHERIT;
			if (capture_)
				result |= NfdForwardingFlags_CAPTURE;
	
			return result;
		}
	
		/// <summary>
		/// Set the flags according to the NFD forwarding flags as used in the
		/// ControlParameters of the command interest.
		/// </summary>
		///
		/// <param name="nfdForwardingFlags">An integer with the bits set.</param>
		public void setNfdForwardingFlags(int nfdForwardingFlags) {
			childInherit_ = (nfdForwardingFlags & NfdForwardingFlags_CHILD_INHERIT) != 0;
			capture_ = (nfdForwardingFlags & NfdForwardingFlags_CAPTURE) != 0;
		}
	
		private const int NfdForwardingFlags_CHILD_INHERIT = 1;
		private const int NfdForwardingFlags_CAPTURE = 2;
	
		private bool childInherit_;
		private bool capture_;
	}
}
