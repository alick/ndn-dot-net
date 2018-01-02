// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2016-2018 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.tests.integration_tests {
	
	using ILOG.J2CsMapping.Threading;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	public class TestFaceCallRegisterMethods {
		public sealed class Anonymous_C3 : OnInterestCallback {
				private readonly TestFaceCallRegisterMethods outer_TestFaceCallRegisterMethods;
				private readonly int[] interestCallbackCount;
		
				public Anonymous_C3(
						TestFaceCallRegisterMethods paramouter_TestFaceCallRegisterMethods,
						int[] interestCallbackCount_0) {
					this.interestCallbackCount = interestCallbackCount_0;
					this.outer_TestFaceCallRegisterMethods = paramouter_TestFaceCallRegisterMethods;
				}
		
				public void onInterest(Name prefix, Interest interest, Face face,
						long interestFilterId, InterestFilter filter) {
					++interestCallbackCount[0];
					Data data = new Data(interest.getName());
					data.setContent(new Blob("SUCCESS"));
		
					try {
						outer_TestFaceCallRegisterMethods.keyChain.sign(data, outer_TestFaceCallRegisterMethods.certificateName);
					} catch (SecurityException ex) {
						net.named_data.jndn.tests.integration_tests.TestFaceCallRegisterMethods.logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex);
					}
					try {
						face.putData(data);
					} catch (IOException ex_0) {
						net.named_data.jndn.tests.integration_tests.TestFaceCallRegisterMethods.logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_0);
					}
				}
			}
	
		public sealed class Anonymous_C2 : OnRegisterFailed {
			private readonly int[] failedCallbackCount;
	
			public Anonymous_C2(int[] failedCallbackCount_0) {
				this.failedCallbackCount = failedCallbackCount_0;
			}
	
			public void onRegisterFailed(Name prefix) {
				++failedCallbackCount[0];
			}
		}
	
		public sealed class Anonymous_C1 : OnData {
			private readonly Data[] receivedData;
			private readonly int[] dataCallbackCount;
	
			public Anonymous_C1(Data[] receivedData_0, int[] dataCallbackCount_1) {
				this.receivedData = receivedData_0;
				this.dataCallbackCount = dataCallbackCount_1;
			}
	
			public void onData(Interest interest, Data data) {
				++dataCallbackCount[0];
				receivedData[0] = data;
			}
		}
	
		public sealed class Anonymous_C0 : OnTimeout {
			private readonly int[] timeoutCallbackCount;
	
			public Anonymous_C0(int[] timeoutCallbackCount_0) {
				this.timeoutCallbackCount = timeoutCallbackCount_0;
			}
	
			public void onTimeout(Interest interest) {
				++timeoutCallbackCount[0];
			}
		}
	
		internal Face faceIn;
		internal Face faceOut;
		internal KeyChain keyChain;
		internal Name certificateName;
	
		public void setUp() {
			Name[] localCertificateName = new Name[1];
			keyChain = net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon.buildKeyChain(localCertificateName);
			certificateName = localCertificateName[0];
	
			faceIn = net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon.buildFaceWithKeyChain("localhost",
					keyChain, certificateName);
			faceOut = net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon.buildFaceWithKeyChain("localhost",
					keyChain, certificateName);
		}
	
		public void testRegisterPrefixResponse() {
			Name prefixName = new Name("/test");
	
			int[] interestCallbackCount_0 = new int[] { 0 };
			int[] failedCallbackCount_1 = new int[] { 0 };
			faceIn.registerPrefix(prefixName, new TestFaceCallRegisterMethods.Anonymous_C3 (this, interestCallbackCount_0), new TestFaceCallRegisterMethods.Anonymous_C2 (failedCallbackCount_1));
	
			// Give the "server" time to register the interest.
			double timeout = 1000;
			double startTime = getNowMilliseconds();
			while (getNowMilliseconds() - startTime < timeout) {
				try {
					faceIn.processEvents();
				} catch (IOException ex) {
					logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex);
					break;
				} catch (EncodingException ex_2) {
					logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_2);
					break;
				}
	
				try {
					// We need to sleep for a few milliseconds so we don't use 100% of the CPU.
					ILOG.J2CsMapping.Threading.ThreadWrapper.sleep(10);
				} catch (ThreadInterruptedException ex_3) {
					logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_3);
					break;
				}
			}
	
			// Now express an interest on this new face, and see if onInterest is called.
			// Add the timestamp so it is unique and we don't get a cached response.
			int[] dataCallbackCount_4 = new int[] { 0 };
			int[] timeoutCallbackCount_5 = new int[] { 0 };
			Data[] receivedData_6 = new Data[1];
			Name interestName = prefixName.append("hello" + getNowMilliseconds());
			faceOut.expressInterest(interestName, new TestFaceCallRegisterMethods.Anonymous_C1 (receivedData_6, dataCallbackCount_4), new TestFaceCallRegisterMethods.Anonymous_C0 (timeoutCallbackCount_5));
	
			// Process events for the in and out faces.
			timeout = 10000;
			startTime = getNowMilliseconds();
			while (getNowMilliseconds() - startTime < timeout) {
				try {
					faceIn.processEvents();
					faceOut.processEvents();
				} catch (IOException ex_7) {
					logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_7);
					break;
				} catch (EncodingException ex_8) {
					logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_8);
					break;
				}
	
				bool done = true;
				if (interestCallbackCount_0[0] == 0 && failedCallbackCount_1[0] == 0)
					// Still processing faceIn.
					done = false;
				if (dataCallbackCount_4[0] == 0 && timeoutCallbackCount_5[0] == 0)
					// Still processing face_out.
					done = false;
	
				if (done)
					break;
	
				try {
					// We need to sleep for a few milliseconds so we don't use 100% of the CPU.
					ILOG.J2CsMapping.Threading.ThreadWrapper.sleep(10);
				} catch (ThreadInterruptedException ex_9) {
					logger.log(ILOG.J2CsMapping.Util.Logging.Level.SEVERE, null, ex_9);
					break;
				}
			}
	
			Assert.AssertEquals("Failed to register prefix at all", 0,
					failedCallbackCount_1[0]);
			Assert.AssertEquals("Expected 1 onInterest callback", 1,
					interestCallbackCount_0[0]);
			Assert.AssertEquals("Expected 1 onData callback", 1, dataCallbackCount_4[0]);
	
			// Check the message content.
			Blob expectedBlob = new Blob("SUCCESS");
			Assert.AssertTrue(
					"Data received on the face does not match the expected format",
					expectedBlob.equals(receivedData_6[0].getContent()));
		}
	
		public static double getNowMilliseconds() {
			return net.named_data.jndn.util.Common.getNowMilliseconds();
		}
	
		static internal readonly Logger logger = ILOG.J2CsMapping.Util.Logging.Logger
				.getLogger(typeof(TestFaceCallRegisterMethods).FullName);
	}
}
