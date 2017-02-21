// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2017 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.tests.integration_tests {
	
	using ILOG.J2CsMapping.NIO;
	using ILOG.J2CsMapping.Util;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.spec;
	using javax.crypto;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encoding.der;
	using net.named_data.jndn.encrypt;
	using net.named_data.jndn.encrypt.algo;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.certificate;
	using net.named_data.jndn.security.identity;
	using net.named_data.jndn.security.policy;
	using net.named_data.jndn.util;
	
	public class TestGroupManager : GroupManager.Friend {
		public TestGroupManager() {
			this.certificate = new IdentityCertificate();
		}
	
		// Convert the int array to a ByteBuffer.
		public static ByteBuffer toBuffer(int[] array) {
			ByteBuffer result = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(array.Length);
			for (int i = 0; i < array.Length; ++i)
				result.put((byte) (array[i] & 0xff));
	
			result.flip();
			return result;
		}
	
		private static readonly ByteBuffer SIG_INFO = toBuffer(new int[] { 0x16,
				0x1b, // SignatureInfo
				0x1b,
				0x01, // SignatureType
				0x01, 0x1c,
				0x16, // KeyLocator
				0x07,
				0x14, // Name
				0x08, 0x04, 0x74, 0x65, 0x73, 0x74, 0x08, 0x03, 0x6b, 0x65, 0x79,
				0x08, 0x07, 0x6c, 0x6f, 0x63, 0x61, 0x74, 0x6f, 0x72 });
	
		private static readonly ByteBuffer SIG_VALUE = toBuffer(new int[] {
				0x17,
				0x80, // SignatureValue
				0x2f, 0xd6, 0xf1, 0x6e, 0x80, 0x6f, 0x10, 0xbe, 0xb1, 0x6f, 0x3e,
				0x31, 0xec, 0xe3, 0xb9, 0xea, 0x83, 0x30, 0x40, 0x03, 0xfc, 0xa0,
				0x13, 0xd9, 0xb3, 0xc6, 0x25, 0x16, 0x2d, 0xa6, 0x58, 0x41, 0x69,
				0x62, 0x56, 0xd8, 0xb3, 0x6a, 0x38, 0x76, 0x56, 0xea, 0x61, 0xb2,
				0x32, 0x70, 0x1c, 0xb6, 0x4d, 0x10, 0x1d, 0xdc, 0x92, 0x8e, 0x52,
				0xa5, 0x8a, 0x1d, 0xd9, 0x96, 0x5e, 0xc0, 0x62, 0x0b, 0xcf, 0x3a,
				0x9d, 0x7f, 0xca, 0xbe, 0xa1, 0x41, 0x71, 0x85, 0x7a, 0x8b, 0x5d,
				0xa9, 0x64, 0xd6, 0x66, 0xb4, 0xe9, 0x8d, 0x0c, 0x28, 0x43, 0xee,
				0xa6, 0x64, 0xe8, 0x55, 0xf6, 0x1c, 0x19, 0x0b, 0xef, 0x99, 0x25,
				0x1e, 0xdc, 0x78, 0xb3, 0xa7, 0xaa, 0x0d, 0x14, 0x58, 0x30, 0xe5,
				0x37, 0x6a, 0x6d, 0xdb, 0x56, 0xac, 0xa3, 0xfc, 0x90, 0x7a, 0xb8,
				0x66, 0x9c, 0x0e, 0xf6, 0xb7, 0x64, 0xd1 });
	
		public void setUp() {
			// Don't show INFO log messages.
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger("").setLevel(ILOG.J2CsMapping.Util.Logging.Level.WARNING);
	
			FileInfo policyConfigDirectory = net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon
					.getPolicyConfigDirectory();
	
			dKeyDatabaseFilePath = new FileInfo(System.IO.Path.Combine(policyConfigDirectory.FullName,"manager-d-key-test.db"));
			dKeyDatabaseFilePath.delete();
	
			eKeyDatabaseFilePath = new FileInfo(System.IO.Path.Combine(policyConfigDirectory.FullName,"manager-e-key-test.db"));
			eKeyDatabaseFilePath.delete();
	
			intervalDatabaseFilePath = new FileInfo(System.IO.Path.Combine(policyConfigDirectory.FullName,"manager-interval-test.db"));
			intervalDatabaseFilePath.delete();
	
			groupKeyDatabaseFilePath = new FileInfo(System.IO.Path.Combine(policyConfigDirectory.FullName,"manager-group-key-test.db"));
			groupKeyDatabaseFilePath.delete();
	
			RsaKeyParams paras = new RsaKeyParams();
			DecryptKey memberDecryptKey = net.named_data.jndn.encrypt.algo.RsaAlgorithm.generateKey(paras);
			decryptKeyBlob = memberDecryptKey.getKeyBits();
			EncryptKey memberEncryptKey = net.named_data.jndn.encrypt.algo.RsaAlgorithm
					.deriveEncryptKey(decryptKeyBlob);
			encryptKeyBlob = memberEncryptKey.getKeyBits();
	
			// Generate the certificate.
			certificate.setName(new Name("/ndn/memberA/KEY/ksk-123/ID-CERT/123"));
			PublicKey contentPublicKey = new PublicKey(encryptKeyBlob);
			certificate.setPublicKeyInfo(contentPublicKey);
			certificate.encode();
	
			Blob signatureInfoBlob = new Blob(SIG_INFO, false);
			Blob signatureValueBlob = new Blob(SIG_VALUE, false);
	
			Signature signature = net.named_data.jndn.encoding.TlvWireFormat.get().decodeSignatureInfoAndValue(
					signatureInfoBlob.buf(), signatureValueBlob.buf());
			certificate.setSignature(signature);
	
			certificate.wireEncode();
	
			// Set up the keyChain.
			MemoryIdentityStorage identityStorage = new MemoryIdentityStorage();
			MemoryPrivateKeyStorage privateKeyStorage = new MemoryPrivateKeyStorage();
			keyChain = new KeyChain(new IdentityManager(identityStorage,
					privateKeyStorage), new NoVerifyPolicyManager());
			Name identityName = new Name("TestGroupManager");
			keyChain.createIdentityAndCertificate(identityName);
			keyChain.getIdentityManager().setDefaultIdentity(identityName);
	
			net.named_data.jndn.encrypt.GroupManager.setFriendAccess(this);
		}
	
		public virtual void setGroupManagerFriendAccess(
				GroupManager.FriendAccess friendAccess) {
			this.friendAccess = friendAccess;
		}
	
		internal void setManager(GroupManager manager) {
			// Set up the first schedule.
			Schedule schedule1 = new Schedule();
			RepetitiveInterval interval11 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY);
			RepetitiveInterval interval12 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"), 6, 8, 1,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY);
			RepetitiveInterval interval13 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"), 7, 8);
			schedule1.addWhiteInterval(interval11);
			schedule1.addWhiteInterval(interval12);
			schedule1.addBlackInterval(interval13);
	
			// Set up the second schedule.
			Schedule schedule2 = new Schedule();
			RepetitiveInterval interval21 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"), 9, 12, 1,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY);
			RepetitiveInterval interval22 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"), 6, 8);
			RepetitiveInterval interval23 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"), 2, 4);
			schedule2.addWhiteInterval(interval21);
			schedule2.addWhiteInterval(interval22);
			schedule2.addBlackInterval(interval23);
	
			// Add them to the group manager database.
			manager.addSchedule("schedule1", schedule1);
			manager.addSchedule("schedule2", schedule2);
	
			// Make some adaptions to certificate.
			Blob dataBlob = certificate.wireEncode();
	
			Data memberA = new Data();
			memberA.wireDecode(dataBlob, net.named_data.jndn.encoding.TlvWireFormat.get());
			memberA.setName(new Name("/ndn/memberA/KEY/ksk-123/ID-CERT/123"));
			Data memberB = new Data();
			memberB.wireDecode(dataBlob, net.named_data.jndn.encoding.TlvWireFormat.get());
			memberB.setName(new Name("/ndn/memberB/KEY/ksk-123/ID-CERT/123"));
			Data memberC = new Data();
			memberC.wireDecode(dataBlob, net.named_data.jndn.encoding.TlvWireFormat.get());
			memberC.setName(new Name("/ndn/memberC/KEY/ksk-123/ID-CERT/123"));
	
			// Add the members to the database.
			manager.addMember("schedule1", memberA);
			manager.addMember("schedule1", memberB);
			manager.addMember("schedule2", memberC);
		}
	
		public void tearDown() {
			dKeyDatabaseFilePath.delete();
			eKeyDatabaseFilePath.delete();
			intervalDatabaseFilePath.delete();
			groupKeyDatabaseFilePath.delete();
		}
	
		public void testCreateDKeyData() {
			// Create the group manager.
			GroupManager manager = new GroupManager(new Name("Alice"), new Name(
					"data_type"), new Sqlite3GroupManagerDb(
					System.IO.Path.GetFullPath(dKeyDatabaseFilePath.Name)), 2048, 1, keyChain);
	
			Blob newCertificateBlob = certificate.wireEncode();
			IdentityCertificate newCertificate = new IdentityCertificate();
			newCertificate.wireDecode(newCertificateBlob);
	
			// Encrypt the D-KEY.
			Data data = friendAccess.createDKeyData(manager, "20150825T000000",
					"20150827T000000", new Name("/ndn/memberA/KEY"),
					decryptKeyBlob, newCertificate.getPublicKeyInfo().getKeyDer());
	
			// Verify the encrypted D-KEY.
			Blob dataContent = data.getContent();
	
			// Get the nonce key.
			// dataContent is a sequence of the two EncryptedContent.
			EncryptedContent encryptedNonce = new EncryptedContent();
			encryptedNonce.wireDecode(dataContent);
			Assert.AssertEquals(0, encryptedNonce.getInitialVector().size());
			Assert.AssertEquals(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep,
					encryptedNonce.getAlgorithmType());
	
			Blob blobNonce = encryptedNonce.getPayload();
			EncryptParams decryptParams = new EncryptParams(
					net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep);
			Blob nonce = net.named_data.jndn.encrypt.algo.RsaAlgorithm.decrypt(decryptKeyBlob, blobNonce,
					decryptParams);
	
			// Get the D-KEY.
			// Use the size of encryptedNonce to find the start of encryptedPayload.
			ByteBuffer payloadContent = dataContent.buf().duplicate();
			payloadContent.position(encryptedNonce.wireEncode().size());
			EncryptedContent encryptedPayload = new EncryptedContent();
			encryptedPayload.wireDecode(payloadContent);
			Assert.AssertEquals(16, encryptedPayload.getInitialVector().size());
			Assert.AssertEquals(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc,
					encryptedPayload.getAlgorithmType());
	
			decryptParams.setAlgorithmType(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc);
			decryptParams.setInitialVector(encryptedPayload.getInitialVector());
			Blob blobPayload = encryptedPayload.getPayload();
			Blob largePayload = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(nonce, blobPayload,
					decryptParams);
	
			Assert.AssertTrue(largePayload.equals(decryptKeyBlob));
		}
	
		public void testCreateEKeyData() {
			// Create the group manager.
			GroupManager manager = new GroupManager(new Name("Alice"), new Name(
					"data_type"), new Sqlite3GroupManagerDb(
					System.IO.Path.GetFullPath(eKeyDatabaseFilePath.Name)), 1024, 1, keyChain);
			setManager(manager);
	
			Data data = friendAccess.createEKeyData(manager, "20150825T090000",
					"20150825T110000", encryptKeyBlob);
			Assert.AssertEquals(
					"/Alice/READ/data_type/E-KEY/20150825T090000/20150825T110000",
					data.getName().toUri());
	
			Blob contentBlob = data.getContent();
			Assert.AssertTrue(encryptKeyBlob.equals(contentBlob));
		}
	
		public void testCalculateInterval() {
			// Create the group manager.
			GroupManager manager = new GroupManager(new Name("Alice"), new Name(
					"data_type"), new Sqlite3GroupManagerDb(
					System.IO.Path.GetFullPath(intervalDatabaseFilePath.Name)), 1024, 1, keyChain);
			setManager(manager);
	
			IDictionary memberKeys = new Hashtable();
			Interval result;
	
			double timePoint1 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T093000");
			result = friendAccess
					.calculateInterval(manager, timePoint1, memberKeys);
			Assert.AssertEquals("20150825T090000", net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.getStartTime()));
			Assert.AssertEquals("20150825T100000", net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.getEndTime()));
	
			double timePoint2 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T073000");
			result = friendAccess
					.calculateInterval(manager, timePoint2, memberKeys);
			Assert.AssertEquals("20150827T070000", net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.getStartTime()));
			Assert.AssertEquals("20150827T080000", net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.getEndTime()));
	
			double timePoint3 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T043000");
			result = friendAccess
					.calculateInterval(manager, timePoint3, memberKeys);
			Assert.AssertEquals(false, result.isValid());
	
			double timePoint4 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T053000");
			result = friendAccess
					.calculateInterval(manager, timePoint4, memberKeys);
			Assert.AssertEquals("20150827T050000", net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.getStartTime()));
			Assert.AssertEquals("20150827T060000", net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.getEndTime()));
		}
	
		public void testGetGroupKey() {
			// Create the group manager.
			GroupManager manager = new GroupManager(new Name("Alice"), new Name(
					"data_type"), new Sqlite3GroupManagerDb(
					System.IO.Path.GetFullPath(groupKeyDatabaseFilePath.Name)), 1024, 1, keyChain);
			setManager(manager);
	
			// Get the data list from the group manager.
			double timePoint1 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T093000");
			IList result = manager.getGroupKey(timePoint1);
	
			Assert.AssertEquals(4, result.Count);
	
			// The first data packet contains the group's encryption key (public key).
			Data data = (Data) result[0];
			Assert.AssertEquals(
					"/Alice/READ/data_type/E-KEY/20150825T090000/20150825T100000",
					data.getName().toUri());
			EncryptKey groupEKey = new EncryptKey(data.getContent());
	
			// Get the second data packet and decrypt.
			data = (Data) result[1];
			Assert.AssertEquals(
					"/Alice/READ/data_type/D-KEY/20150825T090000/20150825T100000/FOR/ndn/memberA/ksk-123",
					data.getName().toUri());
	
			/////////////////////////////////////////////////////// Start decryption.
			Blob dataContent = data.getContent();
	
			// Get the nonce key.
			// dataContent is a sequence of the two EncryptedContent.
			EncryptedContent encryptedNonce = new EncryptedContent();
			encryptedNonce.wireDecode(dataContent);
			Assert.AssertEquals(0, encryptedNonce.getInitialVector().size());
			Assert.AssertEquals(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep,
					encryptedNonce.getAlgorithmType());
	
			EncryptParams decryptParams = new EncryptParams(
					net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.RsaOaep);
			Blob blobNonce = encryptedNonce.getPayload();
			Blob nonce = net.named_data.jndn.encrypt.algo.RsaAlgorithm.decrypt(decryptKeyBlob, blobNonce,
					decryptParams);
	
			// Get the payload.
			// Use the size of encryptedNonce to find the start of encryptedPayload.
			ByteBuffer payloadContent = dataContent.buf().duplicate();
			payloadContent.position(encryptedNonce.wireEncode().size());
			EncryptedContent encryptedPayload = new EncryptedContent();
			encryptedPayload.wireDecode(payloadContent);
			Assert.AssertEquals(16, encryptedPayload.getInitialVector().size());
			Assert.AssertEquals(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc,
					encryptedPayload.getAlgorithmType());
	
			decryptParams.setAlgorithmType(net.named_data.jndn.encrypt.algo.EncryptAlgorithmType.AesCbc);
			decryptParams.setInitialVector(encryptedPayload.getInitialVector());
			Blob blobPayload = encryptedPayload.getPayload();
			Blob largePayload = net.named_data.jndn.encrypt.algo.AesAlgorithm.decrypt(nonce, blobPayload,
					decryptParams);
	
			// Get the group D-KEY.
			DecryptKey groupDKey = new DecryptKey(largePayload);
	
			/////////////////////////////////////////////////////// End decryption.
	
			// Check the D-KEY.
			EncryptKey derivedGroupEKey = net.named_data.jndn.encrypt.algo.RsaAlgorithm.deriveEncryptKey(groupDKey
					.getKeyBits());
			Assert.AssertTrue(groupEKey.getKeyBits().equals(derivedGroupEKey.getKeyBits()));
	
			// Check the third data packet.
			data = (Data) result[2];
			Assert.AssertEquals(
					"/Alice/READ/data_type/D-KEY/20150825T090000/20150825T100000/FOR/ndn/memberB/ksk-123",
					data.getName().toUri());
	
			// Check the fourth data packet.
			data = (Data) result[3];
			Assert.AssertEquals(
					"/Alice/READ/data_type/D-KEY/20150825T090000/20150825T100000/FOR/ndn/memberC/ksk-123",
					data.getName().toUri());
	
			// Check invalid time stamps for getting the group key.
			double timePoint2 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150826T083000");
			Assert.AssertEquals(0, manager.getGroupKey(timePoint2).Count);
	
			double timePoint3 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T023000");
			Assert.AssertEquals(0, manager.getGroupKey(timePoint3).Count);
		}
	
		private FileInfo dKeyDatabaseFilePath;
		private FileInfo eKeyDatabaseFilePath;
		private FileInfo intervalDatabaseFilePath;
		private FileInfo groupKeyDatabaseFilePath;
		private Blob decryptKeyBlob;
		private Blob encryptKeyBlob;
		private readonly IdentityCertificate certificate;
		private KeyChain keyChain;
		private GroupManager.FriendAccess friendAccess;
	}
}