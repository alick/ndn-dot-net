// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A SigningInfo holds the signing parameters passed to the KeyChain. A
	/// SigningInfo is invalid if the specified identity/key/certificate does not
	/// exist, or the PibIdentity or PibKey instance is not valid.
	/// </summary>
	///
	public class SigningInfo {
		public enum SignerType {
			/// <summary>
			/// No signer is specified. Use default settings or follow the trust schema. 
			/// </summary>
			///
			NULL,
			/// <summary>
			/// The signer is an identity. Use its default key and default certificate. 
			/// </summary>
			///
			ID,
			/// <summary>
			/// The signer is a key. Use its default certificate. 
			/// </summary>
			///
			KEY,
			/// <summary>
			/// The signer is a certificate. Use it directly. 
			/// </summary>
			///
			CERT,
			/// <summary>
			/// Use a SHA-256 digest. No signer needs to be specified. 
			/// </summary>
			///
			SHA256
		}
	
		/// <summary>
		/// Create a SigningInfo with the signerType and signerName, with other default
		/// values. The digest algorithm is set to DigestAlgorithm.SHA256.
		/// </summary>
		///
		/// <param name="signerType">The type of signer.</param>
		/// <param name="signerName"></param>
		public SigningInfo(SigningInfo.SignerType  signerType, Name signerName) {
			this.validityPeriod_ = new ValidityPeriod();
			reset(signerType);
			name_ = new Name(signerName);
			digestAlgorithm_ = net.named_data.jndn.security.DigestAlgorithm.SHA256;
		}
	
		/// <summary>
		/// Create a SigningInfo with the signerType and an empty signerName, with
		/// other default values. The digest algorithm is set to DigestAlgorithm.SHA256.
		/// </summary>
		///
		/// <param name="signerType">The type of signer.</param>
		public SigningInfo(SigningInfo.SignerType  signerType) {
			this.validityPeriod_ = new ValidityPeriod();
			reset(signerType);
			digestAlgorithm_ = net.named_data.jndn.security.DigestAlgorithm.SHA256;
		}
	
		/// <summary>
		/// Create a SigningInfo with a signerType of NULL and an empty signerName,
		/// with other default values. The digest algorithm is set to
		/// DigestAlgorithm.SHA256.
		/// </summary>
		///
		public SigningInfo() {
			this.validityPeriod_ = new ValidityPeriod();
			reset(net.named_data.jndn.security.SigningInfo.SignerType.NULL);
			digestAlgorithm_ = net.named_data.jndn.security.DigestAlgorithm.SHA256;
		}
	
		/// <summary>
		/// Create a SigningInfo of type SignerType.ID according to the given
		/// PibIdentity. The digest algorithm is set to DigestAlgorithm.SHA256.
		/// </summary>
		///
		/// <param name="identity">identity.getName().</param>
		public SigningInfo(PibIdentity identity) {
			this.validityPeriod_ = new ValidityPeriod();
			digestAlgorithm_ = net.named_data.jndn.security.DigestAlgorithm.SHA256;
			setPibIdentity(identity);
		}
	
		/// <summary>
		/// Create a SigningInfo of type SignerType.KEY according to the given PibKey.
		/// The digest algorithm is set to DigestAlgorithm.SHA256.
		/// </summary>
		///
		/// <param name="key">key.getName().</param>
		public SigningInfo(PibKey key) {
			this.validityPeriod_ = new ValidityPeriod();
			digestAlgorithm_ = net.named_data.jndn.security.DigestAlgorithm.SHA256;
			setPibKey(key);
		}
	
		/// <summary>
		/// Create a SigningInfo from its string representation.
		/// The digest algorithm is set to DigestAlgorithm.SHA256.
		/// </summary>
		///
		/// <param name="signingString">Default signing: "" (the empty string). Signing with the default certificate of the default key for the identity with the specified name: `id:/my-identity`. Signing with the default certificate of the key with the specified name: `key:/my-identity/ksk-1`. Signing with the certificate with the specified name: `cert:/my-identity/KEY/ksk-1/ID-CERT/%FD%01`. Signing with sha256 digest: `id:/localhost/identity/digest-sha256` (the value returned by getDigestSha256Identity()).</param>
		/// <exception cref="System.ArgumentException">if the signingString format is invalid.</exception>
		public SigningInfo(String signingString) {
			this.validityPeriod_ = new ValidityPeriod();
			reset(net.named_data.jndn.security.SigningInfo.SignerType.NULL);
			digestAlgorithm_ = net.named_data.jndn.security.DigestAlgorithm.SHA256;
	
			if (signingString.equals(""))
				return;
	
			int iColon = signingString.indexOf(':');
			if (iColon < 0)
				throw new ArgumentException(
						"Invalid signing string cannot represent SigningInfo");
	
			String scheme = signingString.Substring(0,(iColon)-(0));
			String nameArg = signingString.Substring(iColon + 1);
	
			if (scheme.equals("id")) {
				if (nameArg.equals(getDigestSha256Identity().toUri()))
					setSha256Signing();
				else
					setSigningIdentity(new Name(nameArg));
			} else if (scheme.equals("key"))
				setSigningKeyName(new Name(nameArg));
			else if (scheme.equals("cert"))
				setSigningCertificateName(new Name(nameArg));
			else
				throw new ArgumentException("Invalid signing string scheme");
		}
	
		/// <summary>
		/// Create a SigningInfo as a copy of the given signingInfo. (This takes a
		/// pointer to the given signingInfo PibIdentity and PibKey without copying.)
		/// </summary>
		///
		/// <param name="signingInfo">The SigningInfo to copy.</param>
		public SigningInfo(SigningInfo signingInfo) {
			this.validityPeriod_ = new ValidityPeriod();
			type_ = signingInfo.type_;
			name_ = new Name(signingInfo.name_);
			identity_ = signingInfo.identity_;
			key_ = signingInfo.key_;
			digestAlgorithm_ = signingInfo.digestAlgorithm_;
			validityPeriod_ = new ValidityPeriod(signingInfo.validityPeriod_);
		}
	
		/// <summary>
		/// Set this to type SignerType.ID and an identity with name identityName.
		/// This does not change the digest algorithm.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity. This copies the Name.</param>
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setSigningIdentity(Name identityName) {
			reset(net.named_data.jndn.security.SigningInfo.SignerType.ID);
			name_ = new Name(identityName);
			return this;
		}
	
		/// <summary>
		/// Set this to type SignerType.KEY and a key with name keyName.
		/// This does not change the digest algorithm.
		/// </summary>
		///
		/// <param name="keyName">The name of the key. This copies the Name.</param>
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setSigningKeyName(Name keyName) {
			reset(net.named_data.jndn.security.SigningInfo.SignerType.KEY);
			name_ = new Name(keyName);
			return this;
		}
	
		/// <summary>
		/// Set this to type SignerType.CERT and a certificate with name
		/// certificateName. This does not change the digest algorithm.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate. This copies the Name.</param>
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setSigningCertificateName(Name certificateName) {
			reset(net.named_data.jndn.security.SigningInfo.SignerType.CERT);
			name_ = new Name(certificateName);
			return this;
		}
	
		/// <summary>
		/// Set this to type SignerType.SHA256, and set the digest algorithm to
		/// DigestAlgorithm.SHA256.
		/// </summary>
		///
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setSha256Signing() {
			reset(net.named_data.jndn.security.SigningInfo.SignerType.SHA256);
			digestAlgorithm_ = net.named_data.jndn.security.DigestAlgorithm.SHA256;
			return this;
		}
	
		/// <summary>
		/// Set this to type SignerType.ID according to the given PibIdentity.
		/// This does not change the digest algorithm.
		/// </summary>
		///
		/// <param name="identity">identity.getName().</param>
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setPibIdentity(PibIdentity identity) {
			reset(net.named_data.jndn.security.SigningInfo.SignerType.ID);
			if (identity != null)
				name_ = identity.getName();
			identity_ = identity;
			return this;
		}
	
		/// <summary>
		/// Set this to type SignerType.KEY according to the given PibKey.
		/// This does not change the digest algorithm.
		/// </summary>
		///
		/// <param name="key">key.getName().</param>
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setPibKey(PibKey key) {
			reset(net.named_data.jndn.security.SigningInfo.SignerType.KEY);
			if (key != null)
				name_ = key.getName();
			key_ = key;
			return this;
		}
	
		/// <summary>
		/// Get the type of the signer.
		/// </summary>
		///
		/// <returns>The type of the signer</returns>
		public SigningInfo.SignerType  getSignerType() {
			return type_;
		}
	
		/// <summary>
		/// Get the name of signer.
		/// </summary>
		///
		/// <returns>The name of signer. The interpretation differs based on the
		/// signerType.</returns>
		public Name getSignerName() {
			return name_;
		}
	
		/// <summary>
		/// Get the PibIdentity of the signer.
		/// </summary>
		///
		/// <returns>The PibIdentity handler of the signer, or null if getSignerName()
		/// should be used to find the identity.</returns>
		/// <exception cref="System.AssertionError">if the signer type is not SignerType.ID.</exception>
		public PibIdentity getPibIdentity() {
			if (type_ != net.named_data.jndn.security.SigningInfo.SignerType.ID)
				throw new AssertionError(
						"getPibIdentity: The signer type is not SignerType.ID");
			return identity_;
		}
	
		/// <summary>
		/// Get the PibKey of the signer.
		/// </summary>
		///
		/// <returns>The PibKey handler of the signer, or null if getSignerName() should
		/// be used to find the key.</returns>
		/// <exception cref="System.AssertionError">if the signer type is not SignerType.KEY.</exception>
		public PibKey getPibKey() {
			if (type_ != net.named_data.jndn.security.SigningInfo.SignerType.KEY)
				throw new AssertionError(
						"getPibKey: The signer type is not SignerType.KEY");
			return key_;
		}
	
		/// <summary>
		/// Set the digest algorithm for public key operations.
		/// </summary>
		///
		/// <param name="digestAlgorithm">The digest algorithm.</param>
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setDigestAlgorithm(DigestAlgorithm digestAlgorithm) {
			digestAlgorithm_ = digestAlgorithm;
			return this;
		}
	
		/// <summary>
		/// Get the digest algorithm for public key operations.
		/// </summary>
		///
		/// <returns>The digest algorithm.</returns>
		public DigestAlgorithm getDigestAlgorithm() {
			return digestAlgorithm_;
		}
	
		/// <summary>
		/// Set the validity period for the signature info.
		/// Note that the equivalent ndn-cxx method uses a semi-prepared SignatureInfo,
		/// but this method only uses the ValidityPeriod from the SignatureInfo.
		/// </summary>
		///
		/// <param name="validityPeriod">The validity period, which is copied.</param>
		/// <returns>This SigningInfo.</returns>
		public SigningInfo setValidityPeriod(ValidityPeriod validityPeriod) {
			validityPeriod_ = new ValidityPeriod(validityPeriod);
			return this;
		}
	
		/// <summary>
		/// Get the validity period for the signature info.
		/// Note that the equivalent ndn-cxx method uses a semi-prepared SignatureInfo,
		/// but this method only uses the ValidityPeriod from the SignatureInfo.
		/// </summary>
		///
		/// <returns>The validity period.</returns>
		public ValidityPeriod getValidityPeriod() {
			return validityPeriod_;
		}
	
		/// <summary>
		/// Get the string representation of this SigningInfo.
		/// </summary>
		///
		/// <returns>The string representation.</returns>
		public override String ToString() {
			if (type_ == net.named_data.jndn.security.SigningInfo.SignerType.NULL)
				return "";
			else if (type_ == net.named_data.jndn.security.SigningInfo.SignerType.ID)
				return "id:" + getSignerName().toUri();
			else if (type_ == net.named_data.jndn.security.SigningInfo.SignerType.KEY)
				return "key:" + getSignerName().toUri();
			else if (type_ == net.named_data.jndn.security.SigningInfo.SignerType.CERT)
				return "cert:" + getSignerName().toUri();
			else if (type_ == net.named_data.jndn.security.SigningInfo.SignerType.SHA256)
				return "id:" + getDigestSha256Identity().toUri();
			else
				// We don't expect this to happen.
				throw new AssertionError("Unknown signer type");
		}
	
		/// <summary>
		/// Get the localhost identity which indicates that the signature is generated
		/// using SHA-256.
		/// </summary>
		///
		/// <returns>A new Name of the SHA-256 identity.</returns>
		public static Name getDigestSha256Identity() {
			return new Name("/localhost/identity/digest-sha256");
		}
	
		/// <summary>
		/// Check and set the signerType, and set others to default values. This does
		/// NOT reset the digest algorithm.
		/// </summary>
		///
		/// <param name="signerType">The The type of signer.</param>
		private void reset(SigningInfo.SignerType  signerType) {
			if (!(signerType == net.named_data.jndn.security.SigningInfo.SignerType.NULL || signerType == net.named_data.jndn.security.SigningInfo.SignerType.ID
					|| signerType == net.named_data.jndn.security.SigningInfo.SignerType.KEY
					|| signerType == net.named_data.jndn.security.SigningInfo.SignerType.CERT || signerType == net.named_data.jndn.security.SigningInfo.SignerType.SHA256))
				throw new AssertionError("SigningInfo: The signerType is not valid");
	
			type_ = signerType;
			name_ = new Name();
			identity_ = null;
			key_ = null;
			validityPeriod_ = new ValidityPeriod();
		}
	
		private SigningInfo.SignerType  type_;
		private Name name_;
		private PibIdentity identity_;
		private PibKey key_;
		private DigestAlgorithm digestAlgorithm_;
		private ValidityPeriod validityPeriod_;
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
