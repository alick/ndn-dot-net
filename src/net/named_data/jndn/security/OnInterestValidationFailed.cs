// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2016-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	
	/// <summary>
	/// A class implements OnInterestValidationFailed if it has
	/// onInterestValidationFailed which is called by verifyInterest to report a
	/// failed verification.
	/// </summary>
	///
	public interface OnInterestValidationFailed {
		/// <summary>
		/// When verifyInterest fails, onInterestValidationFailed is called.
		/// </summary>
		///
		/// <param name="interest">The interest object being verified.</param>
		/// <param name="reason">The reason for the failed validation.</param>
		void onInterestValidationFailed(Interest interest, String reason);
	}
}
