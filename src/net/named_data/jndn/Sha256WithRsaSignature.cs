// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2013-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A Sha256WithRsaSignature extends Signature and holds the signature bits and
	/// other info representing a
	/// SHA256-with-RSA signature in a data packet.
	/// </summary>
	///
	public class Sha256WithRsaSignature : Signature {
		/// <summary>
		/// Create a new Sha256WithRsaSignature with default values.
		/// </summary>
		///
		public Sha256WithRsaSignature() {
			this.signature_ = new Blob();
			this.keyLocator_ = new ChangeCounter(
					new KeyLocator());
			this.validityPeriod_ = new ChangeCounter(
					new ValidityPeriod());
			this.changeCount_ = 0;
		}
	
		/// <summary>
		/// Create a new Sha256WithRsaSignature with a copy of the fields in the given
		/// signature object.
		/// </summary>
		///
		/// <param name="signature">The signature object to copy.</param>
		public Sha256WithRsaSignature(Sha256WithRsaSignature signature) {
			this.signature_ = new Blob();
			this.keyLocator_ = new ChangeCounter(
					new KeyLocator());
			this.validityPeriod_ = new ChangeCounter(
					new ValidityPeriod());
			this.changeCount_ = 0;
			signature_ = signature.signature_;
			keyLocator_.set(new KeyLocator(signature.getKeyLocator()));
		}
	
		/// <summary>
		/// Return a new Signature which is a deep copy of this signature.
		/// </summary>
		///
		/// <returns>A new Sha256WithRsaSignature.</returns>
		/// <exception cref="System.Exception"></exception>
		public override Object Clone() {
			return new Sha256WithRsaSignature(this);
		}
	
		/// <summary>
		/// Get the signature bytes.
		/// </summary>
		///
		/// <returns>The signature bytes. If not specified, the value isNull().</returns>
		public sealed override Blob getSignature() {
			return signature_;
		}
	
		public KeyLocator getKeyLocator() {
			return (KeyLocator) keyLocator_.get();
		}
	
		/// <summary>
		/// Get the validity period.
		/// </summary>
		///
		/// <returns>The validity period.</returns>
		public ValidityPeriod getValidityPeriod() {
			return (ValidityPeriod) validityPeriod_.get();
		}
	
		/// <summary>
		/// Set the signature bytes to the given value.
		/// </summary>
		///
		/// <param name="signature">A Blob with the signature bytes.</param>
		public sealed override void setSignature(Blob signature) {
			signature_ = ((signature == null) ? new Blob() : signature);
			++changeCount_;
		}
	
		public void setKeyLocator(KeyLocator keyLocator) {
			keyLocator_.set(((keyLocator == null) ? new KeyLocator()
					: new KeyLocator(keyLocator)));
			++changeCount_;
		}
	
		/// <summary>
		/// Get the change count, which is incremented each time this object
		/// (or a child object) is changed.
		/// </summary>
		///
		/// <returns>The change count.</returns>
		public sealed override long getChangeCount() {
			// Make sure each of the checkChanged is called.
			bool changed = keyLocator_.checkChanged();
			changed = validityPeriod_.checkChanged() || changed;
			if (changed)
				// A child object has changed, so update the change count.
				++changeCount_;
	
			return changeCount_;
		}
	
		private Blob signature_;
		private readonly ChangeCounter keyLocator_;
		private readonly ChangeCounter validityPeriod_;
		private long changeCount_;
	}
}
