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
namespace net.named_data.jndn.encoding.tlv {
	
	using ILOG.J2CsMapping.NIO;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.util;
	
	/// <summary>
	/// A TlvDecoder has methods to decode an input according to NDN-TLV.
	/// </summary>
	///
	public class TlvDecoder {
		/// <summary>
		/// Create a new TlvDecoder to decode the input.
		/// </summary>
		///
		/// <param name="input">the underlying buffer whose contents must remain valid during the life of this object.</param>
		public TlvDecoder(ByteBuffer input) {
			input_ = input.duplicate();
		}
	
		/// <summary>
		/// Decode a VAR-NUMBER in NDN-TLV and return it. Update the input buffer
		/// position.
		/// </summary>
		///
		/// <returns>The decoded VAR-NUMBER as a Java 32-bit int.</returns>
		/// <exception cref="EncodingException">if the VAR-NUMBER is 64-bit or read past the endof the input.</exception>
		public int readVarNumber() {
			try {
				int firstOctet = (int) input_.get() & 0xff;
				if (firstOctet < 253)
					return firstOctet;
				else
					return readExtendedVarNumber(firstOctet);
			} catch (BufferUnderflowException ex) {
				throw new EncodingException("Read past the end of the input");
			}
		}
	
		/// <summary>
		/// Do the work of readVarNumber, given the firstOctet which is >= 253. Update
		/// the input buffer position.
		/// </summary>
		///
		/// <param name="firstOctet"></param>
		/// <returns>The decoded VAR-NUMBER as a Java 32-bit int.</returns>
		/// <exception cref="EncodingException">if the VAR-NUMBER is 64-bit or read past the end of the input.</exception>
		public int readExtendedVarNumber(int firstOctet) {
			try {
				if (firstOctet == 253)
					return (((int) input_.get() & 0xff) << 8)
							+ ((int) input_.get() & 0xff);
				else if (firstOctet == 254)
					return (((int) input_.get() & 0xff) << 24)
							+ (((int) input_.get() & 0xff) << 16)
							+ (((int) input_.get() & 0xff) << 8)
							+ ((int) input_.get() & 0xff);
				else
					// we are returning a 32-bit int, so can't handle 64-bit.
					throw new EncodingException(
							"Decoding a 64-bit VAR-NUMBER is not supported");
			} catch (BufferUnderflowException ex) {
				throw new EncodingException("Read past the end of the input");
			}
		}
	
		/// <summary>
		/// Decode the type and length from this's input starting at the input buffer
		/// position, expecting the type to be expectedType and return the length.
		/// Update the input buffer position. Also make sure the decoded length does
		/// not exceed the number of bytes remaining in the input.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <returns>The length of the TLV as a 32-bit Java int.</returns>
		/// <exception cref="EncodingException">if did not get the expected TLV type, or the TLVlength exceeds the buffer length, or the type is encoded as a 64-bit value,or the length is encoded as a 64-bit value.</exception>
		public int readTypeAndLength(int expectedType) {
			int type = readVarNumber();
			if (type != expectedType)
				throw new EncodingException("Did not get the expected TLV type");
	
			int length = readVarNumber();
			if (length > input_.remaining())
				throw new EncodingException("TLV length exceeds the buffer length");
	
			return length;
		}
	
		/// <summary>
		/// Decode the type and length from the input starting at the input buffer
		/// position, expecting the type to be expectedType. Update the input buffer
		/// position. Also make sure the decoded length does not exceed the number of
		/// bytes remaining in the input. Return the input buffer position (offset) of
		/// the end of this parent TLV, which is used in decoding optional nested TLVs.
		/// After reading all nested TLVs, you should call finishNestedTlvs.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <returns>The input buffer position (offset) of the end of the parent TLV.</returns>
		/// <exception cref="EncodingException">if did not get the expected TLV type, or the TLVlength exceeds the buffer length, or the type is encoded as a 64-bit value,or the length is encoded as a 64-bit value.</exception>
		public int readNestedTlvsStart(int expectedType) {
			return readTypeAndLength(expectedType) + input_.position();
		}
	
		/// <summary>
		/// Call this after reading all nested TLVs to skip any remaining unrecognized
		/// TLVs and to check if the input buffer position after the final nested TLV
		/// matches the endOffset returned by readNestedTlvsStart. Update the input
		/// buffer position as needed if skipping TLVs.
		/// </summary>
		///
		/// <param name="endOffset"></param>
		/// <exception cref="EncodingException">if the TLV length does not equal the total lengthof the nested TLVs.</exception>
		public void finishNestedTlvs(int endOffset) {
			// We expect the position to be endOffset, so check this first.
			if (input_.position() == endOffset)
				return;
	
			// Skip remaining TLVs.
			while (input_.position() < endOffset) {
				// Skip the type VAR-NUMBER.
				readVarNumber();
				// Read the length and update the position.
				int length = readVarNumber();
				int newPosition = input_.position() + length;
				// Check newPosition before updating input_position since it would
				//   throw its own exception.
				if (newPosition > input_.limit())
					throw new EncodingException(
							"TLV length exceeds the buffer length");
				input_.position(newPosition);
			}
	
			if (input_.position() != endOffset)
				throw new EncodingException(
						"TLV length does not equal the total length of the nested TLVs");
		}
	
		/// <summary>
		/// Decode the type from the input starting at the input buffer position, and
		/// if it is the expectedType, then return true, else false.  However, if the
		/// input buffer position is greater than or equal to endOffset, then return
		/// false and don't try to read the type. Do not update the input buffer
		/// position.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <param name="endOffset"></param>
		/// <returns>true if the type of the next TLV is the expectedType, otherwise
		/// false.</returns>
		public bool peekType(int expectedType, int endOffset) {
			if (input_.position() >= endOffset)
				// No more sub TLVs to look at.
				return false;
			else {
				int savePosition = input_.position();
				int type = readVarNumber();
				// Restore the position.
				input_.position(savePosition);
	
				return type == expectedType;
			}
		}
	
		/// <summary>
		/// Decode a non-negative integer in NDN-TLV and return it. Update the input
		/// buffer position by length.
		/// </summary>
		///
		/// <param name="length">The number of bytes in the encoded integer.</param>
		/// <returns>The integer as a Java 64-bit long.</returns>
		/// <exception cref="EncodingException">if length is an invalid length for a TLVnon-negative integer or read past the end of the input.</exception>
		public long readNonNegativeInteger(int length) {
			try {
				if (length == 1)
					return (long) input_.get() & 0xff;
				else if (length == 2)
					return (((long) input_.get() & 0xff) << 8)
							+ ((long) input_.get() & 0xff);
				else if (length == 4)
					return (((long) input_.get() & 0xff) << 24)
							+ (((long) input_.get() & 0xff) << 16)
							+ (((long) input_.get() & 0xff) << 8)
							+ ((long) input_.get() & 0xff);
				else if (length == 8)
					return (((long) input_.get() & 0xff) << 56)
							+ (((long) input_.get() & 0xff) << 48)
							+ (((long) input_.get() & 0xff) << 40)
							+ (((long) input_.get() & 0xff) << 32)
							+ (((long) input_.get() & 0xff) << 24)
							+ (((long) input_.get() & 0xff) << 16)
							+ (((long) input_.get() & 0xff) << 8)
							+ ((long) input_.get() & 0xff);
				else
					throw new EncodingException(
							"Invalid length for a TLV nonNegativeInteger");
			} catch (BufferUnderflowException ex) {
				throw new EncodingException("Read past the end of the input");
			}
		}
	
		/// <summary>
		/// Decode the type and length from the input starting at the input buffer
		/// position, expecting the type to be expectedType. Then decode a non-negative
		/// integer in NDN-TLV and return it. Update the input buffer position.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <returns>The integer as a Java 64-bit long.</returns>
		/// <exception cref="EncodingException">if did not get the expected TLV type or can'tdecode the value.</exception>
		public long readNonNegativeIntegerTlv(int expectedType) {
			int length = readTypeAndLength(expectedType);
			return readNonNegativeInteger(length);
		}
	
		/// <summary>
		/// Peek at the next TLV, and if it has the expectedType then call
		/// readNonNegativeIntegerTlv and return the integer.  Otherwise, return -1.
		/// However, if the input buffer position is greater than or equal to
		/// endOffset, then return -1 and don't try to read the type.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <param name="endOffset"></param>
		/// <returns>The integer as a Java 64-bit long or -1 if the next TLV doesn't
		/// have the expected type.</returns>
		public long readOptionalNonNegativeIntegerTlv(int expectedType,
				int endOffset) {
			if (peekType(expectedType, endOffset))
				return readNonNegativeIntegerTlv(expectedType);
			else
				return -1;
		}
	
		/// <summary>
		/// Decode the type and length from the input starting at the input buffer
		/// position, expecting the type to be expectedType. Then return a ByteBuffer
		/// of the bytes in the value. Update the input buffer position.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <returns>The bytes in the value as a slice on the input buffer.  This is
		/// not a copy of the bytes in the input buffer. If you need a copy, then you
		/// must make a copy of the return value.</returns>
		/// <exception cref="EncodingException">if did not get the expected TLV type.</exception>
		public ByteBuffer readBlobTlv(int expectedType) {
			int length = readTypeAndLength(expectedType);
			int saveLimit = input_.limit();
			input_.limit(input_.position() + length);
			ByteBuffer result = input_.slice();
			// Restore the limit.
			input_.limit(saveLimit);
	
			// readTypeAndLength already checked if length exceeds the input buffer.
			input_.position(input_.position() + length);
			return result;
		}
	
		/// <summary>
		/// Peek at the next TLV, and if it has the expectedType then call readBlobTlv
		/// and return the value.  Otherwise, return null. However, if the input buffer
		/// position is greater than or equal to endOffset, then return null and don't
		/// try to read the type.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <param name="endOffset"></param>
		/// <returns>The bytes in the value as a slice on the input buffer or null if
		/// the next TLV doesn't have the expected type. This is not a copy of the
		/// bytes in the input buffer. If you need a copy, then you must make a copy of
		/// the return value.</returns>
		public ByteBuffer readOptionalBlobTlv(int expectedType, int endOffset) {
			if (peekType(expectedType, endOffset))
				return readBlobTlv(expectedType);
			else
				return null;
		}
	
		/// <summary>
		/// Peek at the next TLV, and if it has the expectedType then read a type and
		/// value, ignoring the value, and return true. Otherwise, return false.
		/// However, if the input buffer position is greater than or equal to
		/// endOffset, then return false and don't try to read the type and value.
		/// </summary>
		///
		/// <param name="expectedType">The expected type as a 32-bit Java int.</param>
		/// <param name="endOffset"></param>
		/// <returns>true, or else false if the next TLV doesn't have the
		/// expected type.</returns>
		public bool readBooleanTlv(int expectedType, int endOffset) {
			if (peekType(expectedType, endOffset)) {
				int length = readTypeAndLength(expectedType);
				// We expect the length to be 0, but update offset anyway.
				input_.position(input_.position() + length);
				return true;
			} else
				return false;
		}
	
		/// <summary>
		/// Get the input buffer position (offset), used for the next read.
		/// </summary>
		///
		/// <returns>The input buffer position (offset).</returns>
		public int getOffset() {
			return input_.position();
		}
	
		/// <summary>
		/// Set the offset into the input, used for the next read.
		/// </summary>
		///
		/// <param name="offset">The new offset.</param>
		public void seek(int offset) {
			input_.position(offset);
		}
	
		private readonly ByteBuffer input_;
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
