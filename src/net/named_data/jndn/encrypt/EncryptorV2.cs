// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using ILOG.J2CsMapping.Util;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using javax.crypto;
	using javax.crypto.spec;
	using net.named_data.jndn;
	using net.named_data.jndn.in_memory_storage;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.certificate;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// EncryptorV2 encrypts the requested content for name-based access control (NAC)
	/// using security v2. For the meaning of "KEK", etc. see:
	/// https://github.com/named-data/name-based-access-control/blob/new/docs/spec.rst
	/// </summary>
	///
	public class EncryptorV2 {
		public sealed class Anonymous_C6 : OnInterestCallback {
				private readonly EncryptorV2 outer_EncryptorV2;
		
				public Anonymous_C6(EncryptorV2 paramouter_EncryptorV2) {
					this.outer_EncryptorV2 = paramouter_EncryptorV2;
				}
		
				public void onInterest(Name prefix, Interest interest,
						Face face, long interestFilterId,
						InterestFilter filter) {
					Data data = outer_EncryptorV2.storage_.find(interest);
					if (data != null) {
						net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
								"Serving {0} from InMemoryStorage",
								data.getName());
						try {
							face.putData(data);
						} catch (IOException ex) {
							net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
									"Error in Face.putData: {0}", ex);
						}
					} else {
						net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
								"Didn't find CK data for {0}",
								interest.getName());
						// TODO: Send NACK?
					}
				}
			}
	
		public sealed class Anonymous_C5 : OnRegisterFailed {
			public void onRegisterFailed(Name prefix) {
				net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
						"Failed to register prefix {0}", prefix);
			}
		}
	
		public sealed class Anonymous_C4 : IRunnable {
				private readonly EncryptorV2 outer_EncryptorV2;
		
				public Anonymous_C4(EncryptorV2 paramouter_EncryptorV2) {
					this.outer_EncryptorV2 = paramouter_EncryptorV2;
				}
		
				public void run() {
					net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "The KEK was retrieved and published");
					outer_EncryptorV2.isKekRetrievalInProgress_ = false;
				}
			}
	
		public sealed class Anonymous_C3 : net.named_data.jndn.encrypt.EncryptError.OnError  {
				private readonly EncryptorV2 outer_EncryptorV2;
		
				public Anonymous_C3(EncryptorV2 paramouter_EncryptorV2) {
					this.outer_EncryptorV2 = paramouter_EncryptorV2;
				}
		
				public void onError(EncryptError.ErrorCode errorCode, String message) {
					net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Failed to retrieve KEK: {0}", message);
					outer_EncryptorV2.isKekRetrievalInProgress_ = false;
					outer_EncryptorV2.onError_.onError(errorCode, message);
				}
			}
	
		public sealed class Anonymous_C2 : OnData {
				private readonly EncryptorV2 outer_EncryptorV2;
				private readonly IRunnable onReady;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
		
				public Anonymous_C2(EncryptorV2 paramouter_EncryptorV2,
						IRunnable onReady_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
					this.onReady = onReady_0;
					this.onError = onError_1;
					this.outer_EncryptorV2 = paramouter_EncryptorV2;
				}
		
				public void onData(Interest interest, Data kekData) {
					outer_EncryptorV2.kekPendingInterestId_ = 0;
					// TODO: Verify if the key is legitimate.
					outer_EncryptorV2.kekData_ = kekData;
					if (outer_EncryptorV2.makeAndPublishCkData(onError))
						onReady.run();
					// Otherwise, failure has already been reported.
				}
			}
	
		public sealed class Anonymous_C1 : OnTimeout {
				public sealed class Anonymous_C9 : IRunnable {
					private readonly EncryptorV2.Anonymous_C1  outer_Anonymous_C1;
		
					public Anonymous_C9(EncryptorV2.Anonymous_C1  paramouter_Anonymous_C1) {
						this.outer_Anonymous_C1 = paramouter_Anonymous_C1;
					}
		
					public void run() {
						outer_Anonymous_C1.outer_EncryptorV2.retryFetchingKek();
					}
				}
		
				private readonly EncryptorV2 outer_EncryptorV2;
				private readonly int nTriesLeft;
				private readonly IRunnable onReady;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
		
				public Anonymous_C1(EncryptorV2 paramouter_EncryptorV2, int nTriesLeft_0,
						IRunnable onReady_1, net.named_data.jndn.encrypt.EncryptError.OnError  onError_2) {
					this.nTriesLeft = nTriesLeft_0;
					this.onReady = onReady_1;
					this.onError = onError_2;
					this.outer_EncryptorV2 = paramouter_EncryptorV2;
				}
		
				public void onTimeout(Interest interest) {
					outer_EncryptorV2.kekPendingInterestId_ = 0;
					if (nTriesLeft > 1)
						outer_EncryptorV2.fetchKekAndPublishCkData(onReady, onError,
								nTriesLeft - 1);
					else {
						onError.onError(
								net.named_data.jndn.encrypt.EncryptError.ErrorCode.KekRetrievalTimeout,
								"Retrieval of KEK ["
										+ interest.getName().toUri()
										+ "] timed out");
						net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO,
								"Scheduling retry after all timeouts");
						outer_EncryptorV2.face_.callLater(net.named_data.jndn.encrypt.EncryptorV2.RETRY_DELAY_KEK_RETRIEVAL_MS,
								new net.named_data.jndn.encrypt.EncryptorV2.Anonymous_C1.Anonymous_C9 (this));
					}
				}
			}
	
		public sealed class Anonymous_C0 : OnNetworkNack {
				public sealed class Anonymous_C8 : IRunnable {
								private readonly EncryptorV2.Anonymous_C0  outer_Anonymous_C0;
					
								public Anonymous_C8(EncryptorV2.Anonymous_C0  paramouter_Anonymous_C0) {
									this.outer_Anonymous_C0 = paramouter_Anonymous_C0;
								}
					
								public void run() {
									outer_Anonymous_C0.outer_EncryptorV2.fetchKekAndPublishCkData(outer_Anonymous_C0.onReady,
											outer_Anonymous_C0.onError, outer_Anonymous_C0.nTriesLeft - 1);
								}
							}
		
				public sealed class Anonymous_C7 : IRunnable {
					private readonly EncryptorV2.Anonymous_C0  outer_Anonymous_C0;
		
					public Anonymous_C7(EncryptorV2.Anonymous_C0  paramouter_Anonymous_C0) {
						this.outer_Anonymous_C0 = paramouter_Anonymous_C0;
					}
		
					public void run() {
						outer_Anonymous_C0.outer_EncryptorV2.retryFetchingKek();
					}
				}
		
				internal readonly EncryptorV2 outer_EncryptorV2;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				internal readonly IRunnable onReady;
				internal readonly int nTriesLeft;
		
				public Anonymous_C0(EncryptorV2 paramouter_EncryptorV2,
						net.named_data.jndn.encrypt.EncryptError.OnError  onError_0, IRunnable onReady_1, int nTriesLeft_2) {
					this.onError = onError_0;
					this.onReady = onReady_1;
					this.nTriesLeft = nTriesLeft_2;
					this.outer_EncryptorV2 = paramouter_EncryptorV2;
				}
		
				public void onNetworkNack(Interest interest,
						NetworkNack networkNack) {
					outer_EncryptorV2.kekPendingInterestId_ = 0;
					if (nTriesLeft > 1) {
						outer_EncryptorV2.face_.callLater(net.named_data.jndn.encrypt.EncryptorV2.RETRY_DELAY_AFTER_NACK_MS,
								new net.named_data.jndn.encrypt.EncryptorV2.Anonymous_C0.Anonymous_C8 (this));
					} else {
						onError.onError(
								net.named_data.jndn.encrypt.EncryptError.ErrorCode.KekRetrievalFailure,
								"Retrieval of KEK ["
										+ interest.getName().toUri()
										+ "] failed. Got NACK ("
										+ networkNack.getReason() + ")");
						net.named_data.jndn.encrypt.EncryptorV2.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Scheduling retry from NACK");
						outer_EncryptorV2.face_.callLater(net.named_data.jndn.encrypt.EncryptorV2.RETRY_DELAY_KEK_RETRIEVAL_MS,
								new net.named_data.jndn.encrypt.EncryptorV2.Anonymous_C0.Anonymous_C7 (this));
					}
				}
			}
	
		/// <summary>
		/// Create an EncryptorV2 with the given parameters. This uses the face to
		/// register to receive Interests for the prefix {ckPrefix}/CK.
		/// </summary>
		///
		/// <param name="accessPrefix"></param>
		/// <param name="ckPrefix"></param>
		/// <param name="ckDataSigningInfo"></param>
		/// <param name="onError_0">onError.onError(errorCode, message) where errorCode is from the EncryptError.ErrorCode enum, and message is an error string. The encrypt method will continue trying to retrieve the KEK until success (with each attempt separated by RETRY_DELAY_KEK_RETRIEVAL) and onError may be called multiple times. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="validator">The validation policy to ensure correctness of KEK.</param>
		/// <param name="keyChain">The KeyChain used to sign Data packets.</param>
		/// <param name="face">The Face that will be used to fetch the KEK and publish CK data.</param>
		public EncryptorV2(Name accessPrefix, Name ckPrefix,
				SigningInfo ckDataSigningInfo, net.named_data.jndn.encrypt.EncryptError.OnError  onError_0,
				Validator validator, KeyChain keyChain, Face face) {
			this.kekData_ = null;
					this.storage_ = new InMemoryStorageRetaining();
					this.kekPendingInterestId_ = 0;
			// Copy the Name.
			accessPrefix_ = new Name(accessPrefix);
			ckPrefix_ = new Name(ckPrefix);
			ckBits_ = new byte[AES_KEY_SIZE];
			ckDataSigningInfo_ = new SigningInfo(ckDataSigningInfo);
			isKekRetrievalInProgress_ = false;
			onError_ = onError_0;
			keyChain_ = keyChain;
			face_ = face;
	
			regenerateCk();
	
			ckRegisteredPrefixId_ = face_.registerPrefix(
					new Name(ckPrefix).append(NAME_COMPONENT_CK),
					new EncryptorV2.Anonymous_C6 (this), new EncryptorV2.Anonymous_C5 ());
		}
	
		public void shutdown() {
			face_.unsetInterestFilter(ckRegisteredPrefixId_);
			if (kekPendingInterestId_ > 0)
				face_.removePendingInterest(kekPendingInterestId_);
		}
	
		public EncryptedContent encrypt(byte[] plainData) {
			// Generate the initial vector.
			byte[] iv = new byte[AES_IV_SIZE];
			net.named_data.jndn.util.Common.getRandom().nextBytes(iv);
	
			Cipher cipher = javax.crypto.Cipher.getInstance("AES/CBC/PKCS5PADDING");
			try {
				cipher.init(javax.crypto.Cipher.ENCRYPT_MODE, new SecretKeySpec(ckBits_, "AES"),
						new IvParameterSpec(iv));
			} catch (InvalidKeyException ex) {
				throw new Exception(
						"If the error is 'Illegal key size', try installing the "
								+ "Java Cryptography Extension (JCE) Unlimited Strength Jurisdiction Policy Files: "
								+ ex);
			}
			byte[] encryptedData = cipher.doFinal(plainData);
	
			EncryptedContent content = new EncryptedContent();
			content.setInitialVector(new Blob(iv, false));
			content.setPayload(new Blob(encryptedData, false));
			content.setKeyLocatorName(ckName_);
	
			return content;
		}
	
		/// <summary>
		/// Create a new Content Key (CK) and publish the corresponding CK Data packet.
		/// This uses the onError given to the constructor to report errors.
		/// </summary>
		///
		public void regenerateCk() {
			// TODO: Ensure that the CK Data packet for the old CK is published when the
			// CK is updated before the KEK is fetched.
	
			ckName_ = new Name(ckPrefix_);
			ckName_.append(NAME_COMPONENT_CK);
			// The version is the ID of the CK.
			ckName_.appendVersion((long) net.named_data.jndn.util.Common.getNowMilliseconds());
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Generating new CK: {0}", ckName_);
			net.named_data.jndn.util.Common.getRandom().nextBytes(ckBits_);
	
			// One implication: If the CK is updated before the KEK is fetched, then
			// the KDK for the old CK will not be published.
			if (kekData_ == null)
				retryFetchingKek();
			else
				makeAndPublishCkData(onError_);
		}
	
		/// <summary>
		/// The number of packets stored in in-memory storage.
		/// </summary>
		///
		/// <returns>The number of packets.</returns>
		public int size() {
			return storage_.size();
		}
	
		/// <summary>
		/// Get the the storage cache, which should only be used for testing.
		/// </summary>
		///
		/// <returns>The storage cache.</returns>
		public Hashtable getCache_() {
			return storage_.getCache_();
		}
	
		/// <summary>
		/// Get the isKekRetrievalInProgress_ flag. This is only for testing.
		/// </summary>
		///
		/// <returns>The isKekRetrievalInProgress_ flag;</returns>
		public bool getIsKekRetrievalInProgress_() {
			return isKekRetrievalInProgress_;
		}
	
		/// <summary>
		/// Set the internal kekData_ to null. This is only for testing.
		/// </summary>
		///
		public void clearKekData_() {
			kekData_ = null;
		}
	
		internal void retryFetchingKek() {
			if (isKekRetrievalInProgress_)
				return;
	
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Retrying fetching of the KEK");
			isKekRetrievalInProgress_ = true;
			fetchKekAndPublishCkData(new EncryptorV2.Anonymous_C4 (this), new EncryptorV2.Anonymous_C3 (this), N_RETRIES);
		}
	
		/// <summary>
		/// Create an Interest for <access-prefix>/KEK to retrieve the
		/// <access-prefix>/KEK/<key-id> KEK Data packet, and set kekData_.
		/// </summary>
		///
		/// <param name="onReady_0"></param>
		/// <param name="onError_1">error string.</param>
		/// <param name="nTriesLeft_2">The number of retries for expressInterest timeouts.</param>
		internal void fetchKekAndPublishCkData(IRunnable onReady_0,
				net.named_data.jndn.encrypt.EncryptError.OnError  onError_1, int nTriesLeft_2) {
			logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Fetching KEK: {0}",
					new Name(accessPrefix_).append(NAME_COMPONENT_KEK));
	
			if (kekPendingInterestId_ > 0) {
				onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.SecurityException,
						"fetchKekAndPublishCkData: There is already a kekPendingInterestId_");
				return;
			}
			try {
				kekPendingInterestId_ = face_.expressInterest(new Interest(
						new Name(accessPrefix_).append(NAME_COMPONENT_KEK))
						.setMustBeFresh(true).setCanBePrefix(true), new EncryptorV2.Anonymous_C2 (this, onReady_0, onError_1), new EncryptorV2.Anonymous_C1 (this, nTriesLeft_2, onReady_0, onError_1), new EncryptorV2.Anonymous_C0 (this, onError_1, onReady_0, nTriesLeft_2));
			} catch (Exception ex) {
				onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.General,
						"expressInterest error: " + ex);
			}
		}
	
		/// <summary>
		/// Make a CK Data packet for ckName_ encrypted by the KEK in kekData_ and
		/// insert it in the storage_.
		/// </summary>
		///
		/// <param name="onError_0">error string.</param>
		/// <returns>True on success, else false.</returns>
		internal bool makeAndPublishCkData(net.named_data.jndn.encrypt.EncryptError.OnError  onError_0) {
			try {
				PublicKey kek = new PublicKey(kekData_.getContent());
	
				EncryptedContent content = new EncryptedContent();
				content.setPayload(kek.encrypt(ckBits_,
						net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep));
	
				Data ckData = new Data(new Name(ckName_).append(
						NAME_COMPONENT_ENCRYPTED_BY).append(kekData_.getName()));
				ckData.setContent(content.wireEncodeV2());
				// FreshnessPeriod can serve as a soft access control for revoking access
				ckData.getMetaInfo().setFreshnessPeriod(
						DEFAULT_CK_FRESHNESS_PERIOD_MS);
				keyChain_.sign(ckData, ckDataSigningInfo_);
				storage_.insert(ckData);
	
				logger_.log(ILOG.J2CsMapping.Util.Logging.Level.INFO, "Publishing CK data: {0}", ckData.getName());
				return true;
			} catch (Exception ex) {
				onError_0.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.EncryptionFailure,
						"Failed to encrypt generated CK with KEK "
								+ kekData_.getName().toUri());
				return false;
			}
		}
	
		public static readonly Name.Component NAME_COMPONENT_ENCRYPTED_BY = new Name.Component(
				"ENCRYPTED-BY");
		public static readonly Name.Component NAME_COMPONENT_NAC = new Name.Component(
				"NAC");
		public static readonly Name.Component NAME_COMPONENT_KEK = new Name.Component(
				"KEK");
		public static readonly Name.Component NAME_COMPONENT_KDK = new Name.Component(
				"KDK");
		public static readonly Name.Component NAME_COMPONENT_CK = new Name.Component(
				"CK");
	
		public const double RETRY_DELAY_AFTER_NACK_MS = 1000.0d;
		public const double RETRY_DELAY_KEK_RETRIEVAL_MS = 60 * 1000.0d;
	
		private readonly Name accessPrefix_;
		private readonly Name ckPrefix_;
		private Name ckName_;
		private readonly byte[] ckBits_;
		private readonly SigningInfo ckDataSigningInfo_;
	
		internal bool isKekRetrievalInProgress_;
		internal Data kekData_;
		internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError_;
	
		// Storage for encrypted CKs.
		internal readonly InMemoryStorageRetaining storage_;
		private readonly long ckRegisteredPrefixId_;
		internal long kekPendingInterestId_;
	
		private readonly KeyChain keyChain_;
		internal readonly Face face_;
		static internal readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(EncryptorV2).FullName);
	
		public const int AES_KEY_SIZE = 32;
		public const int AES_IV_SIZE = 16;
		public const int N_RETRIES = 3;
	
		private const double DEFAULT_CK_FRESHNESS_PERIOD_MS = 3600 * 1000.0d;
	}
}
