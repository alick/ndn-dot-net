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
namespace net.named_data.jndn.tests.integration_tests {
	
	using ILOG.J2CsMapping.NIO;
	using ILOG.J2CsMapping.Util;
	using ILOG.J2CsMapping.Util.Logging;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn;
	using net.named_data.jndn.encoding;
	using net.named_data.jndn.encrypt;
	using net.named_data.jndn.security;
	using net.named_data.jndn.util;
	
	public class TestGroupManagerDb {
		// Convert the int array to a ByteBuffer.
		public static ByteBuffer toBuffer(int[] array) {
			ByteBuffer result = ILOG.J2CsMapping.NIO.ByteBuffer.allocate(array.Length);
			for (int i = 0; i < array.Length; ++i)
				result.put((byte) (array[i] & 0xff));
	
			result.flip();
			return result;
		}
	
		private static readonly ByteBuffer SCHEDULE = toBuffer(new int[] {
				0x8f,
				0xc4,// Schedule
				0x8d,
				0x90,// WhiteIntervalList
				0x8c,
				0x2e, // RepetitiveInterval
				0x86, 0x0f, 0x32, 0x30, 0x31, 0x35, 0x30, 0x38, 0x32, 0x35, 0x54,
				0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x87, 0x0f, 0x32, 0x30, 0x31,
				0x35, 0x30, 0x38, 0x32, 0x35, 0x54, 0x30, 0x30, 0x30, 0x30, 0x30,
				0x30, 0x88,
				0x01,
				0x04,
				0x89,
				0x01,
				0x07,
				0x8a,
				0x01,
				0x00,
				0x8b,
				0x01,
				0x00,
				0x8c,
				0x2e, // RepetitiveInterval
				0x86, 0x0f, 0x32, 0x30, 0x31, 0x35, 0x30, 0x38, 0x32, 0x35, 0x54,
				0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x87, 0x0f, 0x32, 0x30, 0x31,
				0x35, 0x30, 0x38, 0x32, 0x38, 0x54, 0x30, 0x30, 0x30, 0x30, 0x30,
				0x30, 0x88, 0x01, 0x05, 0x89, 0x01,
				0x0a,
				0x8a,
				0x01,
				0x02,
				0x8b,
				0x01,
				0x01,
				0x8c,
				0x2e, // RepetitiveInterval
				0x86, 0x0f, 0x32, 0x30, 0x31, 0x35, 0x30, 0x38, 0x32, 0x35, 0x54,
				0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x87, 0x0f, 0x32, 0x30, 0x31,
				0x35, 0x30, 0x38, 0x32, 0x38, 0x54, 0x30, 0x30, 0x30, 0x30, 0x30,
				0x30, 0x88, 0x01, 0x06, 0x89, 0x01, 0x08, 0x8a,
				0x01,
				0x01,
				0x8b,
				0x01,
				0x01,
				0x8e,
				0x30, // BlackIntervalList
				0x8c,
				0x2e, // RepetitiveInterval
				0x86, 0x0f, 0x32, 0x30, 0x31, 0x35, 0x30, 0x38, 0x32, 0x37, 0x54,
				0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x87, 0x0f, 0x32, 0x30, 0x31,
				0x35, 0x30, 0x38, 0x32, 0x37, 0x54, 0x30, 0x30, 0x30, 0x30, 0x30,
				0x30, 0x88, 0x01, 0x07, 0x89, 0x01, 0x08, 0x8a, 0x01, 0x00, 0x8b,
				0x01, 0x00 });
	
		public void setUp() {
			// Don't show INFO log messages.
			ILOG.J2CsMapping.Util.Logging.Logger.getLogger("").setLevel(ILOG.J2CsMapping.Util.Logging.Level.WARNING);
	
			FileInfo policyConfigDirectory = net.named_data.jndn.tests.integration_tests.IntegrationTestsCommon
					.getPolicyConfigDirectory();
	
			databaseFilePath = new FileInfo(System.IO.Path.Combine(policyConfigDirectory.FullName,"test.db"));
			databaseFilePath.delete();
	
			database = new Sqlite3GroupManagerDb(databaseFilePath.FullName);
		}
	
		public void tearDown() {
			databaseFilePath.delete();
		}
	
		public void testDatabaseFunctions() {
			Blob scheduleBlob = new Blob(SCHEDULE, false);
	
			// Create a schedule.
			Schedule schedule = new Schedule();
			try {
				schedule.wireDecode(scheduleBlob);
			} catch (EncodingException ex) {
				// We don't expect this to happen.
				Assert.Fail("Error decoding Schedule: " + ex.Message);
			}
	
			// Create a member.
			RsaKeyParams // Create a member.
					paras = new RsaKeyParams();
			DecryptKey decryptKey;
			EncryptKey encryptKey;
			try {
				decryptKey = net.named_data.jndn.encrypt.algo.RsaAlgorithm.generateKey(paras);
				encryptKey = net.named_data.jndn.encrypt.algo.RsaAlgorithm.deriveEncryptKey(decryptKey.getKeyBits());
			} catch (Exception ex_0) {
				// Don't expect this to happen.
				Assert.Fail("Error creating test keys: " + ex_0.Message);
				return;
			}
			Blob keyBlob = encryptKey.getKeyBits();
	
			Name name1 = new Name("/ndn/BoyA/ksk-123");
			Name name2 = new Name("/ndn/BoyB/ksk-1233");
			Name name3 = new Name("/ndn/GirlC/ksk-123");
			Name name4 = new Name("/ndn/GirlD/ksk-123");
			Name name5 = new Name("/ndn/Hello/ksk-123");
	
			// Add schedules into the database.
			try {
				database.addSchedule("work-time", schedule);
				database.addSchedule("rest-time", schedule);
				database.addSchedule("play-time", schedule);
				database.addSchedule("boelter-time", schedule);
			} catch (Exception ex_1) {
				Assert.Fail("Unexpected error adding a schedule: " + ex_1.Message);
			}
	
			// Throw an exception when adding a schedule with an existing name.
			bool gotError = true;
			try {
				database.addSchedule("boelter-time", schedule);
				gotError = false;
			} catch (GroupManagerDb.Error ex_2) {
			}
			if (!gotError)
				Assert.Fail("Expected an error adding a duplicate schedule");
	
			// Add members into the database.
			try {
				database.addMember("work-time", name1, keyBlob);
				database.addMember("rest-time", name2, keyBlob);
				database.addMember("play-time", name3, keyBlob);
				database.addMember("play-time", name4, keyBlob);
			} catch (Exception ex_3) {
				Assert.Fail("Unexpected error adding a member: " + ex_3.Message);
			}
	
			// Throw an exception when adding a member with a non-existing schedule name.
			gotError = true;
			try {
				database.addMember("false-time", name5, keyBlob);
				gotError = false;
			} catch (GroupManagerDb.Error ex_4) {
			}
			if (!gotError)
				Assert.Fail("Expected an error adding a member with non-existing schedule");
	
			try {
				database.addMember("boelter-time", name5, keyBlob);
			} catch (Exception ex_5) {
				Assert.Fail("Unexpected error adding a member: " + ex_5.Message);
			}
	
			// Throw an exception when adding a member having an existing identity.
			gotError = true;
			try {
				database.addMember("work-time", name5, keyBlob);
				gotError = false;
			} catch (GroupManagerDb.Error ex_6) {
			}
			if (!gotError)
				Assert.Fail("Expected an error adding a member with an existing identity");
	
			// Test has functions.
			Assert.AssertEquals(true, database.hasSchedule("work-time"));
			Assert.AssertEquals(true, database.hasSchedule("rest-time"));
			Assert.AssertEquals(true, database.hasSchedule("play-time"));
			Assert.AssertEquals(false, database.hasSchedule("sleep-time"));
			Assert.AssertEquals(false, database.hasSchedule(""));
	
			Assert.AssertEquals(true, database.hasMember(new Name("/ndn/BoyA")));
			Assert.AssertEquals(true, database.hasMember(new Name("/ndn/BoyB")));
			Assert.AssertEquals(false, database.hasMember(new Name("/ndn/BoyC")));
	
			// Get a schedule.
			Schedule scheduleResult = database.getSchedule("work-time");
			Assert.AssertTrue(scheduleResult.wireEncode().equals(scheduleBlob));
	
			scheduleResult = database.getSchedule("play-time");
			Assert.AssertTrue(scheduleResult.wireEncode().equals(scheduleBlob));
	
			// Throw an exception when when there is no such schedule in the database.
			gotError = true;
			try {
				database.getSchedule("work-time-11");
				gotError = false;
			} catch (GroupManagerDb.Error ex_7) {
			}
			if (!gotError)
				Assert.Fail("Expected an error getting a non-existing schedule");
	
			// List all schedule names.
			IList names = database.listAllScheduleNames();
			Assert.AssertTrue(names.Contains("work-time"));
			Assert.AssertTrue(names.Contains("play-time"));
			Assert.AssertTrue(names.Contains("rest-time"));
			Assert.AssertTrue(!names.Contains("sleep-time"));
	
			// List members of a schedule.
			IDictionary memberMap = database.getScheduleMembers("play-time");
			Assert.AssertTrue(memberMap.Count != 0);
	
			// When there's no such schedule, the return map's size should be 0.
			Assert.AssertEquals(0, database.getScheduleMembers("sleep-time").Count);
	
			// List all members.
			IList members = database.listAllMembers();
			Assert.AssertTrue(members.Contains(new Name("/ndn/GirlC")));
			Assert.AssertTrue(members.Contains(new Name("/ndn/GirlD")));
			Assert.AssertTrue(members.Contains(new Name("/ndn/BoyA")));
			Assert.AssertTrue(members.Contains(new Name("/ndn/BoyB")));
	
			// Rename a schedule.
			Assert.AssertEquals(true, database.hasSchedule("boelter-time"));
			database.renameSchedule("boelter-time", "rieber-time");
			Assert.AssertEquals(false, database.hasSchedule("boelter-time"));
			Assert.AssertEquals(true, database.hasSchedule("rieber-time"));
			Assert.AssertEquals("rieber-time",
					database.getMemberSchedule(new Name("/ndn/Hello")));
	
			// Update a schedule.
			Schedule newSchedule = new Schedule();
			try {
				newSchedule.wireDecode(scheduleBlob);
			} catch (EncodingException ex_8) {
				// We don't expect this to happen.
				Assert.Fail("Error decoding Schedule: " + ex_8.Message);
			}
			RepetitiveInterval repetitiveInterval = new RepetitiveInterval(
					net.named_data.jndn.encrypt.Schedule.fromIsoString("20150825T000000"),
					net.named_data.jndn.encrypt.Schedule.fromIsoString("20150921T000000"), 2, 10, 5,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY);
			newSchedule.addWhiteInterval(repetitiveInterval);
			database.updateSchedule("rieber-time", newSchedule);
			scheduleResult = database.getSchedule("rieber-time");
			Assert.AssertTrue(!scheduleResult.wireEncode().equals(scheduleBlob));
			Assert.AssertTrue(scheduleResult.wireEncode().equals(newSchedule.wireEncode()));
	
			// Add a new schedule when updating a non-existing schedule.
			Assert.AssertEquals(false, database.hasSchedule("ralphs-time"));
			database.updateSchedule("ralphs-time", newSchedule);
			Assert.AssertEquals(true, database.hasSchedule("ralphs-time"));
	
			// Update the schedule of a member.
			database.updateMemberSchedule(new Name("/ndn/Hello"), "play-time");
			Assert.AssertEquals("play-time",
					database.getMemberSchedule(new Name("/ndn/Hello")));
	
			// Delete a member.
			Assert.AssertEquals(true, database.hasMember(new Name("/ndn/Hello")));
			database.deleteMember(new Name("/ndn/Hello"));
			Assert.AssertEquals(false, database.hasMember(new Name("/ndn/Hello")));
	
			// Delete a non-existing member.
			try {
				database.deleteMember(new Name("/ndn/notExisting"));
			} catch (Exception ex_9) {
				Assert.Fail("Unexpected error deleting a non-existing member: "
						+ ex_9.Message);
			}
	
			// Delete a schedule. All the members using this schedule should be deleted.
			database.deleteSchedule("play-time");
			Assert.AssertEquals(false, database.hasSchedule("play-time"));
			Assert.AssertEquals(false, database.hasMember(new Name("/ndn/GirlC")));
			Assert.AssertEquals(false, database.hasMember(new Name("/ndn/GirlD")));
	
			// Delete a non-existing schedule.
			try {
				database.deleteSchedule("not-existing-time");
			} catch (Exception ex_10) {
				Assert.Fail("Unexpected error deleting a non-existing schedule: "
						+ ex_10.Message);
			}
		}
	
		private FileInfo databaseFilePath;
		private GroupManagerDb database;
	}
}
