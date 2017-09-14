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
namespace net.named_data.jndn.tests.unit_tests {
	
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	public class TestSigningInfo {
		public void testBasic() {
			Name identityName = new Name("/my-identity");
			Name keyName = new Name("/my-key");
			Name certificateName = new Name("/my-cert");
	
			SigningInfo info = new SigningInfo();
	
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.NULL, info.getSignerType());
			Assert.AssertTrue(new Name().equals(info.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, info.getDigestAlgorithm());
	
			info.setSigningIdentity(identityName);
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.ID, info.getSignerType());
			Assert.AssertTrue(identityName.equals(info.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, info.getDigestAlgorithm());
	
			SigningInfo infoId = new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.ID,
					identityName);
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.ID, infoId.getSignerType());
			Assert.AssertTrue(identityName.equals(infoId.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoId.getDigestAlgorithm());
	
			info.setSigningKeyName(keyName);
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.KEY, info.getSignerType());
			Assert.AssertTrue(keyName.equals(info.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, info.getDigestAlgorithm());
	
			SigningInfo infoKey = new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.KEY,
					keyName);
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.KEY, infoKey.getSignerType());
			Assert.AssertTrue(keyName.equals(infoKey.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoKey.getDigestAlgorithm());
	
			info.setSigningCertificateName(certificateName);
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.CERT, info.getSignerType());
			Assert.AssertTrue(certificateName.equals(info.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, info.getDigestAlgorithm());
	
			SigningInfo infoCert = new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.CERT,
					certificateName);
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.CERT, infoCert.getSignerType());
			Assert.AssertTrue(certificateName.equals(infoCert.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoCert.getDigestAlgorithm());
	
			info.setSha256Signing();
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.SHA256, info.getSignerType());
			Assert.AssertTrue(new Name().equals(info.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, info.getDigestAlgorithm());
	
			SigningInfo infoSha256 = new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.SHA256);
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.SHA256, infoSha256.getSignerType());
			Assert.AssertTrue(new Name().equals(infoSha256.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoSha256.getDigestAlgorithm());
		}
	
		public void testFromString() {
			SigningInfo infoDefault = new SigningInfo("");
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.NULL, infoDefault.getSignerType());
			Assert.AssertTrue(new Name().equals(infoDefault.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoDefault.getDigestAlgorithm());
	
			SigningInfo infoId = new SigningInfo("id:/my-identity");
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.ID, infoId.getSignerType());
			Assert.AssertTrue(new Name("/my-identity").equals(infoId.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoId.getDigestAlgorithm());
	
			SigningInfo infoKey = new SigningInfo("key:/my-key");
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.KEY, infoKey.getSignerType());
			Assert.AssertTrue(new Name("/my-key").equals(infoKey.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoKey.getDigestAlgorithm());
	
			SigningInfo infoCert = new SigningInfo("cert:/my-cert");
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.CERT, infoCert.getSignerType());
			Assert.AssertTrue(new Name("/my-cert").equals(infoCert.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoCert.getDigestAlgorithm());
	
			SigningInfo infoSha = new SigningInfo(
					"id:/localhost/identity/digest-sha256");
			Assert.AssertEquals(net.named_data.jndn.security.SigningInfo.SignerType.SHA256, infoSha.getSignerType());
			Assert.AssertTrue(new Name().equals(infoSha.getSignerName()));
			Assert.AssertEquals(net.named_data.jndn.security.DigestAlgorithm.SHA256, infoSha.getDigestAlgorithm());
		}
	
		public void testToString() {
			Assert.AssertEquals("", "" + new SigningInfo());
	
			Assert.AssertEquals("id:/my-identity", ""
					+ new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.ID, new Name(
							"/my-identity")));
			Assert.AssertEquals("key:/my-key", ""
					+ new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.KEY, new Name(
							"/my-key")));
			Assert.AssertEquals("cert:/my-cert", ""
					+ new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.CERT, new Name(
							"/my-cert")));
			Assert.AssertEquals("id:/localhost/identity/digest-sha256", ""
					+ new SigningInfo(net.named_data.jndn.security.SigningInfo.SignerType.SHA256));
		}
	
		public void testChaining() {
			SigningInfo info = new SigningInfo()
					.setSigningIdentity(new Name("/identity"))
					.setSigningKeyName(new Name("/key/name"))
					.setSigningCertificateName(new Name("/cert/name"))
					.setPibIdentity(null).setPibKey(null).setSha256Signing()
					.setDigestAlgorithm(net.named_data.jndn.security.DigestAlgorithm.SHA256);
	
			Assert.AssertEquals("id:/localhost/identity/digest-sha256", "" + info);
		}
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}
}