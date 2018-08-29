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
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encrypt.algo;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// An EncryptedContent holds an encryption type, a payload and other fields
	/// representing encrypted content.
	/// </summary>
	///
	/// @note This class is an experimental feature. The API may change.
	public class EncryptedContent {
		/// <summary>
		/// Create an EncryptedContent where all the values are unspecified.
		/// </summary>
		///
		public EncryptedContent() {
			this.algorithmType_ = net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.NONE;
			this.keyLocator_ = new KeyLocator();
			this.initialVector_ = new Blob();
			this.payload_ = new Blob();
			this.payloadKey_ = new Blob();
		}
	
		/// <summary>
		/// Create an EncryptedContent as a deep copy of the given object.
		/// </summary>
		///
		/// <param name="encryptedContent">The other encryptedContent to copy.</param>
		public EncryptedContent(EncryptedContent encryptedContent) {
			this.algorithmType_ = net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.NONE;
			this.keyLocator_ = new KeyLocator();
			this.initialVector_ = new Blob();
			this.payload_ = new Blob();
			this.payloadKey_ = new Blob();
			algorithmType_ = encryptedContent.algorithmType_;
			keyLocator_ = new KeyLocator(encryptedContent.keyLocator_);
			initialVector_ = encryptedContent.initialVector_;
			payload_ = encryptedContent.payload_;
		}
	
		/// <summary>
		/// Get the algorithm type.
		/// </summary>
		///
		/// <returns>The algorithm type. If not specified, return
		/// EncryptAlgorithmType.NONE.</returns>
		public EncryptAlgorithmType getAlgorithmType() {
			return algorithmType_;
		}
	
		/// <summary>
		/// Get the key locator.
		/// </summary>
		///
		/// <returns>The key locator. If not specified, getType() is KeyLocatorType.NONE.</returns>
		public KeyLocator getKeyLocator() {
			return keyLocator_;
		}
	
		/// <summary>
		/// Check that the key locator type is KEYNAME and return the key Name.
		/// </summary>
		///
		/// <returns>The key Name.</returns>
		/// <exception cref="System.Exception">if the key locator type is not KEYNAME.</exception>
		public Name getKeyLocatorName() {
			if (keyLocator_.getType() != net.named_data.jndn.KeyLocatorType.KEYNAME)
				throw new Exception(
						"getKeyLocatorName: The KeyLocator type must be KEYNAME");
	
			return keyLocator_.getKeyName();
		}
	
		/// <summary>
		/// Check if the initial vector is specified.
		/// </summary>
		///
		/// <returns>True if the initial vector is specified.</returns>
		public bool hasInitialVector() {
			return !initialVector_.isNull();
		}
	
		/// <summary>
		/// Get the initial vector.
		/// </summary>
		///
		/// <returns>The initial vector. If not specified, isNull() is true.</returns>
		public Blob getInitialVector() {
			return initialVector_;
		}
	
		/// <summary>
		/// Get the payload.
		/// </summary>
		///
		/// <returns>The payload. If not specified, isNull() is true.</returns>
		public Blob getPayload() {
			return payload_;
		}
	
		/// <summary>
		/// Get the encrypted payload key.
		/// </summary>
		///
		/// <returns>The encrypted payload key. If not specified, isNull() is true.</returns>
		public Blob getPayloadKey() {
			return payloadKey_;
		}
	
		/// <summary>
		/// Set the algorithm type.
		/// </summary>
		///
		/// <param name="algorithmType"></param>
		/// <returns>This EncryptedContent so that you can chain calls to update values.</returns>
		public EncryptedContent setAlgorithmType(
				EncryptAlgorithmType algorithmType) {
			algorithmType_ = algorithmType;
			return this;
		}
	
		/// <summary>
		/// Set the key locator.
		/// </summary>
		///
		/// <param name="keyLocator"></param>
		/// <returns>This EncryptedContent so that you can chain calls to update values.</returns>
		public EncryptedContent setKeyLocator(KeyLocator keyLocator) {
			keyLocator_ = (keyLocator == null) ? new KeyLocator() : new KeyLocator(
					keyLocator);
			return this;
		}
	
		/// <summary>
		/// Set the key locator type to KeyLocatorType.KEYNAME and set the key Name.
		/// </summary>
		///
		/// <param name="keyName">The key locator Name, which is copied.</param>
		/// <returns>This EncryptedContent so that you can chain calls to update values.</returns>
		public EncryptedContent setKeyLocatorName(Name keyName) {
			keyLocator_.setType(net.named_data.jndn.KeyLocatorType.KEYNAME);
			keyLocator_.setKeyName(keyName);
			return this;
		}
	
		/// <summary>
		/// Set the initial vector.
		/// </summary>
		///
		/// <param name="initialVector"></param>
		/// <returns>This EncryptedContent so that you can chain calls to update values.</returns>
		public EncryptedContent setInitialVector(Blob initialVector) {
			initialVector_ = ((initialVector == null) ? new Blob() : initialVector);
			return this;
		}
	
		/// <summary>
		/// Set the encrypted payload.
		/// </summary>
		///
		/// <param name="payload"></param>
		/// <returns>This EncryptedContent so that you can chain calls to update values.</returns>
		public EncryptedContent setPayload(Blob payload) {
			payload_ = ((payload == null) ? new Blob() : payload);
			return this;
		}
	
		/// <summary>
		/// Set the encrypted payload key.
		/// </summary>
		///
		/// <param name="payloadKey"></param>
		/// <returns>This EncryptedContent so that you can chain calls to update values.</returns>
		public EncryptedContent setPayloadKey(Blob payloadKey) {
			payloadKey_ = ((payloadKey == null) ? new Blob() : payloadKey);
			return this;
		}
	
		/// <summary>
		/// Set all the fields to indicate unspecified values.
		/// </summary>
		///
		public void clear() {
			algorithmType_ = net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.NONE;
			keyLocator_ = new KeyLocator();
			initialVector_ = new Blob();
			payload_ = new Blob();
			payloadKey_ = new Blob();
		}
	
		/// <summary>
		/// Encode this to an EncryptedContent v1 wire encoding.
		/// </summary>
		///
		/// <param name="wireFormat">A WireFormat object used to encode this EncryptedContent.</param>
		/// <returns>The encoded byte array as a Blob.</returns>
		public Blob wireEncode(WireFormat wireFormat) {
			return wireFormat.encodeEncryptedContent(this);
		}
	
		/// <summary>
		/// Encode this to an EncryptedContent v1 for the default wire format.
		/// </summary>
		///
		/// <returns>The encoded byte array as a Blob.</returns>
		public Blob wireEncode() {
			return wireEncode(net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Encode this to an EncryptedContent v2 wire encoding.
		/// </summary>
		///
		/// <param name="wireFormat">A WireFormat object used to encode this EncryptedContent.</param>
		/// <returns>The encoded byte array as a Blob.</returns>
		public Blob wireEncodeV2(WireFormat wireFormat) {
			return wireFormat.encodeEncryptedContentV2(this);
		}
	
		/// <summary>
		/// Encode this to an EncryptedContent v2 for the default wire format.
		/// </summary>
		///
		/// <returns>The encoded byte array as a Blob.</returns>
		public Blob wireEncodeV2() {
			return wireEncodeV2(net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v1 using a particular wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(ByteBuffer input, WireFormat wireFormat) {
			wireFormat.decodeEncryptedContent(this, input, true);
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v1 using the default wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(ByteBuffer input) {
			wireDecode(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v1 using a particular wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input">The input blob to decode.</param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(Blob input, WireFormat wireFormat) {
			wireFormat.decodeEncryptedContent(this, input.buf(), false);
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v1 using the default wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input">The input blob to decode.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecode(Blob input) {
			wireDecode(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v2 using a particular wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecodeV2(ByteBuffer input, WireFormat wireFormat) {
			wireFormat.decodeEncryptedContentV2(this, input, true);
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v2 using the default wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input"></param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecodeV2(ByteBuffer input) {
			wireDecodeV2(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v2 using a particular wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input">The input blob to decode.</param>
		/// <param name="wireFormat">A WireFormat object used to decode the input.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecodeV2(Blob input, WireFormat wireFormat) {
			wireFormat.decodeEncryptedContentV2(this, input.buf(), false);
		}
	
		/// <summary>
		/// Decode the input as an EncryptedContent v2 using the default wire format
		/// and update this EncryptedContent.
		/// </summary>
		///
		/// <param name="input">The input blob to decode.</param>
		/// <exception cref="EncodingException">For invalid encoding.</exception>
		public void wireDecodeV2(Blob input) {
			wireDecodeV2(input, net.named_data.jndn.encoding.WireFormat.getDefaultWireFormat());
		}
	
		private EncryptAlgorithmType algorithmType_;
		private KeyLocator keyLocator_;
		private Blob initialVector_;
		private Blob payload_;
		private Blob payloadKey_;
	}
}
