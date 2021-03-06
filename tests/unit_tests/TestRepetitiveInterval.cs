// --------------------------------------------------------------------------------------------------
// This file was automatically generated by J2CS Translator (http://j2cstranslator.sourceforge.net/). 
// Version 1.3.6.20110331_01     
//
// ${CustomMessageForDisclaimer}                                                                             
// --------------------------------------------------------------------------------------------------
 /// <summary>
/// Copyright (C) 2015-2019 Regents of the University of California.
/// </summary>
///
namespace net.named_data.jndn.tests.unit_tests {
	
	using ILOG.J2CsMapping.Util;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Runtime.CompilerServices;
	using net.named_data.jndn.encrypt;
	using net.named_data.jndn.util;
	
	public class TestRepetitiveInterval {
		public void testConstruction() {
			RepetitiveInterval repetitiveInterval1 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"), 5, 10);
			Assert.AssertEquals("20150825T000000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(repetitiveInterval1.getStartDate()));
			Assert.AssertEquals("20150825T000000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(repetitiveInterval1.getEndDate()));
			Assert.AssertEquals(5, repetitiveInterval1.getIntervalStartHour());
			Assert.AssertEquals(10, repetitiveInterval1.getIntervalEndHour());
	
			RepetitiveInterval repetitiveInterval2 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150827T000000"), 5, 10, 1,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY);
	
			Assert.AssertEquals(1, repetitiveInterval2.getNRepeats());
			Assert.AssertEquals(net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY,
					repetitiveInterval2.getRepeatUnit());
	
			RepetitiveInterval repetitiveInterval3 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20151227T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.MONTH);
	
			Assert.AssertEquals(2, repetitiveInterval3.getNRepeats());
			Assert.AssertEquals(net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.MONTH,
					repetitiveInterval3.getRepeatUnit());
	
			RepetitiveInterval repetitiveInterval4 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20301227T000000"), 5, 10, 5,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.YEAR);
	
			Assert.AssertEquals(5, repetitiveInterval4.getNRepeats());
			Assert.AssertEquals(net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.YEAR,
					repetitiveInterval4.getRepeatUnit());
	
			RepetitiveInterval repetitiveInterval5 = new RepetitiveInterval();
	
			Assert.AssertEquals(0, repetitiveInterval5.getNRepeats());
			Assert.AssertEquals(net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.NONE,
					repetitiveInterval5.getRepeatUnit());
		}
	
		public void testCoverTimePoint() {
			///////////////////////////////////////////// With the repeat unit DAY.
	
			RepetitiveInterval repetitiveInterval1 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150925T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY);
			RepetitiveInterval.Result result;
	
			double timePoint1 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T050000");
	
			result = repetitiveInterval1.getInterval(timePoint1);
			Assert.AssertEquals(true, result.isPositive);
			Assert.AssertEquals("20150825T050000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getStartTime()));
			Assert.AssertEquals("20150825T100000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getEndTime()));
	
			double timePoint2 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150902T060000");
	
			result = repetitiveInterval1.getInterval(timePoint2);
			Assert.AssertEquals(true, result.isPositive);
			Assert.AssertEquals("20150902T050000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getStartTime()));
			Assert.AssertEquals("20150902T100000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getEndTime()));
	
			double timePoint3 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150929T040000");
	
			result = repetitiveInterval1.getInterval(timePoint3);
			Assert.AssertEquals(false, result.isPositive);
	
			///////////////////////////////////////////// With the repeat unit MONTH.
	
			RepetitiveInterval repetitiveInterval2 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20160825T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.MONTH);
	
			double timePoint4 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T050000");
	
			result = repetitiveInterval2.getInterval(timePoint4);
			Assert.AssertEquals(true, result.isPositive);
			Assert.AssertEquals("20150825T050000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getStartTime()));
			Assert.AssertEquals("20150825T100000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getEndTime()));
	
			double timePoint5 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20151025T060000");
	
			result = repetitiveInterval2.getInterval(timePoint5);
			Assert.AssertEquals(true, result.isPositive);
			Assert.AssertEquals("20151025T050000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getStartTime()));
			Assert.AssertEquals("20151025T100000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getEndTime()));
	
			double timePoint6 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20151226T050000");
	
			result = repetitiveInterval2.getInterval(timePoint6);
			Assert.AssertEquals(false, result.isPositive);
	
			double timePoint7 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20151225T040000");
	
			result = repetitiveInterval2.getInterval(timePoint7);
			Assert.AssertEquals(false, result.isPositive);
	
			///////////////////////////////////////////// With the repeat unit YEAR.
	
			RepetitiveInterval repetitiveInterval3 = new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20300825T000000"), 5, 10, 3,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.YEAR);
	
			double timePoint8 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T050000");
	
			result = repetitiveInterval3.getInterval(timePoint8);
			Assert.AssertEquals(true, result.isPositive);
			Assert.AssertEquals("20150825T050000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getStartTime()));
			Assert.AssertEquals("20150825T100000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getEndTime()));
	
			double timePoint9 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20180825T060000");
	
			result = repetitiveInterval3.getInterval(timePoint9);
			Assert.AssertEquals(true, result.isPositive);
			Assert.AssertEquals("20180825T050000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getStartTime()));
			Assert.AssertEquals("20180825T100000",
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.toIsoString(result.interval.getEndTime()));
	
			double timePoint10 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20180826T050000");
			result = repetitiveInterval3.getInterval(timePoint10);
			Assert.AssertEquals(false, result.isPositive);
	
			double timePoint11 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20210825T040000");
			result = repetitiveInterval3.getInterval(timePoint11);
			Assert.AssertEquals(false, result.isPositive);
	
			double timePoint12 = net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20300825T040000");
			result = repetitiveInterval3.getInterval(timePoint12);
			Assert.AssertEquals(false, result.isPositive);
		}
	
		private static bool check(RepetitiveInterval small,
				RepetitiveInterval big) {
			return small.compare(big) < 0 && !(big.compare(small) < 0);
		}
	
		public void testComparison() {
			Assert.AssertTrue(check(new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY), new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150826T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY)));
	
			Assert.AssertTrue(check(new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY), new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 6, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY)));
	
			Assert.AssertTrue(check(new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY), new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 11, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY)));
	
			Assert.AssertTrue(check(new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY), new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 3,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY)));
	
			Assert.AssertTrue(check(new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.DAY), new RepetitiveInterval(
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150825T000000"),
					net.named_data.jndn.tests.unit_tests.UnitTestsCommon.fromIsoString("20150828T000000"), 5, 10, 2,
					net.named_data.jndn.encrypt.RepetitiveInterval.RepeatUnit.MONTH)));
		}
	
		// This is to force an import of net.named_data.jndn.util.
		private static Common dummyCommon_ = new Common();
	}}
