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
namespace net.named_data.jndn.encoding {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// A Tlv0_1WireFormat extends Tlv0_1_1WireFormat so that it is an alias in case
	/// any applications use Tlv0_1WireFormat directly.  These two wire formats are
	/// the same except that Tlv0_1_1WireFormat adds support for
	/// Sha256WithEcdsaSignature.
	/// </summary>
	///
	public class Tlv0_1WireFormat : Tlv0_1_1WireFormat {
		/// <summary>
		/// Get a singleton instance of a Tlv0_1WireFormat.
		/// </summary>
		///
		/// <returns>The singleton instance.</returns>
		public static Tlv0_1WireFormat get() {
			return instance_;
		}
	
		private static Tlv0_1WireFormat instance_ = new Tlv0_1WireFormat();
	}
}
