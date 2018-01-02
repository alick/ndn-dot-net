// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security.certificate {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encoding.der;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A CertificateExtension represents the Extension entry in a certificate.
	/// </summary>
	///
	public class CertificateExtension {
		/// <summary>
		/// Create a new CertificateExtension.
		/// </summary>
		///
		/// <param name="oid">The oid of subject description entry.</param>
		/// <param name="isCritical">If true, the extension must be handled.</param>
		/// <param name="value">The extension value.</param>
		public CertificateExtension(String oid, bool isCritical, Blob value_ren) {
			extensionId_ = new OID(oid);
			isCritical_ = isCritical;
			extensionValue_ = value_ren;
		}
	
		/// <summary>
		/// Create a new CertificateExtension.
		/// </summary>
		///
		/// <param name="oid">The oid of subject description entry.</param>
		/// <param name="isCritical">If true, the extension must be handled.</param>
		/// <param name="value">The extension value.</param>
		public CertificateExtension(OID oid, bool isCritical, Blob value_ren) {
			extensionId_ = oid;
			isCritical_ = isCritical;
			extensionValue_ = value_ren;
		}
	
		/// <summary>
		/// Encode the object into DER syntax tree.
		/// </summary>
		///
		/// <returns>The encoded DER syntax tree.</returns>
		public DerNode toDer() {
			net.named_data.jndn.encoding.der.DerNode.DerSequence  root = new net.named_data.jndn.encoding.der.DerNode.DerSequence ();
	
			net.named_data.jndn.encoding.der.DerNode.DerOid  extensionId = new net.named_data.jndn.encoding.der.DerNode.DerOid (extensionId_);
			net.named_data.jndn.encoding.der.DerNode.DerBoolean  isCritical = new net.named_data.jndn.encoding.der.DerNode.DerBoolean (isCritical_);
			net.named_data.jndn.encoding.der.DerNode.DerOctetString  extensionValue = new net.named_data.jndn.encoding.der.DerNode.DerOctetString (
					extensionValue_.buf());
	
			root.addChild(extensionId);
			root.addChild(isCritical);
			root.addChild(extensionValue);
	
			root.getSize();
	
			return root;
		}
	
		public Blob toDerBlob() {
			return toDer().encode();
		}
	
		public OID getOid() {
			return extensionId_;
		}
	
		public bool getIsCritical() {
			return isCritical_;
		}
	
		public Blob getValue() {
			return extensionValue_;
		}
	
		protected internal readonly OID extensionId_;
		protected internal readonly bool isCritical_;
		protected internal readonly Blob extensionValue_;
	}
}
