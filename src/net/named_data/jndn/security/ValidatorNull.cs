// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.security.v2;
	
	/// <summary>
	/// A ValidatorNull extends Validator with an "accept-all" policy and an offline
	/// certificate fetcher.
	/// </summary>
	///
	public class ValidatorNull : Validator {
		public ValidatorNull() : base(new ValidationPolicyAcceptAll(), new CertificateFetcherOffline()) {
		}
	}
}
