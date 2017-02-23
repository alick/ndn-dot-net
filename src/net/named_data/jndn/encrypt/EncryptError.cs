// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2016-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// EncryptError holds the ErrorCode enum and OnError callback definition for
	/// errors from the encrypt library.
	/// </summary>
	///
	public class EncryptError {
		public enum ErrorCode {
			Timeout, Validation, UnsupportedEncryptionScheme, InvalidEncryptedFormat, NoDecryptKey, EncryptionFailure, DataRetrievalFailure, SecurityException, IOException	}
	
		/// <summary>
		/// A method calls onError.onError(errorCode, message) for an error.
		/// </summary>
		///
		public interface OnError {
			void onError(EncryptError.ErrorCode  errorCode, String message);
		}
	}
}
