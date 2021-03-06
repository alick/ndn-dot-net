// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2017-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.tests.integration_tests {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.tpm;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	public class TestKeyChain {
		internal IdentityManagementFixture fixture_;
	
		public void setUp() {
			fixture_ = new IdentityManagementFixture();
		}
	
		public void testManagement() {
			Name identityName = new Name("/test/id");
			Name identity2Name = new Name("/test/id2");
	
			Assert.AssertEquals(0, fixture_.keyChain_.getPib().getIdentities_().size());
			try {
				fixture_.keyChain_.getPib().getDefaultIdentity();
				Assert.Fail("Did not throw the expected exception");
			} catch (Pib.Error ex) {
			} catch (Exception ex_0) {
				Assert.Fail("Did not throw the expected exception");
			}
	
			// Create an identity.
			PibIdentity id = fixture_.keyChain_.createIdentityV2(identityName);
			Assert.AssertTrue(id != null);
			Assert.AssertTrue(fixture_.keyChain_.getPib().getIdentities_()
					.getIdentities_().Contains(identityName));
	
			// The first added identity becomes the default identity.
			try {
				fixture_.keyChain_.getPib().getDefaultIdentity();
			} catch (Exception ex_1) {
				Assert.Fail("Unexpected exception: " + ex_1.Message);
			}
	
			// The default key of the added identity must exist.
			PibKey key = null;
			try {
				key = id.getDefaultKey();
			} catch (Exception ex_2) {
				Assert.Fail("Unexpected exception: " + ex_2.Message);
			}
	
			// The default certificate of the default key must exist.
			try {
				key.getDefaultCertificate();
			} catch (Exception ex_3) {
				Assert.Fail("Unexpected exception: " + ex_3.Message);
			}
	
			// Delete the key.
			Name key1Name = key.getName();
			try {
				id.getKey(key1Name);
			} catch (Exception ex_4) {
				Assert.Fail("Unexpected exception: " + ex_4.Message);
			}
	
			Assert.AssertEquals(1, id.getKeys_().size());
			fixture_.keyChain_.deleteKey(id, key);
			/* TODO: Implement key validity.
			    // The key instance should not be valid anymore.
			    assertTrue(!key);
			*/
	
			try {
				id.getKey(key1Name);
				Assert.Fail("Did not throw the expected exception");
			} catch (Pib.Error ex_5) {
			} catch (Exception ex_6) {
				Assert.Fail("Did not throw the expected exception");
			}
	
			Assert.AssertEquals(0, id.getKeys_().size());
	
			// Create another key.
			fixture_.keyChain_.createKey(id);
			// The added key becomes the default key.
			try {
				id.getDefaultKey();
			} catch (Exception ex_7) {
				Assert.Fail("Unexpected exception: " + ex_7.Message);
			}
	
			PibKey key2 = id.getDefaultKey();
			Assert.AssertTrue(key2 != null);
			Assert.AssertTrue(!key2.getName().equals(key1Name));
			Assert.AssertEquals(1, id.getKeys_().size());
			try {
				key2.getDefaultCertificate();
			} catch (Exception ex_8) {
				Assert.Fail("Unexpected exception: " + ex_8.Message);
			}
	
			// Create a third key.
			PibKey key3 = fixture_.keyChain_.createKey(id);
			Assert.AssertTrue(!key3.getName().equals(key2.getName()));
			// The added key will not be the default key, because the default key already exists.
			Assert.AssertTrue(id.getDefaultKey().getName().equals(key2.getName()));
			Assert.AssertEquals(2, id.getKeys_().size());
			try {
				key3.getDefaultCertificate();
			} catch (Exception ex_9) {
				Assert.Fail("Unexpected exception: " + ex_9.Message);
			}
	
			// Delete the certificate.
			Assert.AssertEquals(1, key3.getCertificates_().size());
			CertificateV2 key3Cert1 = (CertificateV2) ILOG.J2CsMapping.Collections.Collections.ToArray(key3.getCertificates_()
									.getCertificates_().Values)[0];
			Name key3CertName = key3Cert1.getName();
			fixture_.keyChain_.deleteCertificate(key3, key3CertName);
			Assert.AssertEquals(0, key3.getCertificates_().size());
			try {
				key3.getDefaultCertificate();
				Assert.Fail("Did not throw the expected exception");
			} catch (Pib.Error ex_10) {
			} catch (Exception ex_11) {
				Assert.Fail("Did not throw the expected exception");
			}
	
			// Add a certificate.
			fixture_.keyChain_.addCertificate(key3, key3Cert1);
			Assert.AssertEquals(1, key3.getCertificates_().size());
			try {
				key3.getDefaultCertificate();
			} catch (Exception ex_12) {
				Assert.Fail("Unexpected exception: " + ex_12.Message);
			}
	
			// Overwriting the certificate should work.
			fixture_.keyChain_.addCertificate(key3, key3Cert1);
			Assert.AssertEquals(1, key3.getCertificates_().size());
			// Add another certificate.
			CertificateV2 key3Cert2 = new CertificateV2(key3Cert1);
			Name key3Cert2Name = new Name(key3.getName());
			key3Cert2Name.append("Self");
			key3Cert2Name.appendVersion(1);
			key3Cert2.setName(key3Cert2Name);
			fixture_.keyChain_.addCertificate(key3, key3Cert2);
			Assert.AssertEquals(2, key3.getCertificates_().size());
	
			// Set the default certificate.
			Assert.AssertTrue(key3.getDefaultCertificate().getName().equals(key3CertName));
			fixture_.keyChain_.setDefaultCertificate(key3, key3Cert2);
			Assert.AssertTrue(key3.getDefaultCertificate().getName().equals(key3Cert2Name));
	
			// Set the default key.
			Assert.AssertTrue(id.getDefaultKey().getName().equals(key2.getName()));
			fixture_.keyChain_.setDefaultKey(id, key3);
			Assert.AssertTrue(id.getDefaultKey().getName().equals(key3.getName()));
	
			// Set the default identity.
			PibIdentity id2 = fixture_.keyChain_.createIdentityV2(identity2Name);
			Assert.AssertTrue(fixture_.keyChain_.getPib().getDefaultIdentity().getName()
					.equals(id.getName()));
			fixture_.keyChain_.setDefaultIdentity(id2);
			Assert.AssertTrue(fixture_.keyChain_.getPib().getDefaultIdentity().getName()
					.equals(id2.getName()));
	
			// Delete an identity.
			fixture_.keyChain_.deleteIdentity(id);
			/* TODO: Implement identity validity.
			    // The identity instance should not be valid any more.
			    BOOST_CHECK(!id);
			*/
			try {
				fixture_.keyChain_.getPib().getIdentity(identityName);
				Assert.Fail("Did not throw the expected exception");
			} catch (Pib.Error ex_13) {
			} catch (Exception ex_14) {
				Assert.Fail("Did not throw the expected exception");
			}
	
			Assert.AssertTrue(!fixture_.keyChain_.getPib().getIdentities_()
					.getIdentities_().Contains(identityName));
		}
	
		public void testSelfSignedCertValidity() {
			CertificateV2 certificate = fixture_
					.addIdentity(
							new Name(
									"/Security/V2/TestKeyChain/SelfSignedCertValidity"))
					.getDefaultKey().getDefaultCertificate();
			Assert.AssertTrue(certificate.isValid());
			// Check 10 years from now.
			Assert.AssertTrue(certificate.isValid(net.named_data.jndn.util.Common.getNowMilliseconds() + 10 * 365
					* 24 * 3600 * 1000.0d));
			// Check that notAfter is later than 10 years from now.
			Assert.AssertTrue(certificate.getValidityPeriod().getNotAfter() > net.named_data.jndn.util.Common
					.getNowMilliseconds() + 10 * 365 * 24 * 3600 * 1000.0d);
		}
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
