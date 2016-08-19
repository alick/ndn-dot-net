// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2014-2016 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security.policy {
	
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.identity;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A SelfVerifyPolicyManager implements a PolicyManager to look in the
	/// IdentityStorage for the public key with the name in the KeyLocator (if
	/// available) and use it to verify the data packet, without searching a
	/// certificate chain.  If the public key can't be found, the verification fails.
	/// </summary>
	///
	public class SelfVerifyPolicyManager : PolicyManager {
		/// <summary>
		/// Create a new SelfVerifyPolicyManager which will look up the public key in
		/// the given identityStorage.
		/// </summary>
		///
		/// <param name="identityStorage">life of this SelfVerifyPolicyManager.</param>
		public SelfVerifyPolicyManager(IdentityStorage identityStorage) {
			identityStorage_ = identityStorage;
		}
	
		/// <summary>
		/// Create a new SelfVerifyPolicyManager which will look up the public key in
		/// the given identityStorage.
		/// Since there is no IdentotyStorage, don't look for a public key with the
		/// name in the KeyLocator and rely on the KeyLocator having the full public
		/// key DER.
		/// </summary>
		///
		public SelfVerifyPolicyManager() {
			identityStorage_ = null;
		}
	
		/// <summary>
		/// Never skip verification.
		/// </summary>
		///
		/// <param name="data">The received data packet.</param>
		/// <returns>false.</returns>
		public override bool skipVerifyAndTrust(Data data) {
			return false;
		}
	
		/// <summary>
		/// Never skip verification.
		/// </summary>
		///
		/// <param name="interest">The received interest.</param>
		/// <returns>false.</returns>
		public override bool skipVerifyAndTrust(Interest interest) {
			return false;
		}
	
		/// <summary>
		/// Always return true to use the self-verification rule for the received data.
		/// </summary>
		///
		/// <param name="data">The received data packet.</param>
		/// <returns>true.</returns>
		public override bool requireVerify(Data data) {
			return true;
		}
	
		/// <summary>
		/// Always return true to use the self-verification rule for the received interest.
		/// </summary>
		///
		/// <param name="interest">The received interest.</param>
		/// <returns>true.</returns>
		public override bool requireVerify(Interest interest) {
			return true;
		}
	
		/// <summary>
		/// Look in the IdentityStorage for the public key with the name in the
		/// KeyLocator (if available) and use it to verify the data packet.  If the
		/// public key can't be found, call onVerifyFailed.
		/// </summary>
		///
		/// <param name="data">The Data object with the signature to check.</param>
		/// <param name="stepCount"></param>
		/// <param name="onVerified">better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onVerifyFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <returns>null for no further step for looking up a certificate chain.</returns>
		public override ValidationRequest checkVerificationPolicy(Data data, int stepCount,
				OnVerified onVerified, OnVerifyFailed onVerifyFailed) {
			// wireEncode returns the cached encoding if available.
			if (verify(data.getSignature(), data.wireEncode())) {
				try {
					onVerified.onVerified(data);
				} catch (Exception ex) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onVerified", ex);
				}
			} else {
				try {
					onVerifyFailed.onVerifyFailed(data);
				} catch (Exception ex_0) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onVerifyFailed", ex_0);
				}
			}
	
			// No more steps, so return a null ValidationRequest.
			return null;
		}
	
		/// <summary>
		/// Use wireFormat.decodeSignatureInfoAndValue to decode the last two name
		/// components of the signed interest. Look in the IdentityStorage for the
		/// public key with the name in the KeyLocator (if available) and use it to
		/// verify the interest. If the public key can't be found, call onVerifyFailed.
		/// </summary>
		///
		/// <param name="interest">The interest with the signature to check.</param>
		/// <param name="stepCount"></param>
		/// <param name="onVerified">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onVerifyFailed">NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <returns>null for no further step for looking up a certificate chain.</returns>
		public override ValidationRequest checkVerificationPolicy(Interest interest,
				int stepCount, OnVerifiedInterest onVerified,
				OnVerifyInterestFailed onVerifyFailed, WireFormat wireFormat) {
			// Decode the last two name components of the signed interest
			net.named_data.jndn.Signature signature;
			try {
				signature = wireFormat.decodeSignatureInfoAndValue(interest
						.getName().get(-2).getValue().buf(), interest.getName()
						.get(-1).getValue().buf(), false);
			} catch (EncodingException ex) {
				logger_.log(
						ILOG.J2CsMapping.Util.Logging.Level.INFO,
						"Cannot decode the signed interest SignatureInfo and value",
						ex);
				try {
					onVerifyFailed.onVerifyInterestFailed(interest);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onVerifyInterestFailed",
							exception);
				}
				return null;
			}
	
			// wireEncode returns the cached encoding if available.
			if (verify(signature, interest.wireEncode(wireFormat))) {
				try {
					onVerified.onVerifiedInterest(interest);
				} catch (Exception ex_0) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onVerifiedInterest", ex_0);
				}
			} else {
				try {
					onVerifyFailed.onVerifyInterestFailed(interest);
				} catch (Exception ex_1) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onVerifyInterestFailed", ex_1);
				}
			}
	
			// No more steps, so return a null ValidationRequest.
			return null;
		}
	
		/// <summary>
		/// Override to always indicate that the signing certificate name and data name
		/// satisfy the signing policy.
		/// </summary>
		///
		/// <param name="dataName">The name of data to be signed.</param>
		/// <param name="certificateName">The name of signing certificate.</param>
		/// <returns>true to indicate that the signing certificate can be used to sign
		/// the data.</returns>
		public override bool checkSigningPolicy(Name dataName, Name certificateName) {
			return true;
		}
	
		/// <summary>
		/// Override to indicate that the signing identity cannot be inferred.
		/// </summary>
		///
		/// <param name="dataName">The name of data to be signed.</param>
		/// <returns>An empty name because cannot infer.</returns>
		public override Name inferSigningIdentity(Name dataName) {
			return new Name();
		}
	
		/// <summary>
		/// Check the type of signatureInfo to get the KeyLocator. Look in the
		/// IdentityStorage for the public key with the name in the KeyLocator (if
		/// available) and use it to verify the signedBlob. If the public key can't be
		/// found, return false. (This is a generalized method which can verify both a
		/// Data packet and an interest.)
		/// </summary>
		///
		/// <param name="signatureInfo"></param>
		/// <param name="signedBlob">the SignedBlob with the signed portion to verify.</param>
		/// <returns>True if the signature is verified, false if failed.</returns>
		private bool verify(net.named_data.jndn.Signature signatureInfo,
				SignedBlob signedBlob) {
			Blob publicKeyDer = null;
			if (net.named_data.jndn.KeyLocator.canGetFromSignature(signatureInfo)) {
				publicKeyDer = getPublicKeyDer(net.named_data.jndn.KeyLocator
						.getFromSignature(signatureInfo));
				if (publicKeyDer.isNull())
					return false;
			}
	
			return net.named_data.jndn.security.policy.PolicyManager.verifySignature(signatureInfo, signedBlob, publicKeyDer);
		}
	
		/// <summary>
		/// Look in the IdentityStorage for the public key with the name in the
		/// KeyLocator (if available). If the public key can't be found, return and
		/// empty Blob.
		/// </summary>
		///
		/// <param name="keyLocator">The KeyLocator.</param>
		/// <returns>The public key DER or an empty Blob if not found.</returns>
		private Blob getPublicKeyDer(KeyLocator keyLocator) {
			if (keyLocator.getType() == net.named_data.jndn.KeyLocatorType.KEYNAME
					&& identityStorage_ != null) {
				try {
					// Assume the key name is a certificate name.
					return identityStorage_
							.getKey(net.named_data.jndn.security.certificate.IdentityCertificate
									.certificateNameToPublicKeyName(keyLocator
											.getKeyName()));
				} catch (SecurityException ex) {
					// The storage doesn't have the key.
					return new Blob();
				}
			} else
				// Can't find a key to verify.
				return new Blob();
		}
	
		private readonly IdentityStorage identityStorage_;
		private static readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(SelfVerifyPolicyManager).FullName);
	}
}
