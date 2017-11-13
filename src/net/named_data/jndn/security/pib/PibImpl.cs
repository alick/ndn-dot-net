// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.security.pib {
	
	using ILOG.J2CsMapping.Collections;
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// PibImpl is an abstract base class for the PIB implementation used by the Pib
	/// class. This class defines the interface that an actual PIB implementation
	/// should provide, for example PibMemory.
	/// </summary>
	///
	public abstract class PibImpl {
		/// <summary>
		/// A PibImpl.Error extends Exception and represents a non-semantic error in
		/// PIB implementation processing. A subclass of PibImpl may throw a subclass
		/// of this class when there's a non-semantic error, such as a storage problem.
		/// Note that even though this is called "Error" to be consistent with the
		/// other libraries, it extends the Java Exception class, not Error.
		/// </summary>
		///
		[Serializable]
		public class Error : Exception {
			public Error(String message) : base(message) {
			}
		}
	
		// TpmLocator management.
	
		/// <summary>
		/// Set the corresponding TPM information to tpmLocator. This method does not
		/// reset the contents of the PIB.
		/// </summary>
		///
		/// <param name="tpmLocator">The TPM locator string.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void setTpmLocator(String tpmLocator);
	
		/// <summary>
		/// Get the TPM Locator.
		/// </summary>
		///
		/// <returns>The TPM locator string.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract String getTpmLocator();
	
		// Identity management.
	
		/// <summary>
		/// Check for the existence of an identity.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <returns>True if the identity exists, otherwise false.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract bool hasIdentity(Name identityName);
	
		/// <summary>
		/// Add the identity. If the identity already exists, do nothing. If no default
		/// identity has been set, set the added identity as the default.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity to add. This copies the name.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void addIdentity(Name identityName);
	
		/// <summary>
		/// Remove the identity and its related keys and certificates. If the default
		/// identity is being removed, no default identity will be selected.  If the
		/// identity does not exist, do nothing.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity to remove.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void removeIdentity(Name identityName);
	
		/// <summary>
		/// Erase all certificates, keys, and identities.
		/// </summary>
		///
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void clearIdentities();
	
		/// <summary>
		/// Get the names of all the identities.
		/// </summary>
		///
		/// <returns>The set of identity names. The Name objects are fresh copies.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract HashedSet<Name> getIdentities();
	
		/// <summary>
		/// Set the identity with the identityName as the default identity. If the
		/// identity with identityName does not exist, then it will be created.
		/// </summary>
		///
		/// <param name="identityName">The name for the default identity. This copies the name.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void setDefaultIdentity(Name identityName);
	
		/// <summary>
		/// Get the default identity.
		/// </summary>
		///
		/// <returns>The name of the default identity, as a fresh copy.</returns>
		/// <exception cref="Pib.Error">for no default identity.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract Name getDefaultIdentity();
	
		// Key management.
	
		/// <summary>
		/// Check for the existence of a key with keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>True if the key exists, otherwise false. Return false if the
		/// identity does not exist.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract bool hasKey(Name keyName);
	
		/// <summary>
		/// Add the key. If a key with the same name already exists, overwrite the key.
		/// If the identity does not exist, it will be created. If no default key for
		/// the identity has been set, then set the added key as the default for the
		/// identity.  If no default identity has been set, identity becomes the
		/// default.
		/// </summary>
		///
		/// <param name="identityName"></param>
		/// <param name="keyName">The name of the key. This copies the name.</param>
		/// <param name="key">The public key bits. This copies the array.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void addKey(Name identityName, Name keyName, ByteBuffer key);
	
		/// <summary>
		/// Remove the key with keyName and its related certificates. If the key does
		/// not exist, do nothing.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void removeKey(Name keyName);
	
		/// <summary>
		/// Get the key bits of a key with name keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The key bits.</returns>
		/// <exception cref="Pib.Error">if the key does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract Blob getKeyBits(Name keyName);
	
		/// <summary>
		/// Get all the key names of the identity with the name identityName. The
		/// returned key names can be used to create a KeyContainer. With a key name
		/// and a backend implementation, one can create a Key front end instance.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <returns>The set of key names. The Name objects are fresh copies. If the
		/// identity does not exist, return an empty set.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract HashedSet<Name> getKeysOfIdentity(Name identityName);
	
		/// <summary>
		/// Set the key with keyName as the default key for the identity with name
		/// identityName.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity. This copies the name.</param>
		/// <param name="keyName">The name of the key. This copies the name.</param>
		/// <exception cref="Pib.Error">if the key does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void setDefaultKeyOfIdentity(Name identityName, Name keyName);
	
		/// <summary>
		/// Get the name of the default key for the identity with name identityName.
		/// </summary>
		///
		/// <param name="identityName">The name of the identity.</param>
		/// <returns>The name of the default key, as a fresh copy.</returns>
		/// <exception cref="Pib.Error">if there is no default key or if the identity does notexist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract Name getDefaultKeyOfIdentity(Name identityName);
	
		// Certificate management.
	
		/// <summary>
		/// Check for the existence of a certificate with name certificateName.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <returns>True if the certificate exists, otherwise false.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract bool hasCertificate(Name certificateName);
	
		/// <summary>
		/// Add the certificate. If a certificate with the same name (without implicit
		/// digest) already exists, then overwrite the certificate. If the key or
		/// identity does not exist, they will be created. If no default certificate
		/// for the key has been set, then set the added certificate as the default for
		/// the key. If no default key was set for the identity, it will be set as
		/// default key for the identity. If no default identity was selected, the
		/// certificate's identity becomes the default.
		/// </summary>
		///
		/// <param name="certificate">The certificate to add. This copies the object.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void addCertificate(CertificateV2 certificate);
	
		/// <summary>
		/// Remove the certificate with name certificateName. If the certificate does
		/// not exist, do nothing.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void removeCertificate(Name certificateName);
	
		/// <summary>
		/// Get the certificate with name certificateName.
		/// </summary>
		///
		/// <param name="certificateName">The name of the certificate.</param>
		/// <returns>A copy of the certificate.</returns>
		/// <exception cref="Pib.Error">if the certificate does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract CertificateV2 getCertificate(Name certificateName);
	
		/// <summary>
		/// Get a list of certificate names of the key with id keyName. The returned
		/// certificate names can be used to create a PibCertificateContainer. With a
		/// certificate name and a backend implementation, one can obtain the
		/// certificate.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>The set of certificate names. The Name objects are fresh copies. If
		/// the key does not exist, return an empty set.</returns>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract HashedSet<Name> getCertificatesOfKey(Name keyName);
	
		/// <summary>
		/// Set the cert with name certificateName as the default for the key with
		/// keyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <param name="certificateName">The name of the certificate. This copies the name.</param>
		/// <exception cref="Pib.Error">if the certificate with name certificateName does notexist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract void setDefaultCertificateOfKey(Name keyName,
				Name certificateName);
	
		/// <summary>
		/// Get the default certificate for the key with eyName.
		/// </summary>
		///
		/// <param name="keyName">The name of the key.</param>
		/// <returns>A copy of the default certificate.</returns>
		/// <exception cref="Pib.Error">if the default certificate does not exist.</exception>
		/// <exception cref="PibImpl.Error">for a non-semantic (database access) error.</exception>
		public abstract CertificateV2 getDefaultCertificateOfKey(Name keyName);
	}
}
