/**
 * Copyright (C) 2015-2017 Regents of the University of California.
 * @author: Jeff Thompson <jefft0@remap.ucla.edu>
 * @author: From ndn-group-encrypt unit tests
 * https://github.com/named-data/ndn-group-encrypt/blob/master/tests/unit-tests/consumer.t.cpp
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * A copy of the GNU Lesser General Public License is in the file COPYING.
 */

package net.named_data.jndn.tests.integration_tests;

import java.io.File;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.spec.InvalidKeySpecException;
import java.util.logging.Level;
import java.util.logging.Logger;
import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;
import net.named_data.jndn.Data;
import net.named_data.jndn.Face;
import net.named_data.jndn.Interest;
import net.named_data.jndn.Link;
import net.named_data.jndn.Name;
import net.named_data.jndn.OnData;
import net.named_data.jndn.OnNetworkNack;
import net.named_data.jndn.OnTimeout;
import net.named_data.jndn.encoding.EncodingException;
import net.named_data.jndn.encoding.WireFormat;
import net.named_data.jndn.encoding.der.DerDecodingException;
import net.named_data.jndn.encrypt.Consumer;
import net.named_data.jndn.encrypt.ConsumerDb;
import net.named_data.jndn.encrypt.EncryptError;
import net.named_data.jndn.encrypt.Sqlite3ConsumerDb;
import net.named_data.jndn.encrypt.algo.EncryptAlgorithmType;
import net.named_data.jndn.encrypt.algo.EncryptParams;
import net.named_data.jndn.encrypt.algo.Encryptor;
import net.named_data.jndn.encrypt.algo.RsaAlgorithm;
import net.named_data.jndn.security.KeyChain;
import net.named_data.jndn.security.KeyType;
import net.named_data.jndn.security.RsaKeyParams;
import net.named_data.jndn.security.SecurityException;
import net.named_data.jndn.security.identity.IdentityManager;
import net.named_data.jndn.security.identity.MemoryIdentityStorage;
import net.named_data.jndn.security.identity.MemoryPrivateKeyStorage;
import net.named_data.jndn.security.policy.NoVerifyPolicyManager;
import net.named_data.jndn.util.Blob;
import org.junit.After;
import org.junit.Before;
import org.junit.Test;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

// Note: We name this differently than TestConsumer so that it is not the
// first integration test to run, which doesn't work for some reason. (Maybe the
// static class initializers don't run?)
public class TestGroupConsumer implements Consumer.Friend {
  // Convert the int array to a ByteBuffer.
  public static ByteBuffer
  toBuffer(int[] array)
  {
    ByteBuffer result = ByteBuffer.allocate(array.length);
    for (int i = 0; i < array.length; ++i)
      result.put((byte)(array[i] & 0xff));

    result.flip();
    return result;
  }

  private static ByteBuffer DATA_CONTENT = toBuffer(new int[] {
    0xcb, 0xe5, 0x6a, 0x80, 0x41, 0x24, 0x58, 0x23,
    0x84, 0x14, 0x15, 0x61, 0x80, 0xb9, 0x5e, 0xbd,
    0xce, 0x32, 0xb4, 0xbe, 0xbc, 0x91, 0x31, 0xd6,
    0x19, 0x00, 0x80, 0x8b, 0xfa, 0x00, 0x05, 0x9c
  });

  private static ByteBuffer AES_KEY = toBuffer(new int[] {
    0xdd, 0x60, 0x77, 0xec, 0xa9, 0x6b, 0x23, 0x1b,
    0x40, 0x6b, 0x5a, 0xf8, 0x7d, 0x3d, 0x55, 0x32
  });

  private static ByteBuffer INITIAL_VECTOR = toBuffer(new int[] {
    0x73, 0x6f, 0x6d, 0x65, 0x72, 0x61, 0x6e, 0x64,
    0x6f, 0x6d, 0x76, 0x65, 0x63, 0x74, 0x6f, 0x72
  });

  private static ByteBuffer DEFAULT_RSA_PUBLIC_KEY_DER = toBuffer(new int[] {
    0x30, 0x82, 0x01, 0x22, 0x30, 0x0d, 0x06, 0x09, 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01,
    0x01, 0x05, 0x00, 0x03, 0x82, 0x01, 0x0f, 0x00, 0x30, 0x82, 0x01, 0x0a, 0x02, 0x82, 0x01, 0x01,
    0x00, 0xd4, 0x4f, 0xd9, 0xae, 0x7a, 0xd2, 0x87, 0x80, 0x67, 0x11, 0x31, 0xb8, 0x5b, 0xac, 0x8b,
    0x5f, 0xf2, 0x21, 0x28, 0x2c, 0x70, 0xec, 0x66, 0xe9, 0x18, 0xee, 0x5e, 0xf1, 0xe3, 0xef, 0x09,
    0xcb, 0x5e, 0xe0, 0xcd, 0xe4, 0x39, 0x6a, 0x3f, 0x43, 0x2a, 0x3e, 0x1a, 0x06, 0xf2, 0xcc, 0xb0,
    0x0f, 0x5b, 0xd8, 0xa1, 0x3f, 0x1c, 0xb8, 0xfa, 0x8c, 0xa4, 0xbf, 0xa0, 0x57, 0x61, 0xcb, 0x35,
    0xa9, 0x0f, 0x56, 0x76, 0x57, 0x05, 0xa4, 0x56, 0x90, 0x64, 0x3d, 0x0e, 0x6e, 0x24, 0x43, 0x5e,
    0x54, 0x02, 0x99, 0x5b, 0xbe, 0x05, 0xab, 0xc9, 0xfb, 0xb7, 0x8f, 0x17, 0xcb, 0x59, 0xc0, 0x42,
    0x47, 0x79, 0xb1, 0xb8, 0x5c, 0x97, 0xef, 0xab, 0x65, 0x21, 0x88, 0xbd, 0x58, 0x3e, 0x9a, 0x8e,
    0x77, 0x84, 0x6c, 0x3d, 0x1a, 0x71, 0x7a, 0xb5, 0x9b, 0xc4, 0xde, 0xe5, 0x24, 0x18, 0x62, 0x61,
    0x58, 0x40, 0x14, 0x65, 0x6d, 0x8f, 0xa4, 0x82, 0x3e, 0xbe, 0xe9, 0x7a, 0xfa, 0x54, 0x9d, 0x9a,
    0xd3, 0x93, 0x44, 0x5c, 0x62, 0x9a, 0x26, 0x5e, 0x6b, 0x4c, 0xb5, 0x15, 0xe4, 0xe9, 0x4b, 0x4f,
    0x06, 0xd7, 0x59, 0x46, 0xfc, 0x4b, 0x3e, 0x09, 0x01, 0x0b, 0xd4, 0xa8, 0xcb, 0x39, 0x15, 0x4d,
    0x05, 0x0f, 0x3f, 0x08, 0x51, 0x8e, 0x3a, 0x20, 0x7e, 0xb3, 0x01, 0x7b, 0xe0, 0xeb, 0x3d, 0x62,
    0xdc, 0x0a, 0x9e, 0x63, 0x57, 0xcd, 0x68, 0xd8, 0xbe, 0xff, 0x3e, 0x3c, 0x33, 0x6c, 0x0d, 0xd8,
    0xb5, 0x4e, 0xdf, 0xeb, 0xef, 0x3b, 0x7d, 0xba, 0x32, 0xc0, 0x53, 0x48, 0x7e, 0x77, 0x91, 0xc7,
    0x7a, 0x2d, 0xb8, 0xaf, 0x8b, 0xe7, 0x8c, 0x0e, 0xa9, 0x39, 0x49, 0xdc, 0xa5, 0x4e, 0x7d, 0x3b,
    0xc9, 0xbf, 0x18, 0x41, 0x5e, 0xc0, 0x55, 0x4f, 0x90, 0x66, 0xfb, 0x19, 0xc8, 0x4b, 0x11, 0x93,
    0xff, 0x02, 0x03, 0x01, 0x00, 0x01
  });

  // Java uses an unencrypted PKCS #8 PrivateKeyInfo, not a PKCS #1 RSAPrivateKey.
  private static ByteBuffer DEFAULT_RSA_PRIVATE_KEY_DER = toBuffer(new int[] {
    0x30, 0x82, 0x04, 0xbe, 0x02, 0x01, 0x00, 0x30, 0x0d, 0x06, 0x09, 0x2a, 0x86, 0x48, 0x86, 0xf7,
    0x0d, 0x01, 0x01, 0x01, 0x05, 0x00, 0x04, 0x82, 0x04, 0xa8, 0x30, 0x82, 0x04, 0xa4, 0x02, 0x01,
    0x00, 0x02, 0x82, 0x01, 0x01, 0x00, 0xd4, 0x4f, 0xd9, 0xae, 0x7a, 0xd2, 0x87, 0x80, 0x67, 0x11,
    0x31, 0xb8, 0x5b, 0xac, 0x8b, 0x5f, 0xf2, 0x21, 0x28, 0x2c, 0x70, 0xec, 0x66, 0xe9, 0x18, 0xee,
    0x5e, 0xf1, 0xe3, 0xef, 0x09, 0xcb, 0x5e, 0xe0, 0xcd, 0xe4, 0x39, 0x6a, 0x3f, 0x43, 0x2a, 0x3e,
    0x1a, 0x06, 0xf2, 0xcc, 0xb0, 0x0f, 0x5b, 0xd8, 0xa1, 0x3f, 0x1c, 0xb8, 0xfa, 0x8c, 0xa4, 0xbf,
    0xa0, 0x57, 0x61, 0xcb, 0x35, 0xa9, 0x0f, 0x56, 0x76, 0x57, 0x05, 0xa4, 0x56, 0x90, 0x64, 0x3d,
    0x0e, 0x6e, 0x24, 0x43, 0x5e, 0x54, 0x02, 0x99, 0x5b, 0xbe, 0x05, 0xab, 0xc9, 0xfb, 0xb7, 0x8f,
    0x17, 0xcb, 0x59, 0xc0, 0x42, 0x47, 0x79, 0xb1, 0xb8, 0x5c, 0x97, 0xef, 0xab, 0x65, 0x21, 0x88,
    0xbd, 0x58, 0x3e, 0x9a, 0x8e, 0x77, 0x84, 0x6c, 0x3d, 0x1a, 0x71, 0x7a, 0xb5, 0x9b, 0xc4, 0xde,
    0xe5, 0x24, 0x18, 0x62, 0x61, 0x58, 0x40, 0x14, 0x65, 0x6d, 0x8f, 0xa4, 0x82, 0x3e, 0xbe, 0xe9,
    0x7a, 0xfa, 0x54, 0x9d, 0x9a, 0xd3, 0x93, 0x44, 0x5c, 0x62, 0x9a, 0x26, 0x5e, 0x6b, 0x4c, 0xb5,
    0x15, 0xe4, 0xe9, 0x4b, 0x4f, 0x06, 0xd7, 0x59, 0x46, 0xfc, 0x4b, 0x3e, 0x09, 0x01, 0x0b, 0xd4,
    0xa8, 0xcb, 0x39, 0x15, 0x4d, 0x05, 0x0f, 0x3f, 0x08, 0x51, 0x8e, 0x3a, 0x20, 0x7e, 0xb3, 0x01,
    0x7b, 0xe0, 0xeb, 0x3d, 0x62, 0xdc, 0x0a, 0x9e, 0x63, 0x57, 0xcd, 0x68, 0xd8, 0xbe, 0xff, 0x3e,
    0x3c, 0x33, 0x6c, 0x0d, 0xd8, 0xb5, 0x4e, 0xdf, 0xeb, 0xef, 0x3b, 0x7d, 0xba, 0x32, 0xc0, 0x53,
    0x48, 0x7e, 0x77, 0x91, 0xc7, 0x7a, 0x2d, 0xb8, 0xaf, 0x8b, 0xe7, 0x8c, 0x0e, 0xa9, 0x39, 0x49,
    0xdc, 0xa5, 0x4e, 0x7d, 0x3b, 0xc9, 0xbf, 0x18, 0x41, 0x5e, 0xc0, 0x55, 0x4f, 0x90, 0x66, 0xfb,
    0x19, 0xc8, 0x4b, 0x11, 0x93, 0xff, 0x02, 0x03, 0x01, 0x00, 0x01, 0x02, 0x82, 0x01, 0x00, 0x0f,
    0xa1, 0x85, 0x5c, 0x44, 0x2c, 0xa5, 0xcf, 0x3d, 0x47, 0x55, 0xca, 0xc5, 0xed, 0x11, 0x21, 0xd2,
    0x38, 0xc0, 0xb5, 0x6c, 0xe6, 0xea, 0xb8, 0xb4, 0x9e, 0x30, 0x1d, 0x4c, 0xf3, 0xb7, 0x5b, 0xe2,
    0xb3, 0x58, 0x55, 0x3a, 0x28, 0xe9, 0x59, 0x6f, 0x8d, 0xbc, 0xea, 0xd0, 0x0b, 0x63, 0xd6, 0xed,
    0xa3, 0x28, 0x53, 0xf6, 0x30, 0x64, 0x39, 0xe0, 0x93, 0x3f, 0x21, 0xcf, 0xd0, 0x5f, 0x36, 0x00,
    0x2c, 0x14, 0x70, 0x59, 0xb8, 0xfc, 0xaa, 0x8a, 0xc6, 0xb7, 0xfe, 0x41, 0xeb, 0x37, 0xd1, 0xa5,
    0x93, 0x56, 0xde, 0xc9, 0x9a, 0x19, 0x37, 0xd0, 0x0e, 0xd7, 0xe8, 0x9f, 0xc5, 0xf8, 0xdb, 0x3c,
    0x49, 0x6a, 0x52, 0x5e, 0xd9, 0x45, 0x5c, 0x1f, 0xb8, 0xea, 0x7f, 0xc9, 0xb4, 0x25, 0x53, 0x05,
    0x4b, 0xd6, 0xbf, 0xd0, 0xa5, 0x01, 0x23, 0xe3, 0xbd, 0xa9, 0x4f, 0x1c, 0x00, 0x7a, 0x3c, 0x1b,
    0xbb, 0xaa, 0x08, 0xd9, 0xd2, 0x8c, 0xdb, 0xb4, 0x6c, 0xff, 0x57, 0x64, 0x82, 0xbb, 0x02, 0x71,
    0x2d, 0x99, 0xea, 0x8a, 0x4e, 0x5a, 0xdb, 0x82, 0x20, 0x32, 0x51, 0xf8, 0x30, 0x98, 0x67, 0x4a,
    0x31, 0x73, 0xb1, 0xd7, 0x51, 0xc5, 0x71, 0x82, 0x2b, 0x99, 0xbc, 0x0c, 0xfa, 0x24, 0x4c, 0x0b,
    0x38, 0x73, 0xd8, 0xef, 0x6f, 0x5b, 0xda, 0x56, 0xc8, 0x6b, 0xcb, 0xf5, 0xc6, 0xaa, 0x4d, 0x8b,
    0x39, 0x0f, 0x0a, 0x43, 0x4e, 0x8b, 0x87, 0xe7, 0x98, 0x5a, 0x0d, 0x94, 0x55, 0xc7, 0x42, 0xb4,
    0x13, 0xfa, 0xed, 0x9c, 0xfe, 0xea, 0x2d, 0x95, 0xc1, 0xdc, 0x2f, 0x5d, 0x44, 0xf5, 0x2d, 0xab,
    0x8b, 0x79, 0x70, 0x0f, 0xe9, 0xa7, 0x17, 0xe8, 0x40, 0xd7, 0xa5, 0x0d, 0x97, 0xe9, 0x53, 0xa4,
    0xb4, 0x70, 0xbe, 0x19, 0x7b, 0x86, 0x2c, 0x26, 0xe7, 0xb1, 0x23, 0x22, 0x5a, 0xbd, 0x91, 0x02,
    0x81, 0x81, 0x00, 0xe2, 0x4d, 0x3c, 0xdc, 0x23, 0xb5, 0x2d, 0xc4, 0x66, 0xe7, 0xf2, 0xa4, 0x33,
    0xb9, 0xd6, 0xdd, 0x39, 0xc6, 0xee, 0x0e, 0xe6, 0x23, 0xbb, 0x9c, 0xf0, 0x6a, 0x10, 0xa8, 0x12,
    0xaa, 0x15, 0x8c, 0x08, 0x51, 0x5d, 0xed, 0x46, 0x33, 0xb0, 0x5d, 0x72, 0x02, 0xa0, 0x16, 0xb8,
    0xcf, 0xaa, 0x27, 0x09, 0x74, 0x97, 0x8c, 0xac, 0x8d, 0x4e, 0xbc, 0xe8, 0x62, 0xe5, 0x1e, 0x3c,
    0x74, 0xbb, 0xe9, 0xb9, 0xa6, 0x91, 0x02, 0x3f, 0x43, 0x4d, 0x2f, 0x01, 0x2a, 0x1c, 0xff, 0x4f,
    0x05, 0xf5, 0x98, 0x57, 0x3f, 0x67, 0xb0, 0x2d, 0x84, 0x2d, 0xd3, 0xf5, 0xb9, 0xd7, 0x37, 0x39,
    0x2a, 0x44, 0x04, 0x58, 0xa4, 0x17, 0x1e, 0x47, 0x38, 0x3f, 0x7d, 0x61, 0x97, 0xf2, 0xe4, 0xe5,
    0xeb, 0xe8, 0xbf, 0x55, 0xac, 0x6b, 0x74, 0xb8, 0x55, 0x2b, 0x1c, 0x12, 0x2a, 0x9c, 0x11, 0xf0,
    0x5b, 0x9d, 0xd7, 0x02, 0x81, 0x81, 0x00, 0xf0, 0x2c, 0x9d, 0xa3, 0x34, 0x0b, 0x6a, 0x01, 0x69,
    0x6c, 0xaa, 0xbf, 0xee, 0x95, 0xcc, 0x12, 0x24, 0x37, 0xeb, 0xda, 0x30, 0xdb, 0xe5, 0x4b, 0x34,
    0x5b, 0x56, 0x9e, 0x46, 0xeb, 0xe5, 0xb5, 0x75, 0x45, 0xac, 0xb7, 0xa2, 0x52, 0x69, 0x04, 0xd2,
    0x5f, 0x98, 0x59, 0x4f, 0xb6, 0xf3, 0x8e, 0x9f, 0x34, 0x8d, 0x07, 0x22, 0x7e, 0xc0, 0x28, 0x79,
    0xe1, 0x25, 0x0a, 0x03, 0x96, 0xb8, 0xa8, 0x0f, 0xc8, 0x37, 0x2d, 0xb0, 0xe8, 0xc0, 0x1e, 0x3b,
    0x4a, 0xf2, 0xcc, 0x6b, 0x60, 0x83, 0x88, 0x2d, 0x71, 0x8f, 0x91, 0xab, 0x1a, 0x02, 0x8e, 0x03,
    0xfb, 0xc2, 0x9a, 0x4e, 0x91, 0xd4, 0x49, 0x2c, 0x4c, 0x69, 0x8c, 0xe9, 0x4b, 0xbe, 0x88, 0xe2,
    0xd9, 0xa8, 0x7f, 0x3d, 0xe9, 0x67, 0x39, 0xd7, 0xd4, 0x11, 0xa0, 0xb1, 0xcd, 0x8b, 0x59, 0x5f,
    0xce, 0x35, 0x16, 0x26, 0x30, 0xe6, 0x19, 0x02, 0x81, 0x81, 0x00, 0x9b, 0x59, 0x44, 0x47, 0x26,
    0xa8, 0x10, 0x63, 0xfb, 0xf4, 0x8c, 0x27, 0xd6, 0x6e, 0x63, 0xa6, 0x78, 0x2c, 0x2c, 0x6d, 0xc3,
    0xe4, 0x91, 0xbd, 0x39, 0x78, 0xc6, 0x38, 0x6a, 0x9f, 0xa1, 0xad, 0x00, 0x64, 0xc2, 0xe2, 0xc8,
    0x47, 0x61, 0x71, 0xb4, 0x7b, 0x42, 0xe4, 0x76, 0x37, 0xf0, 0x69, 0x5d, 0xdf, 0x50, 0xcd, 0xbc,
    0x02, 0x41, 0x24, 0x03, 0x2f, 0x28, 0x73, 0xaa, 0x32, 0xc4, 0x70, 0xbd, 0x06, 0x30, 0x13, 0x67,
    0xd4, 0x4e, 0x9e, 0xce, 0xe0, 0xd7, 0x09, 0x18, 0x79, 0x51, 0xd0, 0x23, 0x4c, 0x9e, 0x64, 0x5d,
    0xca, 0x98, 0x1f, 0x22, 0x57, 0x51, 0xfb, 0x51, 0xdd, 0xc6, 0xd5, 0x68, 0xf8, 0x33, 0xfa, 0x90,
    0x0f, 0x77, 0xde, 0x1d, 0x69, 0xce, 0xce, 0xfd, 0x5b, 0x05, 0xea, 0x9a, 0xe8, 0x82, 0xd7, 0x9c,
    0x56, 0xb3, 0x02, 0x51, 0x22, 0x39, 0x03, 0x43, 0x89, 0xd0, 0xff, 0x02, 0x81, 0x80, 0x13, 0x1c,
    0x89, 0xc2, 0xb5, 0xde, 0x7e, 0xa5, 0xf4, 0x1c, 0xa8, 0x8d, 0xb3, 0x4f, 0x8a, 0x38, 0x9b, 0x57,
    0x33, 0xd6, 0x5d, 0xf2, 0xf1, 0x91, 0x05, 0x6e, 0x8b, 0x3a, 0xf7, 0x0b, 0xc8, 0x70, 0xa3, 0x0f,
    0x53, 0x4a, 0x1d, 0x89, 0x8f, 0x3f, 0xc9, 0xf9, 0xbf, 0x66, 0xc3, 0xf8, 0x1b, 0xf3, 0x6a, 0x69,
    0xc5, 0x1b, 0x1f, 0x3c, 0x94, 0xcf, 0xe3, 0xba, 0xed, 0xb6, 0x99, 0x48, 0x82, 0x13, 0x25, 0x86,
    0x5a, 0x15, 0xb1, 0xb1, 0x23, 0xb0, 0x84, 0x29, 0x57, 0x9e, 0xba, 0xa0, 0xa8, 0x76, 0xca, 0x9e,
    0xf1, 0xbc, 0xb6, 0xaf, 0xd0, 0x2a, 0x3a, 0xd8, 0xea, 0xc8, 0x5a, 0x9e, 0x32, 0x15, 0x4c, 0x88,
    0x1c, 0x12, 0x11, 0x72, 0x6c, 0x8b, 0xf9, 0xf9, 0x35, 0xf6, 0x42, 0x17, 0xf3, 0x95, 0xdf, 0xbd,
    0xc9, 0x55, 0x4f, 0x30, 0xba, 0xf8, 0xf6, 0xad, 0xb2, 0xfd, 0xbb, 0x36, 0x42, 0xe9, 0x02, 0x81,
    0x81, 0x00, 0xad, 0xf0, 0xc0, 0xfc, 0x55, 0x47, 0x8a, 0x03, 0x2b, 0x5c, 0x1c, 0x6e, 0xef, 0xf6,
    0x96, 0x68, 0xee, 0xa8, 0xd0, 0x6d, 0x70, 0x4f, 0x7f, 0x3e, 0x17, 0x2b, 0xfd, 0x7e, 0x22, 0x8c,
    0xea, 0x25, 0xe3, 0xbb, 0xa4, 0xa1, 0x57, 0xe7, 0x3e, 0xc0, 0x47, 0xf8, 0x7b, 0xa6, 0xd2, 0x48,
    0x68, 0xc0, 0x8a, 0xe0, 0xb2, 0x6b, 0x5d, 0xf9, 0x32, 0x6e, 0x70, 0x5a, 0xb9, 0x77, 0xd9, 0xbf,
    0x6d, 0xea, 0x53, 0xe2, 0x4f, 0xa8, 0x4c, 0x1c, 0xfa, 0x69, 0x49, 0x26, 0x48, 0x8a, 0xc5, 0x92,
    0x77, 0x6b, 0x7a, 0x89, 0xc3, 0xef, 0x6d, 0x1c, 0x44, 0x10, 0xe6, 0xaf, 0x47, 0x18, 0x9f, 0x99,
    0x09, 0xb4, 0x3b, 0x63, 0xf7, 0xbf, 0xe4, 0xe7, 0xe5, 0x98, 0xe2, 0x57, 0x85, 0xbb, 0x78, 0xb5,
    0xd1, 0xc3, 0x64, 0x8d, 0x4d, 0x4f, 0x02, 0xdb, 0x2c, 0x51, 0x58, 0xa3, 0xc7, 0x35, 0xf1, 0x2d,
    0x7a, 0x0a
  });

  @Before
  public void
  setUp()
    throws ConsumerDb.Error, NoSuchAlgorithmException,
      InvalidKeySpecException, DerDecodingException
  {
    // Don't show INFO log messages.
    Logger.getLogger("").setLevel(Level.WARNING);

    File policyConfigDirectory = IntegrationTestsCommon.getPolicyConfigDirectory();
    databaseFilePath = new File(policyConfigDirectory, "test.db");
    databaseFilePath.delete();

    groupName = new Name("/Prefix/READ");
    contentName = new Name("/Prefix/SAMPLE/Content");
    cKeyName = new Name("/Prefix/SAMPLE/Content/C-KEY/1");
    eKeyName = new Name("/Prefix/READ/E-KEY/1/2");
    dKeyName = new Name("/Prefix/READ/D-KEY/1/2");
    uKeyName = new Name("/U/Key");
    uName = new Name("/U");

    // Generate the E-KEY and D-KEY.
    RsaKeyParams params = new RsaKeyParams();
    fixtureDKeyBlob = RsaAlgorithm.generateKey(params).getKeyBits();
    fixtureEKeyBlob = RsaAlgorithm.deriveEncryptKey(fixtureDKeyBlob).getKeyBits();

    // Generate the user key.
    fixtureUDKeyBlob = RsaAlgorithm.generateKey(params).getKeyBits();
    fixtureUEKeyBlob = RsaAlgorithm.deriveEncryptKey(fixtureUDKeyBlob).getKeyBits();

    // Load the C-KEY.
    fixtureCKeyBlob = new Blob(AES_KEY, false);

    // Set up the key chain.
    MemoryIdentityStorage identityStorage = new MemoryIdentityStorage();
    MemoryPrivateKeyStorage privateKeyStorage = new MemoryPrivateKeyStorage();
    keyChain = new KeyChain
      (new IdentityManager(identityStorage, privateKeyStorage),
       new NoVerifyPolicyManager());

    // Initialize the storage.
    Name keyName = new Name("/testname/DSK-123");
    certificateName = keyName.getSubName(0, keyName.size() - 1).append
      ("KEY").append(keyName.get(-1)).append("ID-CERT").append("0");
    try {
      identityStorage.addKey
        (keyName, KeyType.RSA, new Blob(DEFAULT_RSA_PUBLIC_KEY_DER, false));
      privateKeyStorage.setKeyPairForKeyName
        (keyName, KeyType.RSA, DEFAULT_RSA_PUBLIC_KEY_DER,
         DEFAULT_RSA_PRIVATE_KEY_DER);
    } catch (net.named_data.jndn.security.SecurityException ex) {
      // We don't expect this to happen.
      Logger.getLogger(TestGroupConsumer.class.getName()).log(Level.SEVERE, null, ex);
    }

    Consumer.setFriendAccess(this);
  }

  public void
  setConsumerFriendAccess(Consumer.FriendAccess friendAccess)
  {
    this.friendAccess = friendAccess;
  }

  @After
  public void
  tearDown()
  {
    databaseFilePath.delete();
  }

  Data
  createEncryptedContent()
    throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException,
      IllegalBlockSizeException, BadPaddingException,
      InvalidAlgorithmParameterException, InvalidKeySpecException, SecurityException
  {
    Data contentData = new Data(contentName);
    EncryptParams encryptParams = new EncryptParams(EncryptAlgorithmType.AesCbc);
    encryptParams.setInitialVector(new Blob(INITIAL_VECTOR, false));
    Encryptor.encryptData
      (contentData, new Blob(DATA_CONTENT, false), cKeyName, fixtureCKeyBlob,
       encryptParams);
    keyChain.sign(contentData, certificateName);
    return contentData;
  }

  Data
  createEncryptedCKey()
    throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException,
      IllegalBlockSizeException, BadPaddingException,
      InvalidAlgorithmParameterException, InvalidKeySpecException, SecurityException
  {
    Data cKeyData = new Data(cKeyName);
    EncryptParams encryptParams = new EncryptParams(EncryptAlgorithmType.RsaOaep);
    Encryptor.encryptData
      (cKeyData, fixtureCKeyBlob, dKeyName, fixtureEKeyBlob, encryptParams);
    keyChain.sign(cKeyData, certificateName);
    return cKeyData;
  }

  Data
  createEncryptedDKey()
    throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException,
      IllegalBlockSizeException, BadPaddingException,
      InvalidAlgorithmParameterException, InvalidKeySpecException, SecurityException
  {
    Data dKeyData = new Data(dKeyName);
    EncryptParams encryptParams = new EncryptParams(EncryptAlgorithmType.RsaOaep);
    Encryptor.encryptData
      (dKeyData, fixtureDKeyBlob, uKeyName, fixtureUEKeyBlob, encryptParams);
    keyChain.sign(dKeyData, certificateName);
    return dKeyData;
  }

  @Test
  public void
  testDecryptContent()
    throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException,
      IllegalBlockSizeException, BadPaddingException,
      InvalidAlgorithmParameterException, InvalidKeySpecException, SecurityException
  {
    // Generate the AES key.
    final Blob aesKeyBlob = new Blob(AES_KEY, false);

    // Generate the C-KEY packet for the same AES_KEY.
    Data cKeyData = createEncryptedCKey();
    // Generate the content packet.
    Data contentData = createEncryptedContent();

    // Decrypt.
    friendAccess.decrypt
      (cKeyData.getContent(), fixtureDKeyBlob,
       new Consumer.OnPlainText() {
         public void onPlainText(Blob result) {
           assertTrue(result.equals(aesKeyBlob));
         }
       },
       new EncryptError.OnError() {
         public void onError(EncryptError.ErrorCode errorCode, String message) {
           fail("decrypt error " + message);
         }
       });

    // Decrypt.
    friendAccess.decrypt
      (contentData.getContent(), fixtureCKeyBlob,
       new Consumer.OnPlainText() {
         public void onPlainText(Blob result) {
           assertTrue(result.equals(new Blob(DATA_CONTENT, false)));
         }
       },
       new EncryptError.OnError() {
         public void onError(EncryptError.ErrorCode errorCode, String message) {
           fail("decrypt error " + message);
         }
       });
  }

  @Test
  public void
  testConsume()
    throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException,
      IllegalBlockSizeException, BadPaddingException,
      InvalidAlgorithmParameterException, InvalidKeySpecException, SecurityException, ConsumerDb.Error
  {
    final Data contentData = createEncryptedContent();
    final Data cKeyData = createEncryptedCKey();
    final Data dKeyData = createEncryptedDKey();

    final int[] contentCount = new int[] { 0 };
    final int[] cKeyCount = new int[] { 0 };
    final int[] dKeyCount = new int[] { 0 };

    // Prepare a TestFace to instantly answer calls to expressInterest.
    class TestFace extends Face {
      public TestFace()
      {
        super("localhost");
      }

      public long
      expressInterest
        (Interest interest, OnData onData, OnTimeout onTimeout,
         OnNetworkNack onNetworkNack, WireFormat wireFormat) throws IOException
      {
        if (interest.matchesName(contentData.getName())) {
          contentCount[0] = 1;
          onData.onData(interest, contentData);
        }
        else if (interest.matchesName(cKeyData.getName())) {
          cKeyCount[0] = 1;
          onData.onData(interest, cKeyData);
        }
        else if (interest.matchesName(dKeyData.getName())) {
          dKeyCount[0] = 1;
          onData.onData(interest, dKeyData);
        }
        else
          onTimeout.onTimeout(interest);

        return 0;
      }
    }

    TestFace face = new TestFace();

    // Create the consumer.
    Consumer consumer = new Consumer
      (face, keyChain, groupName, uName,
       new Sqlite3ConsumerDb(databaseFilePath.getAbsolutePath()));
    consumer.addDecryptionKey(uKeyName, fixtureUDKeyBlob);

    final int[] finalCount = new int[] { 0 };
    consumer.consume
      (contentName,
       new Consumer.OnConsumeComplete() {
         public void onConsumeComplete(Data data, Blob result) {
           finalCount[0] = 1;
           assertTrue("consumeComplete",
                      result.equals(new Blob(DATA_CONTENT, false)));
         }
       },
       new EncryptError.OnError() {
         public void onError(EncryptError.ErrorCode code, String message) {
           fail("consume error " + code + ": " + message);
         }
       });

    assertEquals("contentCount", 1, contentCount[0]);
    assertEquals("cKeyCount", 1, cKeyCount[0]);
    assertEquals("dKeyCount", 1, dKeyCount[0]);
    assertEquals("finalCount", 1, finalCount[0]);
  }

  @Test
  public void
  testCosumerWithLink()
    throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException,
      IllegalBlockSizeException, BadPaddingException,
      InvalidAlgorithmParameterException, InvalidKeySpecException, SecurityException, ConsumerDb.Error
  {
    final Data contentData = createEncryptedContent();
    final Data cKeyData = createEncryptedCKey();
    final Data dKeyData = createEncryptedDKey();

    final int[] contentCount = new int[] { 0 };
    final int[] cKeyCount = new int[] { 0 };
    final int[] dKeyCount = new int[] { 0 };

    // Prepare a TestFace to instantly answer calls to expressInterest.
    class TestFace extends Face {
      public TestFace()
      {
        super("localhost");
      }

      public long
      expressInterest
        (Interest interest, OnData onData, OnTimeout onTimeout,
         OnNetworkNack onNetworkNack, WireFormat wireFormat) throws IOException
      {
        try {
          assertEquals(3, interest.getLink().getDelegations().size());
        } catch (EncodingException ex) {
          fail("Error in getLink: " + ex);
        }

        if (interest.matchesName(contentData.getName())) {
          contentCount[0] = 1;
          onData.onData(interest, contentData);
        }
        else if (interest.matchesName(cKeyData.getName())) {
          cKeyCount[0] = 1;
          onData.onData(interest, cKeyData);
        }
        else if (interest.matchesName(dKeyData.getName())) {
          dKeyCount[0] = 1;
          onData.onData(interest, dKeyData);
        }
        else
          onTimeout.onTimeout(interest);

        return 0;
      }
    }

    TestFace face = new TestFace();

    // Create the consumer.
    Link ckeyLink = new Link();
    ckeyLink.addDelegation(10,  new Name("/ckey1"));
    ckeyLink.addDelegation(20,  new Name("/ckey2"));
    ckeyLink.addDelegation(100, new Name("/ckey3"));
    Link dkeyLink = new Link();
    dkeyLink.addDelegation(10,  new Name("/dkey1"));
    dkeyLink.addDelegation(20,  new Name("/dkey2"));
    dkeyLink.addDelegation(100, new Name("/dkey3"));
    Link dataLink = new Link();
    dataLink.addDelegation(10,  new Name("/data1"));
    dataLink.addDelegation(20,  new Name("/data2"));
    dataLink.addDelegation(100, new Name("/data3"));
    keyChain.sign(ckeyLink);
    keyChain.sign(dkeyLink);
    keyChain.sign(dataLink);

    Consumer consumer = new Consumer
      (face, keyChain, groupName, uName,
       new Sqlite3ConsumerDb(databaseFilePath.getAbsolutePath()), ckeyLink, dkeyLink);
    consumer.addDecryptionKey(uKeyName, fixtureUDKeyBlob);

    final int[] finalCount = new int[] { 0 };
    consumer.consume
      (contentName,
       new Consumer.OnConsumeComplete() {
         public void onConsumeComplete(Data data, Blob result) {
           finalCount[0] = 1;
           assertTrue("consumeComplete",
                      result.equals(new Blob(DATA_CONTENT, false)));
         }
       },
       new EncryptError.OnError() {
         public void onError(EncryptError.ErrorCode code, String message) {
           fail("consume error " + code + ": " + message);
         }
       },
       dataLink);

    assertEquals("contentCount", 1, contentCount[0]);
    assertEquals("cKeyCount", 1, cKeyCount[0]);
    assertEquals("dKeyCount", 1, dKeyCount[0]);
    assertEquals("finalCount", 1, finalCount[0]);
  }

  File databaseFilePath;

  KeyChain keyChain;
  Name certificateName;

  Blob fixtureCKeyBlob;
  Blob fixtureEKeyBlob;
  Blob fixtureDKeyBlob;
  Blob fixtureUEKeyBlob;
  Blob fixtureUDKeyBlob;

  Name groupName;
  Name contentName;
  Name cKeyName;
  Name eKeyName;
  Name dKeyName;
  Name uKeyName;
  Name uName;

  private Consumer.FriendAccess friendAccess;
}
