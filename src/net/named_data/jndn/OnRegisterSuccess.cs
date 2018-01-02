// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A class implements OnRegisterSuccess if it has onRegisterSuccess, called by
	/// Face.registerPrefix when registration succeeds.
	/// </summary>
	///
	public interface OnRegisterSuccess {
		/// <summary>
		/// Face.registerPrefix calls onRegisterSuccess when it receives a success
		/// message from the forwarder.
		/// </summary>
		///
		/// <param name="prefix"></param>
		/// <param name="registeredPrefixId"></param>
		void onRegisterSuccess(Name prefix, long registeredPrefixId);
	}
}
