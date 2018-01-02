// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.encrypt {
	
	using ILOG.J2CsMapping.NIO;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encrypt.algo;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A Consumer manages fetched group keys used to decrypt a data packet in the
	/// group-based encryption protocol.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public class Consumer {
		/// <summary>
		/// Create a Consumer to use the given ConsumerDb, Face and other values.
		/// </summary>
		///
		/// <param name="face">The face used for data packet and key fetching.</param>
		/// <param name="keyChain">The keyChain used to verify data packets.</param>
		/// <param name="groupName"></param>
		/// <param name="consumerName"></param>
		/// <param name="database">The ConsumerDb database for storing decryption keys.</param>
		/// <param name="cKeyLink">getDelegations().size() is zero, don't use it.</param>
		/// <param name="dKeyLink">getDelegations().size() is zero, don't use it.</param>
		public Consumer(Face face, KeyChain keyChain, Name groupName,
				Name consumerName, ConsumerDb database, Link cKeyLink, Link dKeyLink) {
			this.cKeyMap_ = new Hashtable();
					this.dKeyMap_ = new Hashtable();
			database_ = database;
			keyChain_ = keyChain;
			face_ = face;
			groupName_ = new Name(groupName);
			consumerName_ = new Name(consumerName);
			// Copy the Link object.
			cKeyLink_ = new Link(cKeyLink);
			dKeyLink_ = new Link(dKeyLink);
		}
	
		/// <summary>
		/// Create a Consumer to use the given ConsumerDb, Face and other values.
		/// </summary>
		///
		/// <param name="face">The face used for data packet and key fetching.</param>
		/// <param name="keyChain">The keyChain used to verify data packets.</param>
		/// <param name="groupName"></param>
		/// <param name="consumerName"></param>
		/// <param name="database">The ConsumerDb database for storing decryption keys.</param>
		public Consumer(Face face, KeyChain keyChain, Name groupName,
				Name consumerName, ConsumerDb database) {
			this.cKeyMap_ = new Hashtable();
					this.dKeyMap_ = new Hashtable();
			database_ = database;
			keyChain_ = keyChain;
			face_ = face;
			groupName_ = new Name(groupName);
			consumerName_ = new Name(consumerName);
			cKeyLink_ = NO_LINK;
			dKeyLink_ = NO_LINK;
		}
	
		public sealed class Anonymous_C6 : OnVerified {
				public sealed class Anonymous_C10 : Consumer.OnPlainText {
								private readonly Consumer.Anonymous_C6  outer_Anonymous_C6;
								private readonly Data validData;
					
								public Anonymous_C10(Consumer.Anonymous_C6  paramouter_Anonymous_C6,
										Data validData_0) {
									this.validData = validData_0;
									this.outer_Anonymous_C6 = paramouter_Anonymous_C6;
								}
					
								public void onPlainText(Blob plainText) {
									try {
										outer_Anonymous_C6.onConsumeComplete.onConsumeComplete(validData,
												plainText);
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
												"Error in onConsumeComplete", ex);
									}
								}
							}
		
				private readonly Consumer outer_Consumer;
				internal readonly Consumer.OnConsumeComplete  onConsumeComplete;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
		
				public Anonymous_C6(Consumer paramouter_Consumer,
						Consumer.OnConsumeComplete  onConsumeComplete_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
					this.onConsumeComplete = onConsumeComplete_0;
					this.onError = onError_1;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onVerified(Data validData_0) {
					// Decrypt the content.
					outer_Consumer.decryptContent(validData_0, new net.named_data.jndn.encrypt.Consumer.Anonymous_C6.Anonymous_C10 (this, validData_0), onError);
				}
			}
	
		public sealed class Anonymous_C5 : OnVerified {
				public sealed class Anonymous_C9 : Consumer.OnPlainText {
								private readonly Consumer.Anonymous_C5  outer_Anonymous_C5;
					
								public Anonymous_C9(Consumer.Anonymous_C5  paramouter_Anonymous_C5) {
									this.outer_Anonymous_C5 = paramouter_Anonymous_C5;
								}
					
								public void onPlainText(Blob cKeyBits) {
									// cKeyName is already a copy inside the local dataEncryptedContent.
									ILOG.J2CsMapping.Collections.Collections.Put(outer_Anonymous_C5.outer_Consumer.cKeyMap_,outer_Anonymous_C5.cKeyName,cKeyBits);
									Consumer.decrypt(outer_Anonymous_C5.dataEncryptedContent, cKeyBits,
											outer_Anonymous_C5.onPlainText, outer_Anonymous_C5.onError);
								}
							}
		
				internal readonly Consumer outer_Consumer;
				internal readonly Name cKeyName;
				internal readonly EncryptedContent dataEncryptedContent;
				internal readonly Consumer.OnPlainText  onPlainText;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
		
				public Anonymous_C5(Consumer paramouter_Consumer, Name cKeyName_0,
						EncryptedContent dataEncryptedContent_1, Consumer.OnPlainText  onPlainText_2,
						net.named_data.jndn.encrypt.EncryptError.OnError  onError_3) {
					this.cKeyName = cKeyName_0;
					this.dataEncryptedContent = dataEncryptedContent_1;
					this.onPlainText = onPlainText_2;
					this.onError = onError_3;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onVerified(Data validCKeyData) {
					outer_Consumer.decryptCKey(validCKeyData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C5.Anonymous_C9 (this), onError);
				}
			}
	
		public sealed class Anonymous_C4 : OnVerified {
				public sealed class Anonymous_C8 : Consumer.OnPlainText {
								private readonly Consumer.Anonymous_C4  outer_Anonymous_C4;
					
								public Anonymous_C8(Consumer.Anonymous_C4  paramouter_Anonymous_C4) {
									this.outer_Anonymous_C4 = paramouter_Anonymous_C4;
								}
					
								public void onPlainText(Blob dKeyBits) {
									// dKeyName is already a local copy.
									ILOG.J2CsMapping.Collections.Collections.Put(outer_Anonymous_C4.outer_Consumer.dKeyMap_,outer_Anonymous_C4.dKeyName,dKeyBits);
									Consumer.decrypt(outer_Anonymous_C4.cKeyEncryptedContent, dKeyBits,
											outer_Anonymous_C4.onPlainText, outer_Anonymous_C4.onError);
								}
							}
		
				internal readonly Consumer outer_Consumer;
				internal readonly EncryptedContent cKeyEncryptedContent;
				internal readonly Name dKeyName;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				internal readonly Consumer.OnPlainText  onPlainText;
		
				public Anonymous_C4(Consumer paramouter_Consumer,
						EncryptedContent cKeyEncryptedContent_0, Name dKeyName_1,
						net.named_data.jndn.encrypt.EncryptError.OnError  onError_2, Consumer.OnPlainText  onPlainText_3) {
					this.cKeyEncryptedContent = cKeyEncryptedContent_0;
					this.dKeyName = dKeyName_1;
					this.onError = onError_2;
					this.onPlainText = onPlainText_3;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onVerified(Data validDKeyData) {
					outer_Consumer.decryptDKey(validDKeyData, new net.named_data.jndn.encrypt.Consumer.Anonymous_C4.Anonymous_C8 (this), onError);
				}
			}
	
		public sealed class Anonymous_C3 : Consumer.OnPlainText {
			private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
			private readonly Consumer.OnPlainText  callerOnPlainText;
			private readonly Blob encryptedPayloadBlob;
	
			public Anonymous_C3(net.named_data.jndn.encrypt.EncryptError.OnError  onError_0, Consumer.OnPlainText  callerOnPlainText_1,
					Blob encryptedPayloadBlob_2) {
				this.onError = onError_0;
				this.callerOnPlainText = callerOnPlainText_1;
				this.encryptedPayloadBlob = encryptedPayloadBlob_2;
			}
	
			public void onPlainText(Blob nonceKeyBits) {
				decrypt(encryptedPayloadBlob, nonceKeyBits, callerOnPlainText,
						onError);
			}
		}
	
		public sealed class Anonymous_C2 : OnData {
				public sealed class Anonymous_C7 : OnDataValidationFailed {
								private readonly Consumer.Anonymous_C2  outer_Anonymous_C2;
					
								public Anonymous_C7(Consumer.Anonymous_C2  paramouter_Anonymous_C2) {
									this.outer_Anonymous_C2 = paramouter_Anonymous_C2;
								}
					
								public void onDataValidationFailed(Data d,
										String reason) {
									try {
										outer_Anonymous_C2.onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.Validation,
												"verifyData failed. Reason: "
														+ reason);
									} catch (Exception ex) {
										net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE,
												"Error in onError", ex);
									}
								}
							}
		
				private readonly Consumer outer_Consumer;
				private readonly OnVerified onVerified;
				internal readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
		
				public Anonymous_C2(Consumer paramouter_Consumer,
						OnVerified onVerified_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
					this.onVerified = onVerified_0;
					this.onError = onError_1;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onData(Interest contentInterest, Data contentData) {
					// The Interest has no selectors, so assume the library correctly
					// matched with the Data name before calling onData.
		
					try {
						outer_Consumer.keyChain_.verifyData(contentData, onVerified,
								new net.named_data.jndn.encrypt.Consumer.Anonymous_C2.Anonymous_C7 (this));
					} catch (SecurityException ex) {
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.SecurityException,
									"verifyData error: " + ex.Message);
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
						}
					}
				}
			}
	
		public sealed class Anonymous_C1 : OnNetworkNack {
			private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
	
			public Anonymous_C1(net.named_data.jndn.encrypt.EncryptError.OnError  onError_0) {
				this.onError = onError_0;
			}
	
			public void onNetworkNack(Interest interest, NetworkNack networkNack) {
				// We have run out of options. Report a retrieval failure.
				try {
					onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.DataRetrievalFailure, interest
							.getName().toUri());
				} catch (Exception exception) {
					net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
			}
		}
	
		public sealed class Anonymous_C0 : OnTimeout {
				private readonly Consumer outer_Consumer;
				private readonly OnVerified onVerified;
				private readonly net.named_data.jndn.encrypt.EncryptError.OnError  onError;
				private readonly Link link;
				private readonly int nRetrials;
		
				public Anonymous_C0(Consumer paramouter_Consumer,
						OnVerified onVerified_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1, Link link_2, int nRetrials_3) {
					this.onVerified = onVerified_0;
					this.onError = onError_1;
					this.link = link_2;
					this.nRetrials = nRetrials_3;
					this.outer_Consumer = paramouter_Consumer;
				}
		
				public void onTimeout(Interest interest) {
					if (nRetrials > 0)
						outer_Consumer.sendInterest(interest, nRetrials - 1, link, onVerified,
								onError);
					else {
						// We have run out of options. Report a retrieval failure.
						try {
							onError.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.DataRetrievalFailure,
									interest.getName().toUri());
						} catch (Exception exception) {
							net.named_data.jndn.encrypt.Consumer.logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
						}
					}
				}
			}
	
		public interface OnConsumeComplete {
			void onConsumeComplete(Data data, Blob result);
		}
	
		/// <summary>
		/// Express an Interest to fetch the content packet with contentName, and
		/// decrypt it, fetching keys as needed.
		/// </summary>
		///
		/// <param name="contentName">The name of the content packet.</param>
		/// <param name="onConsumeComplete_0">contentData is the fetched Data packet and result is the decrypted plain text Blob. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onError_1">better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="link_2">getDelegations().size() is zero, don't use it.</param>
		public void consume(Name contentName,
				Consumer.OnConsumeComplete  onConsumeComplete_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1,
				Link link_2) {
			Interest interest = new Interest(contentName);
			// Copy the Link object since the passed link may become invalid.
			sendInterest(interest, 1, new Link(link_2), new Consumer.Anonymous_C6 (this, onConsumeComplete_0, onError_1), onError_1);
		}
	
		/// <summary>
		/// Express an Interest to fetch the content packet with contentName, and
		/// decrypt it, fetching keys as needed.
		/// </summary>
		///
		/// <param name="contentName">The name of the content packet.</param>
		/// <param name="onConsumeComplete_0">contentData is the fetched Data packet and result is the decrypted plain text Blob. NOTE: The library will log any exceptions thrown by this callback, but for better error handling the callback should catch and properly handle any exceptions.</param>
		/// <param name="onError_1">better error handling the callback should catch and properly handle any exceptions.</param>
		public void consume(Name contentName,
				Consumer.OnConsumeComplete  onConsumeComplete_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			consume(contentName, onConsumeComplete_0, onError_1, NO_LINK);
		}
	
		/// <summary>
		/// Set the group name.
		/// </summary>
		///
		/// <param name="groupName"></param>
		public void setGroup(Name groupName) {
			groupName_ = new Name(groupName);
		}
	
		/// <summary>
		/// Add a new decryption key with keyName and keyBlob to the database.
		/// </summary>
		///
		/// <param name="keyName">The key name.</param>
		/// <param name="keyBlob">The encoded key.</param>
		/// <exception cref="ConsumerDb.Error">if a key with the same keyName already exists inthe database, or other database error.</exception>
		/// <exception cref="System.Exception">if the consumer name is not a prefix of the key name.</exception>
		public void addDecryptionKey(Name keyName, Blob keyBlob) {
			if (!(consumerName_.match(keyName)))
				throw new Exception(
						"addDecryptionKey: The consumer name must be a prefix of the key name");
	
			database_.addKey(keyName, keyBlob);
		}
	
		public interface OnPlainText {
			void onPlainText(Blob plainText);
		}
	
		/// <summary>
		/// Decode encryptedBlob as an EncryptedContent and decrypt using keyBits.
		/// </summary>
		///
		/// <param name="encryptedBlob">The encoded EncryptedContent to decrypt.</param>
		/// <param name="keyBits">The key value.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		private static void decrypt(Blob encryptedBlob, Blob keyBits,
				Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			EncryptedContent encryptedContent = new EncryptedContent();
			try {
				encryptedContent.wireDecode(encryptedBlob);
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
	
			decrypt(encryptedContent, keyBits, onPlainText_0, onError_1);
		}
	
		/// <summary>
		/// Decrypt encryptedContent using keyBits.
		/// </summary>
		///
		/// <param name="encryptedContent">The EncryptedContent to decrypt.</param>
		/// <param name="keyBits">The key value.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		static internal void decrypt(EncryptedContent encryptedContent,
				Blob keyBits, Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			Blob payload = encryptedContent.getPayload();
	
			if (encryptedContent.getAlgorithmType() == net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc) {
				// Prepare the parameters.
				EncryptParams decryptParams = new EncryptParams(
						net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc);
				decryptParams.setInitialVector(encryptedContent.getInitialVector());
	
				// Decrypt the content.
				Blob content;
				try {
					content = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(keyBits, payload, decryptParams);
				} catch (Exception ex) {
					try {
						onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
								ex.Message);
					} catch (Exception exception) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
					}
					return;
				}
				try {
					onPlainText_0.onPlainText(content);
				} catch (Exception ex_2) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onPlainText", ex_2);
				}
			} else if (encryptedContent.getAlgorithmType() == net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep) {
				// Prepare the parameters.
				EncryptParams decryptParams_3 = new EncryptParams(
						net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep);
	
				// Decrypt the content.
				Blob content_4;
				try {
					content_4 = net.named_data.jndn.encrypt.algo.RsaAlgorithm.decrypt(keyBits, payload, decryptParams_3);
				} catch (Exception ex_5) {
					try {
						onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
								ex_5.Message);
					} catch (Exception exception_6) {
						logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_6);
					}
					return;
				}
				try {
					onPlainText_0.onPlainText(content_4);
				} catch (Exception ex_7) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onPlainText", ex_7);
				}
			} else {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.UnsupportedEncryptionScheme, ""
							+ encryptedContent.getAlgorithmType());
				} catch (Exception ex_8) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", ex_8);
				}
			}
		}
	
		/// <summary>
		/// Decrypt the data packet.
		/// </summary>
		///
		/// <param name="data">The data packet. This does not verify the packet.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		internal void decryptContent(Data data, Consumer.OnPlainText  onPlainText_0,
				net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			// Get the encrypted content.
			EncryptedContent dataEncryptedContent_2 = new EncryptedContent();
			try {
				dataEncryptedContent_2.wireDecode(data.getContent());
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
			Name cKeyName_3 = dataEncryptedContent_2.getKeyLocator().getKeyName();
	
			// Check if the content key is already in the store.
			Blob cKey = (Blob) ILOG.J2CsMapping.Collections.Collections.Get(cKeyMap_,cKeyName_3);
			if (cKey != null)
				decrypt(dataEncryptedContent_2, cKey, onPlainText_0, onError_1);
			else {
				// Retrieve the C-KEY Data from the network.
				Name interestName = new Name(cKeyName_3);
				interestName.append(net.named_data.jndn.encrypt.algo.Encryptor.NAME_COMPONENT_FOR)
						.append(groupName_);
				Interest interest = new Interest(interestName);
				sendInterest(interest, 1, cKeyLink_, new Consumer.Anonymous_C5 (this, cKeyName_3, dataEncryptedContent_2, onPlainText_0,
						onError_1), onError_1);
			}
		}
	
		/// <summary>
		/// Decrypt cKeyData.
		/// </summary>
		///
		/// <param name="cKeyData">The C-KEY data packet.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		internal void decryptCKey(Data cKeyData, Consumer.OnPlainText  onPlainText_0,
				net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			// Get the encrypted content.
			Blob cKeyContent = cKeyData.getContent();
			EncryptedContent cKeyEncryptedContent_2 = new EncryptedContent();
			try {
				cKeyEncryptedContent_2.wireDecode(cKeyContent);
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
			Name eKeyName = cKeyEncryptedContent_2.getKeyLocator().getKeyName();
			Name dKeyName_3 = eKeyName.getPrefix(-3);
			dKeyName_3.append(net.named_data.jndn.encrypt.algo.Encryptor.NAME_COMPONENT_D_KEY).append(
					eKeyName.getSubName(-2));
	
			// Check if the decryption key is already in the store.
			Blob dKey = (Blob) ILOG.J2CsMapping.Collections.Collections.Get(dKeyMap_,dKeyName_3);
			if (dKey != null)
				decrypt(cKeyEncryptedContent_2, dKey, onPlainText_0, onError_1);
			else {
				// Get the D-Key Data.
				Name interestName = new Name(dKeyName_3);
				interestName.append(net.named_data.jndn.encrypt.algo.Encryptor.NAME_COMPONENT_FOR).append(
						consumerName_);
				Interest interest = new Interest(interestName);
				sendInterest(interest, 1, dKeyLink_, new Consumer.Anonymous_C4 (this, cKeyEncryptedContent_2, dKeyName_3, onError_1,
						onPlainText_0), onError_1);
			}
		}
	
		/// <summary>
		/// Decrypt dKeyData.
		/// </summary>
		///
		/// <param name="dKeyData">The D-KEY data packet.</param>
		/// <param name="onPlainText_0"></param>
		/// <param name="onError_1">This calls onError.onError(errorCode, message) for an error.</param>
		internal void decryptDKey(Data dKeyData, Consumer.OnPlainText  onPlainText_0,
				net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
			// Get the encrypted content.
			Blob dataContent = dKeyData.getContent();
	
			// Process the nonce.
			// dataContent is a sequence of the two EncryptedContent.
			EncryptedContent encryptedNonce = new EncryptedContent();
			try {
				encryptedNonce.wireDecode(dataContent);
			} catch (EncodingException ex) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
				return;
			}
			Name consumerKeyName = encryptedNonce.getKeyLocator().getKeyName();
	
			// Get consumer decryption key.
			Blob consumerKeyBlob;
			try {
				consumerKeyBlob = getDecryptionKey(consumerKeyName);
			} catch (ConsumerDb.Error ex_2) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.NoDecryptKey,
							"Database error: " + ex_2.Message);
				} catch (Exception exception_3) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_3);
				}
				return;
			}
			if (consumerKeyBlob.size() == 0) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.NoDecryptKey,
							"The desired consumer decryption key in not in the database");
				} catch (Exception exception_4) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception_4);
				}
				return;
			}
	
			// Process the D-KEY.
			// Use the size of encryptedNonce to find the start of encryptedPayload.
			ByteBuffer encryptedPayloadBuffer = dataContent.buf().duplicate();
			encryptedPayloadBuffer.position(encryptedNonce.wireEncode().size());
			Blob encryptedPayloadBlob_5 = new Blob(encryptedPayloadBuffer,
					false);
			if (encryptedPayloadBlob_5.size() == 0) {
				try {
					onError_1.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.InvalidEncryptedFormat,
							"The data packet does not satisfy the D-KEY packet format");
				} catch (Exception ex_6) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", ex_6);
				}
				return;
			}
	
			// Decrypt the D-KEY.
			Consumer.OnPlainText  callerOnPlainText_7 = onPlainText_0;
			decrypt(encryptedNonce, consumerKeyBlob, new Consumer.Anonymous_C3 (onError_1, callerOnPlainText_7, encryptedPayloadBlob_5), onError_1);
		}
	
		/// <summary>
		/// Express the interest, call verifyData for the fetched Data packet and call
		/// onVerified if verify succeeds. If verify fails, call
		/// onError.onError(ErrorCode.Validation, "verifyData failed"). If the interest
		/// times out, re-express nRetrials times. If the interest times out nRetrials
		/// times, or for a network Nack, call
		/// onError.onError(ErrorCode.DataRetrievalFailure, interest.getName().toUri()).
		/// </summary>
		///
		/// <param name="interest">The Interest to express.</param>
		/// <param name="nRetrials_0">The number of retrials left after a timeout.</param>
		/// <param name="link_1">zero, don't use it.</param>
		/// <param name="onVerified_2"></param>
		/// <param name="onError_3">This calls onError.onError(errorCode, message) for an error.</param>
		internal void sendInterest(Interest interest, int nRetrials_0,
				Link link_1, OnVerified onVerified_2, net.named_data.jndn.encrypt.EncryptError.OnError  onError_3) {
			// Prepare the callback functions.
			OnData onData = new Consumer.Anonymous_C2 (this, onVerified_2, onError_3);
	
			OnNetworkNack onNetworkNack = new Consumer.Anonymous_C1 (onError_3);
	
			OnTimeout onTimeout = new Consumer.Anonymous_C0 (this, onVerified_2, onError_3, link_1, nRetrials_0);
	
			Interest request;
			if (link_1.getDelegations().size() == 0)
				// We can use the supplied interest without copying.
				request = interest;
			else {
				// Copy the supplied interest and add the Link.
				request = new Interest(interest);
				// This will use a cached encoding if available.
				request.setLinkWireEncoding(link_1.wireEncode());
			}
	
			try {
				face_.expressInterest(request, onData, onTimeout, onNetworkNack);
			} catch (IOException ex) {
				try {
					onError_3.onError(net.named_data.jndn.encrypt.EncryptError.ErrorCode.IOException,
							"expressInterest error: " + ex.Message);
				} catch (Exception exception) {
					logger_.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, "Error in onError", exception);
				}
			}
		}
	
		/// <summary>
		/// Get the encoded blob of the decryption key with decryptionKeyName from the
		/// database.
		/// </summary>
		///
		/// <param name="decryptionKeyName">The key name.</param>
		/// <returns>A Blob with the encoded key, or an isNull Blob if cannot find the
		/// key with decryptionKeyName.</returns>
		/// <exception cref="ConsumerDb.Error">for a database error.</exception>
		private Blob getDecryptionKey(Name decryptionKeyName) {
			return database_.getKey(decryptionKeyName);
		}
	
		/// <summary>
		/// A class implements Friend if it has a method setConsumerFriendAccess
		/// which setFriendAccess calls to set the FriendAccess object.
		/// </summary>
		///
		public interface Friend {
			void setConsumerFriendAccess(Consumer.FriendAccess  friendAccess);
		}
	
		/// <summary>
		/// Call friend.setConsumerFriendAccess to pass an instance of
		/// a FriendAccess class to allow a friend class to call private methods.
		/// </summary>
		///
		/// <param name="friend">Therefore, only a friend class gets an implementation of FriendAccess.</param>
		public static void setFriendAccess(Consumer.Friend  friend) {
			if (friend
							.GetType().FullName
					.endsWith(
							"net.named_data.jndn.tests.integration_tests.TestGroupConsumer")) {
				friend.setConsumerFriendAccess(new Consumer.FriendAccessImpl ());
			}
		}
	
		/// <summary>
		/// A friend class can call the methods of FriendAccess to access private
		/// methods.  This abstract class is public, but setFriendAccess passes an
		/// instance of a private class which implements the methods.
		/// </summary>
		///
		public abstract class FriendAccess {
			public abstract void decrypt(Blob encryptedBlob, Blob keyBits,
					Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1);
		}
	
		/// <summary>
		/// setFriendAccess passes an instance of this private class which implements
		/// the FriendAccess methods.
		/// </summary>
		///
		private class FriendAccessImpl : Consumer.FriendAccess  {
			public override void decrypt(Blob encryptedBlob, Blob keyBits,
					Consumer.OnPlainText  onPlainText_0, net.named_data.jndn.encrypt.EncryptError.OnError  onError_1) {
				net.named_data.jndn.encrypt.Consumer.decrypt(encryptedBlob, keyBits, onPlainText_0, onError_1);
			}
		}
	
		private readonly ConsumerDb database_;
		internal readonly KeyChain keyChain_;
		private readonly Face face_;
		private Name groupName_;
		private readonly Name consumerName_;
		private readonly Link cKeyLink_;
		// Use HashMap without generics so it works with older Java compilers.
		internal readonly Hashtable cKeyMap_;
		/// <summary>
		/// < The map key is the C-KEY name. The value is the encoded key Blob. 
		/// </summary>
		///
		private readonly Link dKeyLink_;
		internal readonly Hashtable dKeyMap_;
		/// <summary>
		/// < The map key is the D-KEY name. The value is the encoded key Blob. 
		/// </summary>
		///
		private static readonly Link NO_LINK = new Link();
		static internal readonly Logger logger_ = ILOG.J2CsMapping.Util.Logging.Logger.getLogger(typeof(Consumer).FullName);
	}
}
