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
namespace net.named_data.jndn.tests.integration_tests {
	
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
	using net.named_data.jndn.encrypt;
	using net.named_data.jndn.security;
	using net.named_data.jndn.security.pib;
	using net.named_data.jndn.security.tpm;
	using net.named_data.jndn.security.v2;
	using net.named_data.jndn.util;
	
	public class TestAccessManagerV2 {
		internal class AccessManagerFixture : IdentityManagementFixture {
			public AccessManagerFixture() {
				this.userIdentities_ = new ArrayList<PibIdentity>();
				face_ = new InMemoryStorageFace(new InMemoryStoragePersistent());
				accessIdentity_ = addIdentity(new Name("/access/policy/identity"));
				// This is a hack to get access to the KEK key-id.
				nacIdentity_ = addIdentity(new Name(
						"/access/policy/identity/NAC/dataset"), new RsaKeyParams());
				ILOG.J2CsMapping.Collections.Collections.Add(userIdentities_,addIdentity(new Name("/first/user"),
									new RsaKeyParams()));
				ILOG.J2CsMapping.Collections.Collections.Add(userIdentities_,addIdentity(new Name("/second/user"),
									new RsaKeyParams()));
				manager_ = new AccessManagerV2(accessIdentity_,
						new Name("/dataset"), keyChain_, face_);
	
				/* foreach */
				foreach (PibIdentity user  in  userIdentities_)
					manager_.addMember(user.getDefaultKey().getDefaultCertificate());
			}
	
			public readonly InMemoryStorageFace face_;
			public readonly PibIdentity accessIdentity_;
			public readonly PibIdentity nacIdentity_;
			public readonly ArrayList<PibIdentity> userIdentities_;
			public readonly AccessManagerV2 manager_;
		}
	
		internal TestAccessManagerV2.AccessManagerFixture  fixture_;
	
		public void setUp() {
			// Turn off INFO log messages.
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger("").setLevel(ILOG.J2CsMapping.Util.Logging.Level.SEVERE);
	
			fixture_ = new TestAccessManagerV2.AccessManagerFixture ();
		}
	
		public void testPublishedKek() {
			fixture_.face_.receive(new Interest(new Name(
					"/access/policy/identity/NAC/dataset/KEK"))
					.setCanBePrefix(true).setMustBeFresh(true));
	
			Assert.AssertTrue(fixture_.face_.sentData_[0].getName().getPrefix(-1)
					.equals(new Name("/access/policy/identity/NAC/dataset/KEK")));
			Assert.AssertTrue(fixture_.face_.sentData_[0]
					.getName()
					.get(-1)
					.equals(fixture_.nacIdentity_.getDefaultKey().getName().get(-1)));
		}
	
		public void testPublishedKdks() {
			/* foreach */
			foreach (PibIdentity user  in  fixture_.userIdentities_) {
				Name kdkName = new Name("/access/policy/identity/NAC/dataset/KDK");
				kdkName.append(
						fixture_.nacIdentity_.getDefaultKey().getName().get(-1))
						.append("ENCRYPTED-BY")
						.append(user.getDefaultKey().getName());
	
				fixture_.face_.receive(new Interest(kdkName).setCanBePrefix(true)
						.setMustBeFresh(true));
	
				Assert.AssertTrue(
						"Sent Data does not have the KDK name " + kdkName.toUri(),
						fixture_.face_.sentData_[0].getName().equals(kdkName));
				ILOG.J2CsMapping.Collections.Collections.Clear(fixture_.face_.sentData_);
			}
		}
	
		public void testEnumerateDataFromIms() {
			Assert.AssertEquals(3, fixture_.manager_.size());
	
			int nKek = 0;
			int nKdk = 0;
			/* foreach */
			foreach (Object data  in  fixture_.manager_.getCache_().Values) {
				if (((Data) data).getName().get(5)
						.equals(net.named_data.jndn.encrypt.EncryptorV2.NAME_COMPONENT_KEK))
					++nKek;
				if (((Data) data).getName().get(5)
						.equals(net.named_data.jndn.encrypt.EncryptorV2.NAME_COMPONENT_KDK))
					++nKdk;
			}
	
			Assert.AssertEquals(1, nKek);
			Assert.AssertEquals(2, nKdk);
		}
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}
