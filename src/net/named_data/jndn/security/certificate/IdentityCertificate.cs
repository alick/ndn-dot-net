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
namespace net.named_data.jndn.security.certificate {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Security;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encoding.der;
	using net.named_data.jndn.util;
	
	public class IdentityCertificate : Certificate {
		public IdentityCertificate() {
			this.publicKeyName_ = new Name();
		}
	
		/// <summary>
		/// Create an IdentityCertificate from the content in the data packet.
		/// </summary>
		///
		/// <param name="data">The data packet with the content to decode.</param>
		public IdentityCertificate(Data data) : base(data) {
			this.publicKeyName_ = new Name();
	
			if (!isCorrectName(data.getName()))
				throw new SecurityException("Wrong Identity Certificate Name!");
	
			setPublicKeyName();
		}
	
		/// <summary>
		/// The copy constructor.
		/// </summary>
		///
		public IdentityCertificate(IdentityCertificate identityCertificate) : base(identityCertificate) {
			this.publicKeyName_ = new Name();
			publicKeyName_ = identityCertificate.publicKeyName_;
		}
	
		/// <summary>
		/// Override the base class method to check that the name is a valid identity certificate name.
		/// </summary>
		///
		/// <param name="name">The identity certificate name which is copied.</param>
		/// <returns>This Data so that you can chain calls to update values.</returns>
		public override Data setName(Name name) {
			if (!isCorrectName(name))
				throw new SecurityException("Wrong Identity Certificate Name!");
	
			base.setName(name);
			setPublicKeyName();
			return this;
		}
	
		/// <summary>
		/// Override to call the base class wireDecode then update the public key name.
		/// </summary>
		///
		/// <param name="input">The input byte array to be decoded as an immutable Blob.</param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		public override void wireDecode(Blob input, WireFormat wireFormat) {
			base.wireDecode(input,wireFormat);
			setPublicKeyName();
		}
	
		public Name getPublicKeyName() {
			return publicKeyName_;
		}
	
		public static bool isIdentityCertificate(Certificate certificate) {
			return isCorrectName(certificate.getName());
		}
	
		/// <summary>
		/// Get the public key name from the full certificate name.
		/// </summary>
		///
		/// <param name="certificateName">The full certificate name.</param>
		/// <returns>The related public key name.</returns>
		public static Name certificateNameToPublicKeyName(Name certificateName) {
			String idString = "ID-CERT";
			bool foundIdString = false;
			int idCertComponentIndex = certificateName.size() - 1;
			for (; idCertComponentIndex + 1 > 0; --idCertComponentIndex) {
				if (certificateName.get(idCertComponentIndex).toEscapedString()
						.equals(idString)) {
					foundIdString = true;
					break;
				}
			}
	
			if (!foundIdString)
				throw new Exception("Incorrect identity certificate name "
						+ certificateName.toUri());
	
			Name tempName = certificateName.getSubName(0, idCertComponentIndex);
			String keyString = "KEY";
			bool foundKeyString = false;
			int keyComponentIndex = 0;
			for (; keyComponentIndex < tempName.size(); keyComponentIndex++) {
				if (tempName.get(keyComponentIndex).toEscapedString()
						.equals(keyString)) {
					foundKeyString = true;
					break;
				}
			}
	
			if (!foundKeyString)
				throw new Exception("Incorrect identity certificate name "
						+ certificateName.toUri());
	
			return tempName.getSubName(0, keyComponentIndex).append(
					tempName.getSubName(keyComponentIndex + 1, tempName.size()
							- keyComponentIndex - 1));
		}
	
		private static bool isCorrectName(Name name) {
			int i = name.size() - 1;
	
			String idString = "ID-CERT";
			for (; i >= 0; i--) {
				if (name.get(i).toEscapedString().equals(idString))
					break;
			}
	
			if (i < 0)
				return false;
	
			int keyIdx = 0;
			String keyString = "KEY";
			for (; keyIdx < name.size(); keyIdx++) {
				if (name.get(keyIdx).toEscapedString().equals(keyString))
					break;
			}
	
			if (keyIdx >= name.size())
				return false;
	
			return true;
		}
	
		private void setPublicKeyName() {
			publicKeyName_ = certificateNameToPublicKeyName(getName());
		}
	
		private Name publicKeyName_;
	}
}
